using Eto.Forms;
using System.Collections.Generic;

namespace Eto.Platform.GtkSharp.Forms
{
	public class ScreensHandler : IScreens
	{
		public ScreensHandler ()
		{
		}

		public void Initialize ()
		{
		}

		public Widget Widget { get; set; }

		public Eto.Generator Generator { get; set; }

		public IEnumerable<Screen> Screens
		{
			get
			{
				var display = Gdk.Display.Default;
				for (int i = 0; i < display.NScreens; i++) {
					var screen = display.GetScreen (i);
					for (int monitor = 0; monitor < screen.NMonitors; monitor++) {
						yield return new Screen (Generator, new ScreenHandler (screen, monitor));
					}
				}
			}
		}

		public Screen PrimaryScreen
		{
			get
			{
				return new Screen(Generator, new ScreenHandler(Gdk.Display.Default.DefaultScreen, 0));
			}
		}
	}
}

