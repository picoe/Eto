using Eto.Forms;
using Eto.Wpf.Forms;
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

			// Register the custom themes so they appear in the ThemeSection picker.
			// The control styles come from Eto.Wpf's reusable palette theme (themes/palette.xaml);
			// this app only supplies the palette colors — one xaml file per theme.
			ThemesHandler.Instance.Themes.Add(new PaletteTheme(
				"Custom (Mocha Dark)",
				ThemeStyle.Dark,
				new Uri("pack://application:,,,/Eto.Test.Wpf;component/themes/custom/MochaPalette.xaml")
			));
			ThemesHandler.Instance.Themes.Add(new PaletteTheme(
				"Custom (Midnight Blue)",
				ThemeStyle.Dark,
				new Uri("pack://application:,,,/Eto.Test.Wpf;component/themes/custom/MidnightPalette.xaml")
			));

			ThemesHandler.Instance.Themes.Add(new PaletteTheme(
				"Custom (Latte Light)",
				ThemeStyle.Light,
				new Uri("pack://application:,,,/Eto.Test.Wpf;component/themes/custom/LattePalette.xaml")
			));

			app.Run();
		}

	}
}

