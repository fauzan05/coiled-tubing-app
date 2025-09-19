; Coiled Tubing App NSIS Installer Script
; Modern UI 2

!include "MUI2.nsh"
!include "x64.nsh"

; General
Name "Coiled Tubing Operations App"
OutFile "CoiledTubingAppSetup.exe"
Unicode True
InstallDir "$PROGRAMFILES64\Coiled Tubing App"
InstallDirRegKey HKLM "Software\CoiledTubingApp" "InstallDir"
RequestExecutionLevel admin

; Version Information
VIProductVersion "1.0.0.0"
VIAddVersionKey "ProductName" "Coiled Tubing Operations App"
VIAddVersionKey "Comments" "Professional Coiled Tubing Operations Management Software"
VIAddVersionKey "CompanyName" "Your Company Name"
VIAddVersionKey "LegalCopyright" "© Your Company Name"
VIAddVersionKey "FileDescription" "Coiled Tubing App Installer"
VIAddVersionKey "FileVersion" "1.0.0.0"
VIAddVersionKey "ProductVersion" "1.0.0.0"

; Modern UI Configuration
!define MUI_ABORTWARNING
!define MUI_ICON "app.ico"
!define MUI_UNICON "app.ico"
!define MUI_HEADERIMAGE
!define MUI_HEADERIMAGE_BITMAP "header.bmp"
!define MUI_WELCOMEFINISHPAGE_BITMAP "wizard.bmp"

; Pages
!define MUI_WELCOMEPAGE_TITLE "Welcome to Coiled Tubing App Setup"
!define MUI_WELCOMEPAGE_TEXT "This wizard will guide you through the installation of Coiled Tubing Operations App.$\r$\n$\r$\nThis professional software helps manage coiled tubing operations with real-time monitoring and data analysis.$\r$\n$\r$\nClick Next to continue."
!insertmacro MUI_PAGE_WELCOME

!insertmacro MUI_PAGE_LICENSE "License.txt"
!insertmacro MUI_PAGE_COMPONENTS
!insertmacro MUI_PAGE_DIRECTORY

; Custom page for additional options
Page custom CustomOptionsPage CustomOptionsPageLeave

!insertmacro MUI_PAGE_INSTFILES

!define MUI_FINISHPAGE_RUN "$INSTDIR\coiled-tubing-app.exe"
!define MUI_FINISHPAGE_RUN_TEXT "Launch Coiled Tubing App"
!define MUI_FINISHPAGE_LINK "Visit our website for support and updates"
!define MUI_FINISHPAGE_LINK_LOCATION "http://www.yourcompany.com"
!insertmacro MUI_PAGE_FINISH

; Uninstaller pages
!insertmacro MUI_UNPAGE_CONFIRM
!insertmacro MUI_UNPAGE_INSTFILES

; Languages
!insertmacro MUI_LANGUAGE "English"

; Installer Sections
Section "Core Application" SecCore
  SectionIn RO  ; Read-only, always installed
  
  SetOutPath "$INSTDIR"
  
  ; Main application files
  File "..\coiled-tubing-app\bin\Release\net8.0-windows10.0.19041.0\win-x64\publish\coiled-tubing-app.exe"
  File "..\coiled-tubing-app\bin\Release\net8.0-windows10.0.19041.0\win-x64\publish\*.dll"
  File "..\coiled-tubing-app\bin\Release\net8.0-windows10.0.19041.0\win-x64\publish\*.json"
  File "..\coiled-tubing-app\bin\Release\net8.0-windows10.0.19041.0\win-x64\publish\OptoMMP_Standard_2_0.dll"
  
  ; Write registry keys
  WriteRegStr HKLM "Software\CoiledTubingApp" "InstallDir" "$INSTDIR"
  WriteRegStr HKLM "Software\CoiledTubingApp" "Version" "1.0.0"
  
  ; Write uninstall information
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\CoiledTubingApp" "DisplayName" "Coiled Tubing Operations App"
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\CoiledTubingApp" "UninstallString" '"$INSTDIR\uninstall.exe"'
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\CoiledTubingApp" "InstallLocation" "$INSTDIR"
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\CoiledTubingApp" "Publisher" "Your Company Name"
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\CoiledTubingApp" "DisplayVersion" "1.0.0"
  WriteRegDWORD HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\CoiledTubingApp" "NoModify" 1
  WriteRegDWORD HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\CoiledTubingApp" "NoRepair" 1
  
  WriteUninstaller "$INSTDIR\uninstall.exe"
SectionEnd

Section "Start Menu Shortcuts" SecStartMenu
  CreateDirectory "$SMPROGRAMS\Coiled Tubing App"
  CreateShortcut "$SMPROGRAMS\Coiled Tubing App\Coiled Tubing App.lnk" "$INSTDIR\coiled-tubing-app.exe"
  CreateShortcut "$SMPROGRAMS\Coiled Tubing App\Uninstall.lnk" "$INSTDIR\uninstall.exe"
