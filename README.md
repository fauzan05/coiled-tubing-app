# Coiled Tubing Operations App

A comprehensive Windows application for monitoring and managing coiled tubing operations in the oil and gas industry. Built with .NET 8 and WinUI 3, this application provides real-time data visualization, sensor monitoring, and operational management capabilities.

## ⚙️ Features

- **Real-time Dashboard**: Monitor temperature, pressure, flow rates, and drilling progress
- **Advanced Data Visualization**: Interactive charts using LiveCharts with multiple chart types (line, column, pie, area, scatter)
- **Sensor Management**: Real-time sensor data monitoring and historical data analysis
- **Equipment Integration**: OptoMMP hardware integration for industrial control systems
- **Data Recording**: Save and load operational records with comprehensive history management
- **General Data Management**: Track operational parameters and well information
- **Connection Management**: Configure and manage hardware connections
- **File History System**: Comprehensive tracking of all operational records
- **User Authentication**: Secure login system for operational access

## 💻 Technology Stack

### Core Technologies
- **.NET 8.0** - Latest .NET framework for Windows applications
- **WinUI 3** - Modern Windows UI framework
- **C#** - Primary programming language
- **XAML** - UI markup language

### Key Libraries & Dependencies
- **LiveChartsCore.SkiaSharpView.WinUI** (v2.0.0-rc5.3) - Advanced charting and data visualization
- **SkiaSharp** (v3.119.0) - Cross-platform 2D graphics
- **SkiaSharp.Views.WinUI** (v3.119.0) - SkiaSharp integration for WinUI
- **CommunityToolkit.Mvvm** (v8.4.0) - MVVM toolkit for modern app development
- **NLog** (v6.0.4) - Advanced logging framework
- **Microsoft.WindowsAppSDK** (v1.7.250606001) - Windows App SDK
- **OptoMMP_Standard_2_0** - Industrial hardware integration library

## ⚡ System Requirements

### Minimum Requirements
- **Operating System**: Windows 10 version 1903 (build 18362) or later
- **Architecture**: 64-bit (x64, ARM64 supported)
- **Target Platform**: Windows 10.0.19041.0 (minimum: 10.0.17763.0)
- **.NET Runtime**: .NET 8.0 Desktop Runtime
- **Memory**: 4 GB RAM (8 GB recommended)
- **Storage**: 500 MB available disk space
- **Graphics**: OpenGL 2.0 compatible graphics card

### Additional Requirements
- Microsoft Visual C++ 2015-2022 Redistributable (x64)
- Windows App SDK Runtime

## 📦 Installation & Setup

### 1. Clone the Repository
```bash
git clone https://github.com/fauzan05/coiled-tubing-app.git
cd coiled-tubing-app
```

### 2. Install Prerequisites

