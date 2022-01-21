using Eto.Drawing;
using Eto.Forms;
using System.Linq;

namespace Eto.GtkSharp.Forms
{
	public class WindowHandler : Window.IWindowHandler
	{
		public Window FromPoint(PointF point)
		{
			var screenWindowStackWorks = false;
			var windowStack = Gdk.Screen.Default.WindowStack;
			if (windowStack != null)
			{
				// doesn't work on Mac or Windows.. 🤔
				foreach (var gdkWindow in windowStack.Reverse())
				{
					screenWindowStackWorks = true;
					if (!gdkWindow.FrameExtents.Contains((int)point.X, (int)point.Y))
						continue;
						
					foreach (var window in Application.Instance.Windows)
					{
						if (window.Handler is IGtkWindow handler && handler.Control.GetWindow()?.Handle == gdkWindow.Handle)
						{
							return window;
						}
					}
				}
			}

			if (!screenWindowStackWorks)
			{
				// fallback to looking at all windows in no particular order.
				// TODO: this needs a proper implementation for Mac and Windows since Screen.WindowStack doesn't actually work
				var pt = Point.Round(point);
				foreach (var window in Application.Instance.Windows)
				{
					if (window.Bounds.Contains(pt))
					{
						return window;
					}
				}
			}
			return null;
		}
	}
}