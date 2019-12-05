using Eto;
using Eto.Test;
using System;
using System.Diagnostics;

namespace Eto.Test.Gtk
{
	class Startup
	{
		[STAThread]
		static void Main(string[] args)
		{
			var generator = new Eto.GtkSharp.Platform();
			
			var app = new TestApplication(generator);
			app.TestAssemblies.Add(typeof(Startup).Assembly);
			app.Run();
		}
	}
}

