using coiled_tubing_app.Models;
using System.Collections.Generic;
using System.Linq;

namespace coiled_tubing_app.Services
{
    public static class DeviceService
    {
        // Hardcoded device list - hanya developer yang bisa menambah melalui kode
        private static readonly List<DeviceModel> _hardcodedDevices = new List<DeviceModel>
        {
            new DeviceModel
            {
                Id = 1,
                Name = "SNAP UP1 ADS",
                Manufacturer = "Opto 22",
                Model = "SNAP UP1 ADS",
                Description = "Opto 22 SNAP UP1 Analog/Digital Data Scanner",
                IsActive = true,
                CreatedAt = System.DateTime.Now,
                UpdatedAt = System.DateTime.Now
            },
            //new DeviceModel
            //{
            //    Id = 2,
            //    Name = "PAC Control",
            //    Manufacturer = "Opto 22",
            //    Model = "PAC Control",
            //    Description = "Opto 22 PAC Control System",
            //    IsActive = true,
            //    CreatedAt = System.DateTime.Now,
            //    UpdatedAt = System.DateTime.Now
            //},
            //new DeviceModel
            //{
            //    Id = 3,
            //    Name = "groov EPIC",
            //    Manufacturer = "Opto 22",
            //    Model = "groov EPIC",
            //    Description = "Opto 22 groov EPIC Edge Programmable Industrial Controller",
            //    IsActive = true,
            //    CreatedAt = System.DateTime.Now,
            //    UpdatedAt = System.DateTime.Now
            //}
            
            // Developer bisa menambah device baru di sini
            // Contoh menambah device baru:
            /*
            ,new DeviceModel
            {
                Id = 4,
                Name = "New Device",
                Manufacturer = "Manufacturer Name",
                Model = "Device Model",
                Description = "Device Description",
                IsActive = true,
                CreatedAt = System.DateTime.Now,
                UpdatedAt = System.DateTime.Now
            }
            */
        };

        /// <summary>
        /// Get all active devices from hardcoded list
        /// </summary>
        public static List<DeviceModel> GetAllDevices()
        {
            return _hardcodedDevices
                .Where(d => d.IsActive)
                .OrderBy(d => d.Id)
                .ToList();
        }

        /// <summary>
        /// Get device by ID from hardcoded list
        /// </summary>
        public static DeviceModel GetDeviceById(int id)
        {
            return _hardcodedDevices.FirstOrDefault(d => d.Id == id && d.IsActive);
        }

        /// <summary>
        /// Get device by display name from hardcoded list (for backward compatibility)
        /// </summary>
        public static DeviceModel GetDeviceByName(string name)
        {
            return _hardcodedDevices.FirstOrDefault(d => d.DisplayName == name && d.IsActive);
        }

        /// <summary>
        /// Get device configuration by ID
        /// This method can be extended for device-specific configurations
        /// </summary>
        public static DeviceConfiguration GetDeviceConfiguration(int deviceId)
        {
            var device = GetDeviceById(deviceId);
            if (device == null) return null;

            // Return device-specific configuration based on ID
            return deviceId switch
            {
                1 => new DeviceConfiguration // SNAP UP1 ADS
                {
                    DeviceId = 1,
                    DefaultPort = 2001,
                    DefaultTimeout = 5000,
                    SupportedProtocols = new[] { "Modbus TCP", "Ethernet/IP" },
                    RequiresAuthentication = false,
                    MaxConnections = 1,
                    ConfigurationNotes = "Standard Opto 22 SNAP UP1 ADS configuration"
                },
                2 => new DeviceConfiguration // PAC Control
                {
                    DeviceId = 2,
                    DefaultPort = 22001,
                    DefaultTimeout = 10000,
                    SupportedProtocols = new[] { "OptoMMP", "Modbus TCP" },
                    RequiresAuthentication = true,
                    MaxConnections = 5,
                    ConfigurationNotes = "Opto 22 PAC Control system with authentication"
                },
                3 => new DeviceConfiguration // groov EPIC
                {
                    DeviceId = 3,
                    DefaultPort = 502,
                    DefaultTimeout = 8000,
                    SupportedProtocols = new[] { "Modbus TCP", "OptoMMP", "MQTT" },
                    RequiresAuthentication = false,
                    MaxConnections = 10,
                    ConfigurationNotes = "Opto 22 groov EPIC with multiple protocol support"
                },
                // Developer bisa menambah konfigurasi untuk device baru di sini
                _ => new DeviceConfiguration
                {
                    DeviceId = deviceId,
                    DefaultPort = 502,
                    DefaultTimeout = 5000,
                    SupportedProtocols = new[] { "Unknown" },
                    RequiresAuthentication = false,
                    MaxConnections = 1,
                    ConfigurationNotes = "Default configuration for unknown device"
                }
            };
        }

        /// <summary>
        /// Check if device ID exists and is active
        /// </summary>
        public static bool IsValidDeviceId(int deviceId)
        {
            return _hardcodedDevices.Any(d => d.Id == deviceId && d.IsActive);
        }

        /// <summary>
        /// Get all device IDs (for validation purposes)
        /// </summary>
        public static int[] GetAllDeviceIds()
        {
            return _hardcodedDevices
                .Where(d => d.IsActive)
                .Select(d => d.Id)
                .OrderBy(id => id)
                .ToArray();
        }

        /// <summary>
        /// Developer guide for adding new devices
        /// </summary>
        public static string GetDeveloperGuide()
        {
            return @"
DEVELOPER GUIDE - Adding New Device:

1. Add new DeviceModel to _hardcodedDevices list with unique ID
2. Add device configuration in GetDeviceConfiguration() method  
3. Implement device-specific logic in connection handlers
4. Update any device-specific UI logic if needed
5. Test thoroughly with new device

Example:
- Add to _hardcodedDevices: new DeviceModel { Id = 4, Name = ""MyDevice"", ... }
- Add to GetDeviceConfiguration(): case 4 => new DeviceConfiguration { ... }
- Implement device-specific connection logic
";
        }
    }

    /// <summary>
    /// Device-specific configuration class
    /// </summary>
    public class DeviceConfiguration
    {
        public int DeviceId { get; set; }
        public int DefaultPort { get; set; }
        public int DefaultTimeout { get; set; }
        public string[] SupportedProtocols { get; set; } = new string[0];
        public bool RequiresAuthentication { get; set; }
        public int MaxConnections { get; set; }
        public string ConfigurationNotes { get; set; } = string.Empty;
    }
}