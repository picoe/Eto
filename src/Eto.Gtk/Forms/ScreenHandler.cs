using Eto.Forms;
using Eto.Drawing;
using Eto.GtkSharp.Drawing;
using System;

namespace Eto.GtkSharp.Forms
{
	public class ScreenHandler : WidgetHandler<Gdk.Screen, Screen>, Screen.IHandler
	{
		readonly int monitor;

		public ScreenHandler (Gdk.Screen screen, int monitor)
		{
			this.Control = screen;
			this.monitor = monitor;
		}

		public float RealScale
		{
			get { return (float)Control.Resolution / 72f; }
		}

		public float Scale
		{
			get { return (float)Control.Resolution / 72f; }
		}

		public RectangleF Bounds
		{
			get {
				return Control.GetMonitorGeometry (monitor).ToEto ();
			}
		}

		public RectangleF WorkingArea
		{
			get
			{
				// todo: available with GTK 3.4
				return Control.GetMonitorGeometry (monitor).ToEto ();
			}
		}

		public int BitsPerPixel
		{
			get { return 24; }
		}

		public bool IsPrimary
		{
			get { return monitor == 0; }
		}

		public Image GetImage(RectangleF rect)
		{
			// hm, doesn't seem to work on ubuntu 18.04, but I can't find any other way to do this..
			var rootWindowPtr = NativeMethods.gdk_get_default_root_window();
			if (rootWindowPtr == IntPtr.Zero)
				return null;

			var bounds = Bounds;
			rect.Location += bounds.Location;
			var rectInt = Rectangle.Ceiling(rect);

#if GTK2
			var drawable = new Gdk.Window(rootWindowPtr);
			var pb = Gdk.Pixbuf.FromDrawable(drawable, drawable.Colormap, rectInt.X, rectInt.Y, 0, 0, rectInt.Width, rectInt.Height);
#else
			var pbptr = NativeMethods.gdk_pixbuf_get_from_window(rootWindowPtr, rectInt.X, rectInt.Y, rectInt.Width, rectInt.Height);

			if (pbptr == IntPtr.Zero)
				return null;

			var pb = new Gdk.Pixbuf(pbptr);

#endif
			return new Bitmap(new BitmapHandler(pb));

		}
	}
}

