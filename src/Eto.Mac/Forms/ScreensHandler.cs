using Eto.Forms;
using System.Collections.Generic;
#if XAMMAC2
using AppKit;
using Foundation;
using CoreGraphics;
using ObjCRuntime;
using CoreAnimation;
#else
using MonoMac.AppKit;
using MonoMac.Foundation;
using MonoMac.CoreGraphics;
using MonoMac.ObjCRuntime;
using MonoMac.CoreAnimation;
#endif

namespace Eto.Mac.Forms
{
	public class ScreensHandler : Screen.IScreensHandler
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

