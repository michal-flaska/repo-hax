@echo off

set MANAGED=D:\SteamLibrary\steamapps\common\REPO\REPO_Data\Managed
set DLL_RAW=C:\git-repos\repo-hax\cheat\bin\Release\cheat.dll
set DLL_PROTECTED=C:\git-repos\repo-hax\cheat\bin\Protected\cheat.dll
set HARMONY=C:\git-repos\repo-hax\libs\Harmony-Fat.2.4.2.0\0Harmony.dll
set SMI=D:\reverse-engineering\SharpMonoInjector.Console\smi.exe
set CONFUSER=D:\reverse-engineering\ConfuserEx-CLI\Confuser.CLI.exe

echo [1/4] Copying Harmony...
copy /Y "%HARMONY%" "%MANAGED%\0Harmony.dll"

echo [2/4] Obfuscating...
"%CONFUSER%" "%~dp0repo-hax.crproj"
if errorlevel 1 (
    echo Obfuscation failed, aborting.
    pause
    exit /b 1
)

echo [3/4] Copying protected DLL...
copy /Y "%DLL_PROTECTED%" "%MANAGED%\cheat.dll"

echo [4/4] Injecting...
"%SMI%" inject -p REPO -a "%DLL_PROTECTED%" -n cheat -c Loader -m Load

pause