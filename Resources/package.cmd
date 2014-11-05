echo off

rem set MSBUILD=c:\Windows\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe
set MSBUILD="%ProgramFiles(x86)%\MSBuild\12.0\bin\MSBuild.exe"

%MSBUILD% -t:Package -p:BuildVersion=%1 Publish.targets

pause