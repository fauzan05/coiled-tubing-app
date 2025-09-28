using coiled_tubing_app.Models;
using coiled_tubing_app.Services;
using coiled_tubing_app.Helpers;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.IO;
using System.Linq;
using System.Text.Json;
// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace coiled_tubing_app
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class RecordingPage : Page
    {
        private FileHistoryItem? _currentRecord;

        // Default constructor required for XAML
        public RecordingPage()
        {
            InitializeComponent();
        }

        // Constructor with parameter (keep for backward compatibility)
        public RecordingPage(FileHistoryItem currentRecord)
        {
            InitializeComponent();
            _currentRecord = currentRecord;
        }

        // Handle navigation with parameters
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            // Handle parameter passed from Frame.Navigate
            if (e.Parameter is FileHistoryItem historyItem)
            {
                _currentRecord = historyItem;
                System.Diagnostics.Debug.WriteLine($"RecordingPage: Navigated with record: {_currentRecord?.RecordName}");
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("RecordingPage: Navigated without parameters or invalid parameter type");
                ContentDialog errorDialog = new ContentDialog
                {
                    Title = "Error",
                    Content = "No record information provided. Please select a record from the history.",
                    CloseButtonText = "OK",
                    XamlRoot = this.Content.XamlRoot
                };
                _currentRecord = null;
            }
        }

        private void ConnectDeviceButton_Click(object sender, RoutedEventArgs e)
        {
            Window connectDeviceWindow = new Window();
            connectDeviceWindow.AppWindow.Resize(new Windows.Graphics.SizeInt32(1500, 1500));
            connectDeviceWindow.Title = "Connect Device";

            // Main container with dark background
            Border mainBorder = new Border();
            mainBorder.Background = new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.Black);
            mainBorder.Padding = new Thickness(20);

            StackPanel content = new StackPanel();
            content.Spacing = 20;

            // Title with device info button
            StackPanel titlePanel = new StackPanel();
            titlePanel.Orientation = Orientation.Horizontal;
            titlePanel.Spacing = 15;

            TextBlock titleText = new TextBlock();
            titleText.Text = "Please choose device model";
            titleText.FontSize = 16;
            titleText.Foreground = new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.White);
            titleText.VerticalAlignment = VerticalAlignment.Center;

            Button deviceInfoButton = new Button();
            deviceInfoButton.Content = "Device Info";
            deviceInfoButton.Background = new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.DarkGray);
            deviceInfoButton.Foreground = new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.White);
            deviceInfoButton.FontSize = 12;
            deviceInfoButton.Padding = new Thickness(10, 5, 10, 5);
            deviceInfoButton.IsEnabled = false; // Initially disabled until device selected

            titlePanel.Children.Add(titleText);
            titlePanel.Children.Add(deviceInfoButton);
            content.Children.Add(titlePanel);

            // Form fields container
            StackPanel formPanel = new StackPanel();
            formPanel.Spacing = 15;

            // Device row
            Grid deviceGrid = new Grid();
            deviceGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(100) });
            deviceGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

            TextBlock deviceLabel = new TextBlock();
            deviceLabel.Text = "Device";
            deviceLabel.Foreground = new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.White);
            deviceLabel.VerticalAlignment = VerticalAlignment.Center;
            Grid.SetColumn(deviceLabel, 0);

            ComboBox deviceComboBox = new ComboBox();
            deviceComboBox.PlaceholderText = "Select device";
            deviceComboBox.Width = 300;
            deviceComboBox.HorizontalAlignment = HorizontalAlignment.Left;
            deviceComboBox.Background = new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.Transparent);
            deviceComboBox.BorderBrush = new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.White);
            Grid.SetColumn(deviceComboBox, 1);

            deviceGrid.Children.Add(deviceLabel);
            deviceGrid.Children.Add(deviceComboBox);
            formPanel.Children.Add(deviceGrid);

            // Host row
            Grid hostGrid = new Grid();
            hostGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(100) });
            hostGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

            TextBlock hostLabel = new TextBlock();
            hostLabel.Text = "Host";
            hostLabel.Foreground = new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.White);
            hostLabel.VerticalAlignment = VerticalAlignment.Center;
            Grid.SetColumn(hostLabel, 0);

            TextBox hostTextBox = new TextBox();
            hostTextBox.Width = 300;
            hostTextBox.HorizontalAlignment = HorizontalAlignment.Left;
            hostTextBox.Background = new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.Transparent);
            hostTextBox.BorderBrush = new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.White);
            hostTextBox.Foreground = new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.White);
            Grid.SetColumn(hostTextBox, 1);

            hostGrid.Children.Add(hostLabel);
            hostGrid.Children.Add(hostTextBox);
            formPanel.Children.Add(hostGrid);

            // Port row
            Grid portGrid = new Grid();
            portGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(100) });
            portGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

            TextBlock portLabel = new TextBlock();
            portLabel.Text = "Port";
            portLabel.Foreground = new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.White);
            portLabel.VerticalAlignment = VerticalAlignment.Center;
            Grid.SetColumn(portLabel, 0);

            TextBox portTextBox = new TextBox();
            portTextBox.Width = 100;
            portTextBox.HorizontalAlignment = HorizontalAlignment.Left;
            portTextBox.Background = new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.Transparent);
            portTextBox.BorderBrush = new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.White);
            portTextBox.Foreground = new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.White);
            Grid.SetColumn(portTextBox, 1);

            portGrid.Children.Add(portLabel);
            portGrid.Children.Add(portTextBox);
            formPanel.Children.Add(portGrid);

            // Timeout row
            Grid timeoutGrid = new Grid();
            timeoutGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(100) });
            timeoutGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(100) });
            timeoutGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(30) });

            TextBlock timeoutLabel = new TextBlock();
            timeoutLabel.Text = "Timeout";
            timeoutLabel.Foreground = new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.White);
            timeoutLabel.VerticalAlignment = VerticalAlignment.Center;
            Grid.SetColumn(timeoutLabel, 0);

            TextBox timeoutTextBox = new TextBox();
            timeoutTextBox.Width = 100;
            timeoutTextBox.HorizontalAlignment = HorizontalAlignment.Left;
            timeoutTextBox.Background = new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.Transparent);
            timeoutTextBox.BorderBrush = new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.White);
            timeoutTextBox.Foreground = new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.White);
            Grid.SetColumn(timeoutTextBox, 1);

            TextBlock msLabel = new TextBlock();
            msLabel.Text = "ms";
            msLabel.Foreground = new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.White);
            msLabel.VerticalAlignment = VerticalAlignment.Center;
            msLabel.Margin = new Thickness(10, 0, 0, 0);
            Grid.SetColumn(msLabel, 2);

            timeoutGrid.Children.Add(timeoutLabel);
            timeoutGrid.Children.Add(timeoutTextBox);
            timeoutGrid.Children.Add(msLabel);
            formPanel.Children.Add(timeoutGrid);

            content.Children.Add(formPanel);

            // Populate device ComboBox and load existing settings
            int selectedDeviceId = 0;
            System.Diagnostics.Debug.WriteLine($"ConnectDeviceButton_Click: Current record: {_currentRecord?.RecordName}");
            
            if (_contentLoaded && _currentRecord != null && File.Exists(_currentRecord.FilePath))
            {
                string jsonContent = File.ReadAllText(_currentRecord.FilePath);
                var record = JsonSerializer.Deserialize<ItemRecord>(jsonContent);
                if (record != null && record.ConnectionSettings != null)
                {
                    System.Diagnostics.Debug.WriteLine($"ConnectDeviceButton_Click: Loaded connection settings for record: {_currentRecord.RecordName}");
                    
                    // Get device ID (prioritize DeviceId over Device string)
                    selectedDeviceId = record.ConnectionSettings.DeviceId > 0 
                        ? record.ConnectionSettings.DeviceId 
                        : GetDeviceIdFromName(record.ConnectionSettings.Device);
                    
                    // Pre-fill other form fields
                    hostTextBox.Text = record.ConnectionSettings.Host;
                    portTextBox.Text = record.ConnectionSettings.Port.ToString();
                    timeoutTextBox.Text = record.ConnectionSettings.Timeout.ToString();
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"ConnectDeviceButton_Click: No connection settings found in record: {_currentRecord.RecordName}");
                }
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("ConnectDeviceButton_Click: No current record or file does not exist");
            }

            // Populate device ComboBox with selected device
            DeviceManagementHelper.PopulateDeviceComboBox(deviceComboBox, selectedDeviceId);

            // Device selection changed event handler
            deviceComboBox.SelectionChanged += (s, args) =>
            {
                var selectedDeviceId = DeviceManagementHelper.GetSelectedDeviceId(deviceComboBox);
                
                // Enable/disable device info button
                deviceInfoButton.IsEnabled = selectedDeviceId > 0;
                
                // Apply device-specific defaults
                if (selectedDeviceId > 0)
                {
                    DeviceManagementHelper.ApplyDeviceDefaults(deviceComboBox, hostTextBox, portTextBox, timeoutTextBox);
                }
            };

            // Initially check if we have a selected device
            if (selectedDeviceId > 0)
            {
                deviceInfoButton.IsEnabled = true;
            }

            // Device Info button click handler
            deviceInfoButton.Click += (s, args) => 
            {
                var currentDeviceId = DeviceManagementHelper.GetSelectedDeviceId(deviceComboBox);
                if (currentDeviceId > 0)
                {
                    DeviceManagementHelper.ShowDeviceInfo(connectDeviceWindow.Content.XamlRoot, currentDeviceId);
                }
            };

            // Connect button
            Button connectButton = new Button();
            connectButton.Content = "Connect";
            connectButton.HorizontalAlignment = HorizontalAlignment.Center;
            connectButton.Margin = new Thickness(0, 10, 0, 0);
            connectButton.Padding = new Thickness(30, 8, 30, 8);
            connectButton.Background = new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.Transparent);
            connectButton.BorderBrush = new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.White);
            connectButton.Foreground = new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.White);

            content.Children.Add(connectButton);

            // Loading indicator - positioned right below Connect button
            StackPanel loadingPanel = new StackPanel();
            loadingPanel.Orientation = Orientation.Horizontal;
            loadingPanel.HorizontalAlignment = HorizontalAlignment.Center;
            loadingPanel.VerticalAlignment = VerticalAlignment.Center;
            loadingPanel.Visibility = Visibility.Collapsed;
            loadingPanel.Margin = new Thickness(0, 10, 0, 10);
            
            ProgressRing progressRing = new ProgressRing();
            progressRing.IsActive = false;
            progressRing.Width = 25;
            progressRing.Height = 25;
            progressRing.Margin = new Thickness(0, 0, 8, 0);
            
            TextBlock loadingText = new TextBlock();
            loadingText.Text = "Connecting...";
            loadingText.Foreground = new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.White);
            loadingText.VerticalAlignment = VerticalAlignment.Center;
            loadingText.FontSize = 12;
            
            loadingPanel.Children.Add(progressRing);
            loadingPanel.Children.Add(loadingText);
            content.Children.Add(loadingPanel);

            // Device Information Log
            Border logBorder = new Border();
            logBorder.BorderBrush = new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.White);
            logBorder.BorderThickness = new Thickness(1);
            logBorder.Height = 150;
            logBorder.Margin = new Thickness(0, 10, 0, 0);

            ScrollViewer logScrollViewer = new ScrollViewer();
            TextBlock logTextBlock = new TextBlock();
            logTextBlock.Name = "DeviceLogTextBlock";
            logTextBlock.Text = "Device Information Log\nReady for connection...";
            logTextBlock.Foreground = new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.White);
            logTextBlock.Padding = new Thickness(10);
            logTextBlock.VerticalAlignment = VerticalAlignment.Top;
            logTextBlock.TextWrapping = TextWrapping.Wrap;

            logScrollViewer.Content = logTextBlock;
            logBorder.Child = logScrollViewer;
            content.Children.Add(logBorder);

            // Connect button click handler - Updated to pass UI references
            connectButton.Click += async (s, args) => {
                var connectionSettings = DeviceManagementHelper.CreateConnectionSettings(
                    deviceComboBox, hostTextBox, portTextBox, timeoutTextBox);
                
                // Disable connect button during connection
                connectButton.IsEnabled = false;
                
                // Show loading and update log
                SetConnectionLoading(loadingPanel, progressRing, true);
                LogToDeviceLog(logTextBlock, $"Attempting to connect to {connectionSettings.Host}:{connectionSettings.Port}...");
                
                try
                {
                    await ConnectDeviceWithUI(connectDeviceWindow, connectionSettings, logTextBlock, loadingPanel, progressRing);
                }
                finally
                {
                    // Re-enable connect button
                    connectButton.IsEnabled = true;
                }
            };

            // Bottom buttons
            Grid bottomButtonGrid = new Grid();
            bottomButtonGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            bottomButtonGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            bottomButtonGrid.Margin = new Thickness(0, 20, 0, 0);

            Button saveButton = new Button();
            saveButton.Content = "Save";
            saveButton.Padding = new Thickness(30, 8, 30, 8);
            saveButton.Background = new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.Transparent);
            saveButton.BorderBrush = new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.White);
            saveButton.Foreground = new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.White);
            saveButton.HorizontalAlignment = HorizontalAlignment.Left;
            saveButton.Click += (s, args) => {
                var connectionSettings = DeviceManagementHelper.CreateConnectionSettings(
                    deviceComboBox, hostTextBox, portTextBox, timeoutTextBox);
                SaveConnectionSettings(connectDeviceWindow, connectionSettings, _currentRecord);
            };
            Grid.SetColumn(saveButton, 0);

            Button cancelButton = new Button();
            cancelButton.Content = "Cancel";
            cancelButton.Padding = new Thickness(30, 8, 30, 8);
            cancelButton.Background = new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.Transparent);
            cancelButton.BorderBrush = new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.White);
            cancelButton.Foreground = new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.White);
            cancelButton.HorizontalAlignment = HorizontalAlignment.Right;
            cancelButton.Click += (s, args) => connectDeviceWindow.Close();
            Grid.SetColumn(cancelButton, 1);

            bottomButtonGrid.Children.Add(saveButton);
            bottomButtonGrid.Children.Add(cancelButton);
            content.Children.Add(bottomButtonGrid);

            mainBorder.Child = content;
            connectDeviceWindow.Content = mainBorder;
            connectDeviceWindow.Activate();
        }

        /// <summary>
        /// Show/hide loading animation - positioned below Connect button
        /// </summary>
        private void SetConnectionLoading(StackPanel loadingPanel, ProgressRing progressRing, bool isLoading)
        {
            if (isLoading)
            {
                loadingPanel.Visibility = Visibility.Visible;
                progressRing.IsActive = true;
            }
            else
            {
                loadingPanel.Visibility = Visibility.Collapsed;
                progressRing.IsActive = false;
            }
        }

        /// <summary>
        /// Append message to device log with timestamp
        /// </summary>
        private void LogToDeviceLog(TextBlock logTextBlock, string message)
        {
            var timestamp = DateTime.Now.ToString("HH:mm:ss");
            var logEntry = $"[{timestamp}] {message}";
            
            // Append to existing text
            if (string.IsNullOrEmpty(logTextBlock.Text) || 
                logTextBlock.Text == "Device Information Log" || 
                logTextBlock.Text == "Device Information Log\nReady for connection...")
            {
                logTextBlock.Text = $"Device Information Log\n{logEntry}";
            }
            else
            {
                logTextBlock.Text += $"\n{logEntry}";
            }
            
            // Keep log manageable (max 15 lines to fit in smaller area)
            var lines = logTextBlock.Text.Split('\n');
            if (lines.Length > 16) // 15 + header line
            {
                var recentLines = new string[16];
                recentLines[0] = lines[0]; // Keep header
                Array.Copy(lines, lines.Length - 15, recentLines, 1, 15); // Keep last 15 log entries
                logTextBlock.Text = string.Join('\n', recentLines);
            }
            
            System.Diagnostics.Debug.WriteLine($"Device Log: {logEntry}");
        }

        /// <summary>
        /// Helper method to get device ID from device name for backward compatibility
        /// </summary>
        private int GetDeviceIdFromName(string deviceName)
        {
            if (string.IsNullOrEmpty(deviceName)) return 0;
            
            var device = DeviceService.GetDeviceByName(deviceName);
            return device?.Id ?? 0;
        }

        private void GeneralDataButton_Click(object sender, RoutedEventArgs e)
        {
            Window generalDataWindow = new Window();
            generalDataWindow.Title = "General Data";

            StackPanel content = new StackPanel();
            content.Margin = new Thickness(20);
            content.Spacing = 15;

            TextBlock contentText = new TextBlock();
            contentText.Text = "General Data functionality will be implemented here.";
            contentText.FontSize = 14;
            content.Children.Add(contentText);

            Button okButton = new Button();
            okButton.Content = "OK";
            okButton.HorizontalAlignment = HorizontalAlignment.Right;
            okButton.Click += (s, args) => generalDataWindow.Close();
            content.Children.Add(okButton);

            generalDataWindow.Content = content;
            generalDataWindow.Activate();
        }

        private void IODeviceButton_Click(object sender, RoutedEventArgs e)
        {
            Window ioDeviceWindow = new Window();
            ioDeviceWindow.Title = "I/O Device";

            StackPanel content = new StackPanel();
            content.Margin = new Thickness(20);
            content.Spacing = 15;

            TextBlock contentText = new TextBlock();
            contentText.Text = "I/O Device functionality will be implemented here.";
            contentText.FontSize = 14;
            content.Children.Add(contentText);

            Button okButton = new Button();
            okButton.Content = "OK";
            okButton.HorizontalAlignment = HorizontalAlignment.Right;
            okButton.Click += (s, args) => ioDeviceWindow.Close();
            content.Children.Add(okButton);

            ioDeviceWindow.Content = content;
            ioDeviceWindow.Activate();
        }

        private void SetupButton_Click(object sender, RoutedEventArgs e)
        {
            Window setupWindow = new Window();
            setupWindow.Title = "Setup";

            StackPanel content = new StackPanel();
            content.Margin = new Thickness(20);
            content.Spacing = 15;

            TextBlock contentText = new TextBlock();
            contentText.Text = "Setup functionality will be implemented here.";
            contentText.FontSize = 14;
            content.Children.Add(contentText);

            Button okButton = new Button();
            okButton.Content = "OK";
            okButton.HorizontalAlignment = HorizontalAlignment.Right;
            okButton.Click += (s, args) => setupWindow.Close();
            content.Children.Add(okButton);

            setupWindow.Content = content;
            setupWindow.Activate();
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            Window backWindow = new Window();
            backWindow.Title = "Back";

            StackPanel content = new StackPanel();
            content.Margin = new Thickness(20);
            content.Spacing = 15;

            TextBlock contentText = new TextBlock();
            contentText.Text = "Back navigation will be implemented here.";
            contentText.FontSize = 14;
            content.Children.Add(contentText);

            Button okButton = new Button();
            okButton.Content = "OK";
            okButton.HorizontalAlignment = HorizontalAlignment.Right;
            okButton.Click += (s, args) => backWindow.Close();
            content.Children.Add(okButton);

            backWindow.Content = content;
            backWindow.Activate();
        }

        private void RecordButton_Click(object sender, RoutedEventArgs e)
        {
            Window recordWindow = new Window();
            recordWindow.Title = "Record";

            StackPanel content = new StackPanel();
            content.Margin = new Thickness(20);
            content.Spacing = 15;

            TextBlock contentText = new TextBlock();
            contentText.Text = "Recording started.";
            contentText.FontSize = 14;
            content.Children.Add(contentText);

            Button okButton = new Button();
            okButton.Content = "OK";
            okButton.HorizontalAlignment = HorizontalAlignment.Right;
            okButton.Click += (s, args) => recordWindow.Close();
            content.Children.Add(okButton);

            recordWindow.Content = content;
            recordWindow.Activate();
        }

        private void StopRecordButton_Click(object sender, RoutedEventArgs e)
        {
            Window stopRecordWindow = new Window();
            stopRecordWindow.Title = "Stop Record";

            StackPanel content = new StackPanel();
            content.Margin = new Thickness(20);
            content.Spacing = 15;

            TextBlock contentText = new TextBlock();
            contentText.Text = "Recording stopped.";
            contentText.FontSize = 14;
            content.Children.Add(contentText);

            Button okButton = new Button();
            okButton.Content = "OK";
            okButton.HorizontalAlignment = HorizontalAlignment.Right;
            okButton.Click += (s, args) => stopRecordWindow.Close();
            content.Children.Add(okButton);

            stopRecordWindow.Content = content;
            stopRecordWindow.Activate();
        }

        private void RunButton_Click(object sender, RoutedEventArgs e)
        {
            Window runWindow = new Window();
            runWindow.Title = "Run";

            StackPanel content = new StackPanel();
            content.Margin = new Thickness(20);
            content.Spacing = 15;

            TextBlock contentText = new TextBlock();
            contentText.Text = "Process started running.";
            contentText.FontSize = 14;
            content.Children.Add(contentText);

            Button okButton = new Button();
            okButton.Content = "OK";
            okButton.HorizontalAlignment = HorizontalAlignment.Right;
            okButton.Click += (s, args) => runWindow.Close();
            content.Children.Add(okButton);

            runWindow.Content = content;
            runWindow.Activate();
        }

        private void StopRunButton_Click(object sender, RoutedEventArgs e)
        {
            Window stopRunWindow = new Window();
            stopRunWindow.Title = "Stop Run";

            StackPanel content = new StackPanel();
            content.Margin = new Thickness(20);
            content.Spacing = 15;

            TextBlock contentText = new TextBlock();
            contentText.Text = "Process stopped running.";
            contentText.FontSize = 14;
            content.Children.Add(contentText);

            Button okButton = new Button();
            okButton.Content = "OK";
            okButton.HorizontalAlignment = HorizontalAlignment.Right;
            okButton.Click += (s, args) => stopRunWindow.Close();
            content.Children.Add(okButton);

            stopRunWindow.Content = content;
            stopRunWindow.Activate();
        }

        private void ShowInTableButton_Click(object sender, RoutedEventArgs e)
        {
            Window showTableWindow = new Window();
            showTableWindow.Title = "Show in Table";

            StackPanel content = new StackPanel();
            content.Margin = new Thickness(20);
            content.Spacing = 15;

            TextBlock contentText = new TextBlock();
            contentText.Text = "Show in Table functionality will be implemented here.";
            contentText.FontSize = 14;
            content.Children.Add(contentText);

            Button okButton = new Button();
            okButton.Content = "OK";
            okButton.HorizontalAlignment = HorizontalAlignment.Right;
            okButton.Click += (s, args) => showTableWindow.Close();
            content.Children.Add(okButton);

            showTableWindow.Content = content;
            showTableWindow.Activate();
        }

        private void ExportToCSVButton_Click(object sender, RoutedEventArgs e)
        {
            Window exportCSVWindow = new Window();
            exportCSVWindow.Title = "Export to CSV";

            StackPanel content = new StackPanel();
            content.Margin = new Thickness(20);
            content.Spacing = 15;

            TextBlock contentText = new TextBlock();
            contentText.Text = "Export to CSV functionality will be implemented here.";
            contentText.FontSize = 14;
            content.Children.Add(contentText);

            Button okButton = new Button();
            okButton.Content = "OK";
            okButton.HorizontalAlignment = HorizontalAlignment.Right;
            okButton.Click += (s, args) => exportCSVWindow.Close();
            content.Children.Add(okButton);

            exportCSVWindow.Content = content;
            exportCSVWindow.Activate();
        }

        private void ExportToPDFButton_Click(object sender, RoutedEventArgs e)
        {
            Window exportPDFWindow = new Window();
            exportPDFWindow.Title = "Export to PDF";

            StackPanel content = new StackPanel();
            content.Margin = new Thickness(20);
            content.Spacing = 15;

            TextBlock contentText = new TextBlock();
            contentText.Text = "Export to PDF functionality will be implemented here.";
            contentText.FontSize = 14;
            content.Children.Add(contentText);

            Button okButton = new Button();
            okButton.Content = "OK";
            okButton.HorizontalAlignment = HorizontalAlignment.Right;
            okButton.Click += (s, args) => exportPDFWindow.Close();
            content.Children.Add(okButton);

            exportPDFWindow.Content = content;
            exportPDFWindow.Activate();
        }
    }
}
