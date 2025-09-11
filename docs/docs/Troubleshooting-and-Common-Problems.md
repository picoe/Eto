# Troubleshooting and Common Problems

When troubleshooting errors in Eto.Forms, be sure to check the inner exception(s).  You can do this by adding `$exception.GetBaseException()` to the watch window after an exception is thrown.

## Problem: WPF platform does not run, but other platforms do

**Exception thrown**: Eto.HandlerInvalidException: Could not create instance of type Eto.Forms.IForm  
**Base exception**: InvalidOperationException: The calling thread must be STA, because many UI components require this.  
**Solution**: Add [STAThread] to your Main() method  

## Problem: Newtonsoft.Json is clearly available, Eto.Json refuses to load it.

**Exception thrown**: System.IO.FileLoadException: Could not load file or assembly 'Newtonsoft.Json, Version=4.5.0.0, Culture=neutral, PublicKeyToken=30ad4fe6b2a6aeed' or one of its dependencies. The located assembly's manifest definition does not match the assembly reference.
**Solution**: Use the following runtime redirect in your app.config file:

```xml
<runtime>
    <assemblyBinding xmlns="urn:schemas-microsoft-com:asm.v1">
        <dependentAssembly>
            <assemblyIdentity name="Newtonsoft.Json" publicKeyToken="30ad4fe6b2a6aeed" culture="neutral" />
            <bindingRedirect oldVersion="0.0.0.0-6.0.0.0" newVersion="6.0.0.0" />
        </dependentAssembly>
    </assemblyBinding>
</runtime>
```

### Problem: The .app bundle application compiled on Windows does not run on Mac

This is usually because the executable bit is not set on the executable file within the .app bundle.  Since windows file systems (NTFS, usually) do not support executable bits like *nix file systems, this information is lost.

For development or testing purposes, you can get your apps to run on Mac by running these commands:

```sh
chmod +x MyApplication.Desktop.app/Contents/MacOS/MyApplication.Desktop
xattr -c MyApplication.Desktop.app
```

To distribute your apps to run on the Mac without these steps, they need to be [code signed and notarized](./Mac-Code-Signing-and-Notarization.md).

### Problem: My Mac application doesn't run from the command line

To run an .app bundle on macOS you cannot use mono or dotnet at the command line to run it, you must run the native executable to launch the application using MonoMac or Xamarin.Mac, or use open. The native executable is within the `Contents/MacOS` folder in the .app bundle. E.g. from Terminal:

```sh
./MyApplication.Desktop.app/Contents/MacOS/MyApplication.Desktop
```

or

```sh
open MyApplication.Desktop.app
```

### Problem: My Xamarin.Mac project compiles, but quits immediately

This is usually caused when you have something set for the `NSMainNibFile` setting in your `Info.plist` file.  It is by default set to *MainMenu*.  To fix this, set it to blank.  In Xamarin Studio:

1. Double click on the Info.plist file
2. Set the **Main Interface** box to blank
3. Save and run your application again.

You can also follow [these directions](./Running-and-Debugging-a-Mac-application-in-Xamarin-Studio.md) to create a new MonoMac/Xamarin.Mac project.

### Problem: When distributing an OS X app to users, the application quits with the message "The operation couldn’t be completed. (OSStatus error -67062.)"

This is caused because you are archiving the .app bundle on your Mac, which will include the __MACOS folder in the archive to set file permissions, etc.  You must create a .zip archive without the __MACOS folder by zipping on Windows, or following [these instructions](http://stackoverflow.com/questions/10924236/mac-zip-compress-without-macosx-folder) on OS X.

### Problem: GTK# v2 app is not running on windows.

This can happen sometimes when the paths aren't set up correctly by the GTK#2 installer, and is not a problem with Eto.Forms.  Uninstalling then re-installing GTK# v2 (from [here](http://www.mono-project.com/download/#download-win)) and rebooting your machine usually works.  See [here](https://github.com/picoe/Eto/issues/442) for details if you want to get around this issue.

### Problem: I'd like to run Gtk on Windows or Mac.

See [How to run Gtk platform on Windows, Linux, and Mac.](./How-to-run-Gtk-platform-on-Windows-Linux-and-Mac.md).