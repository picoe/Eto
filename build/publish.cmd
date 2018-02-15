echo off

SET /p ask=Are you sure you want to publish to nuget?
if "%ask%"=="y" goto dopublish
goto end

:dopublish

set MSBUILD=c:\Windows\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe

%MSBUILD% -t:Publish Build.proj 

:end


pause
