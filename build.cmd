@echo off

WHERE dotnet > nul
IF %ERRORLEVEL% NEQ 0 (
ECHO dotnet not found.  Install the .NET 5+ SDK
pause
)

set BUILD_DIR=%~dp0build
dotnet msbuild -v:minimal -t:Package -p:SetVersion=%1 "%BUILD_DIR%\Build.proj"
