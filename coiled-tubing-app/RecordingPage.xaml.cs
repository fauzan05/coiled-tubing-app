using coiled_tubing_app.Models;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
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

            // Title
            TextBlock titleText = new TextBlock();
            titleText.Text = "Please choose device model";
            titleText.FontSize = 16;
            titleText.Foreground = new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.White);
            content.Children.Add(titleText);

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
            deviceComboBox.Items.Add(new ComboBoxItem { Content = "Opto 22 - SNAP UP1 ADS" });
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

            // Device Information Log
            Border logBorder = new Border();
            logBorder.BorderBrush = new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.White);
            logBorder.BorderThickness = new Thickness(1);
            logBorder.Height = 150;
            logBorder.Margin = new Thickness(0, 20, 0, 0);

            ScrollViewer logScrollViewer = new ScrollViewer();
            TextBlock logTextBlock = new TextBlock();
            logTextBlock.Text = "Device Information Log";
            logTextBlock.Foreground = new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.White);
            logTextBlock.Padding = new Thickness(10);
            logTextBlock.VerticalAlignment = VerticalAlignment.Top;

            logScrollViewer.Content = logTextBlock;
            logBorder.Child = logScrollViewer;
            content.Children.Add(logBorder);

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
            saveButton.Click += (s, args) => SaveConnectionSettings(connectDeviceWindow, new Models.ConnectionSettings
            {
                Device = (deviceComboBox.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "",
                Host = hostTextBox.Text,
                Port = int.TryParse(portTextBox.Text, out int port) ? port : 0,
                Timeout = int.TryParse(timeoutTextBox.Text, out int timeout) ? timeout : 0
            },
            _currentRecord
            );
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

            // Temukan apakah ada connection settings yang tersimpan untuk current record
            System.Diagnostics.Debug.WriteLine($"ConnectDeviceButton_Click: Current record: {_currentRecord?.RecordName}");
            // Jika ada, load dan pre-fill form
            if (_contentLoaded && _currentRecord != null && File.Exists(_currentRecord.FilePath))
            {
                string jsonContent = File.ReadAllText(_currentRecord.FilePath);
                var record = JsonSerializer.Deserialize<ItemRecord>(jsonContent);
                if (record != null && record.ConnectionSettings != null)
                {
                    System.Diagnostics.Debug.WriteLine($"ConnectDeviceButton_Click: Loaded connection settings for record: {_currentRecord.RecordName}");
                    // Pre-fill form fields here if needed
                    deviceComboBox.SelectedItem = record.ConnectionSettings.Device != string.Empty ?
                        deviceComboBox.Items.Cast<ComboBoxItem>().FirstOrDefault(item => item.Content.ToString() == record.ConnectionSettings.Device) : null;
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

            mainBorder.Child = content;
            connectDeviceWindow.Content = mainBorder;
            connectDeviceWindow.Activate();
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
