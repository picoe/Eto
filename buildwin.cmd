
set MSBUILD=%WINDIR%\Microsoft.NET\Framework\v4.0.30319\msbuild

rmdir /s /q BuildOutput
%MSBUILD% Source\Eto.sln "/p:Configuration=Windows Release" /t:Clean,Build
%MSBUILD% Source\Eto.sln "/p:Configuration=Windows Debug" /t:Clean,Build