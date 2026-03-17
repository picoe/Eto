using Eto;
using Eto.Test;
namespace Eto.Test.GirCore;

class Startup
{
	[STAThread]
	static void Main(string[] args)
	{
#if DEBUG
		HotReloadService.Initialize();
#endif
		var platform = new Eto.GirCore.Platform();
		// platform.Add<INativeHostControls>(() => new NativeHostControls());
		var app = new TestApplication(platform);
		app.TestAssemblies.Add(typeof(Startup).Assembly);
		app.Run();
	}
}

