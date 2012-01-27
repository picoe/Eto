using System;
using Eto;
using Eto.Misc;
using Eto.Test.Interface;

namespace Eto.Test
{
	class Startup
	{
		[STAThread]
		static void Main (string [] args)
		{
			Generator generator;

			if (Eto.Misc.Platform.IsWindows) {
				// use WPF
				generator = Generator.GetGenerator ("Eto.Platform.Wpf.Generator, Eto.Platform.Wpf");

				// use windows forms
				//generator = Generator.GetGenerator ("Eto.Platform.Windows.Generator, Eto.Platform.Windows");
			} else
				generator = Generator.GetGenerator ("Eto.Platform.GtkSharp.Generator, Eto.Platform.Gtk");
			
			
			var app = new TestApplication (generator);
			app.Run (args);
		}
	}
}

