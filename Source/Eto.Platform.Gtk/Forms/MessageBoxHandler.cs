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

		public MessageBoxButtons Buttons { get; set; }

		public MessageBoxDefaultButton DefaultButton { get; set; }

		public DialogResult ShowDialog(Control parent)
		{
			Gtk.Window parentWindow = null;
			if (parent != null && parent.ParentWindow != null)
				parentWindow = parent.ParentWindow.ControlObject as Gtk.Window;

			control = new Gtk.MessageDialog(parentWindow, Gtk.DialogFlags.Modal, Type.ToGtk(), Buttons.ToGtk(), false, Text);
			control.TypeHint = Gdk.WindowTypeHint.Dialog;
			var caption = Caption ?? ((parent != null && parent.ParentWindow != null) ? parent.ParentWindow.Title : null);
			if (!string.IsNullOrEmpty(caption))
				control.Title = caption;
			if (Buttons == MessageBoxButtons.YesNoCancel)
			{
				// must add cancel manually
				Gtk.Button b = (Gtk.Button)control.AddButton(Gtk.Stock.Cancel, (int)Gtk.ResponseType.Cancel);
				b.UseStock = true;
			}
			control.DefaultResponse = DefaultButton.ToGtk(Buttons);
			int ret = control.Run();
			control.Destroy();
			var result = ((Gtk.ResponseType)ret).ToEto();
			if (result == DialogResult.None)
			{
				switch (Buttons)
				{
					case MessageBoxButtons.OK:
						result = DialogResult.Ok;
						break;
					case MessageBoxButtons.YesNo:
						result = DialogResult.No;
						break;
					case MessageBoxButtons.OKCancel:
					case MessageBoxButtons.YesNoCancel:
						result = DialogResult.Cancel;
						break;
				}
			}
			return result;
		}
	}

}
