#if GTKCORE
using Eto.Forms;
using Eto.Drawing;
using Eto.GtkSharp.Drawing;
using System;

namespace Eto.GtkSharp.Forms
{
	public class ScreenHandler : WidgetHandler<Gdk.Monitor, Screen>, Screen.IHandler
	{
		public ScreenHandler(Gdk.Monitor screen)
		{
			Control = screen;
		}

		public float RealScale => Scale * Control.ScaleFactor;

		public float Scale => (float)Gdk.Screen.Default.Resolution / 72f;

		public RectangleF Bounds => Control.Geometry.ToEto();

		public RectangleF WorkingArea => Control.Workarea.ToEto();

		public int BitsPerPixel => 24;

		public bool IsPrimary => Control.IsPrimary;

		public Image GetImage(RectangleF rect)
		{
			// hm, doesn't seem to work on ubuntu 18.04, but I can't find any other way to do this..
			var rootWindowPtr = NativeMethods.gdk_get_default_root_window();
			if (rootWindowPtr == IntPtr.Zero)
				return null;

			var bounds = Bounds;
			rect.Location += bounds.Location;
			var rectInt = Rectangle.Ceiling(rect);

			var pbptr = NativeMethods.gdk_pixbuf_get_from_window(rootWindowPtr, rectInt.X, rectInt.Y, rectInt.Width, rectInt.Height);

			if (pbptr == IntPtr.Zero)
				return null;

			var pb = new Gdk.Pixbuf(pbptr);

			return new Bitmap(new BitmapHandler(pb));

		}
	}
}
#endif

