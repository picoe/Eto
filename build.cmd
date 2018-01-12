echo off

WHERE msbuild
IF %ERRORLEVEL% NEQ 0 ECHO msbuild not found.  Run in the Developer Command Prompt for VS 2017

cd %~dp0\build
msbuild -t:Package -p:BuildVersion=%1 Build.proj

pause