using Eto.GtkSharp.Forms.Controls;
using Gtk;
namespace Eto.GtkSharp.Forms
{

	public class PixelLayoutHandler : GtkContainer<Gtk.Fixed, PixelLayout, PixelLayout.ICallback>, PixelLayout.IHandler
	{
		public PixelLayoutHandler()
		{
			Control = new EtoFixed { Handler = this };
		}

#if GTK3
		class EtoVBox : Gtk.VBox
		{
			protected override void OnAdjustSizeRequest(Gtk.Orientation orientation, out int minimum_size, out int natural_size)
			{
				base.OnAdjustSizeRequest(orientation, out minimum_size, out natural_size);
				// Gtk.Fixed only uses minimum size, not natural size. ugh.
				minimum_size = natural_size;
			}
		}
#endif

		public void Add(Control child, int x, int y)
		{
			var ctl = child.GetGtkControlHandler();

#if GTK3
			var widget = ctl.ContainerControl;
			if (widget.Parent != null)
				((Gtk.Container)widget.Parent).Remove(widget);
			widget.ShowAll();
			widget = new EtoVBox { Child = widget };
#else
			var widget = ctl.ContainerControl;
			if (widget.Parent != null)
				((Gtk.Container)widget.Parent).Remove(widget);
			widget.ShowAll();
#endif
			Control.Put(widget, x, y);
			ctl.CurrentLocation = new Point(x, y);
			InvalidateMeasure();
		}
		public override void InvalidateMeasure()
		{
			base.InvalidateMeasure();
#if GTKCORE
			if (Widget.Loaded)
			{
				// same as Control.ResizeChildren(), but non-obsolete.
				Control.GetAllocatedSize(out var allocation, out var baseline);
				Control.SizeAllocateWithBaseline(allocation, baseline);
			}
#endif
		}

		public void Move(Control child, int x, int y)
		{
			var ctl = child.GetGtkControlHandler();
			if (ctl.CurrentLocation.X != x || ctl.CurrentLocation.Y != y)
			{
#if GTK3
				var widget = ctl.ContainerControl.Parent;
#else
				var widget = ctl.ContainerControl;
#endif
				Control.Move(widget, x, y);

				ctl.CurrentLocation = new Point(x, y);
			}
			InvalidateMeasure();
		}

		public void Remove(Control child)
		{
#if GTK3
			Control.Remove(child.GetContainerWidget().Parent);
#else
			Control.Remove(child.GetContainerWidget());
#endif
			InvalidateMeasure();
		}

		public void Update()
		{
#if GTK3
			Control.QueueResize();
#else
			Control.ResizeChildren();
#endif
		}

		public override void OnLoadComplete(System.EventArgs e)
		{
			base.OnLoadComplete(e);
			SetFocusChain();
		}
	}
}
