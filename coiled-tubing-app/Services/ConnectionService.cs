using coiled_tubing_app.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Pickers;

namespace coiled_tubing_app.Services
{
    public class ConnectionService
    {
        // Simpan connection settings ke file .fxz
        public async Task<(bool Success, string FilePath, string Directory)> SaveConnectionSettingsAsync(ConnectionSettings connectionSettings)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"SaveConnectionSettingsAsync: Starting save for connection '{connectionSettings.Host}:{connectionSettings.Port}'");

                // Convert connection settings ke JSON
                string jsonString = JsonSerializer.Serialize(connectionSettings, new JsonSerializerOptions
                {
                    WriteIndented = true
                });

                System.Diagnostics.Debug.WriteLine($"SaveConnectionSettingsAsync: JSON serialized, length: {jsonString.Length}");

                // Buat file picker untuk save
                var savePicker = new FileSavePicker();

                // Setup file picker
                var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(App.MainWindow);
                WinRT.Interop.InitializeWithWindow.Initialize(savePicker, hwnd);

                savePicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
                savePicker.FileTypeChoices.Add("FXZ Connection", new List<string>() { ".fxz" });
                savePicker.SuggestedFileName = $"Connection_{connectionSettings.Host}_{connectionSettings.Port}.fxz";

                System.Diagnostics.Debug.WriteLine($"SaveConnectionSettingsAsync: Showing file picker with suggested name 'Connection_{connectionSettings.Host}_{connectionSettings.Port}.fxz'");

                // User pilih lokasi save
                StorageFile file = await savePicker.PickSaveFileAsync();

                if (file != null)
                {
                    System.Diagnostics.Debug.WriteLine($"SaveConnectionSettingsAsync: User selected file: '{file.Path}'");

                    await FileIO.WriteTextAsync(file, jsonString);

                    System.Diagnostics.Debug.WriteLine($"SaveConnectionSettingsAsync: File saved successfully");

                    return (true, file.Path, Path.GetDirectoryName(file.Path) ?? "");
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("SaveConnectionSettingsAsync: User cancelled file picker");
                    return (false, "", "");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"SaveConnectionSettingsAsync error: {ex.Message}");
                return (false, "", "");
            }
        }

        // Simpan connection settings secara otomatis ke folder aplikasi
        public async Task<bool> SaveConnectionSettingsAutoAsync(ConnectionSettings connectionSettings)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"SaveConnectionSettingsAutoAsync: Starting auto save for connection '{connectionSettings.Host}:{connectionSettings.Port}'");

                // Convert connection settings ke JSON
                string jsonString = JsonSerializer.Serialize(connectionSettings, new JsonSerializerOptions
                {
                    WriteIndented = true
                });

                // Dapatkan local app folder
                StorageFolder localFolder = ApplicationData.Current.LocalFolder;

                // Buat nama file yang unik berdasarkan timestamp
                string fileName = $"Connection_{connectionSettings.Host}_{connectionSettings.Port}_{DateTime.Now:yyyyMMdd_HHmmss}.fxz";

                System.Diagnostics.Debug.WriteLine($"SaveConnectionSettingsAutoAsync: Creating file '{fileName}' in local folder");

                // Buat dan simpan file
                StorageFile file = await localFolder.CreateFileAsync(fileName, CreationCollisionOption.ReplaceExisting);
                await FileIO.WriteTextAsync(file, jsonString);

                System.Diagnostics.Debug.WriteLine($"SaveConnectionSettingsAutoAsync: File saved successfully at '{file.Path}'");

                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"SaveConnectionSettingsAutoAsync error: {ex.Message}");
                return false;
            }
        }

        // Simpan connection settings ke file .fxz yang sudah aktif/terpilih
        public async Task<bool> SaveConnectionSettingsToCurrentFileAsync(ConnectionSettings connectionSettings)
        {
            try
            {
                var appState = AppStateService.Instance;
                if (!appState.HasCurrentFile)
                {
                    System.Diagnostics.Debug.WriteLine("SaveConnectionSettingsToCurrentFileAsync: No current file selected");
                    return false;
                }

                var currentFile = appState.GetCurrentFileInfo();
                if (currentFile == null)
                {
                    System.Diagnostics.Debug.WriteLine("SaveConnectionSettingsToCurrentFileAsync: Current file info is null");
                    return false;
                }

                System.Diagnostics.Debug.WriteLine($"SaveConnectionSettingsToCurrentFileAsync: Saving connection settings to current file '{currentFile.FilePath}'");

                // Load existing file content first
                var file = await StorageFile.GetFileFromPathAsync(currentFile.FilePath);
                string existingContent = await FileIO.ReadTextAsync(file);

                // Try to deserialize existing content to check if it's a ChartRecord
                try
                {
                    var existingRecord = JsonSerializer.Deserialize<ChartRecord>(existingContent);
                    if (existingRecord != null)
                    {
                        // Add connection settings to existing ChartRecord's GeneralData
                        if (existingRecord.GeneralData == null)
                        {
                            existingRecord.GeneralData = new GeneralData();
                        }

                        // Add connection settings to GeneralData
                        existingRecord.GeneralData.ConnectionSettings = connectionSettings;

                        string jsonString = JsonSerializer.Serialize(existingRecord, new JsonSerializerOptions
                        {
                            WriteIndented = true
                        });

                        await FileIO.WriteTextAsync(file, jsonString);
                        System.Diagnostics.Debug.WriteLine("SaveConnectionSettingsToCurrentFileAsync: Connection settings added to existing ChartRecord's GeneralData");

                        return true;
                    }
                }
                catch
                {
                    // If it's not a ChartRecord, treat it as a pure connection settings file
                    System.Diagnostics.Debug.WriteLine("SaveConnectionSettingsToCurrentFileAsync: File is not a ChartRecord, updating as connection settings");
                }

                // If file doesn't contain ChartRecord, just overwrite with connection settings
                string connectionJson = JsonSerializer.Serialize(connectionSettings, new JsonSerializerOptions
                {
                    WriteIndented = true
                });

                await FileIO.WriteTextAsync(file, connectionJson);
                System.Diagnostics.Debug.WriteLine("SaveConnectionSettingsToCurrentFileAsync: File updated with connection settings");

                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"SaveConnectionSettingsToCurrentFileAsync error: {ex.Message}");
                return false;
            }
        }
    }
}