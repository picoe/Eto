using System;
using Eto.Forms;
using Eto.Drawing;

namespace Eto.Platform.GtkSharp
{
	public class PixelLayoutHandler : GtkContainer<Gtk.Fixed, PixelLayout>, IPixelLayout
	{
		public PixelLayoutHandler()
		{
			Control = new Gtk.Fixed();
		}

		public void Add(Control child, int x, int y)
		{
			var ctl = ((IGtkControl)child.Handler);

			var gtkcontrol = child.GetContainerWidget();
			Control.Put(gtkcontrol, x, y);
			ctl.CurrentLocation = new Point(x, y);
			if (this.Control.Visible)
				gtkcontrol.ShowAll();
		}

		public void Move(Control child, int x, int y)
		{
			var ctl = ((IGtkControl)child.Handler);
			if (ctl.CurrentLocation.X != x || ctl.CurrentLocation.Y != y)
			{
				Control.Move(child.GetContainerWidget(), x, y);
				
				ctl.CurrentLocation = new Point(x, y);
			}
		}

		public void Remove(Control child)
		{
			Control.Remove(child.GetContainerWidget());
		}

		public void Update()
		{
			Control.ResizeChildren();
		}
	}
}
