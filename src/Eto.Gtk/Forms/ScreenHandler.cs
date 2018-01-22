using Eto.Forms;
using Eto.Drawing;

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
	}
}

