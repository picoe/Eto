using Eto.Test;
using System.Diagnostics;

namespace Eto.Test.Gtk2
{
	class Startup
	{
		//[STAThread]
		static void Main(string[] args)
		{
			var platform = new Eto.GtkSharp.Platform();
			
			var app = new TestApplication(platform, typeof(Startup).Assembly);
			app.Run();
		}
	}
}

