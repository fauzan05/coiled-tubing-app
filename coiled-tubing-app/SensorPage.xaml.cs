using coiled_tubing_app.Models;
using coiled_tubing_app.Services;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Linq;

namespace coiled_tubing_app
{
    public sealed partial class SensorPage : Page
    {
        private readonly ChartService _chartService;

        public SensorPage()
        {
            InitializeComponent();
            _chartService = new ChartService();
        }

        private async void AddRecordButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                StatusTextBlock.Text = "Opening chart selection...";
                StatusTextBlock.Foreground = new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.Blue);

                var dialog = new ChartSelectionDialog();
                dialog.XamlRoot = this.XamlRoot;
                
                var result = await dialog.ShowAsync();
                
                if (result == ContentDialogResult.Primary)
                {
                    StatusTextBlock.Text = "Record saved successfully!";
                    StatusTextBlock.Foreground = new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.Green);
                }
                else
                {
                    StatusTextBlock.Text = "Operation cancelled.";
                    StatusTextBlock.Foreground = new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.Orange);
                }
            }
            catch (Exception ex)
            {
                StatusTextBlock.Text = $"Error: {ex.Message}";
                StatusTextBlock.Foreground = new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.Red);
            }
        }

        private async void LoadRecordButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                StatusTextBlock.Text = "Loading record...";
                StatusTextBlock.Foreground = new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.Blue);

                var record = await _chartService.LoadRecordAsync();
                
                if (record != null)
                {
                    // Clear content panel
                    ContentPanel.Children.Clear();
                    
                    // Show record info
                    var titleBlock = new TextBlock
                    {
                        Text = $"Loaded Record: {record.RecordName}",
                        FontSize = 20,
                        FontWeight = Microsoft.UI.Text.FontWeights.SemiBold,
                        Margin = new Thickness(0, 0, 0, 15)
                    };
                    ContentPanel.Children.Add(titleBlock);

                    var dateBlock = new TextBlock
                    {
                        Text = $"Created: {record.CreatedDate:yyyy-MM-dd HH:mm:ss}",
                        FontSize = 14,
                        Foreground = new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.Gray),
                        Margin = new Thickness(0, 0, 0, 15)
                    };
                    ContentPanel.Children.Add(dateBlock);

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
                    foreach (var chartId in record.SelectedChartIds)
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

                    StatusTextBlock.Text = "Record loaded successfully.";
                    StatusTextBlock.Foreground = new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.Green);
                }
                else
                {
                    StatusTextBlock.Text = "No file selected or failed to load.";
                    StatusTextBlock.Foreground = new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.Orange);
                }
            }
            catch (Exception ex)
            {
                StatusTextBlock.Text = $"Error loading record: {ex.Message}";
                StatusTextBlock.Foreground = new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.Red);
            }
        }
    }
}
