; =============================================================================
; Coiled Tubing Operations App - Professional Installer Script
; Created with Inno Setup 6.5.3
; =============================================================================

[Setup]
; Application Information
AppId={{A1B2C3D4-E5F6-7890-ABCD-1234567890AB}
AppName=Coiled Tubing Operations App
AppVersion=1.0.0
AppVerName=Coiled Tubing Operations App v1.0.0
AppPublisher=Your Company Name
AppPublisherURL=https://www.yourcompany.com/
AppSupportURL=https://www.yourcompany.com/support
AppUpdatesURL=https://www.yourcompany.com/updates
AppCopyright=Copyright (C) 2024 Your Company Name
AppComments=Professional Coiled Tubing Operations Management Software

; Installation Settings
DefaultDirName={autopf}\Coiled Tubing App
DefaultGroupName=Coiled Tubing App
AllowNoIcons=yes
LicenseFile=License.txt
InfoBeforeFile=ReadMe.txt
InfoAfterFile=PostInstall.txt
OutputDir=Output
OutputBaseFilename=CoiledTubingApp-Setup-v1.0.0
SetupIconFile=Assets\app.ico
UninstallDisplayIcon={app}\coiled-tubing-app.exe

; Compression and Performance
Compression=lzma2/ultra64
SolidCompression=yes
InternalCompressLevel=ultra64
LZMAUseSeparateProcess=yes
LZMADictionarySize=1048576

; Visual Style and Appearance
WizardStyle=modern
WizardImageFile=Assets\WizardImage.bmp
WizardSmallImageFile=Assets\WizardSmallImage.bmp
WizardImageStretch=no
WizardImageBackColor=$FFFFFF
SetupLogging=yes

; System Requirements
MinVersion=6.1sp1
OnlyBelowVersion=0,99
ArchitecturesAllowed=x64
ArchitecturesInstallIn64BitMode=x64

; Security and Privileges
PrivilegesRequired=admin
PrivilegesRequiredOverridesAllowed=dialog
AllowUNCPath=no
RestartIfNeededByRun=no

; Uninstall Options
UninstallDisplayName=Coiled Tubing Operations App
UninstallFilesDir={app}\Uninstall
CreateUninstallRegKey=yes
UninstallRestartComputer=no

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"
Name: "indonesian"; MessagesFile: "compiler:Languages\Indonesian.isl"

[Messages]
english.WelcomeLabel1=Welcome to the Coiled Tubing Operations App Setup Wizard
english.WelcomeLabel2=This will install [name/ver] on your computer.%n%nThis professional software provides comprehensive management for coiled tubing operations including real-time monitoring, data analysis, and operational control.%n%nIt is recommended that you close all other applications before continuing.
indonesian.WelcomeLabel1=Selamat datang di Wizard Instalasi Aplikasi Operasi Coiled Tubing
indonesian.WelcomeLabel2=Ini akan menginstal [name/ver] di komputer Anda.%n%nPerangkat lunak profesional ini menyediakan manajemen komprehensif untuk operasi coiled tubing termasuk pemantauan real-time, analisis data, dan kontrol operasional.%n%nDisarankan agar Anda menutup semua aplikasi lain sebelum melanjutkan.

[CustomMessages]
english.LaunchProgram=Launch Coiled Tubing App
english.CreateDesktopIcon=Create a &desktop icon
english.CreateQuickLaunchIcon=Create a &Quick Launch icon
english.AssociateFiles=&Associate .ctdata files with Coiled Tubing App
english.InstallVC2022=Install Microsoft Visual C++ 2015-2022 Redistributable (x64)
english.InstallDotNet8=Install Microsoft .NET 8.0 Runtime (required)
english.OptionalComponents=Optional Components
english.AdditionalOptions=Additional Options

indonesian.LaunchProgram=Jalankan Aplikasi Coiled Tubing
indonesian.CreateDesktopIcon=Buat ikon &desktop
indonesian.CreateQuickLaunchIcon=Buat ikon &Quick Launch
indonesian.AssociateFiles=&Asosiasikan file .ctdata dengan Aplikasi Coiled Tubing
indonesian.InstallVC2022=Instal Microsoft Visual C++ 2015-2022 Redistributable (x64)
indonesian.InstallDotNet8=Instal Microsoft .NET 8.0 Runtime (diperlukan)
indonesian.OptionalComponents=Komponen Opsional
indonesian.AdditionalOptions=Opsi Tambahan

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked
Name: "quicklaunchicon"; Description: "{cm:CreateQuickLaunchIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked; OnlyBelowVersion: 6.1; Check: not IsAdminInstallMode
Name: "associatefiles"; Description: "{cm:AssociateFiles}"; GroupDescription: "{cm:OptionalComponents}"; Flags: unchecked
Name: "autostart"; Description: "Start automatically with Windows"; GroupDescription: "{cm:AdditionalOptions}"; Flags: unchecked

[Types]
Name: "full"; Description: "Full installation"
Name: "compact"; Description: "Compact installation"
Name: "custom"; Description: "Custom installation"; Flags: iscustom

[Components]
Name: "main"; Description: "Core Application Files"; Types: full compact custom; Flags: fixed
Name: "docs"; Description: "Documentation"; Types: full
Name: "samples"; Description: "Sample Data Files"; Types: full
Name: "tools"; Description: "Additional Tools"; Types: full

[Dirs]
Name: "{app}"; Permissions: users-full
Name: "{app}\Data"; Permissions: users-full
Name: "{app}\Logs"; Permissions: users-full
Name: "{app}\Config"; Permissions: users-full
Name: "{app}\Backup"; Permissions: users-full

[Files]
; Core Application Files
Source: "..\coiled-tubing-app\bin\Release\net8.0-windows10.0.19041.0\win-x64\publish\coiled-tubing-app.exe"; DestDir: "{app}"; Flags: ignoreversion; Components: main
Source: "..\coiled-tubing-app\bin\Release\net8.0-windows10.0.19041.0\win-x64\publish\*.dll"; DestDir: "{app}"; Flags: ignoreversion; Components: main
Source: "..\coiled-tubing-app\bin\Release\net8.0-windows10.0.19041.0\win-x64\publish\*.json"; DestDir: "{app}"; Flags: ignoreversion; Components: main
Source: "..\coiled-tubing-app\bin\Release\net8.0-windows10.0.19041.0\win-x64\publish\*.pdb"; DestDir: "{app}"; Flags: ignoreversion; Components: main
Source: "..\coiled-tubing-app\bin\Release\net8.0-windows10.0.19041.0\win-x64\publish\OptoMMP_Standard_2_0.dll"; DestDir: "{app}"; Flags: ignoreversion; Components: main

; Configuration Files
Source: "Config\app.config"; DestDir: "{app}\Config"; Flags: ignoreversion; Components: main
Source: "Config\default-settings.json"; DestDir: "{app}\Config"; Flags: ignoreversion; Components: main

; Documentation
Source: "Docs\User Manual.pdf"; DestDir: "{app}\Documentation"; Flags: ignoreversion; Components: docs
Source: "Docs\Quick Start Guide.pdf"; DestDir: "{app}\Documentation"; Flags: ignoreversion; Components: docs
Source: "Docs\API Reference.pdf"; DestDir: "{app}\Documentation"; Flags: ignoreversion; Components: docs

; Sample Data
Source: "Samples\*.ctdata"; DestDir: "{app}\Samples"; Flags: ignoreversion; Components: samples
Source: "Samples\demo-project.ctproj"; DestDir: "{app}\Samples"; Flags: ignoreversion; Components: samples

; Additional Tools
Source: "Tools\DataConverter.exe"; DestDir: "{app}\Tools"; Flags: ignoreversion; Components: tools
Source: "Tools\LogAnalyzer.exe"; DestDir: "{app}\Tools"; Flags: ignoreversion; Components: tools

; License and Readme
Source: "License.txt"; DestDir: "{app}"; Flags: ignoreversion; Components: main
Source: "ReadMe.txt"; DestDir: "{app}"; Flags: ignoreversion; Components: main
Source: "Changelog.txt"; DestDir: "{app}"; Flags: ignoreversion; Components: main

[Registry]
; Application Registration
Root: HKLM; Subkey: "Software\CoiledTubingApp"; ValueType: string; ValueName: "InstallPath"; ValueData: "{app}"; Flags: uninsdeletekey
Root: HKLM; Subkey: "Software\CoiledTubingApp"; ValueType: string; ValueName: "Version"; ValueData: "1.0.0"; Flags: uninsdeletekey
Root: HKLM; Subkey: "Software\CoiledTubingApp"; ValueType: dword; ValueName: "Installed"; ValueData: 1; Flags: uninsdeletekey

; File Associations
Root: HKCR; Subkey: ".ctdata"; ValueType: string; ValueName: ""; ValueData: "CoiledTubingApp.DataFile"; Flags: uninsdeletevalue; Tasks: associatefiles
Root: HKCR; Subkey: ".ctproj"; ValueType: string; ValueName: ""; ValueData: "CoiledTubingApp.ProjectFile"; Flags: uninsdeletevalue; Tasks: associatefiles
Root: HKCR; Subkey: "CoiledTubingApp.DataFile"; ValueType: string; ValueName: ""; ValueData: "Coiled Tubing Data File"; Flags: uninsdeletekey; Tasks: associatefiles
Root: HKCR; Subkey: "CoiledTubingApp.DataFile\DefaultIcon"; ValueType: string; ValueName: ""; ValueData: "{app}\coiled-tubing-app.exe,0"; Flags: uninsdeletekey; Tasks: associatefiles
Root: HKCR; Subkey: "CoiledTubingApp.DataFile\shell\open\command"; ValueType: string; ValueName: ""; ValueData: """{app}\coiled-tubing-app.exe"" ""%1"""; Flags: uninsdeletekey; Tasks: associatefiles
Root: HKCR; Subkey: "CoiledTubingApp.ProjectFile"; ValueType: string; ValueName: ""; ValueData: "Coiled Tubing Project File"; Flags: uninsdeletekey; Tasks: associatefiles
Root: HKCR; Subkey: "CoiledTubingApp.ProjectFile\DefaultIcon"; ValueType: string; ValueName: ""; ValueData: "{app}\coiled-tubing-app.exe,1"; Flags: uninsdeletekey; Tasks: associatefiles
Root: HKCR; Subkey: "CoiledTubingApp.ProjectFile\shell\open\command"; ValueType: string; ValueName: ""; ValueData: """{app}\coiled-tubing-app.exe"" ""%1"""; Flags: uninsdeletekey; Tasks: associatefiles

; Auto-start with Windows
Root: HKLM; Subkey: "Software\Microsoft\Windows\CurrentVersion\Run"; ValueType: string; ValueName: "CoiledTubingApp"; ValueData: """{app}\coiled-tubing-app.exe"" /autostart"; Flags: uninsdeletevalue; Tasks: autostart

[Icons]
; Start Menu
Name: "{group}\Coiled Tubing Operations App"; Filename: "{app}\coiled-tubing-app.exe"; WorkingDir: "{app}"; Comment: "Professional Coiled Tubing Operations Management"; IconFilename: "{app}\coiled-tubing-app.exe"; IconIndex: 0
Name: "{group}\User Manual"; Filename: "{app}\Documentation\User Manual.pdf"; Components: docs
Name: "{group}\Sample Projects"; Filename: "{app}\Samples"; Components: samples
Name: "{group}\Configuration"; Filename: "{app}\Config"; Flags: foldershortcut
Name: "{group}\{cm:UninstallProgram,Coiled Tubing Operations App}"; Filename: "{uninstallexe}"

; Desktop
Name: "{autodesktop}\Coiled Tubing App"; Filename: "{app}\coiled-tubing-app.exe"; WorkingDir: "{app}"; Tasks: desktopicon; Comment: "Professional Coiled Tubing Operations Management"

; Quick Launch
Name: "{userappdata}\Microsoft\Internet Explorer\Quick Launch\Coiled Tubing App"; Filename: "{app}\coiled-tubing-app.exe"; WorkingDir: "{app}"; Tasks: quicklaunchicon; Comment: "Coiled Tubing App"

[Run]
; Post-installation actions
Filename: "{app}\coiled-tubing-app.exe"; Description: "{cm:LaunchProgram,Coiled Tubing Operations App}"; Flags: nowait postinstall skipifsilent; WorkingDir: "{app}"
Filename: "{app}\Documentation\Quick Start Guide.pdf"; Description: "View Quick Start Guide"; Flags: postinstall skipifsilent shellexec; Components: docs
Filename: "https://www.yourcompany.com/coiled-tubing-app/getting-started"; Description: "Visit online getting started guide"; Flags: postinstall skipifsilent shellexec unchecked

[UninstallRun]
; Pre-uninstall cleanup
Filename: "{cmd}"; Parameters: "/C ""taskkill /F /IM coiled-tubing-app.exe /T"""; Flags: runhidden; RunOnceId: "KillApp"

[UninstallDelete]
; Clean up user data (optional)
Type: filesandordirs; Name: "{app}\Logs"
Type: filesandordirs; Name: "{app}\Temp"
Type: files; Name: "{app}\*.log"
Type: files; Name: "{app}\*.tmp"

[Code]
var
  DotNetMissing, VCRedistMissing: Boolean;

// =============================================================================
// Prerequisite Detection Functions
// =============================================================================

function IsDotNet8Installed(): Boolean;
var
  InstallPath: string;
begin
  Result := RegQueryStringValue(HKLM, 'SOFTWARE\dotnet\Setup\InstalledVersions\x64\sharedfx\Microsoft.WindowsDesktop.App', '8.0', InstallPath) or
            RegQueryStringValue(HKLM, 'SOFTWARE\WOW6432Node\dotnet\Setup\InstalledVersions\x64\sharedfx\Microsoft.WindowsDesktop.App', '8.0', InstallPath);
end;

function IsVCRedist2022Installed(): Boolean;
var
  Version: string;
begin
  Result := RegQueryStringValue(HKLM, 'SOFTWARE\Microsoft\VisualStudio\14.0\VC\Runtimes\x64', 'Version', Version) or
            RegQueryStringValue(HKLM, 'SOFTWARE\WOW6432Node\Microsoft\VisualStudio\14.0\VC\Runtimes\x64', 'Version', Version);
end;

// =============================================================================
// Installation Event Handlers
// =============================================================================

function InitializeSetup(): Boolean;
var
  ErrorMessage: string;
begin
  Result := True;
  ErrorMessage := '';
  
  // Check system requirements
  if not IsWin64 then
  begin
    MsgBox('This application requires 64-bit Windows.', mbError, MB_OK);
    Result := False;
    Exit;
  end;
  
  // Check .NET 8 Runtime
  DotNetMissing := not IsDotNet8Installed();
  if DotNetMissing then
    ErrorMessage := ErrorMessage + '- Microsoft .NET 8.0 Desktop Runtime' + #13#10;
  
  // Check VC++ Redistributable
  VCRedistMissing := not IsVCRedist2022Installed();
  if VCRedistMissing then
    ErrorMessage := ErrorMessage + '- Microsoft Visual C++ 2015-2022 Redistributable (x64)' + #13#10;
  
  // Show missing prerequisites
  if ErrorMessage <> '' then
  begin
    if MsgBox('The following required components are missing:' + #13#10#13#10 + 
              ErrorMessage + #13#10 + 
              'The application may not work correctly without these components.' + #13#10#13#10 + 
              'Do you want to continue with the installation anyway?', 
              mbConfirmation, MB_YESNO) = IDNO then
    begin
      Result := False;
    end;
  end;
end;

function NextButtonClick(CurPageID: Integer): Boolean;
var
  InstallPath: string;
begin
  Result := True;
  
  case CurPageID of
    wpSelectDir:
    begin
      InstallPath := WizardForm.DirEdit.Text;
      
      // Check path length
      if Length(InstallPath) > 100 then
      begin
        MsgBox('The installation path is too long. Please choose a shorter path (maximum 100 characters).', mbError, MB_OK);
        Result := False;
        Exit;
      end;
      
      // Check for invalid characters
      if (Pos('&', InstallPath) > 0) or (Pos('%', InstallPath) > 0) then
      begin
        MsgBox('The installation path contains invalid characters. Please choose a different path.', mbError, MB_OK);
        Result := False;
        Exit;
      end;
      
      // Check available disk space (require at least 500MB)
      if GetSpaceOnDisk(ExtractFileDrive(InstallPath)) < 500 * 1024 * 1024 then
      begin
        MsgBox('Insufficient disk space. At least 500 MB of free space is required.', mbError, MB_OK);
        Result := False;
        Exit;
      end;
    end;
  end;
end;

procedure CurStepChanged(CurStep: TSetupStep);
begin
  case CurStep of
    ssInstall:
    begin
      // Pre-installation setup
      if IsTaskSelected('autostart') then
        Log('Auto-start task selected');
    end;
    
    ssPostInstall:
    begin
      // Post-installation setup
      Log('Installation completed successfully');
    end;
  end;
end;

// =============================================================================
// Progress and Status Updates
// =============================================================================

procedure CurInstallProgressChanged(CurProgress, MaxProgress: Integer);
var
  ProgressPercent: Integer;
begin
  if MaxProgress > 0 then
  begin
    ProgressPercent := (CurProgress * 100) div MaxProgress;
    Log(Format('Installation progress: %d%%', [ProgressPercent]));
  end;
end;

// =============================================================================
// Uninstall Event Handlers
// =============================================================================

function InitializeUninstall(): Boolean;
begin
  Result := True;
  if MsgBox('Are you sure you want to completely remove Coiled Tubing Operations App and all of its components?', 
            mbConfirmation, MB_YESNO or MB_DEFBUTTON2) = IDNO then
  begin
    Result := False;
  end;
end;

procedure CurUninstallStepChanged(CurUninstallStep: TUninstallStep);
begin
  case CurUninstallStep of
    usUninstall:
    begin
      // Pre-uninstall cleanup
      Log('Starting uninstallation process');
    end;
    
    usPostUninstall:
    begin
      // Post-uninstall cleanup
      if MsgBox('Do you want to remove all user data and settings? ' +
                'This includes logs, configuration files, and project data.', 
                mbConfirmation, MB_YESNO or MB_DEFBUTTON2) = IDYES then
      begin
        DelTree(ExpandConstant('{userappdata}\CoiledTubingApp'), True, True, True);
        DelTree(ExpandConstant('{userdocs}\Coiled Tubing Projects'), True, True, True);
        Log('User data removed');
      end;
    end;
  end;
end;