#### Install .NET 8 SDK
Download and install the .NET 8 SDK from [Microsoft's official website](https://dotnet.microsoft.com/download/dotnet/8.0).

#### Verify Installation
```bash
dotnet --version
```
Should return version 8.0.x or higher.

### 3. Restore Dependencies
Navigate to the project directory and restore NuGet packages:
```bash
cd coiled-tubing-app
dotnet restore
```

### 4. Hardware Integration Setup (Optional)
If you plan to use hardware integration features:
1. Ensure OptoMMP_Standard_2_0.dll is available in your system
2. The application expects this DLL in the downloads folder as referenced in the project
3. Contact your hardware vendor for the latest OptoMMP libraries

### 5. Build the Application
```bash
dotnet build --configuration Release
```

### 6. Run the Application
```bash
dotnet run --project coiled-tubing-app
```

## 🚀 Quick Start Guide

### First Launch
1. Launch the application from Visual Studio or using `dotnet run`
2. The login window will appear - enter your credentials
3. Once authenticated, you'll see the main dashboard with operational charts

### Using the Dashboard
- **Temperature Monitoring**: Real-time downhole temperature tracking
- **Pressure Analysis**: Surface and downhole pressure comparison
- **Well Status Overview**: Pie chart showing active, maintenance, standby, and offline wells
- **Flow Rate Trends**: Oil and gas flow rate monitoring
- **Equipment Efficiency**: Scatter plot of equipment performance
- **Drilling Progress**: Current vs target depth visualization

### Recording Data
1. Navigate to the "Sensor" page
2. Click "Add Record" to create new operational records
3. Select charts to include in your record
4. Save records for future analysis
5. Use "Load Record" to view historical data

### Managing Connections
1. Click the "Connection" button to configure hardware connections
2. Set up your OptoMMP device connections
3. Configure communication parameters

## 📁 Project Structure

📂 coiled-tubing-app  
 ┣ 📂 Models  
 ┃ ┣ 📄 ChartRecord.cs  
 ┃ ┣ 📄 ChartItem.cs  
 ┃ ┣ 📄 FileHistoryItem.cs  
 ┃ ┣ 📄 GeneralData.cs  
 ┃ ┗ 📄 HistoryTableItem.cs  
 ┣ 📂 Services  
 ┃ ┣ 📄 ChartService.cs  
 ┃ ┣ 📄 FileHistoryService.cs  
 ┃ ┗ 📄 SimpleFileHistoryService.cs  
 ┣ 📂 ViewModels  
 ┃ ┣ 📄 ChartViewModel.cs  
 ┃ ┗ 📄 DashboardViewModel.cs  
 ┣ 📂 Assets  
 ┣ 📂 Pages & Views  
 ┃ ┣ 📄 DashboardPage.xaml  
 ┃ ┣ 📄 SensorPage.xaml  
 ┃ ┣ 📄 LoginPage.xaml  
 ┃ ┣ 📄 LoginWindow.xaml  
 ┃ ┗ 📄 MainWindow.xaml  
 ┣ 📂 Dialogs  
 ┃ ┣ 📄 ChartSelectionDialog.xaml  
 ┃ ┣ 📄 ConnectionDialog.cs  
 ┃ ┗ 📄 GeneralDataDialog.cs  
 ┣ 📄 App.xaml.cs  
 ┗ 📄 README.md  

## 💡 Key Features Explained

### Data Visualization
The application uses LiveCharts for creating interactive, real-time charts:
- **Line Charts**: Temperature and depth monitoring
- **Column Charts**: Pressure analysis
- **Pie Charts**: Well status distribution
- **Area Charts**: Flow rate trends
- **Scatter Plots**: Equipment efficiency metrics

### File Management
- **Record System**: Save operational snapshots with selected charts
- **History Tracking**: Comprehensive file history with metadata
- **Export/Import**: Load previously saved records for analysis

### Hardware Integration
- **OptoMMP Support**: Connect to industrial control hardware
- **Real-time Data**: Live sensor data acquisition
- **Connection Management**: Configure multiple device connections

## 🔧 Troubleshooting

### Common Issues

#### Build Errors
```bash
# Clean and rebuild
dotnet clean
dotnet restore
dotnet build
```

#### Missing Dependencies
```bash
# Restore NuGet packages
dotnet restore --force
```

#### Runtime Errors
1. Ensure .NET 8 Desktop Runtime is installed
2. Check Windows version compatibility
3. Verify graphics driver support for SkiaSharp

#### Hardware Connection Issues
1. Verify OptoMMP library installation
2. Check device connection settings
3. Ensure proper driver installation

### Logging
The application uses NLog for comprehensive logging. Check logs for detailed error information:
- Debug output in Visual Studio Output window
- Application logs (location configurable via NLog configuration)

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

### Development Guidelines
- Follow C# coding conventions
- Use MVVM pattern for UI components
- Add unit tests for new features
- Update documentation for API changes

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 💬 Support

- **Repository**: [GitHub Issues](https://github.com/fauzan05/coiled-tubing-app/issues)
- **Documentation**: Check the `/docs` folder for detailed documentation
- **Community**: Join our discussions in GitHub Discussions

## 📅 Version History

- **v1.0.0** - Initial release with core dashboard and monitoring features
- See [CHANGELOG.md](CHANGELOG.md) for detailed version history

## 🙏 Acknowledgments

- LiveCharts team for excellent charting library
- Microsoft for WinUI 3 and .NET 8
- SkiaSharp team for cross-platform graphics
- Community contributors

---

**Note**: This application is designed for professional oil and gas operations. Ensure proper training and safety protocols when using with actual equipment.
