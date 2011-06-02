using System;
using Eto.Forms;
using Eto.Drawing;

namespace Eto.Platform.GtkSharp
{
	public class PixelLayoutHandler : GtkLayout<Gtk.Fixed, PixelLayout>, IPixelLayout
	{
		public PixelLayoutHandler()
		{
			Control = new Gtk.Fixed();
		}
		
		public void Add(Control child, int x, int y)
		{
			IGtkControl ctl = ((IGtkControl)child.Handler);
			Control.Put((Gtk.Widget)child.ControlObject, x, y);
			ctl.Location = new Point(x, y);
		}

		public void Move(Control child, int x, int y)
		{
			IGtkControl ctl = ((IGtkControl)child.Handler);
			if (ctl.Location.X != x || ctl.Location.Y != y)
			{
				Control.Move((Gtk.Widget)child.ControlObject, x, y);
				
				ctl.Location = new Point(x, y);
			}
		}
		
		public void Remove(Control child)
		{
			Control.Remove((Gtk.Widget)child.ControlObject);
		}
		
	}
}
