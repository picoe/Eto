using Eto.Forms;
using Eto.Drawing;
using Eto.GtkSharp.Forms.Controls;

namespace Eto.GtkSharp.Forms
{
	public class PixelLayoutHandler : GtkContainer<Gtk.Fixed, PixelLayout, PixelLayout.ICallback>, PixelLayout.IHandler
	{
		public PixelLayoutHandler()
		{
			Control = new Gtk.Fixed();
		}

		public void Add(Control child, int x, int y)
		{
			var ctl = child.GetGtkControlHandler();

			var widget = ctl.ContainerControl;
			if (widget.Parent != null)
				((Gtk.Container)widget.Parent).Remove(widget);
			Control.Put(widget, x, y);
			ctl.CurrentLocation = new Point(x, y);
			widget.ShowAll();
		}

		public void Move(Control child, int x, int y)
		{
			var ctl = child.GetGtkControlHandler();
			if (ctl.CurrentLocation.X != x || ctl.CurrentLocation.Y != y)
			{
				Control.Move(ctl.ContainerControl, x, y);

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

		public override void OnLoadComplete(System.EventArgs e)
		{
			base.OnLoadComplete(e);
			SetFocusChain();
		}
	}
}
