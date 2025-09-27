using coiled_tubing_app.Models;
using coiled_tubing_app.Services;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace coiled_tubing_app;

/// <summary>
/// Monitoring page for record management.
/// </summary>
public sealed partial class MonitoringPage : Page
{
    private ChartService? _chartService;
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
                _chartService = new ChartService();
                _fileHistoryService = _chartService.GetFileHistoryService();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"InitializeServicesAsync error: {ex.Message}");

                // Create fallback instances to prevent null reference errors
                try
                {
                    _chartService = new ChartService();
                    _fileHistoryService = _chartService.GetFileHistoryService();
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
        if (_chartService == null)
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
            var record = new ChartRecord
            {
                RecordName = dialog.RecordName,
                CreatedDate = DateTime.Now
            };

            // Save the record
            var saveResult = await _chartService.SaveRecordAsync(record);

            System.Diagnostics.Debug.WriteLine($"AddRecordButton_Click: SaveRecordAsync result:");
            System.Diagnostics.Debug.WriteLine($"  - Success: {saveResult.Success}");
            System.Diagnostics.Debug.WriteLine($"  - FilePath: '{saveResult.FilePath}'");
            System.Diagnostics.Debug.WriteLine($"  - Directory: '{saveResult.Directory}'");

            if (saveResult.Success)
            {
                // Force refresh service instance to get latest data
                _fileHistoryService = _chartService?.GetFileHistoryService();

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
        if (_chartService == null)
        {
            await InitializeServicesAsync();
        }

        var result = await _chartService.LoadRecordAsync();

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
                    ChartsPreview = "Simple Record", // No charts anymore
                    LastAccessed = item.LastAccessed,
                    FilePath = item.FilePath,
                    Directory = item.Directory,
                    HistoryType = item.HistoryType,
                    SelectedChartIds = new List<string>(), // Empty list
                    OriginalItem = item
                };

                _tableItems.Add(tableItem);
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"ConvertToTableItems error: {ex.Message}");
        }
    }
}