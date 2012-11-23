using System;
using Eto.Forms;

namespace Eto.Platform.GtkSharp
{
	public class MessageBoxHandler : WidgetHandler<Widget>, IMessageBox
	{
		Gtk.MessageDialog control;
		
		public string Text { get; set; }

		public string Caption { get; set; }

		public MessageBoxType Type { get; set; }
		
		public DialogResult ShowDialog (Control parent)
		{
			Gtk.Widget c = (parent == null) ? null : (Gtk.Widget)parent.ControlObject;
			while (!(c is Gtk.Window) && c != null) {
				c = c.Parent;
			}
			control = new Gtk.MessageDialog ((Gtk.Window)c, Gtk.DialogFlags.Modal, Type.ToGtk (), Gtk.ButtonsType.Ok, false, Text);
			control.TypeHint = Gdk.WindowTypeHint.Dialog;
			var caption = Caption ?? ((parent != null && parent.ParentWindow != null) ? parent.ParentWindow.Title : null);
			if (!string.IsNullOrEmpty (caption))
				control.Title = caption;
			int ret = control.Run ();
			control.Destroy ();
			return ((Gtk.ResponseType)ret).ToEto ();
		}

		public DialogResult ShowDialog (Control parent, MessageBoxButtons buttons)
		{
			Gtk.Widget c = (parent == null) ? null : (Gtk.Widget)parent.ControlObject;
			while (!(c is Gtk.Window) && c != null) {
				c = c.Parent;
			}
			control = new Gtk.MessageDialog ((Gtk.Window)c, Gtk.DialogFlags.Modal, Type.ToGtk (), buttons.ToGtk (), false, Text);
			control.TypeHint = Gdk.WindowTypeHint.Dialog;
			var caption = Caption ?? ((parent != null && parent.ParentWindow != null) ? parent.ParentWindow.Title : null);
			if (!string.IsNullOrEmpty (caption))
				control.Title = caption;
			if (buttons == MessageBoxButtons.YesNoCancel) {
				// must add cancel manually
				Gtk.Button b = (Gtk.Button)control.AddButton (Gtk.Stock.Cancel, (int)Gtk.ResponseType.Cancel);
				b.UseStock = true;
			}
			int ret = control.Run ();
			control.Destroy ();
			return ((Gtk.ResponseType)ret).ToEto ();
		}
	}

}
