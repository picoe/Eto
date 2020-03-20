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
			var realRect = Rectangle.Ceiling(rect * Widget.LogicalPixelSize);
			using (var screenBmp = new sd.Bitmap(realRect.Width, realRect.Height, sd.Imaging.PixelFormat.Format32bppArgb))
			{
				using (var bmpGraphics = sd.Graphics.FromImage(screenBmp))
				{
					bmpGraphics.CopyFromScreen(realRect.X, realRect.Y, 0, 0, realRect.Size.ToSD());
					var bitmapSource = sw.Interop.Imaging.CreateBitmapSourceFromHBitmap(
						screenBmp.GetHbitmap(),
						IntPtr.Zero,
						sw.Int32Rect.Empty,
						sw.Media.Imaging.BitmapSizeOptions.FromEmptyOptions());

					return new Bitmap(new BitmapHandler(bitmapSource));
				}
			}
		}

		public float RealScale => GetRealScale() * Scale;

		public float Scale => 96f / 72f;

		public RectangleF Bounds => Control.Bounds.ScreenToLogical();

		public RectangleF WorkingArea => Control.WorkingArea.ScreenToLogical();

		public int BitsPerPixel => Control.BitsPerPixel;

		public bool IsPrimary => Control.Primary;
	}
}