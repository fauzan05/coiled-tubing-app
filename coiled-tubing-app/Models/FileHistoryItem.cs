using System;

namespace coiled_tubing_app.Models
{
    public class FileHistoryItem
    {
        public string RecordName { get; set; } = string.Empty;
        public string FilePath { get; set; } = string.Empty;
        public string Directory { get; set; } = string.Empty;
        public DateTime LastAccessed { get; set; }
        public FileHistoryType HistoryType { get; set; }
    }

    public enum FileHistoryType
    {
        Created,
        Loaded,
        Updated
    }
}