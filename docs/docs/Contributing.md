# Contributing to Eto.Forms

## Code conventions

We use [EditorConfig](http://editorconfig.org) to automatically use correct coding conventions.

## System Requirements

These are the system requirements for developing and/or contributing to Eto.Forms. Note that these are not necessarily the same requirements for running Eto.Forms apps, which are outlined in [Supported Platforms](./Supported-Platforms.md).

### Windows

On Windows, you can target WPF, WinRT, Windows Forms, and GTK+3.

* [Visual Studio 2022](https://visualstudio.com) or [Visual Studio Code](https://code.visualstudio.com)
* [.NET Core SDK](https://dotnet.microsoft.com/download)
* (optional) For macOS: [.NET macos workload](https://learn.microsoft.com/en-us/dotnet/core/tools/dotnet-workload-install)
* (optional) For GTK+3: [GTK+3](http://www.mono-project.com/download/#download-win)

Notes:

* Can build but not run macOS or Mac64 applications

### macOS

On Mac OS X, you can target Mac64, macOS, and Gtk+3.

* [Visual Studio Code](https://code.visualstudio.com)
* [.NET Core SDK](https://dotnet.microsoft.com/download)
* [GTK+3](https://formulae.brew.sh/formula/gtk+3)

Notes:

* Can build but not run WPF or WinForms applications

### Linux

On Linux, you can target Gtk+3 for either .NET Core or mono.

* [Visual Studio Code](https://code.visualstudio.com)
* [.NET Core SDK](https://dotnet.microsoft.com/download)
* [Mono 6.x](http://www.mono-project.com/download/)
* For Gtk+3 (Eto.Gtk): Gtk+ 3.20 or greater

Notes:

* Can build but not run WPF, WinForms, macOS, or Mac64 applications

## Building

To build you can use the VS Code Tasks, or load up the **Eto.sln** file.

To build Eto.Forms and associated NuGet packages outside of your IDE, you can run the **Resources/build.sh** (mac/linux) or **Resources\build.cmd** (windows) scripts.
You can also pass a parameter to these scripts to set the new version.  E.g.

``./build.sh 2.1.0`` or ``build.cmd 2.1.0``

## Pull Requests

Ensure your pull request builds and passes all checks. A check or X will show next to your pull request on github showing you the build status.

## Contributing Guidelines

1. **You agree to the licensing terms**

Eto.Forms is [BSD-3 Licensed](http://opensource.org/licenses/BSD-3-Clause). By sending a pull request, you agree to license your contributions by the same license.
