Eto - Cross platform desktop and mobile user interface framework
================================================================

Description
-----------

This framework is built so that you can target multiple platforms with one set of code.

The goal of this framework is NOT to expose all functionality of each platform, but to expose
the common set to build functional applications that can be run on all platforms.

A great use of this is to build smaller portions of the UI that is needed from core libraries
such as plugins, etc, and to use the platform-specific UI to wrap around that.

This library will suffer from "Lowest Common Denominator" and is no replacement for each
platform's framework.  Ideally, one would create the user interface for each platform to take 
full advantage of its capabilities, but may take much more time to create than to use this 
framework for portions of your app.

Note that it is planned to have some different controls for Mobile platforms vs. Desktop.

This framework was built so that using it in .NET is natural.
For example, a simple hello-world application might look like:

	public class MyForm : Form {
	
		public MyForm() {
			Title = "My Cross-Platform App";
			Size = new Size(200, 200);
			var label = new Label{
				Text = "Hello World!"
			};
			this.AddDockedControl(label);
		}
		
		public static void Main() {
		
			// run application!	
			var app = new Application{
				MainForm = new MyForm()
			};
			app.Run();
		}
	}


Namespaces
----------

* [Eto.Forms](https://github.com/picoe/Eto/tree/master/Source/Eto/Forms) - User interface 
* [Eto.Drawing](https://github.com/picoe/Eto/tree/master/Source/Eto/Drawing) - Drawing/graphical routines
* [Eto.IO](https://github.com/picoe/Eto/tree/master/Source/Eto/IO) - Disk/Virtual directory abstraction
* Eto.Platform.[Platform] - platform implementations

Currently supported targets
---------------------------

* Mac OS X using MonoMac
* Linux using GTK#
* Windows using Windows Forms
	
Under development
-----------------

* iOS using MonoTouch
	
Future plans
------------

* Android using Mono for Android (or equivalent)

Not Working
-----------

These were built a LONG time ago with an older version of the core framework, and would need substantial updates to get working again.

* Web using ASP.NET
* WXWidgets
