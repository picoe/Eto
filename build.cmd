@echo off

WHERE msbuild > nul
IF %ERRORLEVEL% NEQ 0 (
ECHO msbuild not found.  Run in the Developer Command Prompt for VS 2019
pause
)

set BUILD_DIR=%~dp0build
msbuild -v:minimal -t:Package -p:BuildVersion=%1 "%BUILD_DIR%\Build.proj"
