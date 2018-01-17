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
				var display = Gdk.Display.Default;
				for (int i = 0; i < display.NScreens; i++) {
					var screen = display.GetScreen (i);
					for (int monitor = 0; monitor < screen.NMonitors; monitor++) {
						yield return new Screen (new ScreenHandler (screen, monitor));
					}
				}
			}
		}

		public Screen PrimaryScreen
		{
			get
			{
				return new Screen(new ScreenHandler(Gdk.Display.Default.DefaultScreen, 0));
			}
		}
	}
}

