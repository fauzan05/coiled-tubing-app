; Coiled Tubing App Installer Script
; Created with Inno Setup

[Setup]
; NOTE: The value of AppId uniquely identifies this application.
AppId={{12345678-1234-1234-1234-123456789012}
AppName=Coiled Tubing Operations App
AppVersion=1.0.0
AppVerName=Coiled Tubing Operations App 1.0.0
AppPublisher=Your Company Name
AppPublisherURL=http://www.yourcompany.com/
AppSupportURL=http://www.yourcompany.com/support
AppUpdatesURL=http://www.yourcompany.com/updates
DefaultDirName={autopf}\Coiled Tubing App
DefaultGroupName=Coiled Tubing App
AllowNoIcons=yes
LicenseFile=License.txt
OutputDir=Output
OutputBaseFilename=CoiledTubingAppSetup
Compression=lzma
SolidCompression=yes
SetupIconFile=app.ico
WizardImageFile=WizardImage.bmp
WizardSmallImageFile=WizardSmallImage.bmp

; Minimum Windows version
MinVersion=6.1sp1

; Architecture
ArchitecturesAllowed=x64
ArchitecturesInstallIn64BitMode=x64

; Privileges
PrivilegesRequired=admin

; Visual appearance
WizardStyle=modern
ShowLanguageDialog=auto
DisableWelcomePage=no

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"
Name: "indonesian"; MessagesFile: "compiler:Languages\Indonesian.isl"

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked
Name: "quicklaunchicon"; Description: "{cm:CreateQuickLaunchIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked; OnlyBelowVersion: 0,6.1

[Files]
; Main application files
Source: "..\coiled-tubing-app\bin\Release\net8.0-windows10.0.19041.0\win-x64\publish\coiled-tubing-app.exe"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\coiled-tubing-app\bin\Release\net8.0-windows10.0.19041.0\win-x64\publish\*.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\coiled-tubing-app\bin\Release\net8.0-windows10.0.19041.0\win-x64\publish\*.json"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\coiled-tubing-app\bin\Release\net8.0-windows10.0.19041.0\win-x64\publish\OptoMMP_Standard_2_0.dll"; DestDir: "{app}"; Flags: ignoreversion

; Documentation
Source: "README.txt"; DestDir: "{app}"; Flags: ignoreversion
; NOTE: Don't use "Flags: ignoreversion" on any shared system files

[Icons]
Name: "{group}\Coiled Tubing App"; Filename: "{app}\coiled-tubing-app.exe"
Name: "{group}\{cm:UninstallProgram,Coiled Tubing App}"; Filename: "{uninstallexe}"
Name: "{autodesktop}\Coiled Tubing App"; Filename: "{app}\coiled-tubing-app.exe"; Tasks: desktopicon
Name: "{userappdata}\Microsoft\Internet Explorer\Quick Launch\Coiled Tubing App"; Filename: "{app}\coiled-tubing-app.exe"; Tasks: quicklaunchicon

[Run]
Filename: "{app}\coiled-tubing-app.exe"; Description: "{cm:LaunchProgram,Coiled Tubing App}"; Flags: nowait postinstall skipifsilent

[Registry]
; Register application
Root: HKLM; Subkey: "Software\Microsoft\Windows\CurrentVersion\Uninstall\CoiledTubingApp"; ValueType: string; ValueName: "DisplayName"; ValueData: "Coiled Tubing Operations App"
Root: HKLM; Subkey: "Software\Microsoft\Windows\CurrentVersion\Uninstall\CoiledTubingApp"; ValueType: string; ValueName: "UninstallString"; ValueData: "{uninstallexe}"
Root: HKLM; Subkey: "Software\Microsoft\Windows\CurrentVersion\Uninstall\CoiledTubingApp"; ValueType: string; ValueName: "InstallLocation"; ValueData: "{app}"
Root: HKLM; Subkey: "Software\Microsoft\Windows\CurrentVersion\Uninstall\CoiledTubingApp"; ValueType: string; ValueName: "Publisher"; ValueData: "Your Company Name"
Root: HKLM; Subkey: "Software\Microsoft\Windows\CurrentVersion\Uninstall\CoiledTubingApp"; ValueType: string; ValueName: "DisplayVersion"; ValueData: "1.0.0"

[UninstallDelete]
Type: filesandordirs; Name: "{app}"

[Code]
// Custom installation logic can be added here
function InitializeSetup(): Boolean;
begin
  // Check if .NET 8 Runtime is installed
  Result := True;
  if not RegKeyExists(HKEY_LOCAL_MACHINE, 'SOFTWARE\dotnet\Setup\InstalledVersions\x64\sharedhost') then
  begin
    if MsgBox('Microsoft .NET 8 Runtime is required but not installed. Do you want to continue anyway?', 
              mbConfirmation, MB_YESNO) = IDNO then
      Result := False;
  end;
end;

function NextButtonClick(CurPageID: Integer): Boolean;
begin
  Result := True;
  
  // Custom validation for installation directory
  if CurPageID = wpSelectDir then
  begin
    if Length(WizardForm.DirEdit.Text) > 100 then
    begin
      MsgBox('Installation path is too long. Please choose a shorter path.', mbError, MB_OK);
      Result := False;
    end;
  end;
end;

// Progress callback
procedure CurInstallProgressChanged(CurProgress, MaxProgress: Integer);
begin
  // Custom progress handling if needed
end;