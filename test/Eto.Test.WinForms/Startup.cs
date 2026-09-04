using Eto;
using Eto.Test;

namespace Eto.Test.WinForms
{
	class Startup
	{
		[STAThread]
		static void Main(string[] args)
		{
#if DEBUG
			HotReloadService.Initialize();
#endif
			var platform = new Eto.WinForms.Platform();
			platform.Add<INativeHostControls>(() => new NativeHostControls());
			platform.Add<Eto.Test.UnitTests.ITestInput>(() => new TestInput());

			var app = new TestApplication(platform);
#if NET9_0_OR_GREATER
			swf.Application.SetHighDpiMode(swf.HighDpiMode.PerMonitorV2);
#endif			
			app.TestAssemblies.Add(typeof(Startup).Assembly);
			app.Run();
		}
	}
}