SectionEnd

Section "Desktop Shortcut" SecDesktop
  CreateShortcut "$DESKTOP\Coiled Tubing App.lnk" "$INSTDIR\coiled-tubing-app.exe"
SectionEnd

Section "Quick Launch Shortcut" SecQuickLaunch
  CreateShortcut "$QUICKLAUNCH\Coiled Tubing App.lnk" "$INSTDIR\coiled-tubing-app.exe"
SectionEnd

; Section descriptions
!insertmacro MUI_FUNCTION_DESCRIPTION_BEGIN
  !insertmacro MUI_DESCRIPTION_TEXT ${SecCore} "Core application files (required)"
  !insertmacro MUI_DESCRIPTION_TEXT ${SecStartMenu} "Start Menu shortcuts"
  !insertmacro MUI_DESCRIPTION_TEXT ${SecDesktop} "Desktop shortcut"
  !insertmacro MUI_DESCRIPTION_TEXT ${SecQuickLaunch} "Quick Launch shortcut"
!insertmacro MUI_FUNCTION_DESCRIPTION_END

; Custom Options Page
Var CheckBox_AutoStart
Var CheckBox_FileAssoc

Function CustomOptionsPage
  !insertmacro MUI_HEADER_TEXT "Additional Options" "Configure additional installation options"
  
  nsDialogs::Create 1018
  Pop $0
  
  ${NSD_CreateCheckBox} 10 10 300 12 "Start Coiled Tubing App automatically with Windows"
  Pop $CheckBox_AutoStart
  
  ${NSD_CreateCheckBox} 10 30 300 12 "Associate .ctdata files with Coiled Tubing App"
  Pop $CheckBox_FileAssoc
  
  nsDialogs::Show
FunctionEnd

Function CustomOptionsPageLeave
  ${NSD_GetState} $CheckBox_AutoStart $0
  ${If} $0 == 1
    WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Run" "CoiledTubingApp" "$INSTDIR\coiled-tubing-app.exe"
  ${EndIf}
  
  ${NSD_GetState} $CheckBox_FileAssoc $0
  ${If} $0 == 1
    WriteRegStr HKCR ".ctdata" "" "CoiledTubingApp.DataFile"
    WriteRegStr HKCR "CoiledTubingApp.DataFile" "" "Coiled Tubing Data File"
    WriteRegStr HKCR "CoiledTubingApp.DataFile\shell\open\command" "" '"$INSTDIR\coiled-tubing-app.exe" "%1"'
  ${EndIf}
FunctionEnd

; Installer Functions
Function .onInit
  ; Check if running on 64-bit Windows
  ${IfNot} ${RunningX64}
    MessageBox MB_OK|MB_ICONSTOP "This application requires 64-bit Windows."
    Abort
  ${EndIf}
  
  ; Check if already installed
  ReadRegStr $R0 HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\CoiledTubingApp" "UninstallString"
  StrCmp $R0 "" done
  
  MessageBox MB_OKCANCEL|MB_ICONEXCLAMATION "Coiled Tubing App is already installed. $\n$\nClick OK to remove the previous version or Cancel to cancel this upgrade." IDOK uninst
  Abort
  
uninst:
  ClearErrors
  ExecWait '$R0 /S _?=$INSTDIR'
  
  IfErrors no_remove_uninstaller done
  Delete $R0
  RMDir $INSTDIR
  
no_remove_uninstaller:
done:
FunctionEnd

; Uninstaller
Section "Uninstall"
  ; Remove files
  Delete "$INSTDIR\coiled-tubing-app.exe"
  Delete "$INSTDIR\*.dll"
  Delete "$INSTDIR\*.json"
  Delete "$INSTDIR\OptoMMP_Standard_2_0.dll"
  Delete "$INSTDIR\uninstall.exe"
  
  ; Remove shortcuts
  Delete "$SMPROGRAMS\Coiled Tubing App\Coiled Tubing App.lnk"
  Delete "$SMPROGRAMS\Coiled Tubing App\Uninstall.lnk"
  Delete "$DESKTOP\Coiled Tubing App.lnk"
  Delete "$QUICKLAUNCH\Coiled Tubing App.lnk"
  RMDir "$SMPROGRAMS\Coiled Tubing App"
  
  ; Remove registry keys
  DeleteRegKey HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\CoiledTubingApp"
  DeleteRegKey HKLM "Software\CoiledTubingApp"
  DeleteRegValue HKLM "Software\Microsoft\Windows\CurrentVersion\Run" "CoiledTubingApp"
  
  ; Remove file associations
  DeleteRegKey HKCR ".ctdata"
  DeleteRegKey HKCR "CoiledTubingApp.DataFile"
  
  RMDir "$INSTDIR"
SectionEnd