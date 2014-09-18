echo off

set MSBUILD=c:\Windows\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe

%MSBUILD% -t:Package -p:BuildVersion=%1 Publish.targets

pause