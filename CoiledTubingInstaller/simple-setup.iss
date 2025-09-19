; Simple Working Inno Setup Script for Coiled Tubing App
[Setup]
AppId={{A1B2C3D4-E5F6-7890-ABCD-1234567890AB}
AppName=Coiled Tubing Operations App
AppVersion=1.0.0
AppPublisher=Your Company Name
DefaultDirName={autopf}\Coiled Tubing App
DefaultGroupName=Coiled Tubing App
OutputBaseFilename=CoiledTubingApp-Setup-v1.0.0
Compression=lzma
SolidCompression=yes
WizardStyle=modern
PrivilegesRequired=admin
ArchitecturesAllowed=x64
ArchitecturesInstallIn64BitMode=x64
SetupLogging=yes

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked

[Files]
Source: "..\coiled-tubing-app\bin\Release\net8.0-windows10.0.19041.0\win-x64\publish\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs

[Icons]
Name: "{group}\Coiled Tubing App"; Filename: "{app}\coiled-tubing-app.exe"
Name: "{group}\{cm:UninstallProgram,Coiled Tubing App}"; Filename: "{uninstallexe}"
Name: "{autodesktop}\Coiled Tubing App"; Filename: "{app}\coiled-tubing-app.exe"; Tasks: desktopicon

[Run]
Filename: "{app}\coiled-tubing-app.exe"; Description: "{cm:LaunchProgram,Coiled Tubing App}"; Flags: nowait postinstall skipifsilent

[Code]
function InitializeSetup(): Boolean;
begin
  Result := True;
  if not IsWin64 then
  begin
    MsgBox('This application requires 64-bit Windows.', mbError, MB_OK);
    Result := False;
  end;
end;