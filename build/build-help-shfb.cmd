echo off

WHERE msbuild > nul
IF %ERRORLEVEL% NEQ 0 (
ECHO msbuild not found.  Run in the Developer Command Prompt for VS 2017
pause
)

set BUILD_DIR=%~dp0
msbuild -t:BuildHelpShfb "%BUILD_DIR%\Build.proj"

pause