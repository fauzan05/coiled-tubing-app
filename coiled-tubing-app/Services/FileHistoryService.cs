using coiled_tubing_app.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace coiled_tubing_app.Services
{
    public class FileHistoryService
    {
        private const string HISTORY_FILE_NAME = "file_history.json";
        private List<FileHistoryItem> _historyItems;
        private bool _isInitialized = false;
        private string _historyFilePath;

        public FileHistoryService()
        {
            _historyItems = new List<FileHistoryItem>();
            
            // Use a more reliable path for storing history
            var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            var appFolder = Path.Combine(appDataPath, "CoiledTubingApp");
            
            // Create directory if it doesn't exist
            try
            {
                if (!Directory.Exists(appFolder))
                {
                    Directory.CreateDirectory(appFolder);
                }
                _historyFilePath = Path.Combine(appFolder, HISTORY_FILE_NAME);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"FileHistoryService constructor error: {ex.Message}");
                // Fallback to temp directory
                _historyFilePath = Path.Combine(Path.GetTempPath(), "CoiledTubingApp_" + HISTORY_FILE_NAME);
            }
        }

        public async Task<List<FileHistoryItem>> GetHistoryAsync()
        {
            if (!_isInitialized)
            {
                await LoadHistoryAsync();
                _isInitialized = true;
            }
            return _historyItems.OrderByDescending(x => x.LastAccessed).ToList();
        }

        public async Task AddHistoryItemAsync(string recordName, string filePath, FileHistoryType historyType)
        {
            try
            {
                if (!_isInitialized)
                {
                    await LoadHistoryAsync();
                    _isInitialized = true;
                }

                var directory = Path.GetDirectoryName(filePath) ?? "";
                
                // Remove existing item with same file path if exists
                _historyItems.RemoveAll(x => x.FilePath == filePath);

                // Add new item
                var historyItem = new FileHistoryItem
                {
                    RecordName = recordName,
                    FilePath = filePath,
                    Directory = directory,
                    LastAccessed = DateTime.Now,
                    HistoryType = historyType
                };

                _historyItems.Add(historyItem);
                await SaveHistoryAsync();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"AddHistoryItemAsync error: {ex.Message}");
                // Continue without failing
            }
        }

        public async Task RemoveHistoryItemAsync(FileHistoryItem item)
        {
            try
            {
                _historyItems.Remove(item);
                await SaveHistoryAsync();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"RemoveHistoryItemAsync error: {ex.Message}");
                // Continue without failing
            }
        }

        public async Task ClearHistoryAsync()
        {
            try
            {
                _historyItems.Clear();
                await SaveHistoryAsync();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"ClearHistoryAsync error: {ex.Message}");
                // Continue without failing
            }
        }

        private async Task LoadHistoryAsync()
        {
            await Task.Run(() =>
            {
                try
                {
                    if (File.Exists(_historyFilePath))
                    {
                        var jsonContent = File.ReadAllText(_historyFilePath);
                        if (!string.IsNullOrEmpty(jsonContent))
                        {
                            var loadedItems = JsonSerializer.Deserialize<List<FileHistoryItem>>(jsonContent);
                            
                            if (loadedItems != null)
                            {
                                // Verify files still exist and filter out non-existent ones
                                _historyItems = loadedItems.Where(item => File.Exists(item.FilePath)).ToList();
                                
                                // Save filtered list back if items were removed
                                if (_historyItems.Count != loadedItems.Count)
                                {
                                    // Save synchronously here since we're already in Task.Run
                                    SaveHistorySync();
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"LoadHistoryAsync error: {ex.Message}");
                    // If loading fails, start with empty list
                    _historyItems = new List<FileHistoryItem>();
                }
            });
        }

        private async Task SaveHistoryAsync()
        {
            await Task.Run(() => SaveHistorySync());
        }

        private void SaveHistorySync()
        {
            try
            {
                var jsonContent = JsonSerializer.Serialize(_historyItems, new JsonSerializerOptions
                {
                    WriteIndented = true
                });
                
                File.WriteAllText(_historyFilePath, jsonContent);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"SaveHistorySync error: {ex.Message}");
                // Ignore save errors for now
            }
        }
    }
}