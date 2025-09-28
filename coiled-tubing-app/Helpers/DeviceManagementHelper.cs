using coiled_tubing_app.Models;
using coiled_tubing_app.Services;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Linq;

namespace coiled_tubing_app.Helpers
{
    public static class DeviceManagementHelper
    {
        /// <summary>
        /// Populate a ComboBox with available devices from hardcoded DeviceService
        /// </summary>
        /// <param name="comboBox">The ComboBox to populate</param>
        /// <param name="selectedDeviceId">Optional device ID to select by default</param>
        public static void PopulateDeviceComboBox(ComboBox comboBox, int selectedDeviceId = 0)
        {
            if (comboBox == null) return;

            // Clear existing items
            comboBox.Items.Clear();

            // Get devices from hardcoded service
            var devices = DeviceService.GetAllDevices();

            // Add devices to ComboBox
            ComboBoxItem selectedItem = null;
            foreach (var device in devices)
            {
                var item = new ComboBoxItem
                {
                    Content = device.DisplayName,
                    Tag = device.Id
                };

                comboBox.Items.Add(item);

                // Mark for selection if this is the requested device
                if (selectedDeviceId > 0 && device.Id == selectedDeviceId)
                {
                    selectedItem = item;
                }
            }

            // Set selected item if found
            if (selectedItem != null)
            {
                comboBox.SelectedItem = selectedItem;
            }
        }

        /// <summary>
        /// Get device ID from selected ComboBox item
        /// </summary>
        /// <param name="comboBox">The ComboBox to get selection from</param>
        /// <returns>Device ID or 0 if nothing selected</returns>
        public static int GetSelectedDeviceId(ComboBox comboBox)
        {
            if (comboBox?.SelectedItem is ComboBoxItem selectedItem && selectedItem.Tag is int deviceId)
            {
                return deviceId;
            }
            return 0;
        }

        /// <summary>
        /// Get device name from selected ComboBox item
        /// </summary>
        /// <param name="comboBox">The ComboBox to get selection from</param>
        /// <returns>Device name or empty string if nothing selected</returns>
        public static string GetSelectedDeviceName(ComboBox comboBox)
        {
            if (comboBox?.SelectedItem is ComboBoxItem selectedItem)
            {
                return selectedItem.Content?.ToString() ?? string.Empty;
            }
            return string.Empty;
        }

        /// <summary>
        /// Get device model by ID from hardcoded list
        /// </summary>
        /// <param name="deviceId">Device ID</param>
        /// <returns>DeviceModel or null if not found</returns>
        public static DeviceModel GetDeviceById(int deviceId)
        {
            return DeviceService.GetDeviceById(deviceId);
        }

        /// <summary>
        /// Get device configuration by ID
        /// </summary>
        /// <param name="deviceId">Device ID</param>
        /// <returns>DeviceConfiguration or null if not found</returns>
        public static DeviceConfiguration GetDeviceConfiguration(int deviceId)
        {
            return DeviceService.GetDeviceConfiguration(deviceId);
        }

        /// <summary>
        /// Create ConnectionSettings from form inputs with device ID
        /// </summary>
        /// <param name="deviceComboBox">Device selection ComboBox</param>
        /// <param name="hostTextBox">Host input TextBox</param>
        /// <param name="portTextBox">Port input TextBox</param>
        /// <param name="timeoutTextBox">Timeout input TextBox</param>
        /// <returns>ConnectionSettings object</returns>
        public static ConnectionSettings CreateConnectionSettings(
            ComboBox deviceComboBox, 
            TextBox hostTextBox, 
            TextBox portTextBox, 
            TextBox timeoutTextBox)
        {
            var deviceId = GetSelectedDeviceId(deviceComboBox);
            var deviceConfig = GetDeviceConfiguration(deviceId);

            return new ConnectionSettings
            {
                DeviceId = deviceId,
                Device = GetSelectedDeviceName(deviceComboBox), // Keep for backward compatibility
                Host = hostTextBox?.Text?.Trim() ?? string.Empty,
                Port = int.TryParse(portTextBox?.Text?.Trim(), out int port) ? port : deviceConfig?.DefaultPort ?? 502,
                Timeout = int.TryParse(timeoutTextBox?.Text?.Trim(), out int timeout) ? timeout : deviceConfig?.DefaultTimeout ?? 5000
            };
        }

