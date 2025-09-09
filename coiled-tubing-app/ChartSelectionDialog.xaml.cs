using coiled_tubing_app.Models;
using coiled_tubing_app.Services;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using System;
using System.Collections.Generic;
using System.Linq;

namespace coiled_tubing_app
{
    // ContentDialog dibuat sepenuhnya di code-behind
    public sealed class ChartSelectionDialog : ContentDialog
    {
        private readonly ChartService _chartService;
        private readonly List<ChartItem> _charts;
        private readonly TextBox _recordNameTextBox;
        private readonly StackPanel _chartsPanel;

        public string SavedFilePath { get; private set; } = "";
        public string SavedDirectory { get; private set; } = "";

        public ChartSelectionDialog()
        {
            _chartService = new ChartService();
            _charts = _chartService.GetAvailableCharts();
            
            // Setup dialog properties
            this.Title = "Select Charts";
            this.PrimaryButtonText = "Save";
            this.SecondaryButtonText = "Cancel";
            
            // Create UI elements
            var mainGrid = new Grid
            {
                Width = 500,
                Height = 400
            };
            
            // Add row definitions
            mainGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            mainGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            mainGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });

            // Record name input
            var namePanel = new StackPanel
            {
                Margin = new Thickness(0, 0, 0, 20)
            };
            
            var nameLabel = new TextBlock
            {
                Text = "Record Name:",
                FontWeight = Microsoft.UI.Text.FontWeights.SemiBold,
                Margin = new Thickness(0, 0, 0, 5)
            };
            
            _recordNameTextBox = new TextBox
            {
                PlaceholderText = "Enter record name..."
            };
            
            namePanel.Children.Add(nameLabel);
            namePanel.Children.Add(_recordNameTextBox);
            Grid.SetRow(namePanel, 0);

            // Charts title
            var chartsTitle = new TextBlock
            {
                Text = "Select Charts:",
                FontWeight = Microsoft.UI.Text.FontWeights.SemiBold,
                Margin = new Thickness(0, 0, 0, 10)
            };
            Grid.SetRow(chartsTitle, 1);

            // Charts list in scrollviewer
            var scrollViewer = new ScrollViewer
            {
                BorderBrush = new SolidColorBrush(Microsoft.UI.Colors.LightGray),
                BorderThickness = new Thickness(1),
                Padding = new Thickness(10)
            };
            
            _chartsPanel = new StackPanel();
            scrollViewer.Content = _chartsPanel;
            Grid.SetRow(scrollViewer, 2);

            // Add elements to grid
            mainGrid.Children.Add(namePanel);
            mainGrid.Children.Add(chartsTitle);
            mainGrid.Children.Add(scrollViewer);
            
            // Set content
            this.Content = mainGrid;
            
            LoadCharts();
            
            // Event handler untuk tombol save
            this.PrimaryButtonClick += OnSaveButtonClick;
        }

        private void LoadCharts()
        {
            // Buat checkbox untuk setiap chart
            foreach (var chart in _charts)
            {
                var checkBox = new CheckBox
                {
                    Content = $"{chart.Name} - {chart.Description}",
                    Tag = chart.Id,
                    Margin = new Thickness(0, 5, 0, 5)
                };
                
                _chartsPanel.Children.Add(checkBox);
            }
        }

        private async void OnSaveButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            // Validasi nama record
            string recordName = _recordNameTextBox.Text?.Trim() ?? "";
            if (string.IsNullOrEmpty(recordName))
            {
                args.Cancel = true;
                return;
            }

            // Ambil chart yang dipilih
            var selectedChartIds = new List<string>();
            foreach (var child in _chartsPanel.Children)
            {
                if (child is CheckBox checkBox && checkBox.IsChecked == true && checkBox.Tag != null)
                {
                    selectedChartIds.Add(checkBox.Tag.ToString()!);
                }
            }

            if (selectedChartIds.Count == 0)
            {
                args.Cancel = true;
                return;
            }

            // Buat record
            var record = new ChartRecord
            {
                RecordName = recordName,
                SelectedChartIds = selectedChartIds,
                CreatedDate = DateTime.Now
            };

            // Simpan file dengan return value yang baru
            var result = await _chartService.SaveRecordAsync(record);
            if (!result.Success)
            {
                args.Cancel = true;
            }
            else
            {
                SavedFilePath = result.FilePath;
                SavedDirectory = result.Directory;
            }
        }
    }
}