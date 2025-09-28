# Connection Loading & Logging System

## Overview
Sistem untuk menampilkan animasi loading dan log informasi detail saat proses koneksi device di RecordingPage.

## Features Implemented

### ? **Loading Animation**
- Progress ring dengan animasi saat proses koneksi
- Loading text "Connecting..." 
- Duration sesuai dengan timeout yang diberikan
- Auto-hide setelah proses selesai

### ? **Device Information Log**
- Real-time logging selama proses koneksi
- Timestamp untuk setiap log entry
- Connection status (success/failed) ditampilkan di log
- Device-specific information
- Error descriptions yang user-friendly
- Auto-scroll dan log management (max 20 entries)

### ? **Enhanced Connection Feedback**
- Success message dengan duration info
- Error messages dengan error codes dan descriptions
- Exception handling dengan detailed error info
- Professional dialogs untuk success/error feedback

## How It Works

### **UI Components**
```csharp
// Loading indicator
ProgressRing progressRing = new ProgressRing();
StackPanel loadingPanel = new StackPanel(); // Contains progress ring + text

// Device log
TextBlock logTextBlock = new TextBlock();
logTextBlock.Text = "Device Information Log\nReady for connection...";
```

### **Connection Process**
1. **User clicks Connect button**
2. **Loading starts**: Progress ring activated, connect button disabled
3. **Log updates**: Real-time status updates in Device Information Log
4. **Connection attempt**: Based on device ID (professional approach)
5. **Result handling**: Success/error displayed in both log and dialog
6. **Loading ends**: Progress ring deactivated, connect button re-enabled

### **Log Examples**

#### **Successful Connection**
```
Device Information Log
[14:23:45] Attempting to connect to 192.168.1.100:502...
[14:23:45] Connecting to Opto 22 - SNAP UP1 ADS...
[14:23:45] Host: 192.168.1.100, Port: 502, Timeout: 5000ms
[14:23:45] Using connection method for Opto 22 - SNAP UP1 ADS
[14:23:45] Initializing MMP6 connection...
[14:23:46] MMP6 protocol handshake completed
[14:23:46] ? Connection successful! (1247ms)
[14:23:46] Device: Opto 22 - SNAP UP1 ADS  
[14:23:46] Status: Connected and ready
[14:23:46] ????????????????????????????
```

#### **Failed Connection**
```
Device Information Log
[14:25:12] Attempting to connect to 192.168.1.200:502...
[14:25:12] Connecting to Opto 22 - SNAP UP1 ADS...
[14:25:12] Host: 192.168.1.200, Port: 502, Timeout: 5000ms
[14:25:12] Using connection method for Opto 22 - SNAP UP1 ADS
[14:25:12] Initializing MMP6 connection...
[14:25:17] MMP6 connection failed with code 3
[14:25:17] ? Connection failed! Error code: 3 (5123ms)
[14:25:17] Connection refused by remote host
[14:25:17] ????????????????????????????
```

## Technical Implementation

### **Key Methods**

#### **SetConnectionLoading()**
```csharp
private void SetConnectionLoading(TextBlock logTextBlock, StackPanel loadingPanel, ProgressRing progressRing, bool isLoading)
{
    if (isLoading)
    {
        loadingPanel.Visibility = Visibility.Visible;
        progressRing.IsActive = true;
    }
    else
    {
        loadingPanel.Visibility = Visibility.Collapsed;
        progressRing.IsActive = false;
    }
}
```

#### **LogToDeviceLog()**
```csharp
private void LogToDeviceLog(TextBlock logTextBlock, string message)
{
    var timestamp = DateTime.Now.ToString("HH:mm:ss");
    var logEntry = $"[{timestamp}] {message}";
    
    // Append with timestamp and manage log size (max 20 entries)
    // ...
}
```

#### **ConnectDeviceWithUI()** 
```csharp
private async Task ConnectDeviceWithUI(Window currentWindow, ConnectionSettings connectionSettings, 
    TextBlock logTextBlock, StackPanel loadingPanel, ProgressRing progressRing)
{
    // 1. Start loading animation
    // 2. Log connection attempt details
    // 3. Perform connection based on device ID
    // 4. Handle success/failure with detailed logging
    // 5. Stop loading animation
    // 6. Show result dialog
}
```

### **Device-Specific Connection Logic**
```csharp
private async Task<int> ConnectByDeviceId(ConnectionSettings settings, TextBlock logTextBlock)
{
    switch (settings.DeviceId)
    {
        case 1: // Opto 22 SNAP UP1 ADS
        case 2: // Opto 22 PAC Control  
        case 3: // Opto 22 groov EPIC
            return await MMP6_Connect(settings, logTextBlock);
        
        // Future device implementations
        // case 4: return await ConnectAllenBradleyPLC(settings, logTextBlock);
        
        default:
            return await MMP6_Connect(settings, logTextBlock);
    }
}
```

## Benefits

### ? **Professional User Experience**
- Visual feedback selama proses koneksi
- Clear indication of what's happening
- Detailed information untuk troubleshooting
- Professional error handling

### ? **Developer-Friendly Logging** 
- Comprehensive debug information
- Easy to extend untuk device baru
- Consistent logging pattern
- Error descriptions yang helpful

### ? **Robust Connection Handling**
- Timeout handling sesuai device configuration
- Device-specific connection methods
- Graceful error recovery
- Connection duration tracking

## Usage for End Users

1. **Select device** dari dropdown
2. **Fill connection details** (host, port, timeout)
3. **Click Connect button**
4. **Watch loading animation** dan log updates
5. **See result** dalam log dan dialog popup
6. **Connection successful** = ready untuk operations
7. **Connection failed** = check log untuk troubleshooting info

## Future Enhancements

- ? **Real-time connection monitoring**
- ? **Connection retry logic**  
- ? **Save/load connection profiles**
- ? **Connection health indicators**
- ? **Auto-reconnect functionality**

---

Sistem ini memberikan feedback yang professional dan informatif untuk proses koneksi device, making troubleshooting much easier untuk end users dan developers.