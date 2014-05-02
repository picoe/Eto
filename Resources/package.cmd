echo off

set MSBUILD=c:\Windows\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe

%MSBUILD% -t:Package Publish.targets

pause