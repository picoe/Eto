using Eto.Test;
using System.Diagnostics;

namespace Eto.Test.Gtk
{
	class Startup
	{
		//[STAThread]
		static void Main (string [] args)
		{
#if DEBUG
			Debug.Listeners.Add (new ConsoleTraceListener());
#endif
			var generator = new Eto.Platform.GtkSharp.Generator ();
			
			var app = new TestApplication (generator);
			app.Run (args);
		}
	}
}

