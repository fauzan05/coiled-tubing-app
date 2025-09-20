using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using OptoMMP6;
using System;
using System.Threading.Tasks;
using System.Text;


namespace coiled_tubing_app
{
    public sealed class ConnectionDialog : ContentDialog
    {
        internal static OptoMMP mmp = new OptoMMP();

        // Input fields
        private readonly TextBox _hostTextBox;
        private readonly TextBox _portTextBox;
        private readonly TextBox _timeoutTextBox;

        // UI elements for loading and results
        private readonly Button _buttonFindDevice;
        private readonly ProgressRing _loadingProgressRing;
        private readonly TextBlock _loadingTextBlock;
        private readonly TextBlock _resultTextBlock;
        private readonly TextBlock _resultDataAnalog;
        private readonly StackPanel _resultPanel;
        private readonly FontIcon _resultIcon;

        // Add debug info display
        private readonly TextBlock _debugInfoTextBlock;
        // Realtime polling
        private readonly DispatcherTimer _pollTimer;
        private bool _isReading = false;

        private int i32Result;

        public ConnectionDialog()
        {
            this.Title = "Connection Settings";
            this.PrimaryButtonText = "Save";
            this.SecondaryButtonText = "Cancel";

            // ScrollViewer for the dialog content
            var scrollViewer = new ScrollViewer
            {
                Height = double.NaN, // Auto height
                VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
                HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled,
                Margin = new Thickness(5),
                Padding = new Thickness(5),
                HorizontalAlignment = HorizontalAlignment.Stretch,
                Width = 400,
            };

            var mainPanel = new StackPanel
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                Width = 350,
                Height = double.NaN, // Auto height
                Spacing = 10
            };

            // Add placeholder content with icon
            var headerPanel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(20, 10, 20, 20),
                Spacing = 10
            };

