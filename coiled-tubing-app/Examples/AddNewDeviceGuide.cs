/* 
 * DEVELOPER GUIDE: How to Add New Device
 * =====================================
 * 
 * This file shows step-by-step process untuk menambahkan device baru
 * ke dalam sistem hardcoded device management.
 */

namespace coiled_tubing_app.Examples
{
    // STEP 1: Add Device Model to DeviceService._hardcodedDevices
    // ==========================================================
    /*
    Edit file: Services/DeviceService.cs
    
    private static readonly List<DeviceModel> _hardcodedDevices = new List<DeviceModel>
    {
        // ... existing devices ...
        
        // ADD YOUR NEW DEVICE HERE:
        new DeviceModel
        {
            Id = 4,                                    // Use next available ID
            Name = "PLC Controller",                   // Short device name
            Manufacturer = "Allen-Bradley",            // Manufacturer name  
            Model = "CompactLogix 5370",              // Specific model
            Description = "Allen-Bradley CompactLogix 5370 PLC Controller with Ethernet/IP", 
            IsActive = true,
            CreatedAt = System.DateTime.Now,
            UpdatedAt = System.DateTime.Now
        }
    };
    */

    // STEP 2: Add Device Configuration 
    // =================================
    /*
    Edit method GetDeviceConfiguration() in Services/DeviceService.cs
    
    return deviceId switch
    {
        // ... existing cases ...
        
        // ADD YOUR DEVICE CONFIGURATION HERE:
        4 => new DeviceConfiguration // Allen-Bradley PLC
        {
            DeviceId = 4,
            DefaultPort = 44818,                      // EtherNet/IP default port
            DefaultTimeout = 10000,                   // 10 seconds timeout
            SupportedProtocols = new[] { "EtherNet/IP", "CIP" },
            RequiresAuthentication = false,           // No auth required
            MaxConnections = 8,                       // Multiple connections supported
            ConfigurationNotes = "Allen-Bradley CompactLogix with EtherNet/IP. Use slot 0 for CPU communication."
        },
        
        // Add more devices as needed...
        5 => new DeviceConfiguration // Siemens S7
        {
            DeviceId = 5,
            DefaultPort = 102,                        // S7 Communication port
            DefaultTimeout = 8000,                    
            SupportedProtocols = new[] { "S7", "TCP" },
            RequiresAuthentication = true,            // May require authentication
            MaxConnections = 4,                       
            ConfigurationNotes = "Siemens S7 PLC with S7 protocol. Configure rack and slot numbers."
        },
        
        // ... default case ...
    };
    */

    // STEP 3: Implement Device-Specific Logic
    // =======================================
    /*
    Create device-specific connection logic based on device type:
    
    Example for connection handling:
    
    private async Task<bool> ConnectToDevice(int deviceId, ConnectionSettings settings)
    {
        switch (deviceId)
        {
            case 1: // Opto 22 SNAP
                return await ConnectOpto22Snap(settings);
                
            case 2: // Opto 22 PAC
                return await ConnectOpto22PAC(settings);
                
            case 3: // Opto 22 groov EPIC
                return await ConnectGroovEPIC(settings);
                
            case 4: // Allen-Bradley PLC
                return await ConnectAllenBradleyPLC(settings);
                
            case 5: // Siemens S7
                return await ConnectSiemensS7(settings);
                
            default:
                throw new NotSupportedException($"Device ID {deviceId} not supported");
        }
    }
    
    private async Task<bool> ConnectAllenBradleyPLC(ConnectionSettings settings)
    {
        try
        {
            // Implement Allen-Bradley specific connection logic
            // Example: Using EtherNet/IP library
            
            var plc = new AllenBradleyPLC();
            plc.IPAddress = settings.Host;
            plc.Port = settings.Port;
            plc.Timeout = settings.Timeout;
            
            return await plc.ConnectAsync();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Allen-Bradley PLC connection failed: {ex.Message}");
            return false;
        }
    }
    */

    // STEP 4: Update UI Logic (if needed)
    // ===================================
    /*
    If device requires special UI handling, update relevant pages:
    
    Example: Device-specific validation or UI elements
    
    private bool ValidateDeviceSettings(int deviceId, ConnectionSettings settings)
    {
        switch (deviceId)
        {
            case 4: // Allen-Bradley PLC
                if (settings.Port != 44818 && settings.Port != 2222)
                {
                    ShowError("Allen-Bradley PLC typically uses port 44818 or 2222");
                    return false;
                }
                break;
                
            case 5: // Siemens S7
                if (string.IsNullOrEmpty(settings.Host))
                {
                    ShowError("Host IP address is required for Siemens S7");
                    return false;
                }
                break;
        }
        return true;
    }
    */

    // STEP 5: Testing Your New Device
    // ===============================
    /*
    1. Build the application
    2. Run and navigate to Connect Device dialog
    3. Verify new device appears in dropdown
    4. Select new device and verify default values are applied
    5. Click "Device Info" to verify device information is displayed correctly
    6. Test actual connection with real device (if available)
    7. Verify connection settings are saved correctly
    8. Test backward compatibility with existing connection data
    */

    // DEVICE ID REFERENCE
    // ==================
    /*
    Current device IDs:
    1 = Opto 22 - SNAP UP1 ADS
    2 = Opto 22 - PAC Control  
    3 = Opto 22 - groov EPIC
    4 = [Available] - Your new device
    5 = [Available] - Your next device
    
    Always use sequential IDs and never reuse deleted IDs to maintain data integrity.
    */

    // COMMON DEVICE TYPES AND CONFIGURATIONS
    // =====================================
    /*
    Modbus TCP Devices:
    - Default Port: 502
    - Protocol: "Modbus TCP"
    - Authentication: Usually false
    - Timeout: 3000-5000ms
    
    EtherNet/IP Devices (Allen-Bradley):
    - Default Port: 44818 or 2222
    - Protocol: "EtherNet/IP", "CIP"
    - Authentication: false
    - Timeout: 8000-15000ms
    
    Siemens S7:
    - Default Port: 102
    - Protocol: "S7", "TCP"
    - Authentication: May be required
    - Timeout: 5000-10000ms
    
    OPC UA:
    - Default Port: 4840
    - Protocol: "OPC UA"
    - Authentication: Usually required
    - Timeout: 10000-30000ms
    */
}