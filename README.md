# Coiled Tubing Operations App

A Windows application for monitoring and managing coiled tubing operations in the oil and gas industry. Built with .NET 8 and WinUI 3, it provides real-time data visualization, sensor monitoring, and operational management.

## Features

- Real-time dashboard: temperature, pressure, flow rates, drilling progress
- Interactive charts using LiveCharts: line, column, pie, area, scatter
- Sensor management and historical data analysis
- Hardware integration via OptoMMP
- Record system: save and load operational records with history
- General data management for operations and wells
- Connection settings for hardware
- File history tracking
- Login window for basic access control

## Technology Stack

Core technologies
- .NET 8.0
- WinUI 3
- C#
- XAML

Libraries and dependencies
- LiveChartsCore.SkiaSharpView.WinUI (2.0.0-rc5.3)
- SkiaSharp (3.119.0)
- SkiaSharp.Views.WinUI (3.119.0)
- CommunityToolkit.Mvvm (8.4.0)
- NLog (6.0.4)
- Microsoft.WindowsAppSDK (1.7.250606001)
- OptoMMP_Standard_2_0 (external DLL for hardware integration)

## System Requirements

Minimum
- Windows 10 version 1903 (build 18362) or later
- 64-bit OS (x64 or ARM64)
- Target platform: Windows 10.0.19041.0 (minimum 10.0.17763.0)
- .NET 8.0 Desktop Runtime
- 4 GB RAM (8 GB recommended)
- 500 MB free disk space
- OpenGL 2.0 compatible graphics

Additional
- Microsoft Visual C++ 2015-2022 Redistributable (x64)
- Windows App SDK Runtime

## Installation and Setup

1) Clone the repository

   git clone https://github.com/fauzan05/coiled-tubing-app.git
   cd coiled-tubing-app

2) Install .NET 8 SDK
- Download from https://dotnet.microsoft.com/download/dotnet/8.0
- Verify: dotnet --version (should print 8.0.x)

3) Restore dependencies

   cd coiled-tubing-app
   dotnet restore

4) Optional: hardware integration setup
- Ensure OptoMMP_Standard_2_0.dll is available
- The project references a local path in the .csproj; adjust the HintPath if needed

5) Build

   dotnet build --configuration Release

6) Run

   dotnet run --project coiled-tubing-app

## Quick Start

- Run the app via Visual Studio or dotnet run
- Login window appears; sign in
- The dashboard shows multiple operational charts

## Using the Dashboard

- Temperature monitoring: downhole temperature trends
- Pressure analysis: surface and downhole comparison
- Well status overview: distribution of states
- Flow rate trends: oil and gas
- Equipment efficiency: scatter plot
- Drilling progress: current vs target depth

## Recording Data

- Open the Sensor page
- Add Record to create a new operational record and select charts
- Save records and load them later
- Use history actions to refresh or delete entries

## Managing Connections

- Open Connection settings
- Configure device parameters and OptoMMP connectivity

## Project Structure

coiled-tubing-app/
  coiled-tubing-app/
    Models/
      ChartRecord.cs
      ChartItem.cs
      FileHistoryItem.cs
      GeneralData.cs
      HistoryTableItem.cs
    Services/
      ChartService.cs
      FileHistoryService.cs
      SimpleFileHistoryService.cs
    ViewModels/
      ChartViewModel.cs
      DashboardViewModel.cs
    Pages and Views/
      DashboardPage.xaml
      SensorPage.xaml
      LoginPage.xaml
      LoginWindow.xaml
      MainWindow.xaml
    Dialogs/
      ChartSelectionDialog.xaml
      ConnectionDialog.cs
      GeneralDataDialog.cs
    App.xaml.cs
  README.md

## Troubleshooting

Build errors
- Run: dotnet clean, dotnet restore, dotnet build

Missing dependencies
- Run: dotnet restore --force
- Reinstall .NET 8 Desktop Runtime and Windows App SDK Runtime

Runtime or rendering issues
- Verify Windows version
- Update graphics drivers (SkiaSharp depends on GPU stack)

Hardware connection issues
- Verify OptoMMP library location and drivers
- Check device connectivity settings

Logging
- NLog is used for logging
- See Visual Studio Output window and configured log targets (if present)

## Contributing

- Fork the repository
- Create a feature branch (git checkout -b feature/name)
- Commit and push changes
- Open a Pull Request

Guidelines
- Follow C# coding conventions
- Use MVVM for UI code
- Add tests where applicable
- Update documentation when public behavior changes

## License

This project is licensed under the MIT License. See the LICENSE file if included.

## Support

- GitHub issues: https://github.com/fauzan05/coiled-tubing-app/issues

## Version History

- v1.0.0 - Initial release with dashboard and monitoring features

## Acknowledgments

- LiveChartsCore
- Microsoft Windows App SDK and WinUI 3
- SkiaSharp
- CommunityToolkit.Mvvm