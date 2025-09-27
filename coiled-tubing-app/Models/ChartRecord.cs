using System;

namespace coiled_tubing_app.Models
{
    public class ChartRecord
    {
        public string RecordName { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; } = DateTime.Now;
    }
}