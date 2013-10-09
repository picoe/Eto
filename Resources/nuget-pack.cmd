REM echo off

SET CONFIG=Release
SET OUT=..\BuildOutput\nuget
SET VERSION=1.2.1

mkdir "%OUT%"
nuget pack ..\Source\Eto\Eto.csproj -Build -OutputDirectory "%OUT%" -Version %VERSION% -Prop Configuration=%CONFIG%
nuget pack ..\Source\Eto.Platform.Gtk\Eto.Platform.Gtk.csproj -Build -OutputDirectory "%OUT%" -Version %VERSION% -Prop Configuration=%CONFIG%
nuget pack ..\Source\Eto.Platform.Windows\Eto.Platform.Windows.csproj -Build -OutputDirectory "%OUT%" -Version %VERSION% -Prop Configuration=%CONFIG%
nuget pack ..\Source\Eto.Platform.Wpf\Eto.Platform.Wpf.csproj -Build -OutputDirectory "%OUT%" -Version %VERSION% -Prop Configuration=%CONFIG%
nuget pack ..\Source\Eto.Platform.Mac\Eto.Platform.Mac.csproj -Build -OutputDirectory "%OUT%" -Version %VERSION% -Prop Configuration=%CONFIG%
nuget pack .\Eto.Platform.Mac.Template.nuspec -OutputDirectory "%OUT%" -Version %VERSION%
nuget pack ..\Source\Eto.Platform.Mac\Eto.Platform.XamMac.csproj -OutputDirectory "%OUT%" -Version %VERSION% -Prop Configuration=%CONFIG%
nuget pack ..\Source\Eto.Sample\Eto.Forms.Sample.nuspec -OutputDirectory "%OUT%" -Version %VERSION%
