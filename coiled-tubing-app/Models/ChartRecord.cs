using System;
using System.Collections.Generic;

namespace coiled_tubing_app.Models
{
    public class ChartRecord
    {
        public string RecordName { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public List<string> SelectedChartIds { get; set; } = new List<string>();
    }
}