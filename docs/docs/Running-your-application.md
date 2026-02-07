# Running Your Application

To run your application, you will need to include one or more of the platform nuget packages in your executable project.

**For Windows:**

- [Eto.Platform.Wpf](https://www.nuget.org/packages/Eto.Platform.Wpf/): for Windows Presentation Foundation (recommended)
- [Eto.Platform.Windows](https://www.nuget.org/packages/Eto.Platform.Windows/): for Windows Forms

**For Linux / Unix:**

- [Eto.Platform.Gtk](https://www.nuget.org/packages/Eto.Platform.Gtk/): for Gtk 3.14+ and includes Gtk# assemblies as nuget dependencies (recommended)
- [Eto.Platform.Gtk2](https://www.nuget.org/packages/Eto.Platform.Gtk2/): for Gtk# 2.12, and requires gtk-sharp2
- [Eto.Platform.Gtk3](https://www.nuget.org/packages/Eto.Platform.Gtk3/): for Gtk# 3.0 and requires gtk-sharp3

**For macOS:**

- [Eto.Platform.Mac64](https://www.nuget.org/packages/Eto.Platform.Mac64/): 64-bit via MonoMac (recommended)
- [Eto.Platform.macOS](https://www.nuget.org/packages/Eto.Platform.macOS/): using .NET Core's macos workload (recommended)

The `Eto.Platform.Mac64` platform target will create an .app bundle (or folder) in the project output directory, even when built on non-mac platforms. In release builds it will bundle the .NET Core runtime, which you can override using `<MacBundleDotNet>` property in your .csproj, which allows the application to run without installing any dependencies. Note that if you build on Windows then copy to macOS you need to perform the following steps after copying:

```bash
> chmod +x YourApp.app/Contents/MacOS/YourApp # set the executable bit
> xattr -c YourApp.app # Clear the extended attributes of the .app
```

The `Eto.Platform.macOS` platform target will also automatically bundle the .NET Core runtime inside the .app.  However, it can only be built on a Mac.

## Next Steps

After building and testing your application, you can follow the [Publishing Your Application](./Publishing-your-app.md) to learn how to distribute the application to users.