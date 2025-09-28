using coiled_tubing_app.Models;
using coiled_tubing_app.Services;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using OptoMMP6;
using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace coiled_tubing_app
{
    public partial class RecordingPage
    {
        internal static OptoMMP mmp = new OptoMMP();

        private async void SaveConnectionSettings(Window currentWindow, ConnectionSettings connectionSettings, FileHistoryItem currentRecord)
        {
            if (connectionSettings is not null)
            {
                System.Diagnostics.Debug.WriteLine($"Get file history item: {currentRecord}");
                if (File.Exists(currentRecord.FilePath))
                {
                    string jsonContent = await File.ReadAllTextAsync(currentRecord.FilePath);
                    var record = JsonSerializer.Deserialize<ItemRecord>(jsonContent);

                    if (record != null)
                    {
                        // Update the connection settings
                        record.ConnectionSettings = connectionSettings;
                        string updatedJson = JsonSerializer.Serialize(record, new JsonSerializerOptions
                        {
                            WriteIndented = true
                        });

                        await File.WriteAllTextAsync(currentRecord.FilePath, updatedJson);
                        System.Diagnostics.Debug.WriteLine($"SaveConnectionSettings: Updated connection settings saved to '{currentRecord.FilePath}'");
                        // Show success dialog
                        ContentDialog successDialog = new ContentDialog
                        {
                            Title = "Success",
                            Content = "Connection settings updated successfully.",
                            CloseButtonText = "OK",
                            XamlRoot = currentWindow.Content.XamlRoot
                        };
                        await successDialog.ShowAsync();
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine($"SaveConnectionSettings: Failed to deserialize record from '{currentRecord.FilePath}'");
                        // tampilkan pesan error berupa dialog ke user
                        ContentDialog errorDialog = new ContentDialog
                        {
                            Title = "Error",
                            Content = "Failed to load the record. The file may be corrupted.",
                            CloseButtonText = "OK",
                            XamlRoot = currentWindow.Content.XamlRoot
                        };
                        await errorDialog.ShowAsync();
                    }
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"SaveConnectionSettings: File does not exist at '{currentRecord.FilePath}'");
                }
            }
        }

        /// <summary>
        /// Original ConnectDevice method for backward compatibility
        /// </summary>
        private async void ConnectDevice(Window currentWindow, ConnectionSettings connectionSettings)
        {
            // Create dummy UI elements to maintain backward compatibility
            var dummyLogTextBlock = new TextBlock();
            var dummyLoadingPanel = new StackPanel();
            var dummyProgressRing = new ProgressRing();

            await ConnectDeviceWithUI(currentWindow, connectionSettings, dummyLogTextBlock, dummyLoadingPanel, dummyProgressRing);
        }

        /// <summary>
        /// Enhanced ConnectDevice method with UI feedback
        /// </summary>
        private async Task ConnectDeviceWithUI(Window currentWindow, ConnectionSettings connectionSettings,
            TextBlock logTextBlock, StackPanel loadingPanel, ProgressRing progressRing)
        {
            if (connectionSettings is null) return;

            var startTime = DateTime.Now;
            var deviceName = DeviceService.GetDeviceById(connectionSettings.DeviceId)?.DisplayName ?? connectionSettings.Device;

            System.Diagnostics.Debug.WriteLine($"ConnectDevice: Attempting to connect to device '{deviceName}' at IP '{connectionSettings.Host}' on Port '{connectionSettings.Port}'");

            try
            {
                // Update log with connection attempt
                LogToDeviceLog(logTextBlock, $"Connecting to {deviceName}...");
                LogToDeviceLog(logTextBlock, $"Host: {connectionSettings.Host}, Port: {connectionSettings.Port}, Timeout: {connectionSettings.Timeout}ms");

                // Determine connection method based on device ID
                int result;
                if (connectionSettings.DeviceId == 0)
                {
                    // Default to MMP6 for backward compatibility or unknown devices
                    result = await MMP6_Connect(connectionSettings, logTextBlock);
                }
                else
                {
                    // Use device-specific connection logic
                    result = await ConnectByDeviceId(connectionSettings, logTextBlock);
                }

                // Calculate connection duration
                var connectionDuration = DateTime.Now - startTime;

                // Hide loading animation
                SetConnectionLoading(logTextBlock, loadingPanel, progressRing, false);

                // Handle connection result
                if (result == 0)
                {
                    // Success - Update log with success message
                    var successMessage = $"✓ Connection successful! ({connectionDuration.TotalMilliseconds:F0}ms)";
                    LogToDeviceLog(logTextBlock, successMessage);
                    LogToDeviceLog(logTextBlock, $"Device: {deviceName}");
                    LogToDeviceLog(logTextBlock, $"Status: Connected and ready");
                    LogToDeviceLog(logTextBlock, "────────────────────────────");

                    System.Diagnostics.Debug.WriteLine("ConnectDevice: Successfully connected to device.");

                    // Show success dialog
                    ContentDialog successDialog = new ContentDialog
                    {
                        Title = "Connection Successful",
                        Content = $"Successfully connected to {deviceName}\n\nConnection established in {connectionDuration.TotalMilliseconds:F0}ms",
                        CloseButtonText = "OK",
                        XamlRoot = currentWindow.Content.XamlRoot
                    };
                    await successDialog.ShowAsync();
                }
                else
                {
                    // Failure - Update log with error message
                    var errorMessage = $"✗ Connection failed! Error code: {result} ({connectionDuration.TotalMilliseconds:F0}ms)";
                    LogToDeviceLog(logTextBlock, errorMessage);
                    LogToDeviceLog(logTextBlock, GetErrorDescription(result));
                    LogToDeviceLog(logTextBlock, "────────────────────────────");

                    System.Diagnostics.Debug.WriteLine($"ConnectDevice: Failed to connect to device. Error code: {result}");

                    // Show error dialog
                    ContentDialog errorDialog = new ContentDialog
                    {
                        Title = "Connection Failed",
                        Content = $"Failed to connect to {deviceName}\n\nError code: {result}\nDuration: {connectionDuration.TotalMilliseconds:F0}ms\n\n{GetErrorDescription(result)}",
                        CloseButtonText = "OK",
                        XamlRoot = currentWindow.Content.XamlRoot
                    };
                    await errorDialog.ShowAsync();
                }
            }
            catch (Exception ex)
            {
                // Hide loading animation
                SetConnectionLoading(logTextBlock, loadingPanel, progressRing, false);

                // Handle exception
                var connectionDuration = DateTime.Now - startTime;
                var exceptionMessage = $"✗ Connection error: {ex.Message} ({connectionDuration.TotalMilliseconds:F0}ms)";
                LogToDeviceLog(logTextBlock, exceptionMessage);
                LogToDeviceLog(logTextBlock, "────────────────────────────");

                System.Diagnostics.Debug.WriteLine($"ConnectDevice: Exception occurred: {ex.Message}");

                // Show exception dialog
                ContentDialog exceptionDialog = new ContentDialog
                {
                    Title = "Connection Error",
                    Content = $"An error occurred while connecting to {deviceName}\n\nError: {ex.Message}\nDuration: {connectionDuration.TotalMilliseconds:F0}ms",
                    CloseButtonText = "OK",
                    XamlRoot = currentWindow.Content.XamlRoot
                };
                await exceptionDialog.ShowAsync();
            }
        }

        /// <summary>
        /// Connect to device based on device ID
        /// </summary>
        private async Task<int> ConnectByDeviceId(ConnectionSettings settings, TextBlock logTextBlock)
        {
            var device = DeviceService.GetDeviceById(settings.DeviceId);
            if (device == null)
            {
                LogToDeviceLog(logTextBlock, $"Warning: Unknown device ID {settings.DeviceId}, using default connection method");
                return await MMP6_Connect(settings, logTextBlock);
            }

            LogToDeviceLog(logTextBlock, $"Using connection method for {device.DisplayName}");

            // Device-specific connection logic
            switch (settings.DeviceId)
            {
                case 1: // Opto 22 SNAP UP1 ADS
                case 2: // Opto 22 PAC Control
                case 3: // Opto 22 groov EPIC
                    return await MMP6_Connect(settings, logTextBlock);

                // Add cases for new devices as they are implemented
                // case 4: // Allen-Bradley PLC
                //     return await ConnectAllenBradleyPLC(settings, logTextBlock);

                default:
                    LogToDeviceLog(logTextBlock, $"No specific connection method for device ID {settings.DeviceId}, using default");
                    return await MMP6_Connect(settings, logTextBlock);
            }
        }

        /// <summary>
        /// Enhanced MMP6 connection with logging
        /// </summary>
        private async Task<int> MMP6_Connect(ConnectionSettings settings, TextBlock logTextBlock)
        {
            try
            {
                LogToDeviceLog(logTextBlock, "Initializing MMP6 connection...");

                var result = await Task.Run(() =>
                {
                    return mmp.Open(settings.Host, settings.Port, OptoMMP.Connection.Tcp, settings.Timeout, true);
                });

                if (result == 0)
                {
                    LogToDeviceLog(logTextBlock, "MMP6 protocol handshake completed");
                }
                else
                {
                    LogToDeviceLog(logTextBlock, $"MMP6 connection failed with code {result}");
                }

                return result;
            }
            catch (Exception ex)
            {
                LogToDeviceLog(logTextBlock, $"MMP6 connection exception: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"MMP6_Connect error: {ex.Message}");
                return -1;
            }
        }

        /// <summary>
        /// Get human-readable error description
        /// </summary>
        private string GetErrorDescription(int errorCode)
        {
            return errorCode switch
            {
                -1 => "Connection timeout or network error",
                1 => "Invalid host address",
                2 => "Invalid port number",
                3 => "Connection refused by remote host",
                4 => "Authentication failed",
                5 => "Protocol error",
                _ => "Unknown error occurred"
            };
        }

        /// <summary>
        /// Show/hide loading animation
        /// </summary>
        private void SetConnectionLoading(TextBlock logTextBlock, StackPanel loadingPanel, ProgressRing progressRing, bool isLoading)
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
        /// Append message to device log
        /// </summary>
        private void LogToDeviceLog(TextBlock logTextBlock, string message)
        {
            var timestamp = DateTime.Now.ToString("HH:mm:ss");
            var logEntry = $"[{timestamp}] {message}";

            // Append to existing text
            if (string.IsNullOrEmpty(logTextBlock.Text) || logTextBlock.Text == "Device Information Log")
            {
                logTextBlock.Text = $"Device Information Log\n{logEntry}";
            }
            else
            {
                logTextBlock.Text += $"\n{logEntry}";
            }

            // Keep log manageable (max 20 lines)
            var lines = logTextBlock.Text.Split('\n');
            if (lines.Length > 21) // 20 + header line
            {
                var recentLines = new string[21];
                recentLines[0] = lines[0]; // Keep header
                Array.Copy(lines, lines.Length - 20, recentLines, 1, 20); // Keep last 20 log entries
                logTextBlock.Text = string.Join('\n', recentLines);
            }

            System.Diagnostics.Debug.WriteLine($"Device Log: {logEntry}");
        }
    }
}
