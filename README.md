Eto.Forms
=========
### A cross platform desktop and mobile user interface framework

[![Build Status](https://dev.azure.com/picoe/Eto.Forms/_apis/build/status/picoe.Eto?branchName=develop)](https://dev.azure.com/picoe/Eto.Forms/_build/latest?definitionId=1&branchName=develop)
[![forums](https://img.shields.io/badge/join-the%20forums-yellowgreen.svg)](http://groups.google.com/group/eto-forms)
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
* [RegexTestBench](https://github.com/CommonLoon102/RegexTestBench) - Cross-platform regex testing tool in .NET Core
* [RegexFileSearcher](https://github.com/CommonLoon102/RegexFileSearcher) - Cross-platform regex file searching tool in .NET Core

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

* iOS using Xamarin.iOS
* OS X: MonoMac or Xamarin.Mac
* Linux: GTK# 2 or 3
* Windows: Windows Forms (using GDI or Direct2D) or WPF
	
Under development
-----------------

These platforms are currently in development. Any eager bodies willing to help feel free to do so!

* Android using Xamarin.Android (Eto.Android)
* Windows 8.x and Windows Phone (Eto.WinRT)
