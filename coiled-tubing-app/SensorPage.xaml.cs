using coiled_tubing_app.Models;
using coiled_tubing_app.Services;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace coiled_tubing_app
{
    public sealed partial class SensorPage : Page
    {
        private ChartService? _chartService;
        private FileHistoryService? _fileHistoryService;
        private List<FileHistoryItem> _currentHistoryItems;

        public SensorPage()
        {
            try
            {
                InitializeComponent();
                _currentHistoryItems = new List<FileHistoryItem>();

                // Initialize services after the page is loaded to avoid blocking UI thread
                Loaded += SensorPage_Loaded;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"SensorPage constructor error: {ex.Message}");
            }
        }

        private async void SensorPage_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                // Initialize services on UI thread
                await InitializeServicesAsync();
                await RefreshFileHistory();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"SensorPage_Loaded error: {ex.Message}");
                if (StatusTextBlock != null)
                {
                    StatusTextBlock.Text = $"Warning: Some features may not work properly. {ex.Message}";
                    StatusTextBlock.Foreground = new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.Orange);
                }
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

                    // Update UI on UI thread
                    DispatcherQueue.TryEnqueue(() =>
                    {
                        if (StatusTextBlock != null)
                        {
                            StatusTextBlock.Text = "Ready - All services initialized successfully";
                            StatusTextBlock.Foreground = new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.Green);
                        }
                    });
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
                    }

                    throw; // Re-throw to be handled by caller
                }
            });
        }

        private async void AddRecordButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_chartService == null)
                {
                    await InitializeServicesAsync();
                }

                if (StatusTextBlock != null)
                {
                    StatusTextBlock.Text = "Opening chart selection...";
                    StatusTextBlock.Foreground = new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.Blue);
                }

                var dialog = new ChartSelectionDialog();
                dialog.XamlRoot = this.XamlRoot;

                var result = await dialog.ShowAsync();

                if (result == ContentDialogResult.Primary)
                {
                    if (StatusTextBlock != null)
                    {
                        StatusTextBlock.Text = $"Record saved successfully! Directory: {dialog.SavedDirectory}";
                        StatusTextBlock.Foreground = new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.Green);
                    }

                    // Show directory automatically
                    if (!string.IsNullOrEmpty(dialog.SavedDirectory))
                    {
                        await ShowDirectoryAsync(dialog.SavedDirectory);
                    }

                    // Refresh history
                    await RefreshFileHistory();
                }
                else
                {
                    if (StatusTextBlock != null)
                    {
                        StatusTextBlock.Text = "Operation cancelled.";
                        StatusTextBlock.Foreground = new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.Orange);
                    }
                }
            }
            catch (Exception ex)
            {
                if (StatusTextBlock != null)
                {
                    StatusTextBlock.Text = $"Error: {ex.Message}";
                    StatusTextBlock.Foreground = new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.Red);
                }
            }
        }

        private async void LoadRecordButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_chartService == null)
                {
                    await InitializeServicesAsync();
                }

                if (StatusTextBlock != null)
                {
                    StatusTextBlock.Text = "Loading record...";
                    StatusTextBlock.Foreground = new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.Blue);
                }

                var result = await _chartService.LoadRecordAsync();

                if (result.Record != null)
                {
                    // Clear content panel
                    if (ContentPanel != null)
                    {
                        ContentPanel.Children.Clear();

                        // Show record info
                        var titleBlock = new TextBlock
                        {
                            Text = $"Loaded Record: {result.Record.RecordName}",
                            FontSize = 20,
                            FontWeight = Microsoft.UI.Text.FontWeights.SemiBold,
                            Margin = new Thickness(0, 0, 0, 15)
                        };
                        ContentPanel.Children.Add(titleBlock);

                        var dateBlock = new TextBlock
                        {
                            Text = $"Created: {result.Record.CreatedDate:yyyy-MM-dd HH:mm:ss}",
                            FontSize = 14,
                            Foreground = new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.Gray),
                            Margin = new Thickness(0, 0, 0, 10)
                        };
                        ContentPanel.Children.Add(dateBlock);

                        var directoryBlock = new TextBlock
                        {
                            Text = $"Directory: {result.Directory}",
                            FontSize = 14,
                            Foreground = new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.Gray),
                            Margin = new Thickness(0, 0, 0, 15)
                        };
                        ContentPanel.Children.Add(directoryBlock);

                        var chartsLabel = new TextBlock
                        {
                            Text = "Selected Charts:",
                            FontSize = 16,
                            FontWeight = Microsoft.UI.Text.FontWeights.SemiBold,
                            Margin = new Thickness(0, 0, 0, 10)
                        };
                        ContentPanel.Children.Add(chartsLabel);

                        // Show selected charts
                        var availableCharts = _chartService.GetAvailableCharts();
                        foreach (var chartId in result.Record.SelectedChartIds)
                        {
                            var chart = availableCharts.FirstOrDefault(c => c.Id == chartId);
                            if (chart != null)
                            {
                                var chartBlock = new TextBlock
                                {
                                    Text = $"• {chart.Name} - {chart.Description}",
                                    FontSize = 14,
                                    Margin = new Thickness(20, 2, 0, 2)
                                };
                                ContentPanel.Children.Add(chartBlock);
                            }
                        }
                    }

                    if (StatusTextBlock != null)
                    {
                        StatusTextBlock.Text = $"Record loaded successfully from: {result.Directory}";
                        StatusTextBlock.Foreground = new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.Green);
                    }

                    // Refresh history
                    await RefreshFileHistory();
                }
                else
                {
                    if (StatusTextBlock != null)
                    {
                        StatusTextBlock.Text = "No file selected or failed to load.";
                        StatusTextBlock.Foreground = new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.Orange);
                    }
                }
            }
            catch (Exception ex)
            {
                if (StatusTextBlock != null)
                {
                    StatusTextBlock.Text = $"Error loading record: {ex.Message}";
                    StatusTextBlock.Foreground = new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.Red);
                }
            }
        }

        private async void RefreshHistoryButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                await RefreshFileHistory();
                if (StatusTextBlock != null)
                {
                    StatusTextBlock.Text = "File history refreshed.";
                    StatusTextBlock.Foreground = new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.Green);
                }
            }
            catch (Exception ex)
            {
                if (StatusTextBlock != null)
                {
                    StatusTextBlock.Text = $"Error refreshing history: {ex.Message}";
                    StatusTextBlock.Foreground = new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.Red);
                }
            }
        }

        private async void ClearHistoryButton_Click(object sender, RoutedEventArgs e)
        {
            try
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
                    await _fileHistoryService.ClearHistoryAsync();
                    await RefreshFileHistory();
                    if (StatusTextBlock != null)
                    {
                        StatusTextBlock.Text = "File history cleared.";
                        StatusTextBlock.Foreground = new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.Green);
                    }
                }
            }
            catch (Exception ex)
            {
                if (StatusTextBlock != null)
                {
                    StatusTextBlock.Text = $"Error clearing history: {ex.Message}";
                    StatusTextBlock.Foreground = new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.Red);
                }
            }
        }

        private async void OpenDirectoryButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (FileHistoryListView?.SelectedItem is FileHistoryItem selectedItem)
                {
                    await ShowDirectoryAsync(selectedItem.Directory);
                }
            }
            catch (Exception ex)
            {
                if (StatusTextBlock != null)
                {
                    StatusTextBlock.Text = $"Error opening directory: {ex.Message}";
                    StatusTextBlock.Foreground = new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.Red);
                }
            }
        }

        private async void FileHistoryListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                var selectedItem = FileHistoryListView?.SelectedItem as FileHistoryItem;
                if (selectedItem != null)
                {
                    // menampilkan informasi seperti chartService.LoadRecordAsync();
                    var result = await _chartService.LoadRecordFromPathAsync(selectedItem.FilePath);
                    if (result.Record != null)
                    {
                        // Clear content panel
                        if (ContentPanel != null)
                        {
                            ContentPanel.Children.Clear();
                            // Show record info
                            var titleBlock = new TextBlock
                            {
                                Text = $"Loaded Record: {result.Record.RecordName}",
                                FontSize = 20,
                                FontWeight = Microsoft.UI.Text.FontWeights.SemiBold,
                                Margin = new Thickness(0, 0, 0, 15)
                            };
                            ContentPanel.Children.Add(titleBlock);
                            var dateBlock = new TextBlock
                            {
                                Text = $"Created: {result.Record.CreatedDate:yyyy-MM-dd HH:mm:ss}",
                                FontSize = 14,
                                Foreground = new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.Gray),
                                Margin = new Thickness(0, 0, 0, 10)
                            };
                            ContentPanel.Children.Add(dateBlock);
                            var directoryBlock = new TextBlock
                            {
                                Text = $"Directory: {result.Directory}",
                                FontSize = 14,
                                Foreground = new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.Gray),
                                Margin = new Thickness(0, 0, 0, 15)
                            };
                            ContentPanel.Children.Add(directoryBlock);
                            var chartsLabel = new TextBlock
                            {
                                Text = "Selected Charts:",
                                FontSize = 16,
                                FontWeight = Microsoft.UI.Text.FontWeights.SemiBold,
                                Margin = new Thickness(0, 0, 0, 10)
                            };
                            ContentPanel.Children.Add(chartsLabel);
                            // Show selected charts
                            var availableCharts = _chartService.GetAvailableCharts();
                            foreach (var chartId in result.Record.SelectedChartIds)
                            {
                                var chart = availableCharts.FirstOrDefault(c => c.Id == chartId);
                                if (chart != null)
                                {
                                    var chartBlock = new TextBlock
                                    {
                                        Text = $"• {chart.Name} - {chart.Description}",
                                        FontSize = 14,
                                        Margin = new Thickness(20, 2, 0, 2)
                                    };
                                    ContentPanel.Children.Add(chartBlock);
                                }
                            }
                        }
                        if (StatusTextBlock != null)
                        {
                            StatusTextBlock.Text = $"Record loaded successfully from: {result.Directory}";
                            StatusTextBlock.Foreground = new Microsoft.UI.Xaml.Media.SolidColorBrush
                                (Microsoft.UI.Colors.Green);
                        }

                        if (OpenDirectoryButton != null && FileHistoryListView != null)
                        {
                            OpenDirectoryButton.IsEnabled = FileHistoryListView.SelectedItem != null;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"FileHistoryListView_SelectionChanged error: {ex.Message}");
            }
        }

        private async void RemoveHistoryItem_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_fileHistoryService == null)
                {
                    await InitializeServicesAsync();
                }

                if (sender is Button button && button.Tag is FileHistoryItem item)
                {
                    await _fileHistoryService.RemoveHistoryItemAsync(item);
                    await RefreshFileHistory();
                    if (StatusTextBlock != null)
                    {
                        StatusTextBlock.Text = "File removed from history.";
                        StatusTextBlock.Foreground = new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.Green);
                    }
                }
            }
            catch (Exception ex)
            {
                if (StatusTextBlock != null)
                {
                    StatusTextBlock.Text = $"Error removing file: {ex.Message}";
                    StatusTextBlock.Foreground = new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.Red);
                }
            }
        }

        private async Task RefreshFileHistory()
        {
            try
            {
                if (_fileHistoryService != null)
                {
                    _currentHistoryItems = await _fileHistoryService.GetHistoryAsync();

                    if (FileHistoryListView != null)
                    {
                        FileHistoryListView.ItemsSource = _currentHistoryItems;
                    }

                    if (HistoryStatusTextBlock != null)
                    {
                        if (_currentHistoryItems.Count == 0)
                        {
                            HistoryStatusTextBlock.Text = "No files in history";
                            HistoryStatusTextBlock.Visibility = Visibility.Visible;
                        }
                        else
                        {
                            HistoryStatusTextBlock.Text = $"{_currentHistoryItems.Count} file(s) in history";
                            HistoryStatusTextBlock.Visibility = Visibility.Visible;
                        }
                    }
                }
                else
                {
                    if (HistoryStatusTextBlock != null)
                    {
                        HistoryStatusTextBlock.Text = "History service not available";
                        HistoryStatusTextBlock.Visibility = Visibility.Visible;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"RefreshFileHistory error: {ex.Message}");
                if (HistoryStatusTextBlock != null)
                {
                    HistoryStatusTextBlock.Text = $"Error loading history: {ex.Message}";
                    HistoryStatusTextBlock.Visibility = Visibility.Visible;
                }
            }
        }

        private async Task ShowDirectoryAsync(string directoryPath)
        {
            try
            {
                if (!string.IsNullOrEmpty(directoryPath))
                {
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = "explorer.exe",
                        Arguments = $"\"{directoryPath}\"",
                        UseShellExecute = true
                    });
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"ShowDirectoryAsync error: {ex.Message}");
                if (StatusTextBlock != null)
                {
                    StatusTextBlock.Text = $"Could not open directory: {ex.Message}";
                    StatusTextBlock.Foreground = new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.Red);
                }
            }
        }
    }
}
