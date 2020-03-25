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
			var ss = Bounds.Size;

			// get scale based on actual pixel size, we don't support high DPI on winforms (yet)
			// and the CopyFromScreen API always requires actual pixel size.
			using (var g = sd.Graphics.FromHwnd(IntPtr.Zero))
			{
				var hdc = g.GetHdc();

				ss.Width = GetDeviceCaps(hdc, DESKTOPHORZRES);
				ss.Height = GetDeviceCaps(hdc, DESKTOPVERTRES);

				g.ReleaseHdc(hdc);
			}

			var realRect = Rectangle.Ceiling(rect * (float)(ss.Width / Bounds.Width));
			var screenBmp = new sd.Bitmap(realRect.Width, realRect.Height, sd.Imaging.PixelFormat.Format32bppArgb);
			using (var bmpGraphics = sd.Graphics.FromImage(screenBmp))
			{
				bmpGraphics.CopyFromScreen(realRect.X, realRect.Y, 0, 0, realRect.Size.ToSD());

			}
			return new Bitmap(new BitmapHandler(screenBmp));
		}
	}
}

