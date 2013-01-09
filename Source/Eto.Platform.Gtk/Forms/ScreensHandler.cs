using System;
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
				foreach (var display in Gdk.DisplayManager.Get ().ListDisplays ())
				{
					yield return new Screen(Generator, new ScreenHandler(display));
				}
			}
		}

		public Screen PrimaryScreen
		{
			get
			{
				var display = Gdk.DisplayManager.Get ().DefaultDisplay;
				return new Screen(Generator, new ScreenHandler(display));
			}
		}
	}
}

