# Publishing Your Eto.Forms Application

Publishing your app can be quite different depending on the OS you're publishing to.

## Publish for Windows

Simply compile your Wpf or Winforms project in the IDE of your choice, or compile it with `dotnet build` from the command line.

To compile WPF and WinForms on Mac or Linux, you can add `<EnableWindowsTargeting>true</EnableWindowsTargeting>` to your project properties.

To distribute your application to users, an MSI or MSIX installer is the usual mechanism.  You can follow [Microsoft's guide](https://learn.microsoft.com/en-us/windows/msix/desktop/desktop-to-uwp-packaging-dot-net) on how to set that up.

## Publish for Linux

Simply compile your Gtk project in the IDE of your choice, or compile it with `dotnet build` from the command line.  
Please keep the following notes in mind:

- If you compile from Windows, due to NTFS not supporting executable file bits, users will first have to mark the application as executable. This can either be done via a command line with `chmod +x MyApp.Gtk`, or by **right-clicking -> Properties -> Permissions -> Mark as executable**.

- On distros using GNOME as their DE (Ubuntu, Fedora etc.) your icon will not be displayed unless you provide a .desktop file along with it. For details on how to write one, consult the following documentation pages:
  - <https://specifications.freedesktop.org/desktop-entry-spec/latest/>
  - <https://wiki.archlinux.org/title/Desktop_entries>
  - <https://help.ubuntu.com/community/UnityLaunchersAndDesktopFiles> 

- Users will have to install dependencies to run your program first. This includes: [the .NET Core runtime version of the project you're using](https://docs.microsoft.com/en-us/dotnet/core/install/linux) **or** mono if you're using .NET Framework, `gtk3`, `webkitgtk`, `openssl`, `icu` and `libappindicator`. The latter 5 should be preinstalled on most distros, but this isn't a guarantee.  
NOTE FOR DEBIAN USERS: Debian 11 "bullseye" removed `libappindicator` from their package repos, and their alternative `libayatana-appindicator` currently does not work with Eto. They can circumvent this by going [here](https://packages.debian.org/buster/amd64/libappindicator3-1/download) to either install `libappindicator` from there or follow the instructions listed there to add the "buster" repos to their package manager

- If you want to package the dependencies with your app, you have a few choices:
  - Create distro-specific packaging formats.
    - For .deb's (Debian and Ubuntu based) packages, you can consult this guide: <https://www.internalpointers.com/post/build-binary-deb-package-practical-guide>  
          Note that you likely need different .deb files for Debian and Ubuntu, due to them having different package names
    - For .rpm's (Red Hat based) packages, you can consult Red Hat's documentation: https://www.redhat.com/sysadmin/create-rpm-package
  
  - Create a universal packaging format:  
    - For AppImage, a one file click-and-run distribution method, see their guides here: <https://github.com/AppImage/AppImageKit/wiki/Bundling-.NET-Core-apps>
    - For Flatpaks, a sandboxed environment for your application, install-able via a package manger, consult their guide here:  <https://docs.flatpak.org/en/latest/first-build.html>  

      Note that on Linux it's preferred to not have duplicate dependencies. So while universal packaging formats with .NET Core bundled might be tempting, try to create distro-specific package formats that specify .NET Core as a dependency if it's possible. Unfortunately, this isn't always possible as some distos (Debian or Ubuntu for example) don't have it in their package repos.

## Publish for Mac

Simply compile your Mac project in the IDE of your choice, or compile it with `dotnet build` from the command line.

For `Eto.Platform.Mac64` platform targets, see [Mac Code Signing and Notarization](./Mac-Code-Signing-and-Notarization.md) page for more information on packaging your app for distribution, including codesigning, notarization, and bundling in a .dmg.

If you use a `Eto.Platforms.macOS` project, it can only be compiled (fully) from MacOS. It is worth noting that you need to ensure the following:

- That you have the macos dotnet workload installed. If you don't, you can install it via `dotnet workload install macos`.
- That your project's output type is `Exe` and not `WinExe` as it otherwise won't compile
- That your project's TFM has a `-macos` suffix (i.e. `net6.0-macos`)
- That your RID is set to `osx-x64;osx-arm64` if you want to support both Intel and M1 Macs
- That your project's `csproj` contains a `SupportedOSPlatformVersion` attribute, which should be set to the same version as the `LSMinimumSystemVersion` attribute in your `plist` file.

If you compile from Windows, due to NTFS not supporting executable file bits, users will first have to mark the application as executable. This can either be done via a command line with:

```sh
chmod +x MyApplication.Desktop.app/Contents/MacOS/MyApplication.Desktop
xattr -c MyApplication.Desktop.app
```

In order to distribute your apps to run on the Mac they should be [code signed and notarized](https://developer.apple.com/developer-id/).  Details on how to do that via command line are [here for code signing](https://developer.apple.com/library/archive/documentation/Security/Conceptual/CodeSigningGuide/Procedures/Procedures.html#//apple_ref/doc/uid/TP40005929-CH4-SW2) and [here for notarization](https://developer.apple.com/documentation/xcode/notarizing_macos_software_before_distribution/customizing_the_notarization_workflow).

You should also package your app in a .dmg or .pkg for distribution. [This site](https://www.recitalsoftware.com/blogs/148-howto-build-a-dmg-file-from-the-command-line-on-mac-os-x) has some good details on how to do that.