            var headerIcon = new FontIcon
            {
                FontFamily = new Microsoft.UI.Xaml.Media.FontFamily("Segoe MDL2 Assets"),
                Glyph = "\uE968", // Connection/Network icon
                FontSize = 20,
                Foreground = new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.DodgerBlue),
                VerticalAlignment = VerticalAlignment.Center
            };

            var placeholderText = new TextBlock
            {
                Text = "Please enter connection settings and click find device button to show all device available.",
                FontSize = 14,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                TextWrapping = TextWrapping.Wrap
            };

            headerPanel.Children.Add(headerIcon);
            headerPanel.Children.Add(placeholderText);
            mainPanel.Children.Add(headerPanel);

            // Initialize input fields with default values
            _hostTextBox = CreateTextBox("192.168.1.100");
            _portTextBox = CreateTextBox("502");
            _timeoutTextBox = CreateTextBox("5000");

            // Add input fields
            mainPanel.Children.Add(CreateFormFieldGrid("Host:", _hostTextBox));
            mainPanel.Children.Add(CreateFormFieldGrid("Port:", _portTextBox));
            mainPanel.Children.Add(CreateFormFieldGrid("Timeout:", _timeoutTextBox));

            // Create and add the Find Device button with icon
            var findButtonContent = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                Spacing = 8
            };

            var findButtonIcon = new FontIcon
            {
                FontFamily = new Microsoft.UI.Xaml.Media.FontFamily("Segoe MDL2 Assets"),
                Glyph = "\uE721", // Search icon
                FontSize = 14
            };

            var findButtonText = new TextBlock
            {
                Text = "Find Device"
            };

            findButtonContent.Children.Add(findButtonIcon);
            findButtonContent.Children.Add(findButtonText);

            _buttonFindDevice = new Button
            {
                Content = findButtonContent,
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(0, 20, 0, 0),
                Padding = new Thickness(15, 8, 15, 8),
                MinWidth = 120
            };

            // Wire up the event handler for Find Device button
            _buttonFindDevice.Click += OnButtonFindDeviceClick;

            // Add the button to the main panel
            mainPanel.Children.Add(_buttonFindDevice);

            // Create loading progress ring (initially hidden)
            _loadingProgressRing = new ProgressRing
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(0, 10, 0, 0),
                Width = 30,
                Height = 30,
                IsActive = false,
                Visibility = Visibility.Collapsed
            };

            mainPanel.Children.Add(_loadingProgressRing);

            // Create loading text block (initially hidden)
            _loadingTextBlock = new TextBlock
            {
                Text = "Please wait while searching for devices...",
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(0, 5, 0, 0),
                FontSize = 12,
                Foreground = new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.Gray),
                Visibility = Visibility.Collapsed,
                TextWrapping = TextWrapping.Wrap
            };

            mainPanel.Children.Add(_loadingTextBlock);

            // Create result panel (initially hidden)
            _resultPanel = new StackPanel
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                Margin = new Thickness(10, 15, 10, 0),
                Visibility = Visibility.Collapsed,
                Spacing = 8
            };

            // Result icon and text
            var resultHeaderPanel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                HorizontalAlignment = HorizontalAlignment.Center,
                Spacing = 10
            };

            _resultIcon = new FontIcon
            {
                FontFamily = new Microsoft.UI.Xaml.Media.FontFamily("Segoe MDL2 Assets"),
                FontSize = 16
            };

            _resultTextBlock = new TextBlock
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                FontSize = 14,
                FontWeight = Microsoft.UI.Text.FontWeights.SemiBold,
                TextWrapping = TextWrapping.Wrap
            };

            resultHeaderPanel.Children.Add(_resultIcon);
            resultHeaderPanel.Children.Add(_resultTextBlock);
            _resultPanel.Children.Add(resultHeaderPanel);

            // Initialize analog data display TextBlock
            _resultDataAnalog = new TextBlock
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                FontSize = 12,
                Margin = new Thickness(0, 10, 0, 0),
                TextWrapping = TextWrapping.Wrap,
                Foreground = new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.DarkBlue),
                Visibility = Visibility.Collapsed
            };

            _resultPanel.Children.Add(_resultDataAnalog);

            // Add debug info display
            _debugInfoTextBlock = new TextBlock
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                FontSize = 10,
                Margin = new Thickness(0, 10, 0, 0),
                TextWrapping = TextWrapping.Wrap,
                Foreground = new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.Gray),
                Visibility = Visibility.Collapsed,
                FontFamily = new Microsoft.UI.Xaml.Media.FontFamily("Consolas")
            };

            _resultPanel.Children.Add(_debugInfoTextBlock);
            mainPanel.Children.Add(_resultPanel);

            scrollViewer.Content = mainPanel;
            this.Height = 450;
            this.MaxHeight = 550;
            this.Content = scrollViewer;

            // Set to center
            this.HorizontalAlignment = HorizontalAlignment.Center;
            this.VerticalAlignment = VerticalAlignment.Center;

            // Event handler for connect button
            this.PrimaryButtonClick += OnConnectButtonClick;
            // Realtime polling: 1x per detik
            _pollTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
            _pollTimer.Tick += PollTimer_Tick;

            // Hentikan polling ketika dialog ditutup
            this.Closed += (s, e) => StopPolling();

        }

        private void LogDebugInfo(string message)
        {
            // Write to debug output
            System.Diagnostics.Debug.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] {message}");

            // Also write to console if available
            Console.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] {message}");

            // Show in UI for better visibility
            _debugInfoTextBlock.Visibility = Visibility.Visible;
            _debugInfoTextBlock.Text += $"[{DateTime.Now:HH:mm:ss.fff}] {message}\n";
        }

        private TextBox CreateTextBox(string defaultText = "")
        {
            return new TextBox
            {
                Text = defaultText,
                Height = 32,
                MinWidth = 200,
                BorderThickness = new Thickness(1),
                Padding = new Thickness(8, 4, 8, 4),
                FontSize = 13,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Stretch
            };
        }

        private Grid CreateFormFieldGrid(string label, TextBox textBox)
        {
            var grid = new Grid
            {
                Height = 40,
                Margin = new Thickness(10, 5, 10, 5)
            };

            // Set column definitions
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(80) }); // Label width
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) }); // TextBox width

            var labelBlock = new TextBlock
            {
                Text = label,
                FontWeight = Microsoft.UI.Text.FontWeights.SemiBold,
                FontSize = 13,
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(0, 0, 10, 0)
            };

            Grid.SetColumn(labelBlock, 0);
            Grid.SetColumn(textBox, 1);

            grid.Children.Add(labelBlock);
            grid.Children.Add(textBox);

            return grid;
        }

        private async void OnButtonFindDeviceClick(object sender, RoutedEventArgs e)
        {
            try
            {
                // Show loading animation and disable button
                ShowLoadingState(true);

                // Get values from input fields and convert to appropriate types
                string host = _hostTextBox.Text?.Trim() ?? "";

                // Convert port and timeout to int with validation
                if (!int.TryParse(_portTextBox.Text?.Trim(), out int port))
                {
                    port = 502; // Default port if parsing fails
                    LogDebugInfo("ConnectionDialog: Invalid port, using default 502");
                }

                if (!int.TryParse(_timeoutTextBox.Text?.Trim(), out int timeout))
                {
                    timeout = 5000; // Default timeout if parsing fails
                    LogDebugInfo("ConnectionDialog: Invalid timeout, using default 5000");
                }

                LogDebugInfo($"ConnectionDialog: Find Device clicked with Host={host}, Port={port}, Timeout={timeout}");

                // Run device discovery on background thread to avoid blocking UI
                i32Result = await Task.Run(() =>
                {
                    return mmp.Open(host, port, OptoMMP.Connection.Tcp, timeout, true);
                });

                // Hide loading and show results
                ShowLoadingState(false);
                ShowConnectionResult(i32Result, host, port);

                if (i32Result == 0)
                {
                    LogDebugInfo("ConnectionDialog: Device connection successful");
                    // Jika berhasil maka coba test dapatkan data analog
                    await ReadAllSensorsOnceAsync();
                    StartPolling();
                  

                }
                else
                {
                    LogDebugInfo($"ConnectionDialog: Device connection failed with error code: {i32Result}");
                }
            }
            catch (Exception ex)
            {
                // Hide loading on error
                ShowLoadingState(false);
                ShowErrorResult(ex.Message);
                LogDebugInfo($"ConnectionDialog: Error in find device action: {ex.Message}");
            }
        }

        private void ShowLoadingState(bool isLoading)
        {
            _buttonFindDevice.IsEnabled = !isLoading;
            _loadingProgressRing.IsActive = isLoading;
            _loadingProgressRing.Visibility = isLoading ? Visibility.Visible : Visibility.Collapsed;
            _loadingTextBlock.Visibility = isLoading ? Visibility.Visible : Visibility.Collapsed;

            if (isLoading)
            {
                var loadingContent = new StackPanel
                {
                    Orientation = Orientation.Horizontal,
                    Spacing = 8
                };

                var loadingIcon = new FontIcon
                {
                    FontFamily = new Microsoft.UI.Xaml.Media.FontFamily("Segoe MDL2 Assets"),
                    Glyph = "\uE777", // Sync icon
                    FontSize = 14
                };

                var loadingText = new TextBlock
                {
                    Text = "Searching Device..."
                };

                loadingContent.Children.Add(loadingIcon);
                loadingContent.Children.Add(loadingText);
                _buttonFindDevice.Content = loadingContent;

                // Hide result panel while loading
                _resultPanel.Visibility = Visibility.Collapsed;

                // Make the progress ring more prominent
                _loadingProgressRing.Width = 40;
                _loadingProgressRing.Height = 40;
                _loadingProgressRing.Margin = new Thickness(0, 15, 0, 5);

                // Update loading text
                _loadingTextBlock.Text = "Please wait while searching for devices...";
            }
            else
            {
                var normalContent = new StackPanel
                {
                    Orientation = Orientation.Horizontal,
                    Spacing = 8
                };

                var normalIcon = new FontIcon
                {
                    FontFamily = new Microsoft.UI.Xaml.Media.FontFamily("Segoe MDL2 Assets"),
                    Glyph = "\uE721", // Search icon
                    FontSize = 14
                };

                var normalText = new TextBlock
                {
                    Text = "Find Device"
                };

                normalContent.Children.Add(normalIcon);
                normalContent.Children.Add(normalText);
                _buttonFindDevice.Content = normalContent;

                // Reset progress ring size
                _loadingProgressRing.Width = 30;
                _loadingProgressRing.Height = 30;
                _loadingProgressRing.Margin = new Thickness(0, 10, 0, 0);
            }
        }

        private void ShowConnectionResult(int resultCode, string host, int port)
        {
            _resultPanel.Visibility = Visibility.Visible;

            if (resultCode == 0)
            {
                // Success
                _resultIcon.Glyph = "\uE73E"; // Checkmark icon
                _resultIcon.Foreground = new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.Green);
                _resultTextBlock.Text = $"Connection Successful!\n\nDevice found at {host}:{port}\nResult Code: {resultCode}";
                _resultTextBlock.Foreground = new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.Green);
            }
            else
            {
                // Failure
                _resultIcon.Glyph = "\uE783"; // Error icon
                _resultIcon.Foreground = new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.Red);
                _resultTextBlock.Text = $"Connection Failed!\n\nCould not connect to {host}:{port}\nError Code: {resultCode}";
                _resultTextBlock.Foreground = new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.Red);
                StopPolling();

            }
        }
        // Convert 4–20 mA to 0–15000 psi
        private static double MilliampToPsi(
            double mA,
            double minmA = 4.0,
            double maxmA = 20.0,
            double minPsi = 0.0,
            double maxPsi = 15000.0)
        {
            if (double.IsNaN(mA) || double.IsInfinity(mA)) return double.NaN;
            if (mA < minmA) mA = minmA;
            if (mA > maxmA) mA = maxmA;
            return (mA - minmA) * (maxPsi - minPsi) / (maxmA - minmA) + minPsi;
        }

        private void ShowAnalogData(float analogValue, int readResult)
        {
            _resultDataAnalog.Visibility = Visibility.Visible;

            if (readResult == 0)
            {
                // Assume analogValue in mA (4–20 mA). Convert to psi (0–15000 psi)
                double mA = analogValue;
                double psi = MilliampToPsi(mA);

                _resultDataAnalog.Text =
                    $"Analog Data Reading:\n" +
                    $"Point 6.0: {mA:F3} mA  ≈  {psi:F1} psi\n" +
                    $"Read Result: Success ({readResult})";

                _resultDataAnalog.Foreground =
                    new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.DarkBlue);
            }
            else
            {
                _resultDataAnalog.Text =
                    $"Analog Data Reading:\nFailed to read Point 6.0\nRead Result: Error ({readResult})";

                _resultDataAnalog.Foreground =
                    new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.Red);
            }
        }


        private void ShowErrorResult(string errorMessage)
        {
            _resultPanel.Visibility = Visibility.Visible;
            _resultIcon.Glyph = "\uE7BA"; // Warning icon
            _resultIcon.Foreground = new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.Orange);
            _resultTextBlock.Text = $"Error Occurred!\n\n{errorMessage}";
            _resultTextBlock.Foreground = new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.Orange);
        }
        private void StartPolling()
        {
            if (!_pollTimer.IsEnabled)
            {
                _pollTimer.Start();
                LogDebugInfo("Polling started (1 Hz).");
            }
        }

        private void StopPolling()
        {
            if (_pollTimer.IsEnabled)
            {
                _pollTimer.Stop();
                LogDebugInfo("Polling stopped.");
            }
        }

        private async void PollTimer_Tick(object? sender, object e)
        {
            // Jangan jalan kalau belum konek atau masih baca
            if (i32Result != 0 || _isReading) return;

            _isReading = true;
            try
            {
                await ReadAllSensorsOnceAsync();
            }
            catch (Exception ex)
            {
                LogDebugInfo($"PollTimer_Tick error: {ex.Message}");
            }
            finally
            {
                _isReading = false;
            }
        }

        private async void OnConnectButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            try
            {
                // Get values from input fields and convert to appropriate types
                string host = _hostTextBox.Text?.Trim() ?? "";

                // Convert port and timeout to int with validation
                if (!int.TryParse(_portTextBox.Text?.Trim(), out int port))
                {
                    port = 502; // Default port if parsing fails
                    LogDebugInfo("ConnectionDialog: Invalid port, using default 502");
                }

                if (!int.TryParse(_timeoutTextBox.Text?.Trim(), out int timeout))
                {
                    timeout = 5000; // Default timeout if parsing fails
                    LogDebugInfo("ConnectionDialog: Invalid timeout, using default 5000");
                }

                LogDebugInfo($"ConnectionDialog: Connect clicked with Host={host}, Port={port}, Timeout={timeout}");

                // If device hasn't been found yet, try to connect first
                if (i32Result != 0)
                {
                    // Defer the dialog closing to perform connection
                    args.Cancel = true;

                    // Show loading
                    ShowLoadingState(true);

                    // Run connection on background thread to avoid blocking UI
                    i32Result = await Task.Run(() =>
                    {
                        return mmp.Open(host, port, OptoMMP.Connection.Tcp, timeout, true);
                    });

                    // Hide loading and show result
                    ShowLoadingState(false);
                    ShowConnectionResult(i32Result, host, port);

                    if (i32Result == 0)
                    {
                        LogDebugInfo("ConnectionDialog: Connection successful, dialog will close");

                        await ReadAllSensorsOnceAsync();
                        StartPolling();
                    }
                    else
                    {
                        LogDebugInfo($"ConnectionDialog: Connection failed with error code: {i32Result}");
                    }
                }
                else
                {
                    // Device already connected successfully
                    LogDebugInfo("ConnectionDialog: Using existing successful connection");

                    await ReadAllSensorsOnceAsync();
                    StartPolling();
                }
            }
            catch (Exception ex)
            {
                ShowLoadingState(false);
                ShowErrorResult(ex.Message);
                LogDebugInfo($"ConnectionDialog: Error in connect action: {ex.Message}");
                args.Cancel = true;
            }
        }
        // Baca semua sensor sekali (6 analog) + placeholder 3 sensor lainnya
        private async Task ReadAllSensorsOnceAsync()
        {
            float a1 = 0, a2 = 0, a3 = 0, a4 = 0, a5 = 0, a6 = 0;

            int r1 = await Task.Run(() => mmp.ReadAnalogState64(OptoMMP.GetPointNumberFor64(5, 0), out a1)); // A1 mod5.0
            int r2 = await Task.Run(() => mmp.ReadAnalogState64(OptoMMP.GetPointNumberFor64(5, 1), out a2)); // A2 mod5.1
            int r3 = await Task.Run(() => mmp.ReadAnalogState64(OptoMMP.GetPointNumberFor64(5, 2), out a3)); // A3 mod5.2
            int r4 = await Task.Run(() => mmp.ReadAnalogState64(OptoMMP.GetPointNumberFor64(5, 3), out a4)); // A4 mod5.3
            int r5 = await Task.Run(() => mmp.ReadAnalogState64(OptoMMP.GetPointNumberFor64(6, 0), out a5)); // A5 mod6.0
            int r6 = await Task.Run(() => mmp.ReadAnalogState64(OptoMMP.GetPointNumberFor64(6, 1), out a6)); // A6 mod6.1

            var sb = new System.Text.StringBuilder();
            sb.AppendLine("Sensors:");

            // Konversi 4–20 mA ke psi sesuai rentang tiap sensor
            sb.AppendLine(r1 == 0 ? $"A1 Wellhead (mod5.0, 0–5000 psi):  {a1:F3} mA  ≈  {MilliampToPsi(a1, 4, 20, 0, 5000):F1} psi" : $"A1 Wellhead: READ ERROR ({r1})");
            sb.AppendLine(r2 == 0 ? $"A2 Circulating (mod5.1, 0–5000 psi):  {a2:F3} mA  ≈  {MilliampToPsi(a2, 4, 20, 0, 5000):F1} psi" : $"A2 Circulating: READ ERROR ({r2})");
            sb.AppendLine(r3 == 0 ? $"A3 Pipe Heavy (mod5.2, 0–1500 psi):  {a3:F3} mA  ≈  {MilliampToPsi(a3, 4, 20, 0, 1500):F1} psi" : $"A3 Pipe Heavy: READ ERROR ({r3})");
            sb.AppendLine(r4 == 0 ? $"A4 Pipe Light (mod5.3, 0–1500 psi):  {a4:F3} mA  ≈  {MilliampToPsi(a4, 4, 20, 0, 1500):F1} psi" : $"A4 Pipe Light: READ ERROR ({r4})");
            sb.AppendLine(r5 == 0 ? $"A5 Wellhead HP (mod6.0, 0–15000 psi):  {a5:F3} mA  ≈  {MilliampToPsi(a5, 4, 20, 0, 15000):F1} psi" : $"A5 Wellhead HP: READ ERROR ({r5})");
            sb.AppendLine(r6 == 0 ? $"A6 Circulating HP (mod6.1, 0–15000 psi):  {a6:F3} mA  ≈  {MilliampToPsi(a6, 4, 20, 0, 15000):F1} psi" : $"A6 Circulating HP: READ ERROR ({r6})");

            // Placeholder 3 sensor lain (implementasi DI / encoder tergantung modul yang dipakai)
            sb.AppendLine("D1 Fluid Prox (DI @ modul 2 pin 1) - TODO implement baca DI");
            sb.AppendLine("D2 N2 Prox (DI @ modul 2 pin 3) - TODO implement baca DI");
            sb.AppendLine("E1 CT Depth/Speed Encoder (Quadrature A/B) - TODO implement baca encoder");

            _resultDataAnalog.Visibility = Visibility.Visible;
            _resultDataAnalog.Text = sb.ToString();
            _resultDataAnalog.Foreground = new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.DarkBlue);
        }

    }
}