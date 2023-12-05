using Eto;
using Eto.Test;
namespace Eto.Test.Gtk
{
	class Startup
	{
		[STAThread]
		static void Main(string[] args)
		{
			var platform = new Eto.GtkSharp.Platform();
			platform.Add<INativeHostControls>(() => new NativeHostControls());
			
			var app = new TestApplication(platform);
			app.TestAssemblies.Add(typeof(Startup).Assembly);
			app.Run();
		}
	}
}

