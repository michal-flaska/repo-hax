@echo off

set MANAGED=D:\SteamLibrary\steamapps\common\REPO\REPO_Data\Managed
set DLL=C:\git-repos\repo-hax\cheat\bin\Release\cheat.dll
set HARMONY=C:\git-repos\repo-hax\libs\Harmony-Fat.2.4.2.0\0Harmony.dll
set SMI=D:\reverse-engineering\SharpMonoInjector.Console\smi.exe

echo Copying files...
copy /Y "%HARMONY%" "%MANAGED%\0Harmony.dll"
copy /Y "%DLL%" "%MANAGED%\cheat.dll"

echo Injecting...
"%SMI%" inject -p REPO -a "%DLL%" -n cheat -c Loader -m Load

pause