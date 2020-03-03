using System;
using Eto.Forms;

namespace Eto.GtkSharp.Forms
{
	public class MessageBoxHandler : WidgetHandler<Widget>, MessageBox.IHandler
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

			control = new Gtk.MessageDialog(parentWindow, Gtk.DialogFlags.Modal, Type.ToGtk(), Buttons.ToGtk(), false, string.Empty);
			control.Text = Text;
			control.TypeHint = Gdk.WindowTypeHint.Dialog;
			var caption = Caption ?? ((parent != null && parent.ParentWindow != null) ? parent.ParentWindow.Title : null);
			if (!string.IsNullOrEmpty(caption))
				control.Title = caption;
			// must add buttons manually for this case
			if (Buttons == MessageBoxButtons.YesNoCancel)
			{
				var bn = (Gtk.Button)control.AddButton(Gtk.Stock.No, (int)Gtk.ResponseType.No);
				bn.UseStock = true;
				var bc = (Gtk.Button)control.AddButton(Gtk.Stock.Cancel, (int)Gtk.ResponseType.Cancel);
				bc.UseStock = true;
				var by = (Gtk.Button)control.AddButton(Gtk.Stock.Yes, (int)Gtk.ResponseType.Yes);
				by.UseStock = true;
			}
			control.DefaultResponse = DefaultButton.ToGtk(Buttons);
			int ret = control.Run();
			control.Hide();
#if GTKCORE
			control.Dispose();
#else
			control.Destroy();
#endif
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
