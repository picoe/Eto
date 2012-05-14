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
			var gtkcontrol = (Gtk.Widget)child.ControlObject;
			Control.Put(gtkcontrol, x, y);
			ctl.Location = new Point(x, y);
			gtkcontrol.ShowAll();
		}

		public void Move(Control child, int x, int y)
		{
			IGtkControl ctl = ((IGtkControl)child.Handler);
			if (ctl.Location.X != x || ctl.Location.Y != y)
			{
				Control.Move (child.GetContainerWidget (), x, y);
				
				ctl.Location = new Point(x, y);
			}
		}
		
		public void Remove(Control child)
		{
			Control.Remove (child.GetContainerWidget ());
		}
		
	}
}
