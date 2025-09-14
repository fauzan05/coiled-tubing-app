using System;
using System.Collections.Generic;

namespace coiled_tubing_app.Models
{
    public class HistoryTableItem
    {
        public int RowNumber { get; set; }
        public string RecordName { get; set; } = string.Empty;
        public string ChartsPreview { get; set; } = string.Empty;
        public DateTime LastAccessed { get; set; }
        public string FormattedDate => LastAccessed.ToString("yyyy-MM-dd HH:mm");
        public string FilePath { get; set; } = string.Empty;
        public string Directory { get; set; } = string.Empty;
        public FileHistoryType HistoryType { get; set; }
        public List<string> SelectedChartIds { get; set; } = new List<string>();
        
        // Original FileHistoryItem for reference
        public FileHistoryItem OriginalItem { get; set; }
    }
}