using System;
using Eto;
using Eto.Misc;
using Eto.Test;

namespace Eto.Test.Gtk
{
	class Startup
	{
		//[STAThread]
		static void Main (string [] args)
		{
			var generator = Generator.GetGenerator ("Eto.Platform.GtkSharp.Generator, Eto.Platform.Gtk");
			
			var app = new TestApplication (generator);
			app.Run (args);
		}
	}
}

