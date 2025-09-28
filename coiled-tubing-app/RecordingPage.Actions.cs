using coiled_tubing_app.Models;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System.IO;
using System.Text.Json;

namespace coiled_tubing_app
{
    public partial class RecordingPage
    {
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
                    }
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"SaveConnectionSettings: File does not exist at '{currentRecord.FilePath}'");
                }
            }
        }
    }
}
