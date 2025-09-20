using System;

namespace coiled_tubing_app.Models
{
    public class ConnectionSettings
    {
        public string Host { get; set; } = string.Empty;
        public int Port { get; set; }
        public int Timeout { get; set; }
        public DateTime LastConnected { get; set; } = DateTime.Now;
        public bool IsSuccessful { get; set; }
        public int ResultCode { get; set; }
    }
}