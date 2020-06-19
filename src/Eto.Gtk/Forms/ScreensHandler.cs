using Eto.Forms;
using System.Collections.Generic;

namespace Eto.GtkSharp.Forms
{
	public class ScreensHandler : Screen.IScreensHandler
	{
		public void Initialize ()
		{
		}

		public Widget Widget { get; set; }

		public Eto.Platform Platform { get; set; }

		public IEnumerable<Screen> Screens
		{
			get
			{
#if GTKCORE
				var display = Gdk.Display.Default;
				for (int i = 0; i < display.NMonitors; i++)
				{
					var monitor = display.GetMonitor(i);
					yield return new Screen(new ScreenHandler(monitor));
				}

#else
				var display = Gdk.Display.Default;
				for (int i = 0; i < display.NScreens; i++) {
					var screen = display.GetScreen (i);
					for (int monitor = 0; monitor < screen.NMonitors; monitor++) {
						yield return new Screen (new ScreenHandler (screen, monitor));
					}
				}
#endif
			}
		}

		public Screen PrimaryScreen
		{
			get
			{
#if GTKCORE
				return new Screen(new ScreenHandler(Gdk.Display.Default.PrimaryMonitor));
#else
				return new Screen(new ScreenHandler(Gdk.Display.Default.DefaultScreen, 0));
#endif
			}
		}
	}
}

