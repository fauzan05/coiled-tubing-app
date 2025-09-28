using coiled_tubing_app.Models;
using coiled_tubing_app.Services;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Animation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace coiled_tubing_app;

/// <summary>
/// Monitoring page for record management.
/// </summary>
public sealed partial class MonitoringPage : Page
{
    private RecordService? _recordService;
    private FileHistoryService? _fileHistoryService;
    private List<FileHistoryItem> _currentHistoryItems;
    private List<HistoryTableItem> _tableItems;

    public MonitoringPage()
    {
        try
        {
            InitializeComponent();
            _currentHistoryItems = new List<FileHistoryItem>();
            _tableItems = new List<HistoryTableItem>();

            // Initialize services after the page is loaded to avoid blocking UI thread
            this.Loaded += MonitoringPage_Loaded;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"MonitoringPage constructor error: {ex.Message}");
        }
    }

    private async void MonitoringPage_Loaded(object sender, RoutedEventArgs e)
    {
        try
        {
            System.Diagnostics.Debug.WriteLine("MonitoringPage_Loaded: Starting initialization");

            // Initialize services on UI thread
            await InitializeServicesAsync();
            await RefreshFileHistory();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"MonitoringPage_Loaded error: {ex.Message}");
        }
    }

    private Task InitializeServicesAsync()
    {
        return Task.Run(() =>
        {
            try
            {
                // Initialize services with proper error handling
                _recordService = new RecordService();
                _fileHistoryService = _recordService.GetFileHistoryService();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"InitializeServicesAsync error: {ex.Message}");

                // Create fallback instances to prevent null reference errors
                try
                {
                    _recordService = new RecordService();
                    _fileHistoryService = _recordService.GetFileHistoryService();
                }
                catch
                {
                    // Last resort - create minimal service instances
                    _currentHistoryItems = new List<FileHistoryItem>();
                    _tableItems = new List<HistoryTableItem>();
                }

                throw; // Re-throw to be handled by caller
            }
        });
    }

    private async void AddRecordButton_Click(object sender, RoutedEventArgs e)
    {
        if (_recordService == null)
        {
            await InitializeServicesAsync();
        }

        // Use the simple dialog for just record name input
        var dialog = new SimpleRecordDialog();
        dialog.XamlRoot = this.XamlRoot;

        var result = await dialog.ShowAsync();

        System.Diagnostics.Debug.WriteLine($"AddRecordButton_Click: Dialog result = {result}");
        System.Diagnostics.Debug.WriteLine($"AddRecordButton_Click: RecordName = '{dialog.RecordName}'");

        if (result == ContentDialogResult.Primary && !string.IsNullOrEmpty(dialog.RecordName))
        {
            // Create a basic record with just the name and created date
            var record = new ItemRecord
            {
                RecordName = dialog.RecordName,
                CreatedDate = DateTime.Now
            };

            // Save the record
            var saveResult = await _recordService!.SaveRecordAsync(record);

            System.Diagnostics.Debug.WriteLine($"AddRecordButton_Click: SaveRecordAsync result:");
            System.Diagnostics.Debug.WriteLine($"  - Success: {saveResult.Success}");
            System.Diagnostics.Debug.WriteLine($"  - FilePath: '{saveResult.FilePath}'");
            System.Diagnostics.Debug.WriteLine($"  - Directory: '{saveResult.Directory}'");

            if (saveResult.Success)
            {
                // Force refresh service instance to get latest data
                _fileHistoryService = _recordService?.GetFileHistoryService();

                // Refresh history list
                await RefreshFileHistory();
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("AddRecordButton_Click: SaveRecordAsync reported failure");
            }
        }
    }

    private async void LoadRecordButton_Click(object sender, RoutedEventArgs e)
    {
        if (_recordService == null)
        {
            await InitializeServicesAsync();
        }

        var result = await _recordService!.LoadRecordAsync();

        if (result.Record != null)
        {
            // Refresh history to show the loaded record
            await RefreshFileHistory();
        }
    }

    private async void RefreshHistoryButton_Click(object sender, RoutedEventArgs e)
    {
        await RefreshFileHistory();
    }

    private async void ClearHistoryButton_Click(object sender, RoutedEventArgs e)
    {
        if (_fileHistoryService == null)
        {
            await InitializeServicesAsync();
        }

        var dialog = new ContentDialog
        {
            Title = "Clear History",
            Content = "Are you sure you want to clear all file history? This action cannot be undone.",
            PrimaryButtonText = "Yes, Clear",
            SecondaryButtonText = "Cancel",
            XamlRoot = this.XamlRoot
        };

        var result = await dialog.ShowAsync();
        if (result == ContentDialogResult.Primary)
        {
            await _fileHistoryService!.ClearHistoryAsync();
            await RefreshFileHistory();
        }
    }

    private async void DeleteHistoryItem_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button button && button.Tag is HistoryTableItem tableItem && tableItem.OriginalItem != null)
        {
            System.Diagnostics.Debug.WriteLine($"DeleteHistoryItem_Click: Deleting item '{tableItem.OriginalItem.FilePath}'");
            var dialog = new ContentDialog
            {
                Title = "Delete Record",
                Content = $"Are you sure you want to remove '{tableItem.RecordName}' from history?",
                PrimaryButtonText = "Delete",
                SecondaryButtonText = "Cancel",
                XamlRoot = this.XamlRoot
            };

            var result = await dialog.ShowAsync();
            if (result == ContentDialogResult.Primary)
            {
                if (_fileHistoryService == null)
                {
                    await InitializeServicesAsync();
                }

                await _fileHistoryService!.RemoveHistoryItemAsync(tableItem.OriginalItem);
                await RefreshFileHistory();
            }
        }
    }

    private async void EditHistoryItem_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button button && button.Tag is HistoryTableItem tableItem && tableItem.OriginalItem != null)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"EditHistoryItem_Click: Editing item '{tableItem.OriginalItem.FilePath}'");

                var editDialog = new EditRecordDialog(tableItem.RecordName);
                editDialog.XamlRoot = this.XamlRoot;

                var result = await editDialog.ShowAsync();

                if (result == ContentDialogResult.Primary && !string.IsNullOrEmpty(editDialog.RecordName))
                {
                    string newRecordName = editDialog.RecordName.Trim();

                    // Check if the name actually changed
                    if (newRecordName != tableItem.RecordName)
                    {
                        // Load the existing record from file
                        if (File.Exists(tableItem.OriginalItem.FilePath))
                        {
                            try
                            {
                                // Read the current record from file
                                string jsonContent = await File.ReadAllTextAsync(tableItem.OriginalItem.FilePath);
                                var record = JsonSerializer.Deserialize<ItemRecord>(jsonContent);

                                if (record != null)
                                {
                                    // Update the record name
                                    record.RecordName = newRecordName;

                                    // Save the updated record back to the same file
                                    if (_recordService == null)
                                    {
                                        await InitializeServicesAsync();
                                    }

                                    var saveResult = await _recordService!.SaveRecordToPathAsync(record, tableItem.OriginalItem.FilePath);

                                    if (saveResult.Success)
                                    {
                                        System.Diagnostics.Debug.WriteLine($"EditHistoryItem_Click: Record renamed successfully from '{tableItem.RecordName}' to '{newRecordName}'");

                                        // Refresh the history to show the updated name
                                        await RefreshFileHistory();
                                    }
                                    else
                                    {
                                        System.Diagnostics.Debug.WriteLine("EditHistoryItem_Click: Failed to save renamed record");

                                        // Show error dialog
                                        var errorDialog = new ContentDialog
                                        {
                                            Title = "Error",
                                            Content = "Failed to save the renamed record. Please try again.",
                                            PrimaryButtonText = "OK",
                                            XamlRoot = this.XamlRoot
                                        };
                                        await errorDialog.ShowAsync();
                                    }
                                }
                                else
                                {
                                    System.Diagnostics.Debug.WriteLine("EditHistoryItem_Click: Failed to deserialize record from file");
                                }
                            }
                            catch (Exception fileEx)
                            {
                                System.Diagnostics.Debug.WriteLine($"EditHistoryItem_Click: File operation error: {fileEx.Message}");

                                // Show error dialog
                                var errorDialog = new ContentDialog
                                {
                                    Title = "Error",
                                    Content = "Failed to read the record file. The file might be corrupted or inaccessible.",
                                    PrimaryButtonText = "OK",
                                    XamlRoot = this.XamlRoot
                                };
                                await errorDialog.ShowAsync();
                            }
                        }
                        else
                        {
                            System.Diagnostics.Debug.WriteLine($"EditHistoryItem_Click: File does not exist: {tableItem.OriginalItem.FilePath}");

                            // Show error dialog
                            var errorDialog = new ContentDialog
                            {
                                Title = "File Not Found",
                                Content = "The record file could not be found. It might have been moved or deleted.",
                                PrimaryButtonText = "OK",
                                XamlRoot = this.XamlRoot
                            };
                            await errorDialog.ShowAsync();
                        }
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine("EditHistoryItem_Click: Record name unchanged, no action needed");
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"EditHistoryItem_Click: Unexpected error: {ex.Message}");

                // Show error dialog
                var errorDialog = new ContentDialog
                {
                    Title = "Unexpected Error",
                    Content = "An unexpected error occurred while editing the record. Please try again.",
                    PrimaryButtonText = "OK",
                    XamlRoot = this.XamlRoot
                };
                await errorDialog.ShowAsync();
            }
        }
    }

    private async void OpenInDirectoryButton_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button button && button.Tag is HistoryTableItem tableItem && tableItem.OriginalItem != null)
        {
            try
            {
                string filePath = tableItem.OriginalItem.FilePath;
                System.Diagnostics.Debug.WriteLine($"OpenInDirectoryButton_Click: Opening directory for file '{filePath}'");

                // Check if file exists
                if (File.Exists(filePath))
                {
                    // Use Windows Explorer to open the directory and select the file
                    var processStartInfo = new System.Diagnostics.ProcessStartInfo
                    {
                        FileName = "explorer.exe",
                        Arguments = $"/select,\"{filePath}\"",
                        UseShellExecute = true
                    };

                    System.Diagnostics.Process.Start(processStartInfo);
                    System.Diagnostics.Debug.WriteLine($"OpenInDirectoryButton_Click: Successfully opened directory for '{filePath}'");
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"OpenInDirectoryButton_Click: File does not exist: '{filePath}'");

                    // If file doesn't exist, try to open just the directory
                    string directory = Path.GetDirectoryName(filePath);
                    if (!string.IsNullOrEmpty(directory) && Directory.Exists(directory))
                    {
                        var processStartInfo = new System.Diagnostics.ProcessStartInfo
                        {
                            FileName = "explorer.exe",
                            Arguments = $"\"{directory}\"",
                            UseShellExecute = true
                        };

                        System.Diagnostics.Process.Start(processStartInfo);

                        // Show info dialog that file was not found but directory was opened
                        var infoDialog = new ContentDialog
                        {
                            Title = "File Not Found",
                            Content = $"The file could not be found, but the directory has been opened.\n\nOriginal location: {filePath}",
                            PrimaryButtonText = "OK",
                            XamlRoot = this.XamlRoot
                        };
                        await infoDialog.ShowAsync();
                    }
                    else
                    {
                        // Neither file nor directory exists
                        var errorDialog = new ContentDialog
                        {
                            Title = "Location Not Found",
                            Content = $"Neither the file nor its directory could be found.\n\nOriginal location: {filePath}",
                            PrimaryButtonText = "OK",
                            XamlRoot = this.XamlRoot
                        };
                        await errorDialog.ShowAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"OpenInDirectoryButton_Click: Error opening directory: {ex.Message}");

                // Show error dialog
                var errorDialog = new ContentDialog
                {
                    Title = "Error Opening Directory",
                    Content = "An error occurred while trying to open the file location. Please check if the file still exists.",
                    PrimaryButtonText = "OK",
                    XamlRoot = this.XamlRoot
                };
                await errorDialog.ShowAsync();
            }
        }
    }

    private async Task RefreshFileHistory()
    {
        try
        {
            System.Diagnostics.Debug.WriteLine("RefreshFileHistory: Starting...");

            if (_fileHistoryService != null)
            {
                _currentHistoryItems = await _fileHistoryService.GetHistoryAsync();
                await ConvertToTableItems();

                if (HistoryDataGrid != null)
                {
                    HistoryDataGrid.ItemsSource = null;
                    HistoryDataGrid.ItemsSource = _tableItems;
                }

                if (HistoryStatusTextBlock != null)
                {
                    if (_tableItems.Count == 0)
                    {
                        HistoryStatusTextBlock.Text = "No records in history";
                    }
                    else
                    {
                        HistoryStatusTextBlock.Text = $"{_tableItems.Count} record(s) in history";
                    }
                }
            }
            else
            {
                if (HistoryStatusTextBlock != null)
                {
                    HistoryStatusTextBlock.Text = "History service not available";
                }
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"RefreshFileHistory error: {ex.Message}");
            if (HistoryStatusTextBlock != null)
            {
                HistoryStatusTextBlock.Text = $"Error loading history: {ex.Message}";
            }
        }
    }

    private async Task ConvertToTableItems()
    {
        try
        {
            _tableItems.Clear();

            for (int i = 0; i < _currentHistoryItems.Count; i++)
            {
                var item = _currentHistoryItems[i];

                var tableItem = new HistoryTableItem
                {
                    RowNumber = i + 1,
                    RecordName = item.RecordName,
                    LastAccessed = item.LastAccessed,
                    CreatedAt = item.CreatedAt,
                    FilePath = item.FilePath,
                    Directory = item.Directory,
                    HistoryType = item.HistoryType,
                    SelectedChartIds = new List<string>(), // Empty list
                    OriginalItem = item
                };

                await Task.Run(() =>
                {
                    _tableItems.Add(tableItem);
                });
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"ConvertToTableItems error: {ex.Message}");
        }
    }

    private void OpenRecordingPage(object sender, ItemClickEventArgs e)
    {
        var selectedRecord = e.ClickedItem as HistoryTableItem;
        if (selectedRecord == null || selectedRecord.OriginalItem == null)
        {
            System.Diagnostics.Debug.WriteLine("OpenRecordingPage: Selected record is null or has no original item");
            return;
        }
        this.Frame.Navigate(typeof(RecordingPage), selectedRecord.OriginalItem, new EntranceNavigationTransitionInfo());
    }
}