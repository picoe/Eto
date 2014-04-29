using Eto;
using Eto.Test;
using System.Diagnostics;

namespace Eto.Test.Gtk3
{
	class Startup
	{
		//[STAThread]
		static void Main (string [] args)
		{
#if DEBUG
			Debug.Listeners.Add (new ConsoleTraceListener());
#endif
			var generator = Platform.Get (Platforms.Gtk3);
			
			var app = new TestApplication (generator);
			app.Run (args);
		}
	}
}

