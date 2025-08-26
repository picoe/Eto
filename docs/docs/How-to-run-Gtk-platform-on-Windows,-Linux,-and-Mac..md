Most platforms can only run on a single Operating System. For example, WinForms and Wpf only run on Windows and Mac/XamMac only run on macOS.  However, Gtk+ applications can run on many operating systems.

Eto _does not_ package Gtk+, as this is a huge dependency and is not necessary because of Eto's mission (run using the native toolkit on each platform).  For development purposes however, you may wish to run your Gtk port on either Mac or Windows for easier testing and debugging.  This document outlines what you need to install to get that running.

Note that **Eto.Gtk2** and **Eto.Gtk3** platforms are considered obsolete, and you should use **Eto.Gtk** instead. If you've used Eto's project templates to create your project, then you're good to go!  In some circumstances the old versions may still be desired.  If you can't use Eto.Gtk for any reason, please file an issue to help us understand why to help migrate everyone to the newer platform.

**Eto.Gtk** is built upon this excellent [GtkSharp package](https://github.com/cra0zy/GtkSharp), which supports Gtk+ 3.x.

## Windows

On Windows, you can run either **Eto.Gtk** or **Eto.Gtk2**.

For **Eto.Gtk**, you must install [Gtk+ 3](https://www.gtk.org/docs/installations/windows/).

For **Eto.Gtk2**, you must install [gtk# for .NET](https://www.mono-project.com/docs/gui/gtksharp/installer-for-net-framework/), then restart your computer.

## Mac

On macOS, you can run either **Eto.Gtk** or **Eto.Gtk2**.

For **Eto.Gtk**, you can install Gtk+3 via [homebrew](https://brew.sh) with `brew install gtk+3`

For **Eto.Gtk2**, you must install and run using [mono](https://www.mono-project.com/download/stable/#download-mac).

## Linux

On Linux, you can run either  **Eto.Gtk**, **Eto.Gtk2**, or **Eto.Gtk3**.

For **Eto.Gtk**, you only need Gtk+ 3.x installed which is usually already installed with the system.  You can run your application using .NET Core or mono.

For **Eto.Gtk2**, you must install the [gtk-sharp2 package](https://launchpad.net/ubuntu/+source/gtk-sharp2) and run via mono.

For **Eto.Gtk3**, you must install the [gtk-sharp3 package](https://launchpad.net/ubuntu/+source/gtk-sharp3) and run via mono.