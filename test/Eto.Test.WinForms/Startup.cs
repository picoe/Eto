using Eto;
using Eto.Test;

namespace Eto.Test.WinForms
{
	class Startup
	{
		[STAThread]
		static void Main(string[] args)
		{
			var platform = new Eto.WinForms.Platform();
			platform.Add<INativeHostControls>(() => new NativeHostControls());

			var app = new TestApplication(platform);
			app.TestAssemblies.Add(typeof(Startup).Assembly);
			app.Run();
		}
	}
}

