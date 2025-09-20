using System;

namespace coiled_tubing_app.Services
{
    public class AppStateService
    {
        private static AppStateService? _instance;
        private static readonly object _lock = new object();
        
        // Current active file information
        public string? CurrentFilePath { get; private set; }
        public string? CurrentFileName { get; private set; }
        public string? CurrentDirectory { get; private set; }
        
        // Singleton pattern
        public static AppStateService Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                        {
                            _instance = new AppStateService();
                        }
                    }
                }
                return _instance;
            }
        }
        
        private AppStateService() { }
        
        // Set current active file
        public void SetCurrentFile(string filePath, string fileName, string directory)
        {
            CurrentFilePath = filePath;
            CurrentFileName = fileName;
            CurrentDirectory = directory;
            
            System.Diagnostics.Debug.WriteLine($"AppStateService: Current file set to '{filePath}'");
            
            // Notify about file change
            CurrentFileChanged?.Invoke(new CurrentFileInfo
            {
                FilePath = CurrentFilePath,
                FileName = CurrentFileName,
                Directory = CurrentDirectory
            });
        }
        
        // Clear current file
        public void ClearCurrentFile()
        {
            CurrentFilePath = null;
            CurrentFileName = null;
            CurrentDirectory = null;
            
            System.Diagnostics.Debug.WriteLine("AppStateService: Current file cleared");
            
            // Notify about file change
            CurrentFileChanged?.Invoke(null);
        }
        
        // Check if there's a current active file
        public bool HasCurrentFile => !string.IsNullOrEmpty(CurrentFilePath);
        
        // Get current file info
        public CurrentFileInfo? GetCurrentFileInfo()
        {
            if (HasCurrentFile)
            {
                return new CurrentFileInfo
                {
                    FilePath = CurrentFilePath!,
                    FileName = CurrentFileName ?? "",
                    Directory = CurrentDirectory ?? ""
                };
            }
            return null;
        }
        
        // Event for when current file changes
        public event Action<CurrentFileInfo?> CurrentFileChanged;
    }
    
    public class CurrentFileInfo
    {
        public string FilePath { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;
        public string Directory { get; set; } = string.Empty;
    }
}