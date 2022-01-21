using Eto.Drawing;
using Eto.Forms;
using System.Linq;
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
	public class WindowHandler : Window.IWindowHandler
	{
		public Window FromPoint(PointF point)
		{
			var mainFrame = NSScreen.Screens[0].Frame;
			var nspoint = new CGPoint(point.X, mainFrame.Height - point.Y);
			var windowNumber = NSWindow.WindowNumberAtPoint(nspoint, 0);
			foreach (var window in Application.Instance.Windows)
			{
				if (window.Handler is IMacWindow handler && handler.Control.WindowNumber == windowNumber)
				{
					return window;
				}
			}
			return null;
		}
	}
}