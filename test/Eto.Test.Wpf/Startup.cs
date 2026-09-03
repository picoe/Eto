using Eto.Wpf.Forms.Controls;
using System.Windows.Media;

namespace Eto.Test.Wpf
{
	class Startup
	{
		[STAThread]
		static void Main(string[] args)
		{
#if DEBUG
			Eto.HotReloadService.Initialize();
#endif
			var platform = new Eto.Wpf.Platform();
			platform.Add<INativeHostControls>(() => new NativeHostControls());
			platform.Add<Eto.Test.UnitTests.ITestInput>(() => new TestInput());

			// optional - enables GDI text display mode
			/**
			Style.Add<Eto.Wpf.Forms.FormHandler>(null, handler => TextOptions.SetTextFormattingMode(handler.Control, TextFormattingMode.Display));
			Style.Add<Eto.Wpf.Forms.DialogHandler>(null, handler => TextOptions.SetTextFormattingMode(handler.Control, TextFormattingMode.Display));
			/**/

			var app = new TestApplication(platform);
			app.TestAssemblies.Add(typeof(Startup).Assembly);
			app.Run();
		}

	}
}

