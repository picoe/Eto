Eto.Forms
=========
### A cross platform desktop and mobile user interface framework

Discussion
----------

* Join the [forums](http://groups.google.com/group/eto-forms)
* Chat in [#eto.forms](http://chat.mibbit.com/?server=irc.gimp.org&channel=%23eto.forms) on irc.gimp.org

Description
-----------

This framework is built so that you can target multiple platforms with one UI codebase.

The overall goal of this framework is to expose a common API that can be used to build functional
applications that run across platforms using their native toolkit.  This will make your applications look and 
work as if it were a native application on all platforms.

For advanced scenarios, you can take advantage of each platform's capabilities by wrapping your common UI
in a larger application, or even create your own high-level controls with a custom implementations per platform.

This framework currently supports creating Desktop applications that work across Windows Forms, WPF, MonoMac, and GTK#.
There is a Mobile/iOS port in the works, but is considered incomplete.

This framework was built so that using it in .NET is natural. For example, a simple hello-world application might look like:

	public class MyForm : Form
	{
		public MyForm ()
		{
			Text = "My Cross-Platform App";
			Size = new Size (200, 200);
			var label = new Label { Text = "Hello World!" };
			this.AddDockedControl (label);
		}
		
		[STAThread]
		static void Main () {
			var app = new Application();
			app.Initialized += delegate {
				app.MainForm = new MyForm ();
				app.MainForm.Show ();
			};
			app.Run ();
		}
	}

Download
--------

Binaries are available in the [Downloads](https://github.com/picoe/Eto/downloads) section

Applications
------------
* [Manager](https://www.manager.io) - Accounting Software
* [PabloDraw](http://picoe.ca/products/pablodraw/alpha) - Character based drawing application
* [JabbR.Eto](https://github.com/cwensley/JabbR.Eto) - JabbR client
* [Notedown by Mod Monkeys](https://github.com/modmonkeys/Notedown) - Note taking application
* [Eto.Test](https://github.com/picoe/Eto/tree/master/Source/Eto.Test) - Application to test the functionality of each widget

Namespaces
----------

* [Eto.Forms](https://github.com/picoe/Eto/tree/master/Source/Eto/Forms) - User interface 
* [Eto.Drawing](https://github.com/picoe/Eto/tree/master/Source/Eto/Drawing) - Drawing/graphical routines
* [Eto.IO](https://github.com/picoe/Eto/tree/master/Source/Eto/IO) - Disk/Virtual directory abstraction
* Eto.Platform.[Platform] - platform implementations

Currently supported targets
---------------------------

* OS X: MonoMac (Cocoa)
* Linux: GTK#
* Windows: Windows Forms or WPF
	
Under development
-----------------

* iOS using MonoTouch

	
Future plans
------------

* Android using Mono for Android (or equivalent)
