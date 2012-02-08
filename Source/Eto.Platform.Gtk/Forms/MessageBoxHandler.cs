using System;
using Eto.Forms;

namespace Eto.Platform.GtkSharp
{
	public class MessageBoxHandler : IMessageBox
	{
		Gtk.MessageDialog control;
		
		public string Text { get; set; }
		public string Caption { get; set; }
		public IWidget Handler { get; set; }
		public MessageBoxType Type { get; set; }
		
		public void Initialize()
		{
		}
		

		public DialogResult ShowDialog(Control parent)
		{
			Gtk.Widget c = (parent == null) ? null : (Gtk.Widget)parent.ControlObject;
			while (!(c is Gtk.Window) && c != null)
			{
				c = c.Parent;
			}
			control = new Gtk.MessageDialog((Gtk.Window)c, Gtk.DialogFlags.Modal, Convert (Type), Gtk.ButtonsType.Ok, false, Text);
			control.TypeHint = Gdk.WindowTypeHint.Dialog;
            var caption = Caption ?? ((parent != null && parent.ParentWindow != null) ? parent.ParentWindow.Text : null);
            if (!string.IsNullOrEmpty(caption)) control.Title = caption;
			int ret = control.Run();
			control.Destroy();
			return Generator.Convert((Gtk.ResponseType)ret);
		}

		public DialogResult ShowDialog(Control parent, MessageBoxButtons buttons)
		{
			Gtk.Widget c = (parent == null) ? null : (Gtk.Widget)parent.ControlObject;
			while (!(c is Gtk.Window) && c != null)
			{
				c = c.Parent;
			}
			control = new Gtk.MessageDialog((Gtk.Window)c, Gtk.DialogFlags.Modal, Convert (Type), Convert(buttons), false, Text);
			control.TypeHint = Gdk.WindowTypeHint.Dialog;
            var caption = Caption ?? ((parent != null && parent.ParentWindow != null) ? parent.ParentWindow.Text : null);
            if (!string.IsNullOrEmpty(caption)) control.Title = caption;
			if (buttons == MessageBoxButtons.YesNoCancel)
			{
				// must add cancel manually
				Gtk.Button b = (Gtk.Button)control.AddButton(Gtk.Stock.Cancel, (int)Gtk.ResponseType.Cancel);
				b.UseStock = true;
			}
			int ret = control.Run();
			control.Destroy();
			return Generator.Convert((Gtk.ResponseType)ret);
		}
		
		public static Gtk.ButtonsType Convert(MessageBoxButtons buttons)
		{
			switch (buttons)
			{
				default:
				case MessageBoxButtons.OK: return Gtk.ButtonsType.Ok;
				case MessageBoxButtons.OKCancel: return Gtk.ButtonsType.OkCancel;
				case MessageBoxButtons.YesNo: return Gtk.ButtonsType.YesNo;
				case MessageBoxButtons.YesNoCancel: return Gtk.ButtonsType.YesNo;
			}
		}
		
		public static Gtk.MessageType Convert(MessageBoxType type)
		{
			switch (type)
			{
				default:
				case MessageBoxType.Information: return Gtk.MessageType.Info;
				case MessageBoxType.Error: return Gtk.MessageType.Error;
				case MessageBoxType.Warning: return Gtk.MessageType.Warning;
				case MessageBoxType.Question: return Gtk.MessageType.Question;
			}
		}

	}

}
