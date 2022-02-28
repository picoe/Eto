Eto.Forms
=========
### A cross platform desktop and mobile user interface framework

[![Build](https://github.com/picoe/Eto/actions/workflows/build.yml/badge.svg)](https://github.com/picoe/Eto/actions/workflows/build.yml)
[![discussions](https://img.shields.io/badge/join-the%20discussions-yellowgreen.svg)](https://github.com/picoe/Eto/discussions)
[![Gitter](https://img.shields.io/gitter/room/nwjs/nw.js.svg)](https://gitter.im/picoe/Eto)
[![wiki](https://img.shields.io/badge/browse-the%20wiki-orange.svg)](https://github.com/picoe/Eto/wiki)
[![NuGet](http://img.shields.io/nuget/v/Eto.Forms.svg?style=flat)](https://www.nuget.org/packages/Eto.Forms/)
[![MyGet](http://img.shields.io/myget/eto/vpre/Eto.Forms.svg?style=flat&label=MyGet)](https://www.myget.org/gallery/eto)

Description
-----------

This framework can be used to build applications that run across multiple platforms using their native toolkit, with an easy to use API. This will make your applications look and work as a native application on all platforms, using a single UI codebase.

For advanced scenarios, you can take advantage of each platform's capabilities by wrapping your common UI in a larger application, or even create your own high-level controls with a custom implementations per platform.

This framework currently supports creating Desktop applications that work across Windows Forms, WPF, MonoMac, and GTK#.
There is a Mobile/iOS port in the works, but is considered incomplete.

This framework was built so that using it in .NET is natural. For example, a simple hello-world application might look like:

```C#
using Eto.Forms;
using Eto.Drawing;

public class MyForm : Form
{
	public MyForm ()
	{
		Title = "My Cross-Platform App";
		ClientSize = new Size(200, 200);
		Content = new Label { Text = "Hello World!" };
	}
	
	[STAThread]
	static void Main()
	{
		new Application().Run(new MyForm());
	}
}
```

or in a F# script:

```fsharp
#load ".paket/load/eto.platform.windows.fsx"
// see https://fsprojects.github.io/Paket/paket-generate-load-scripts.html

open Eto.Drawing
open Eto.Forms

type MyForm() as this =
    inherit Form()
    do
        this.Title      <- "My Cross-Platform App"
        this.ClientSize <- Size (200, 200)
        this.Content    <- new Label(Text = "Hello F# World!")

Eto.Platform.Initialize(Eto.Platforms.WinForms)
let app = new Application()
let form = new MyForm()
form.Show()
```

Getting Started
---------------

To begin creating apps using Eto.Forms, follow the [Quick Start Guide](https://github.com/picoe/Eto/wiki/Quick-Start).

To compile or contribute to Eto.Forms, read the [Contributing Guide](https://github.com/picoe/Eto/wiki/Contributing).


Screenshots
-----------
Windows via WPF:  
<img src="./images/windows.png" width=75% height=75%>  
Mac via MonoMac:  
<img src="./images/mac.png" width=75% height=75%>  
Linux via GTK#3:  
<img src="./images/linux.png" width=75% height=75%>  

Applications
------------
* [MonoGame Pipeline Tool](https://github.com/MonoGame/MonoGame) - Content manager for MonoGame
* [Manager](http://www.manager.io) - Accounting Software
* [PabloDraw](http://picoe.ca/products/pablodraw/alpha) - Character based drawing application
* [Notedown](https://github.com/cwensley/Notedown) - Note taking application
* [Eto.Test](https://github.com/picoe/Eto/tree/master/test/Eto.Test) - Application to test the functionality of each widget
* [DWSIM](https://github.com/DanWBR/dwsim5) - Chemical Process Simulator
* [Termission](https://github.com/junian/termission) - Cross-platform Serial/TCP Terminal with Scriptable Auto-Response
* [Visual SEO Studio](https://visual-seo.com/) - Technical SEO Auditing Tool
* [RegexFileSearcher](https://github.com/CommonLoon102/RegexFileSearcher) - Cross-platform regex file searching tool in .NET 5
* [RegexTestBench](https://github.com/CommonLoon102/RegexTestBench) - Cross-platform regex testing tool in .NET 5
* [GEDKeeper (v3)](https://github.com/Serg-Norseman/GEDKeeper) - Cross-platform application for working with personal genealogical databases

Third party libraries
----------

| |Pure Eto.Forms|[SkiaSharp](https://github.com/mono/SkiaSharp) edition| |
|---|---|---|---|
|[ScottPlot](https://scottplot.net/)|[![pure](https://img.shields.io/nuget/v/ScottPlot.Eto.svg)](https://www.nuget.org/packages/ScottPlot.Eto/)||Plotting library that makes it easy to interactively display large datasets.
|[LiveCharts](https://lvcharts.net/)||[![skia](https://img.shields.io/nuget/v/LiveChartsCore.SkiaSharpView.Eto.svg)](https://www.nuget.org/packages/LiveChartsCore.SkiaSharpView.Eto/)|Simple, flexible, powerful and open source data visualization for .Net.
|[Microcharts](https://devblogs.microsoft.com/xamarin/microcharts-elegant-cross-platform-charts-for-any-app/)||[![skia](https://img.shields.io/nuget/v/Microcharts.Eto.svg)](https://www.nuget.org/packages/Microcharts.Eto/)|Elegant Cross-Platform Charts for Every App.
|[OxyPlot](https://oxyplot.github.io/)|[![pure](https://img.shields.io/nuget/v/OxyPlot.Eto.svg)](https://www.nuget.org/packages/OxyPlot.Eto/)|[![skia](https://img.shields.io/nuget/v/OxyPlot.Eto.Skia.svg)](https://www.nuget.org/packages/OxyPlot.Eto.Skia/)|Cross-platform plotting library for .NET.
|[Mapsui](https://mapsui.com/)||[![skia](https://img.shields.io/nuget/v/Mapsui.Eto.svg)](https://www.nuget.org/packages/Mapsui.Eto/)|A C# map component for apps.
|[Eto.RainbowLoading](https://github.com/rafntor/Eto.RainbowLoading)|[![pure](https://img.shields.io/nuget/v/Eto.RainbowLoading.svg)](https://www.nuget.org/packages/Eto.RainbowLoading/)|[![skia](https://img.shields.io/nuget/v/Eto.RainbowLoading.Skia.svg)](https://www.nuget.org/packages/Eto.RainbowLoading.Skia/)|A control showing the famous Android loading indicator.
|[Eto.GifImageView](https://github.com/rafntor/Eto.GifImageView)|[![pure](https://img.shields.io/nuget/v/Eto.GifImageView.svg)](https://www.nuget.org/packages/Eto.GifImageView/)||A control for displaying GIF's.
|[Eto.SkiaDraw](https://github.com/rafntor/Eto.SkiaDraw)|[![pure](https://img.shields.io/nuget/v/Eto.SkiaDraw.svg)](https://www.nuget.org/packages/Eto.SkiaDraw/)||A control that allows using SkiaSharp drawing in Eto.
|[Eto.Containers](https://github.com/rafntor/Eto.Containers)|[![pure](https://img.shields.io/nuget/v/Eto.Containers.svg)](https://www.nuget.org/packages/Eto.Containers/)||Some extra Eto.Forms container controls.

Assemblies
----------

Your project only needs to reference Eto.dll, and include the corresponding platform assembly that you wish to target. To run on a Mac platform, you need to [bundle your app](https://github.com/picoe/Eto/wiki/Running-your-application).

* Eto.dll - Eto.Forms (UI), Eto.Drawing (Graphics), and platform loading
* Eto.Mac.dll - MonoMac platform for OS X using 32-bit mono
* Eto.Mac64.dll - MonoMac platform for OS X using 64-bit mono
* Eto.XamMac.dll - Xamarin.Mac Classic platform for OS X to embed mono
* Eto.XamMac2.dll - Xamarin.Mac Unified platform for OS X to embed mono
* Eto.WinForms.dll - Windows Forms platform using GDI+ for graphics
* Eto.Direct2D.dll - Windows Forms platform using Direct2D for graphics
* Eto.Wpf.dll - Windows Presentation Foundation platform
* Eto.Gtk.dll - Gtk+3 platform for Mac, Windows, and Linux.
* Eto.Gtk2.dll - Gtk2 platform using gtk-sharp2 on Mac, Windows, and Linux.
* Eto.Gtk3.dll - [deprecated] Gtk3 platform for running on Linux with gtk-sharp3 package
* Eto.iOS.dll - Xamarin.iOS platform
* Eto.Android.dll - Xamarin.Android platform

Currently supported targets
---------------------------

* OS X: MonoMac or Xamarin.Mac
* Linux: GTK# 2 or 3
* Windows: Windows Forms (using GDI or Direct2D) or WPF
	
Under development
-----------------

These platforms are currently incomplete or in development. Any eager bodies willing to help feel free to do so!

* iOS using Xamarin.iOS
* Android using Xamarin.Android (Eto.Android)
