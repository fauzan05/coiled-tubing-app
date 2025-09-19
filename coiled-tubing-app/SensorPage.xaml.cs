using coiled_tubing_app.Models;
using coiled_tubing_app.Services;
using coiled_tubing_app.ViewModels;
using LiveChartsCore.SkiaSharpView.WinUI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace coiled_tubing_app
{
    public sealed partial class SensorPage : Page
    {
        private ChartService? _chartService;
        private FileHistoryService? _fileHistoryService;
        private List<FileHistoryItem> _currentHistoryItems;
        private List<HistoryTableItem> _tableItems;
        private bool _isInChartView = false;
        private HistoryTableItem? _currentDetailItem;

        public SensorPage()
        {
            try
            {
                InitializeComponent();
                _currentHistoryItems = new List<FileHistoryItem>();
                _tableItems = new List<HistoryTableItem>();

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
                System.Diagnostics.Debug.WriteLine("SensorPage_Loaded: Starting initialization");
                System.Diagnostics.Debug.WriteLine($"SensorPage_Loaded: StatusTextBlock is null = {StatusTextBlock == null}");

                // Initialize services on UI thread
                await InitializeServicesAsync();
                await RefreshFileHistory();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"SensorPage_Loaded error: {ex.Message}");
                UpdateStatusText($"Warning: Some features may not work properly. {ex.Message}", Microsoft.UI.Colors.Orange);
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

                    // Update UI on UI thread using our helper method
                    DispatcherQueue.TryEnqueue(() =>
                    {
                        UpdateStatusText("Ready - All services initialized successfully", Microsoft.UI.Colors.Green);
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
                        _tableItems = new List<HistoryTableItem>();
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

                UpdateStatusText("Opening chart selection...", Microsoft.UI.Colors.Blue);

                var dialog = new ChartSelectionDialog();
                dialog.XamlRoot = this.XamlRoot;

                var result = await dialog.ShowAsync();

                System.Diagnostics.Debug.WriteLine($"AddRecordButton_Click: Dialog result = {result}");
                System.Diagnostics.Debug.WriteLine($"AddRecordButton_Click: SavedFilePath = '{dialog.SavedFilePath}'");
                System.Diagnostics.Debug.WriteLine($"AddRecordButton_Click: SavedDirectory = '{dialog.SavedDirectory}'");

                // Check if file was saved successfully regardless of dialog result
                bool fileWasSaved = !string.IsNullOrEmpty(dialog.SavedFilePath);

                if (fileWasSaved)
                {
                    UpdateStatusText("Record saved successfully! Refreshing history...", Microsoft.UI.Colors.Blue);

                    // Force refresh service instance to get latest data
                    _fileHistoryService = _chartService?.GetFileHistoryService();

                    // Refresh history list
                    await RefreshFileHistory();

                    // Update final status
                    UpdateStatusText($"Record saved successfully!", Microsoft.UI.Colors.Green);
                }
                else
                {
                    if (result == ContentDialogResult.Primary)
                    {
                        UpdateStatusText("Save operation failed. Please try again.", Microsoft.UI.Colors.Red);
                    }
                    else
                    {
                        UpdateStatusText("Operation cancelled.", Microsoft.UI.Colors.Orange);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"AddRecordButton_Click error: {ex.Message}");
                UpdateStatusText($"Error: {ex.Message}", Microsoft.UI.Colors.Red);
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

                UpdateStatusText("Loading record...", Microsoft.UI.Colors.Blue);

                var result = await _chartService.LoadRecordAsync();

                if (result.Record != null)
                {
                    UpdateStatusText($"Record loaded successfully from: {result.Directory}", Microsoft.UI.Colors.Green);
                    // Refresh history to show the loaded record
                    await RefreshFileHistory();
                }
                else
                {
                    UpdateStatusText("No file selected or failed to load.", Microsoft.UI.Colors.Orange);
                }
            }
            catch (Exception ex)
            {
                UpdateStatusText($"Error loading record: {ex.Message}", Microsoft.UI.Colors.Red);
            }
        }

        private async void RefreshHistoryButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                await RefreshFileHistory();
                UpdateStatusText("File history refreshed.", Microsoft.UI.Colors.Green);
            }
            catch (Exception ex)
            {
                UpdateStatusText($"Error refreshing history: {ex.Message}", Microsoft.UI.Colors.Red);
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
                    UpdateStatusText("File history cleared.", Microsoft.UI.Colors.Green);
                }
            }
            catch (Exception ex)
            {
                UpdateStatusText($"Error clearing history: {ex.Message}", Microsoft.UI.Colors.Red);
            }
        }

        private async void DeleteHistoryItem_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (sender is Button button && button.Tag is HistoryTableItem tableItem)
                {
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

                        await _fileHistoryService.RemoveHistoryItemAsync(tableItem.OriginalItem);
                        await RefreshFileHistory();
                        UpdateStatusText("Record removed from history.", Microsoft.UI.Colors.Green);
                    }
                }
            }
            catch (Exception ex)
            {
                UpdateStatusText($"Error removing record: {ex.Message}", Microsoft.UI.Colors.Red);
            }
        }

        private async void HistoryDataGrid_DoubleTapped(object sender, Microsoft.UI.Xaml.Input.DoubleTappedRoutedEventArgs e)
        {
            try
            {
                if (HistoryDataGrid.SelectedItem is HistoryTableItem selectedItem)
                {
                    await ShowChartTabView(selectedItem);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"HistoryDataGrid_DoubleTapped error: {ex.Message}");
                UpdateStatusText($"Error opening chart view: {ex.Message}", Microsoft.UI.Colors.Red);
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ShowTableView();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"BackButton_Click error: {ex.Message}");
            }
        }

        private async void GeneralDataButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_currentDetailItem == null)
                {
                    UpdateStatusText("No record selected.", Microsoft.UI.Colors.Orange);
                    return;
                }

                // Load current record to get existing general data
                GeneralData existingData = null;
                if (_chartService != null)
                {
                    var result = await _chartService.LoadRecordFromPathAsync(_currentDetailItem.FilePath);
                    if (result.Record != null)
                    {
                        existingData = result.Record.GeneralData;
                    }
                }

                // Show general data dialog
                var dialog = existingData != null
                    ? new GeneralDataDialog(existingData)
                    : new GeneralDataDialog();

                dialog.XamlRoot = this.XamlRoot;

                var dialogResult = await dialog.ShowAsync();

                if (dialogResult == ContentDialogResult.Primary)
                {
                    // Save the general data back to the record file
                    await SaveGeneralDataToRecord(dialog.GeneralData);
                    UpdateStatusText("General data saved successfully!", Microsoft.UI.Colors.Green);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"GeneralDataButton_Click error: {ex.Message}");
                UpdateStatusText($"Error opening general data form: {ex.Message}", Microsoft.UI.Colors.Red);
            }
        }

        private async void ConnectionButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                UpdateStatusText("Opening connection settings...", Microsoft.UI.Colors.Blue);

                var dialog = new ConnectionDialog();
                dialog.XamlRoot = this.XamlRoot;

                var result = await dialog.ShowAsync();

                if (result == ContentDialogResult.Primary)
                {
                    UpdateStatusText("Connection established successfully!", Microsoft.UI.Colors.Green);
                }
                else
                {
                    UpdateStatusText("Connection cancelled.", Microsoft.UI.Colors.Orange);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"ConnectionButton_Click error: {ex.Message}");
                UpdateStatusText($"Error opening connection dialog: {ex.Message}", Microsoft.UI.Colors.Red);
            }
        }

        private async Task SaveGeneralDataToRecord(GeneralData generalData)
        {
            try
            {
                if (_currentDetailItem == null || _chartService == null)
                    return;

                // Load current record
                var result = await _chartService.LoadRecordFromPathAsync(_currentDetailItem.FilePath);
                if (result.Record != null)
                {
                    // Update general data
                    result.Record.GeneralData = generalData;

                    // Save back to file using the existing path
                    await _chartService.SaveRecordToPathAsync(result.Record, _currentDetailItem.FilePath);

                    System.Diagnostics.Debug.WriteLine("General data saved successfully to record file");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"SaveGeneralDataToRecord error: {ex.Message}");
                throw;
            }
        }

        // Helper method yang lebih robust untuk update status
        private void UpdateStatusText(string message, Windows.UI.Color color)
        {
            try
            {
                // Try using DispatcherQueue to ensure we're on UI thread
                DispatcherQueue.TryEnqueue(() =>
                {
                    if (StatusTextBlock != null)
                    {
                        StatusTextBlock.Text = message;
                        StatusTextBlock.Foreground = new Microsoft.UI.Xaml.Media.SolidColorBrush(color);
                        System.Diagnostics.Debug.WriteLine($"UpdateStatusText: Successfully updated to '{message}'");
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine($"UpdateStatusText: StatusTextBlock is NULL, message was '{message}'");

                        // Try to find StatusTextBlock by name as fallback
                        var statusBlock = this.FindName("StatusTextBlock") as TextBlock;
                        if (statusBlock != null)
                        {
                            statusBlock.Text = message;
                            statusBlock.Foreground = new Microsoft.UI.Xaml.Media.SolidColorBrush(color);
                            System.Diagnostics.Debug.WriteLine($"UpdateStatusText: Found StatusTextBlock by name and updated");
                        }
                        else
                        {
                            System.Diagnostics.Debug.WriteLine($"UpdateStatusText: Could not find StatusTextBlock by name either");
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"UpdateStatusText error: {ex.Message}");
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
                var availableCharts = _chartService?.GetAvailableCharts() ?? new List<ChartItem>();

                for (int i = 0; i < _currentHistoryItems.Count; i++)
                {
                    var item = _currentHistoryItems[i];

                    // Load chart IDs from file to get preview
                    var chartIds = await GetChartIdsFromFile(item.FilePath);
                    var chartsPreview = GetChartsPreview(chartIds, availableCharts);

                    var tableItem = new HistoryTableItem
                    {
                        RowNumber = i + 1,
                        RecordName = item.RecordName,
                        ChartsPreview = chartsPreview,
                        LastAccessed = item.LastAccessed,
                        FilePath = item.FilePath,
                        Directory = item.Directory,
                        HistoryType = item.HistoryType,
                        SelectedChartIds = chartIds,
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

        private async Task<List<string>> GetChartIdsFromFile(string filePath)
        {
            try
            {
                if (_chartService != null)
                {
                    var result = await _chartService.LoadRecordFromPathAsync(filePath);
                    return result.Record?.SelectedChartIds ?? new List<string>();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"GetChartIdsFromFile error: {ex.Message}");
            }
            return new List<string>();
        }

        private string GetChartsPreview(List<string> chartIds, List<ChartItem> availableCharts)
        {
            if (chartIds.Count == 0)
                return "No charts selected";

            var chartNames = new List<string>();
            foreach (var id in chartIds.Take(2)) // Show max 2 charts in preview
            {
                var chart = availableCharts.FirstOrDefault(c => c.Id == id);
                if (chart != null)
                {
                    chartNames.Add(chart.Name);
                }
            }

            var preview = string.Join(", ", chartNames);
            if (chartIds.Count > 2)
            {
                preview += $" (+{chartIds.Count - 2} more)";
            }

            return preview;
        }

        private async Task ShowChartTabView(HistoryTableItem item)
        {
            try
            {
                _currentDetailItem = item;
                _isInChartView = true;

                // Load full record data
                if (_chartService != null)
                {
                    var result = await _chartService.LoadRecordFromPathAsync(item.FilePath);
                    if (result.Record != null)
                    {
                        await PopulateChartTabs(result.Record);
                    }
                }

                // Switch to chart view
                HistoryTableView.Visibility = Visibility.Collapsed;
                ActionButtonsPanel.Visibility = Visibility.Collapsed;
                ChartTabView.Visibility = Visibility.Visible;
                ActionButton_ChartTabView.Visibility = Visibility.Visible;
                SensorPage_TitleView.Visibility = Visibility.Collapsed;
                ColumnSensorPage_TitleView.Width = GridLength.Auto;
                ColumnSensorPage_ActionButtonView.Width = new GridLength(1, GridUnitType.Star);
                HistoryStatusTextBlockContainer.Visibility = Visibility.Collapsed;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"ShowChartTabView error: {ex.Message}");
                UpdateStatusText($"Error loading chart view: {ex.Message}", Microsoft.UI.Colors.Red);
            }
        }

        private void ShowTableView()
        {
            _isInChartView = false;
            _currentDetailItem = null;

            // Switch back to table view
            ChartTabView.Visibility = Visibility.Collapsed;
            ActionButton_ChartTabView.Visibility = Visibility.Collapsed;
            HistoryTableView.Visibility = Visibility.Visible;
            ActionButtonsPanel.Visibility = Visibility.Visible;
            SensorPage_TitleView.Visibility = Visibility.Visible;
            ColumnSensorPage_TitleView.Width = GridLength.Auto;
            ColumnSensorPage_ActionButtonView.Width = new GridLength(1, GridUnitType.Star);
            HistoryStatusTextBlockContainer.Visibility = Visibility.Visible;

            // Clear tabs
            ChartsTabView.TabItems.Clear();
        }

        private async Task PopulateChartTabs(ChartRecord record)
        {
            try
            {
                // Set record title
                if (RecordTitleTextBlock != null)
                {
                    RecordTitleTextBlock.Text = record.RecordName;
                }

                // Clear existing tabs
                ChartsTabView.TabItems.Clear();

                var availableCharts = _chartService?.GetAvailableCharts() ?? new List<ChartItem>();

                if (record.SelectedChartIds.Count == 0)
                {
                    // Create a "No Charts" tab
                    var noChartsTab = new TabViewItem
                    {
                        Header = "No Charts",
                        IsClosable = false,  // Disable close button
                        Content = new TextBlock
                        {
                            Text = "No charts selected for this record.",
                            FontSize = 16,
                            HorizontalAlignment = HorizontalAlignment.Center,
                            VerticalAlignment = VerticalAlignment.Center,
                        }
                    };
                    ChartsTabView.TabItems.Add(noChartsTab);
                }
                else
                {
                    // Create tabs for each selected chart
                    foreach (var chartId in record.SelectedChartIds)
                    {
                        var chart = availableCharts.FirstOrDefault(c => c.Id == chartId);
                        if (chart != null)
                        {
                            var chartViewModel = new ChartViewModel(chartId, chart.Name);
                            var chartTab = CreateChartTab(chart.Name, chartViewModel);
                            ChartsTabView.TabItems.Add(chartTab);
                        }
                    }
                }

                // Select first tab
                if (ChartsTabView.TabItems.Count > 0)
                {
                    ChartsTabView.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"PopulateChartTabs error: {ex.Message}");
            }
        }

        private TabViewItem CreateChartTab(string chartName, ChartViewModel viewModel)
        {
            var tab = new TabViewItem
            {
                Header = chartName,
                IsClosable = false  // Disable close button for this tab
            };

            // Create chart content based on chart type
            var chartContainer = new Border
            {
                CornerRadius = new CornerRadius(8),
                BorderBrush = new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.LightGray),
                BorderThickness = new Thickness(1),
                Padding = new Thickness(15),
                Margin = new Thickness(10)
            };

            var chartGrid = new Grid();
            chartGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            chartGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });

            // Chart title
            var titleBlock = new TextBlock
            {
                Text = chartName,
                FontSize = 18,
                FontWeight = Microsoft.UI.Text.FontWeights.Bold,
                Margin = new Thickness(0, 0, 0, 15),
                HorizontalAlignment = HorizontalAlignment.Center
            };
            Grid.SetRow(titleBlock, 0);
            chartGrid.Children.Add(titleBlock);

            // Create appropriate chart control based on chart type
            FrameworkElement chartControl;

            if (viewModel.ChartId == "sensor_status")
            {
                // Use PieChart for sensor status
                chartControl = new PieChart
                {
                    Series = viewModel.Series,
                    MinHeight = 300
                };
            }
            else
            {
                // Use CartesianChart for other chart types
                chartControl = new CartesianChart
                {
                    Series = viewModel.Series,
                    MinHeight = 300
                };
            }

            Grid.SetRow(chartControl, 1);
            chartGrid.Children.Add(chartControl);

            chartContainer.Child = chartGrid;
            tab.Content = chartContainer;

            return tab;
        }
    }
}
