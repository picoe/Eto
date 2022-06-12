using System;
using Eto.Forms;
using sd = System.Drawing;
using swf = System.Windows.Forms;
using Eto.Drawing;
using Eto.WinForms.Drawing;
using System.Runtime.InteropServices;

namespace Eto.WinForms.Forms
{
	public class ScreenHandler : WidgetHandler<swf.Screen, Screen>, Screen.IHandler
	{
		float? scale;

		public ScreenHandler(swf.Screen screen)
		{
			Control = screen;
		}

		public float RealScale => scale ?? (scale = Control.GetLogicalPixelSize()) ?? 1;

		public float Scale => 96f / 72f;
		public RectangleF Bounds => Control.Bounds.ToEto();

		public RectangleF WorkingArea => Control.WorkingArea.ToEto();

		public int BitsPerPixel => Control.BitsPerPixel;

		public bool IsPrimary => Control.Primary;

		const int DESKTOPVERTRES = 0x75;
		const int DESKTOPHORZRES = 0x76;

		[DllImport("gdi32.dll")]
		static extern int GetDeviceCaps(IntPtr hdc, int nIndex);

		public Image GetImage(RectangleF rect)
		{
			var bounds = Bounds;
			var realBounds = bounds;

			var hmonitor = Win32.MonitorFromPoint(bounds.Location.ToSDPoint(), 0);
			if (hmonitor != IntPtr.Zero)
			{
				// get actual monitor dimentions
				var oldDpiAwareness = Win32.SetThreadDpiAwarenessContextSafe(Win32.DPI_AWARENESS_CONTEXT.PER_MONITOR_AWARE);

				var info = new Win32.MONITORINFOEX();
				Win32.GetMonitorInfo(new HandleRef(null, hmonitor), info);
				realBounds = info.rcMonitor.ToSD().ToEto();

				if (oldDpiAwareness != Win32.DPI_AWARENESS_CONTEXT.NONE)
					Win32.SetThreadDpiAwarenessContextSafe(oldDpiAwareness);
			}

			var adjustedRect = rect;
			adjustedRect.Size *= (float)(realBounds.Width / bounds.Width);
			adjustedRect.Location += realBounds.Location;
			var realRect = Rectangle.Ceiling(adjustedRect);
			var screenBmp = new sd.Bitmap(realRect.Width, realRect.Height, sd.Imaging.PixelFormat.Format32bppRgb);
			using (var bmpGraphics = sd.Graphics.FromImage(screenBmp))
			{
				bmpGraphics.CopyFromScreen(realRect.X, realRect.Y, 0, 0, realRect.Size.ToSD());

			}
			return new Bitmap(new BitmapHandler(screenBmp));
		}
	}
}

