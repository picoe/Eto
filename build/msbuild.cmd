@echo off
setlocal enabledelayedexpansion

set vswhere=%ProgramFiles(x86)%\Microsoft Visual Studio\Installer\vswhere.exe
set version=16.0

for /f "usebackq tokens=*" %%i in (`"%vswhere%" -version %version% -requires Microsoft.Component.MSBuild -find MSBuild\**\Bin\MSBuild.exe`) do (
  "%%i" %*
  exit /b !errorlevel!
)