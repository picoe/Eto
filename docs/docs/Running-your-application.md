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
- [Eto.Platform.XamMac2](https://www.nuget.org/packages/Eto.Platform.XamMac2/): For Xamarin.Mac unified mobile or full projects (recommended)

The Eto.Platform.Mac64 nuget package will create an .app bundle (or folder) in the project output directory, even when built on non-mac platforms. In release builds it will bundle mono or .NET Core, which you can override using `<MacBundleMono>` or `<MacBundleDotNet>` properties in your .csproj, which allows the application to run without installing any dependencies. Note that if you build on Windows then copy to macOS you need to perform the following steps after copying:

```bash
> chmod +x YourApp.app/Contents/MacOS/YourApp # set the executable bit
> xattr -c YourApp.app # Clear the extended attributes of the .app
```

If you use a [Xamarin.Mac](http://xamarin.com) project, it automatically bundles the mono framework inside the .app as well.  However, it can only be built on a Mac using VS for Mac.  This is required if you want your app to be published on the Mac App Store.

To distribute your app for macOS (even without publishing to the App Store), you need to code sign and notarize your application, otherwise your users will need to execute the `xattr -c` command above on macOS Catalina 10.15 or later.