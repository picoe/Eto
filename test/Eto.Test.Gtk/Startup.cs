using Eto;
using Eto.Test;
namespace Eto.Test.Gtk
{
	class Startup
	{
		[STAThread]
		static void Main(string[] args)
		{
#if DEBUG
			HotReloadService.Initialize();
#endif
			var platform = new Eto.GtkSharp.Platform();
			platform.Add<INativeHostControls>(() => new NativeHostControls());
			platform.Add<Eto.Test.UnitTests.ITestInput>(() => new TestInput());
			
			var app = new TestApplication(platform);
			app.TestAssemblies.Add(typeof(Startup).Assembly);
			app.Run();
		}
	}
}

