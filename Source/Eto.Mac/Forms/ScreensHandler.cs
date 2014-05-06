using Eto.Forms;
using System.Collections.Generic;
using MonoMac.AppKit;

namespace Eto.Mac.Forms
{
	public class ScreensHandler : IScreens
	{
		public IEnumerable<Screen> Screens
		{
			get
			{
				foreach (var screen in NSScreen.Screens)
				{
					yield return new Screen(new ScreenHandler(screen));
				}
			}
		}

		public Screen PrimaryScreen
		{
			get
			{
				var screen = NSScreen.Screens[0];
				return new Screen(new ScreenHandler(screen));
			}
		}
	}
}

