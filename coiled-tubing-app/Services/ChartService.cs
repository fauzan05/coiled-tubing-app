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
    public class ChartService
    {
        private readonly FileHistoryService _fileHistoryService;

        public ChartService()
        {
            _fileHistoryService = new FileHistoryService();
        }

        // Daftar chart yang tersedia
        public List<ChartItem> GetAvailableCharts()
        {
            return new List<ChartItem>
            {
                new ChartItem { Id = "temp_chart", Name = "Temperature Chart", Description = "Grafik suhu" },
                new ChartItem { Id = "pressure_chart", Name = "Pressure Chart", Description = "Grafik tekanan" },
                new ChartItem { Id = "flow_chart", Name = "Flow Rate Chart", Description = "Grafik laju aliran" },
                new ChartItem { Id = "depth_chart", Name = "Depth Chart", Description = "Grafik kedalaman" },
                new ChartItem { Id = "sensor_status", Name = "Sensor Status", Description = "Status sensor" },
                new ChartItem { Id = "system_load", Name = "System Load", Description = "Beban sistem" }
            };
        }

        // Simpan record ke file .fxz dan return info file
        public async Task<(bool Success, string FilePath, string Directory)> SaveRecordAsync(ChartRecord record)
        {
            try
            {
                // Convert record ke JSON
                string jsonString = JsonSerializer.Serialize(record, new JsonSerializerOptions
                {
                    WriteIndented = true
                });

                // Buat file picker untuk save
                var savePicker = new FileSavePicker();

                // Setup file picker
                var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(App.MainWindow);
                WinRT.Interop.InitializeWithWindow.Initialize(savePicker, hwnd);

                savePicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
                savePicker.FileTypeChoices.Add("FXZ Record", new List<string>() { ".fxz" });
                savePicker.SuggestedFileName = $"{record.RecordName}.fxz";

                // User pilih lokasi save
                StorageFile file = await savePicker.PickSaveFileAsync();
                if (file != null)
                {
                    await FileIO.WriteTextAsync(file, jsonString);
                    
                    // Add to history
                    await _fileHistoryService.AddHistoryItemAsync(record.RecordName, file.Path, FileHistoryType.Created);
                    
                    var directory = Path.GetDirectoryName(file.Path) ?? "";
                    return (true, file.Path, directory);
                }
                return (false, "", "");
            }
            catch
            {
                return (false, "", "");
            }
        }

        // Load record dari file .fxz dan return record dengan file info
        public async Task<(ChartRecord? Record, string FilePath, string Directory)> LoadRecordAsync()
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
                    var record = JsonSerializer.Deserialize<ChartRecord>(jsonString);
                    
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
    }
}