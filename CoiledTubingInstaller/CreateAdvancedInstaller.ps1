# PowerShell script to create Advanced Installer project
# This creates a professional installer with modern UI

$ProjectName = "CoiledTubingApp"
$AppName = "Coiled Tubing Operations App"
$Version = "1.0.0"
$Publisher = "Your Company Name"
$InstallDir = "[ProgramFilesFolder]Coiled Tubing App"

@"
<?xml version="1.0" encoding="UTF-8" standalone="yes"?>
<DOCUMENT Type="Advanced Installer" CreateVersion="20.0" version="20.0" Modules="simple" RootPath="." Language="en" Id="{12345678-1234-1234-1234-123456789012}">
  <COMPONENT cid="caphyon.advinst.msicomp.ProjectOptionsComponent">
    <ROW Name="HiddenItems" Value="AppXProductDetailsComponent;AppXDependenciesComponent;AppXAppDeclarationsComponent;AppXCapabilitiesComponent;AppXAppExtensionsComponent;AppXAdvancedOptionsComponent;AppXVisualAssetsComponent;AppXLanguagesComponent"/>
  </COMPONENT>
  <COMPONENT cid="caphyon.advinst.msicomp.MsiPropsComponent">
    <ROW Property="AI_BITMAP_DISPLAY_MODE" Value="0"/>
    <ROW Property="AI_CURRENT_YEAR" Value="2024" ValueLocId="-"/>
    <ROW Property="AI_VERSION_EDIT_DISABLED" Value="1"/>
    <ROW Property="ALLUSERS" Value="1"/>
    <ROW Property="ARPCONTACT" Value="support@yourcompany.com"/>
    <ROW Property="ARPHELPLINK" Value="http://www.yourcompany.com/support"/>
    <ROW Property="ARPURLINFOABOUT" Value="http://www.yourcompany.com"/>
    <ROW Property="Manufacturer" Value="$Publisher"/>
    <ROW Property="ProductCode" Value="1033:{12345678-1234-1234-1234-123456789012} "/>
    <ROW Property="ProductLanguage" Value="1033"/>
    <ROW Property="ProductName" Value="$AppName"/>
    <ROW Property="ProductVersion" Value="$Version"/>
    <ROW Property="REBOOT" Value="ReallySuppress"/>
    <ROW Property="UpgradeCode" Value="{12345678-1234-1234-1234-123456789013}"/>
    <ROW Property="WindowsType9X" MultiBuildValue="AnyCPU_Release:Windows 9x/ME" ValueLocId="-"/>
    <ROW Property="WindowsTypeNT40" MultiBuildValue="AnyCPU_Release:Windows NT 4.0" ValueLocId="-"/>
    <ROW Property="WindowsTypeNT50" MultiBuildValue="AnyCPU_Release:Windows 2000" ValueLocId="-"/>
    <ROW Property="WindowsTypeNT5X" MultiBuildValue="AnyCPU_Release:Windows XP/2003" ValueLocId="-"/>
  </COMPONENT>
  <COMPONENT cid="caphyon.advinst.msicomp.MsiDirsComponent">
    <ROW Directory="APPDIR" Directory_Parent="TARGETDIR" DefaultDir="APPDIR:." IsPseudoRoot="1"/>
    <ROW Directory="CoiledTubingApp_Dir" Directory_Parent="ProgramFilesFolder" DefaultDir="COILED~1|Coiled Tubing App"/>
    <ROW Directory="DesktopFolder" Directory_Parent="TARGETDIR" DefaultDir="DESKTO~1|DesktopFolder" IsPseudoRoot="1"/>
    <ROW Directory="ProgramFilesFolder" Directory_Parent="TARGETDIR" DefaultDir="PROGRA~1|ProgramFilesFolder" IsPseudoRoot="1"/>
    <ROW Directory="ProgramMenuFolder" Directory_Parent="TARGETDIR" DefaultDir="PROGRA~2|ProgramMenuFolder" IsPseudoRoot="1"/>
    <ROW Directory="SHORTCUTDIR" Directory_Parent="ProgramMenuFolder" DefaultDir="SHORTC~1|Coiled Tubing App"/>
    <ROW Directory="TARGETDIR" DefaultDir="SourceDir"/>
  </COMPONENT>
  <COMPONENT cid="caphyon.advinst.msicomp.MsiCompsComponent">
    <ROW Component="APPDIR" ComponentId="{12345678-1234-1234-1234-123456789014}" Directory_="APPDIR" Attributes="0"/>
    <ROW Component="DesktopShortcut" ComponentId="{12345678-1234-1234-1234-123456789015}" Directory_="DesktopFolder" Attributes="4"/>
    <ROW Component="MainExecutable" ComponentId="{12345678-1234-1234-1234-123456789016}" Directory_="CoiledTubingApp_Dir" Attributes="256" KeyPath="coiled_tubing_app.exe"/>
    <ROW Component="ProgramMenuShortcut" ComponentId="{12345678-1234-1234-1234-123456789017}" Directory_="SHORTCUTDIR" Attributes="4"/>
  </COMPONENT>
  <COMPONENT cid="caphyon.advinst.msicomp.MsiFeatsComponent">
    <ROW Feature="MainFeature" Title="Main Feature" Description="Main Feature" Display="1" Level="1" Directory_="APPDIR" Attributes="0"/>
    <ROW Feature="DesktopShortcut" Title="Desktop Shortcut" Description="Desktop Shortcut" Display="3" Level="1000" Directory_="APPDIR" Attributes="0"/>
  </COMPONENT>
  <COMPONENT cid="caphyon.advinst.msicomp.MsiFilesComponent">
    <ROW File="coiled_tubing_app.exe" Component_="MainExecutable" FileName="COILED~1.EXE|coiled-tubing-app.exe" Attributes="0" SourcePath="../coiled-tubing-app/bin/Release/net8.0-windows10.0.19041.0/win-x64/publish/coiled-tubing-app.exe" SelfReg="false" DigSign="true"/>
  </COMPONENT>
  <COMPONENT cid="caphyon.advinst.msicomp.BootstrOptComponent">
    <ROW BootstrOptKey="GlobalOptions" DownloadFolder="[AppDataFolder][|Manufacturer]\[|ProductName]\prerequisites" Options="2"/>
  </COMPONENT>
  <COMPONENT cid="caphyon.advinst.msicomp.BootstrapperUISequenceComponent">
    <ROW Action="AI_DetectSoftware" Sequence="151"/>
    <ROW Action="AI_DpiContentScale" Sequence="52"/>
    <ROW Action="AI_EnableDebugLog" Sequence="51"/>
  </COMPONENT>
</DOCUMENT>
"@ | Out-File -FilePath "CoiledTubingApp.aip" -Encoding UTF8

Write-Host "Advanced Installer project created: CoiledTubingApp.aip"
Write-Host "Open this file with Advanced Installer to customize the installer further."