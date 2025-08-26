Contributing to Eto.Forms
=========================

Code conventions
----------------

We use [EditorConfig](http://editorconfig.org) to automatically use correct coding conventions.

System Requirements
-------------------

These are the system requirements for developing and/or contributing to Eto.Forms. Note that these are not necessarily the same requirements for running Eto.Forms apps, which are outlined in [[Supported Platforms]].

### Windows

On Windows, you can target WPF, WinRT, Windows Forms, and GTK#2.

* Visual Studio 2019
* [.NET Core SDK](https://dotnet.microsoft.com/download)
* (optional) For GTK#2: [GTK# for Microsoft .NET](http://www.mono-project.com/download/#download-win)
* (optional) To bundle Eto.Mac64 apps with mono: [mono for windows](http://www.mono-project.com/download/#download-win)

Notes:

* Cannot build or run Xamarin.Mac applications
* Can build and package MonoMac applications, but cannot run them

### macOS - Visual Studio for Mac

On Mac OS X, you can target MonoMac, Xamarin.Mac, Gtk+3, or Gtk#2.

* [Mono](https://www.mono-project.com/download/stable/#download-mac)
* [.NET Core SDK](https://dotnet.microsoft.com/download)
* [Xamarin Studio 8.6 or greater](http://monodevelop.com)
* For App Store Mac apps: [Xamarin.Mac](http://xamarin.com/platform)

Notes:

* Cannot build or run WPF or WinForms applications

### Linux

On Linux, you can target Gtk#2 on mono, and Gtk+3 either for .NET Core or mono.

* [Mono 6.x](http://www.mono-project.com/download/)
* [.NET Core SDK](https://dotnet.microsoft.com/download)
* [Visual Studio Code](https://code.visualstudio.com)
* For Gtk+3 (Eto.Gtk): Gtk+ 3.20 or greater
* For Gtk#2 (Eto.Gtk2): **gtk-sharp2** package from your distribution

Notes:

* For Nuget packages to download properly on linux, you need to import the SSL root certificates using the `mozroots --import --sync` command.
* Cannot build WPF, WinForms, Xamarin.iOS, Xamarin.Android, or Xamarin.Mac applications
* Can build and package MonoMac applications, but not run them

### Visual Studio Code (Windows, Linux, Mac)

* [Mono](https://www.mono-project.com/download/stable/#download-mac)
* [.NET Core SDK](https://dotnet.microsoft.com/download)
* [Visual Studio Code](https://code.visualstudio.com)
* Mac only (optional): [Xamarin.Mac](http://xamarin.com/platform)

Notes:

* Can only build/run WPF and WinForms applications on Windows
* Can only build/run Xamarin.Mac applications on macOS

Building
--------

To build you can use one of the solution files:

* **Eto.sln** - All projects for both .NET Framework and .NET Core
* **Eto.Core.sln** - .NET Core only projects

To build Eto.Forms and associated NuGet packages outside of your IDE, you can run the **Resources/build.sh** (mac/linux) or **Resources\build.cmd** (windows) scripts.
You can also pass a parameter to these scripts to set the new version.  E.g.

``./build.sh 2.1.0`` or ``build.cmd 2.1.0``

Pull Requests
-------------

Ensure your pull request builds and passes all checks. A check or X will show next to your pull request on github showing you the build status.

Contributing Guidelines
-----------------------

1. **You agree to the licensing terms**

	Eto.Forms is [BSD-3 Licensed](http://opensource.org/licenses/BSD-3-Clause). By sending a pull request, you agree to license your contributions by the same license.