# Quick Start

The easiest way to get started with Eto.Forms is to use the Visual Studio extensions or dotnet new. These provide a way to create new Eto.Forms projects very easily, either with a single project or separate launcher projects per platform.

## Visual Studio for Windows

Make sure you have installed the latest Visual Studio updates.  Then, install the extension by following these steps (or download for [VS 2022](https://marketplace.visualstudio.com/items?itemName=CurtisWensley.EtoDevExtensionVisualStudio2022)):

1. Open **Extensions > Manage Extensions**
2. Select **Online > Visual Studio Marketplace**
3. In the search box, type ``Eto.Forms``
4. Select **Eto.Forms Visual Studio Addin** and click **Download**
5. Restart Visual Studio to install the extension.

Now you can easily create a new project using the **Eto.Forms Application** template.

## Command Line with dotnet new

With the [.NET Core SDK](https://www.microsoft.com/net/download) installed, run the following from the command line:

```bash
> dotnet new -i Eto.Forms.Templates
> dotnet new etoapp --help  # shows project creation options
> mkdir MyApp
> cd MyApp
> dotnet new etoapp -m xaml  # create a project xaml definitions
> dotnet build MyApp.Mac/MyApp.Mac.csproj
> dotnet build MyApp.Wpf/MyApp.Wpf.csproj
> dotnet build MyApp.Gtk/MyApp.Gtk.csproj
```

You can then run using `dotnet run` in the target folders for the platform you are on.

**Note** you currently cannot build .NET Core based WPF or WinForms apps on non-windows platforms.  Instead, you can build each of the .csproj's directly or create solution configurations for each platform.

## VS Code

VS Code is a great alternative to using VS for Mac or Windows and supports .NET Core and mono debugging.

For Eto.Mac targets, you need to set the "program" variable to the executable within the .app bundle, something like `${workspaceFolder}/MyApp.Mac/bin/Debug/net8.0/MyApp.Mac.app/Contents/MacOS/MyApp.Mac`

## JetBrains Rider

JetBrains Rider is a great cross-platform alternative for Visual Studio.  
With the [.NET Core SDK](https://www.microsoft.com/net/download) installed, run the following from the command line: `dotnet new -i "Eto.Forms.Templates::*"` 

Now you can easily create a new project using the **Eto Application** template.

For Eto.Mac targets, you need to set the startup program to the executable within the .app bundle, something like `bin/Debug/net8.0/MyApp.Mac.app/Contents/MacOS/MyApp.Mac`.
