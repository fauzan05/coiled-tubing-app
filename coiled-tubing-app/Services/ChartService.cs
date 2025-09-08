using coiled_tubing_app.Models;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Pickers;

namespace coiled_tubing_app.Services
{
    public class ChartService
    {
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

        // Simpan record ke file .fxz
        public async Task<bool> SaveRecordAsync(ChartRecord record)
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
                    return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        // Load record dari file .fxz
        public async Task<ChartRecord?> LoadRecordAsync()
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
                    return JsonSerializer.Deserialize<ChartRecord>(jsonString);
                }
                return null;
            }
            catch
            {
                return null;
            }
        }
    }
}