        /// <summary>
        /// Apply device-specific default settings to form controls
        /// </summary>
        /// <param name="deviceComboBox">Device ComboBox</param>
        /// <param name="hostTextBox">Host TextBox</param>
        /// <param name="portTextBox">Port TextBox</param>
        /// <param name="timeoutTextBox">Timeout TextBox</param>
        public static void ApplyDeviceDefaults(ComboBox deviceComboBox, TextBox hostTextBox, TextBox portTextBox, TextBox timeoutTextBox)
        {
            var deviceId = GetSelectedDeviceId(deviceComboBox);
            var deviceConfig = GetDeviceConfiguration(deviceId);

            if (deviceConfig != null)
            {
                // Apply defaults jika field kosong
                if (string.IsNullOrEmpty(portTextBox?.Text))
                {
                    portTextBox.Text = deviceConfig.DefaultPort.ToString();
                }

                if (string.IsNullOrEmpty(timeoutTextBox?.Text))
                {
                    timeoutTextBox.Text = deviceConfig.DefaultTimeout.ToString();
                }

                System.Diagnostics.Debug.WriteLine($"Applied defaults for device {deviceId}: Port={deviceConfig.DefaultPort}, Timeout={deviceConfig.DefaultTimeout}");
            }
        }

        /// <summary>
        /// Show device information dialog (read-only)
        /// </summary>
        /// <param name="xamlRoot">XamlRoot for dialog display</param>
        /// <param name="deviceId">Device ID to show info for</param>
        public static async void ShowDeviceInfo(XamlRoot xamlRoot, int deviceId)
        {
            var device = DeviceService.GetDeviceById(deviceId);
            var config = DeviceService.GetDeviceConfiguration(deviceId);

            if (device == null)
            {
                var errorDialog = new ContentDialog
                {
                    Title = "Error",
                    Content = "Device not found.",
                    CloseButtonText = "OK",
                    XamlRoot = xamlRoot
                };
                await errorDialog.ShowAsync();
                return;
            }

            var dialog = new ContentDialog
            {
                Title = $"Device Information - {device.DisplayName}",
                CloseButtonText = "OK",
                XamlRoot = xamlRoot
            };

            var content = new StackPanel { Spacing = 10, Margin = new Thickness(15) };

            // Device basic info
            content.Children.Add(new TextBlock { Text = $"ID: {device.Id}", FontWeight = Microsoft.UI.Text.FontWeights.Bold });
            content.Children.Add(new TextBlock { Text = $"Name: {device.Name}" });
            content.Children.Add(new TextBlock { Text = $"Manufacturer: {device.Manufacturer}" });
            content.Children.Add(new TextBlock { Text = $"Model: {device.Model}" });
            content.Children.Add(new TextBlock { Text = $"Description: {device.Description}" });

            if (config != null)
            {
                content.Children.Add(new TextBlock { Text = "Configuration:", FontWeight = Microsoft.UI.Text.FontWeights.Bold, Margin = new Thickness(0, 10, 0, 0) });
                content.Children.Add(new TextBlock { Text = $"Default Port: {config.DefaultPort}" });
                content.Children.Add(new TextBlock { Text = $"Default Timeout: {config.DefaultTimeout}ms" });
                content.Children.Add(new TextBlock { Text = $"Supported Protocols: {string.Join(", ", config.SupportedProtocols)}" });
                content.Children.Add(new TextBlock { Text = $"Requires Authentication: {(config.RequiresAuthentication ? "Yes" : "No")}" });
                content.Children.Add(new TextBlock { Text = $"Max Connections: {config.MaxConnections}" });
                
                if (!string.IsNullOrEmpty(config.ConfigurationNotes))
                {
                    content.Children.Add(new TextBlock { Text = "Notes:", FontWeight = Microsoft.UI.Text.FontWeights.Bold, Margin = new Thickness(0, 5, 0, 0) });
                    content.Children.Add(new TextBlock { Text = config.ConfigurationNotes, TextWrapping = TextWrapping.Wrap });
                }
            }

            // Developer note
            content.Children.Add(new TextBlock 
            { 
                Text = "Note: Device list is hardcoded and can only be modified by developers in the source code.",
                FontStyle = Windows.UI.Text.FontStyle.Italic,
                Foreground = new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.Gray),
                Margin = new Thickness(0, 15, 0, 0),
                TextWrapping = TextWrapping.Wrap
            });

            dialog.Content = new ScrollViewer 
            { 
                Content = content,
                MaxHeight = 400,
                VerticalScrollBarVisibility = ScrollBarVisibility.Auto
            };

            await dialog.ShowAsync();
        }

        /// <summary>
        /// Validate if device ID is valid
        /// </summary>
        /// <param name="deviceId">Device ID to validate</param>
        /// <returns>True if valid, false otherwise</returns>
        public static bool IsValidDeviceId(int deviceId)
        {
            return DeviceService.IsValidDeviceId(deviceId);
        }

        /// <summary>
        /// Get all available device IDs
        /// </summary>
        /// <returns>Array of device IDs</returns>
        public static int[] GetAvailableDeviceIds()
        {
            return DeviceService.GetAllDeviceIds();
        }
    }
}