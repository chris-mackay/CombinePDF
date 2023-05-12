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
OutputDir=.
OutputBaseFilename=Setup
Compression=lzma
SolidCompression=yes
UninstallDisplayIcon={app}\CombinePDF.exe
DisableDirPage=yes
PrivilegesRequired=lowest
DisableProgramGroupPage=yes
DisableStartupPrompt=yes
DisableWelcomePage=yes
DisableReadyPage=yes
WizardSmallImageFile=setupicon.bmp
WizardImageFile=bannericon.bmp

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"

[Files]
Source: "CombinePDF\bin\Release\CombinePDF.exe"; DestDir: "{app}"; Flags: ignoreversion
Source: "CombinePDF\bin\Release\*.dll"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs

[Icons]
Name: "{group}\CombinePDF"; Filename: "{app}\CombinePDF.exe"

[UninstallDelete]
Type: filesandordirs; Name: "{localappdata}\Christopher Mackay"