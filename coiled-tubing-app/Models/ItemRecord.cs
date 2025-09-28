using System;

namespace coiled_tubing_app.Models
{
    public class ItemRecord
    {
        public string RecordName { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public ConnectionSettings? ConnectionSettings { get; set; }
    }
}