using System;
using Eto.Forms;
using Eto.Drawing;

namespace Eto.Platform.GtkSharp
{
	public class DockLayoutHandler : GtkLayout<Gtk.Alignment, DockLayout>, IDockLayout
	{
		
		public DockLayoutHandler()
		{
			Control = new Gtk.Alignment(0, 0, 1, 1);
			this.Padding = DockLayout.DefaultPadding;
		}
		
		public Eto.Drawing.Padding Padding {
			get {
				uint top, left, right, bottom;
				Control.GetPadding(out top, out bottom, out left, out right);
				return new Eto.Drawing.Padding((int)left, (int)top, (int)right, (int)bottom);
			}
			set {
				Control.SetPadding((uint)value.Top, (uint)value.Bottom, (uint)value.Left, (uint)value.Right);
			}
		}

		public void Add(Control child)
		{
			if (Control.Child != null) Control.Remove(Control.Child);
			var widget = (Gtk.Widget)child.ControlObject;
			Control.Add(widget);
			widget.ShowAll ();
		}

		public void Remove(Control child)
		{
			Control.Remove((Gtk.Widget)child.ControlObject);
		}

	}
}
