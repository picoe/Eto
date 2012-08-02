using System;
using System.Collections.Generic;
using System.Linq;

using MonoTouch.Foundation;
using MonoTouch.UIKit;
using Eto.Forms;
using Eto.Drawing;

namespace Eto.Test.iOS
{
	public class Startup
	{
		// This is the main entry point of the application.
		static void Main (string[] args)
		{
			// TODO: make desktop tests work in iOS
			// This will require much more work on iOS port to implement required events and controls
			//var app = new TestApplication (new Eto.Platform.iOS.Generator ());
			//app.Run (args);

			var app = new Application (new Eto.Platform.iOS.Generator ());
			app.Initialized += delegate {
				app.MainForm = new MainForm ();
				app.MainForm.Show ();
			};
			app.Run (args);
		}
	}
}
