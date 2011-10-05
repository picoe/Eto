using System;
using Eto.Forms;
using Eto.Drawing;

namespace Eto.Platform.GtkSharp
{
	public class DockLayoutHandler : GtkLayout<Gtk.Alignment, DockLayout>, IDockLayout
	{
		Control content;
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
		
		public Control Content {
			get { return content; }
			set {
				if (content == value) return;
				if (Control.Child != null) {
					Control.Remove (Control.Child);
				}
				
				content = value;
				if (content != null) {
					var widget = (Gtk.Widget)content.ControlObject;
					Control.Child = widget;
					widget.ShowAll ();
				}
				
			}
		}
	}
}