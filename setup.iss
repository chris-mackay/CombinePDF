[Setup]
AppId={{BB021374-4336-4E98-9002-4953BF28238F}
AppName=CombinePDF
AppCopyright=Copyright © 2021 Christopher Mackay
AppVersion=1.0.0
VersionInfoVersion=1.0.0
AppVerName=CombinePDF
AppPublisher=Christopher Mackay
AppPublisherURL=https://github.com/chris-mackay/CombinePDF/
AppSupportURL=https://github.com/chris-mackay/CombinePDF/
AppUpdatesURL=https://github.com/chris-mackay/CombinePDF/releases/
DefaultDirName={pf}\Christopher Mackay\CombinePDF
DefaultGroupName=CombinePDF
OutputDir=C:\Programming\CombinePDF
OutputBaseFilename=CombinePDF-Setup
Compression=lzma
SolidCompression=yes
UninstallDisplayIcon={app}\CombinePDF.exe
DisableDirPage=yes
LicenseFile=C:\Programming\CombinePDF\LICENSE.txt
PrivilegesRequired=admin

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}";

[Files]
Source: "C:\Programming\CombinePDF\CombinePDF\bin\x64\Release\CombinePDF.exe"; DestDir: "{app}"; Flags: ignoreversion
Source: "C:\Programming\CombinePDF\CombinePDF\bin\x64\Release\Microsoft.WindowsAPICodePack.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "C:\Programming\CombinePDF\CombinePDF\bin\x64\Release\Microsoft.WindowsAPICodePack.Shell.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "C:\Programming\CombinePDF\CombinePDF\bin\x64\Release\PdfSharp.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "C:\Programming\CombinePDF\icons\combine_pdf.ico"; DestDir: "{app}\Icons";

[Icons]
Name: "{group}\CombinePDF"; Filename: "{app}\CombinePDF.exe"
Name: "{commondesktop}\CombinePDF"; Filename: "{app}\CombinePDF.exe"; Tasks: desktopicon

[Run]
Filename: "{app}\CombinePDF.exe"; Description: "{cm:LaunchProgram,CombinePDF}"; Flags: nowait postinstall skipifsilent

[UninstallDelete]
Type: filesandordirs; Name: "{localappdata}\Chris Mackay"