using System;
using Eto.Drawing;
using Eto.Forms;
using Eto.Platform.GtkSharp.Drawing;

namespace Eto.Platform.GtkSharp
{
	public class DrawableHandler : GtkControl<Gtk.DrawingArea, Drawable>, IDrawable
	{
		public void Create ()
		{
			Control = new Gtk.DrawingArea ();
			Control.ExposeEvent += control_ExposeEvent;
			Control.Events |= Gdk.EventMask.ExposureMask;
			//Control.ModifyBg(Gtk.StateType.Normal, new Gdk.Color(0, 0, 0));
			//Control.DoubleBuffered = false;
			Control.CanFocus = false;
			Control.CanDefault = true;
			
		}
		
		public bool CanFocus {
			get {
				return Control.CanFocus;
			}
			set {
				Control.CanFocus = value;
			}
		}

		void control_ExposeEvent (object o, Gtk.ExposeEventArgs args)
		{
			Gdk.EventExpose ev = args.Event;
			using (var graphics = new Graphics (Widget.Generator, new GraphicsHandler (Control, ev.Window, Control.Style.BlackGC))) {
				Rectangle rect = Generator.Convert (ev.Region.Clipbox);
				Widget.OnPaint (new PaintEventArgs (graphics, rect));
			}
		}

		public void Update (Rectangle rect)
		{
			using (var graphics = new Graphics (Widget.Generator, new GraphicsHandler (Control, Control.GdkWindow, Control.Style.BlackGC))) {
				Widget.OnPaint (new PaintEventArgs (graphics, rect));
			}
		}
	}
}
