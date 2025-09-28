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
    public class RecordService
    {
        private readonly FileHistoryService _fileHistoryService;

        public RecordService()
        {
            _fileHistoryService = new FileHistoryService();
        }

        public async Task<(bool Success, string FilePath, string Directory)> SaveRecordAsync(ItemRecord record)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"SaveRecordAsync: Starting save for record '{record.RecordName}'");

                // Convert record ke JSON
                string jsonString = JsonSerializer.Serialize(record, new JsonSerializerOptions
                {
                    WriteIndented = true
                });

                System.Diagnostics.Debug.WriteLine($"SaveRecordAsync: JSON serialized, length: {jsonString.Length}");

                // Buat file picker untuk save
                var savePicker = new FileSavePicker();

                // Setup file picker
                var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(App.MainWindow);
                WinRT.Interop.InitializeWithWindow.Initialize(savePicker, hwnd);

                savePicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
                savePicker.FileTypeChoices.Add("FXZ Record", new List<string>() { ".fxz" });
                savePicker.SuggestedFileName = $"{record.RecordName}.fxz";

                System.Diagnostics.Debug.WriteLine($"SaveRecordAsync: Showing file picker with suggested name '{record.RecordName}.fxz'");

                // User pilih lokasi save
                StorageFile file = await savePicker.PickSaveFileAsync();

                if (file != null)
                {
                    System.Diagnostics.Debug.WriteLine($"SaveRecordAsync: User selected file: '{file.Path}'");

                    await FileIO.WriteTextAsync(file, jsonString);
                    System.Diagnostics.Debug.WriteLine($"SaveRecordAsync: File written successfully");

                    // Add to history immediately after saving
                    System.Diagnostics.Debug.WriteLine($"SaveRecordAsync: Adding to history...");
                    await _fileHistoryService.AddHistoryItemAsync(record.RecordName, file.Path, FileHistoryType.Created);

                    var directory = Path.GetDirectoryName(file.Path) ?? "";
                    System.Diagnostics.Debug.WriteLine($"SaveRecordAsync: Success - FilePath: '{file.Path}', Directory: '{directory}'");

                    return (true, file.Path, directory);
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("SaveRecordAsync: User cancelled file picker");
                    return (false, "", "");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"SaveRecordAsync error: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"SaveRecordAsync stack trace: {ex.StackTrace}");
                return (false, "", "");
            }
        }

        // Overload untuk save ke path yang sudah ada (untuk update existing record)
        public async Task<(bool Success, string FilePath, string Directory)> SaveRecordToPathAsync(ItemRecord record, string filePath)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"SaveRecordToPathAsync: Starting save for record '{record.RecordName}' to '{filePath}'");

                // Convert record ke JSON
                string jsonString = JsonSerializer.Serialize(record, new JsonSerializerOptions
                {
                    WriteIndented = true
                });

                System.Diagnostics.Debug.WriteLine($"SaveRecordToPathAsync: JSON serialized, length: {jsonString.Length}");

                // Write to existing file
                var file = await StorageFile.GetFileFromPathAsync(filePath);
                await FileIO.WriteTextAsync(file, jsonString);
                System.Diagnostics.Debug.WriteLine($"SaveRecordToPathAsync: File written successfully");

                // Add to history
                await _fileHistoryService.AddHistoryItemAsync(record.RecordName, filePath, FileHistoryType.Updated);

                var directory = Path.GetDirectoryName(filePath) ?? "";
                System.Diagnostics.Debug.WriteLine($"SaveRecordToPathAsync: Success - FilePath: '{filePath}', Directory: '{directory}'");

                return (true, filePath, directory);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"SaveRecordToPathAsync error: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"SaveRecordToPathAsync stack trace: {ex.StackTrace}");
                return (false, "", "");
            }
        }

        // Load record dari file .fxz dan return record dengan file info
        public async Task<(ItemRecord? Record, string FilePath, string Directory)> LoadRecordAsync()
        {
            try
            {
                var openPicker = new FileOpenPicker();

                var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(App.MainWindow);
                WinRT.Interop.InitializeWithWindow.Initialize(openPicker, hwnd);

                openPicker.ViewMode = PickerViewMode.Thumbnail;
                openPicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
                openPicker.FileTypeFilter.Add(".fxz");

                StorageFile file = await openPicker.PickSingleFileAsync();
                if (file != null)
                {
                    string jsonString = await FileIO.ReadTextAsync(file);
                    var record = JsonSerializer.Deserialize<ItemRecord>(jsonString);

                    if (record != null)
                    {
                        // Add to history
                        await _fileHistoryService.AddHistoryItemAsync(record.RecordName, file.Path, FileHistoryType.Loaded);

                        var directory = Path.GetDirectoryName(file.Path) ?? "";
                        return (record, file.Path, directory);
                    }
                }
                return (null, "", "");
            }
            catch
            {
                return (null, "", "");
            }
        }

        // Get file history service
        public FileHistoryService GetFileHistoryService()
        {
            return _fileHistoryService;
        }

        public async Task<(ItemRecord? Record, string FilePath, string Directory)> LoadRecordFromPathAsync(string filePath)
        {
            try
            {
                if (File.Exists(filePath))
                {
                    string jsonString = await FileIO.ReadTextAsync(await StorageFile.GetFileFromPathAsync(filePath));
                    var record = JsonSerializer.Deserialize<ItemRecord>(jsonString);
                    if (record != null)
                    {
                        // Add to history
                        await _fileHistoryService.AddHistoryItemAsync(record.RecordName, filePath, FileHistoryType.Loaded);
                        return (record, filePath, Path.GetDirectoryName(filePath) ?? "");
                    }
                }
                return (null, "", "");
            }
            catch
            {
                return (null, "", "");
            }
        }
    }
}
