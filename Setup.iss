[Setup]
AppId={{0dffa03c-fbdf-413b-b15f-0e655d044bfe}
AppName=Combine PDF
AppCopyright=Copyright � 2023 Christopher Mackay
AppVersion=1.0.1
VersionInfoVersion=1.0.1
AppVerName=Combine PDF
AppPublisher=Christopher Mackay
AppUpdatesURL=github.com/chris-mackay/CombinePDF/releases/tag/v1.0.1
DefaultDirName={userdocs}\Christopher Mackay\CombinePDF
DefaultGroupName=CombinePDF
OutputDir=C:\Users\cmack\source\repos\CombinePDF
OutputBaseFilename=Setup
Compression=lzma
SolidCompression=yes
UninstallDisplayIcon={app}\CombinePDF.exe
DisableDirPage=yes
PrivilegesRequired=admin

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}";

[Files]
Source: "C:\Users\cmack\source\repos\CombinePDF\CombinePDF\bin\Release\CombinePDF.exe"; DestDir: "{app}"; Flags: ignoreversion
Source: "C:\Users\cmack\source\repos\CombinePDF\CombinePDF\bin\Release\*.dll"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs

[Icons]
Name: "{group}\CombinePDF"; Filename: "{app}\CombinePDF.exe"
Name: "{commondesktop}\CombinePDF"; Filename: "{app}\CombinePDF.exe"; Tasks: desktopicon

[Run]
Filename: "{app}\CombinePDF.exe"; Description: "{cm:LaunchProgram,CombinePDF}"; Flags: nowait postinstall skipifsilent

[UninstallDelete]
Type: filesandordirs; Name: "{localappdata}\Christopher Mackay"