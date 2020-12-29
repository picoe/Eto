using System;
using Eto.Forms;
using sw = System.Windows;
using sd = System.Drawing;
using swm = System.Windows.Media;
using swf = System.Windows.Forms;
using Eto.Drawing;
using System.Linq;
using Eto.Wpf.Drawing;

namespace Eto.Wpf.Forms
{
	public class ScreenHandler : WidgetHandler<swf.Screen, Screen>, Screen.IHandler
	{
		float? realScale;
		sw.Window window;

		public ScreenHandler(sw.Window window, swf.Screen screen)
		{
			Control = screen;
			this.window = window;
		}

		public ScreenHandler(swf.Screen screen)
		{
			Control = screen;
		}

		float GetRealScale()
		{
			if (realScale != null)
				return realScale.Value;

			if (window != null)
			{
				var source = sw.PresentationSource.FromVisual(window);
				if (source != null)
				{
					realScale = (float)source.CompositionTarget.TransformToDevice.M22;
					window = null;
				}
			}

			if (realScale == null)
			{
				realScale = Control.GetLogicalPixelSize();
			}
			return realScale ?? 1f;
		}

		public Image GetImage(RectangleF rect)
		{
			var adjustedRect = rect * Widget.LogicalPixelSize;
			//adjustedRect.Location += Control.Bounds.Location.ToEto();

			var info = new Win32.MONITORINFOEX();
			Win32.GetMonitorInfo(Control, ref info);
			adjustedRect.Location += info.rcMonitor.ToEto().Location;

			var oldDpiAwareness = Win32.SetThreadDpiAwarenessContextSafe(Win32.DPI_AWARENESS_CONTEXT.PER_MONITOR_AWARE_v2);
			var realRect = Rectangle.Ceiling(adjustedRect);
			using (var screenBmp = new sd.Bitmap(realRect.Width, realRect.Height, sd.Imaging.PixelFormat.Format32bppRgb))
			{
				using (var bmpGraphics = sd.Graphics.FromImage(screenBmp))
				{
					bmpGraphics.CopyFromScreen(realRect.X, realRect.Y, 0, 0, realRect.Size.ToSD());
					var bitmapSource = sw.Interop.Imaging.CreateBitmapSourceFromHBitmap(
						screenBmp.GetHbitmap(),
						IntPtr.Zero,
						sw.Int32Rect.Empty,
						sw.Media.Imaging.BitmapSizeOptions.FromEmptyOptions());

					if (oldDpiAwareness != Win32.DPI_AWARENESS_CONTEXT.NONE)
						Win32.SetThreadDpiAwarenessContextSafe(oldDpiAwareness);

					return new Bitmap(new BitmapHandler(bitmapSource));
				}
			}
		}

		public float RealScale => GetRealScale() * Scale;

		public float Scale => 96f / 72f;

		public RectangleF Bounds => Control.GetBounds().ScreenToLogical(Control);

		public RectangleF WorkingArea => Control.GetWorkingArea().ScreenToLogical(Control);

		public int BitsPerPixel => Control.BitsPerPixel;

		public bool IsPrimary => Control.Primary;
	}
}