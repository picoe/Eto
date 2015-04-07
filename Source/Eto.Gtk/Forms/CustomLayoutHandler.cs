using Eto.Forms;
using Eto.Drawing;
using Eto.GtkSharp.Forms.Controls;

namespace Eto.GtkSharp.Forms
{
	public class CustomLayoutHandler : GtkContainer<Gtk.Fixed, CustomLayout, CustomLayout.ICallback>, CustomLayout.IHandler
	{
		public CustomLayoutHandler()
		{
			Control = new Gtk.Fixed();
		}

		public void Add(Control child)
		{
			var ctl = child.GetGtkControlHandler();

			var widget = ctl.ContainerControl;
			if (widget.Parent != null)
				((Gtk.Container)widget.Parent).Remove(widget);
			Control.Put(widget, 0, 0);
			//ctl.CurrentLocation = Point.Empty;
			widget.ShowAll();
		}

		public void Move(Control child, Rectangle location)
		{
			var ctl = child.GetGtkControlHandler();
			Control.Move(ctl.ContainerControl, location.X, location.Y);
			ctl.ContainerControl.SetSizeRequest(location.Width, location.Height);
			ctl.CurrentLocation = new Point(location.X, location.Y);
		}

		public void RemoveAll()
		{
			foreach (var ctl in Control.Children)
			{
				Control.Remove(ctl);
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
