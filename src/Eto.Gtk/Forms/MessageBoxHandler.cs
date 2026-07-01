namespace Eto.GtkSharp.Forms
{
	public class MessageBoxHandler : WidgetHandler<Widget>, MessageBox.IHandler, MessageBox.ICancellableHandler
	{
		Gtk.MessageDialog control;

		public string Text { get; set; }

		public string Caption { get; set; }

		public MessageBoxType Type { get; set; }

		public MessageBoxButtons Buttons { get; set; }

		public MessageBoxDefaultButton DefaultButton { get; set; }

		public DialogResult ShowDialog(Control parent)
		{
			control = CreateDialog(parent);
			int ret = control.Run();
			var result = ToResult((Gtk.ResponseType)ret);
			CleanupDialog();
			return result;
		}

		public void CancelDialog() => control?.Respond((int)Gtk.ResponseType.None);

		Gtk.MessageDialog CreateDialog(Control parent)
		{
			Gtk.Window parentWindow = null;
			if (parent != null && parent.ParentWindow != null)
				parentWindow = parent.ParentWindow.ControlObject as Gtk.Window;

			var dialog = new Gtk.MessageDialog(parentWindow, Gtk.DialogFlags.Modal, Type.ToGtk(), Buttons.ToGtk(), false, string.Empty)
			{
				Text = Text,
				TypeHint = Gdk.WindowTypeHint.Dialog
			};

			var caption = Caption ?? ((parent != null && parent.ParentWindow != null) ? parent.ParentWindow.Title : null);
			if (!string.IsNullOrEmpty(caption))
				dialog.Title = caption;
			// must add buttons manually for this case
			if (Buttons == MessageBoxButtons.YesNoCancel)
			{
				var bn = (Gtk.Button)dialog.AddButton(Gtk.Stock.No, (int)Gtk.ResponseType.No);
				bn.UseStock = true;
				var bc = (Gtk.Button)dialog.AddButton(Gtk.Stock.Cancel, (int)Gtk.ResponseType.Cancel);
				bc.UseStock = true;
				var by = (Gtk.Button)dialog.AddButton(Gtk.Stock.Yes, (int)Gtk.ResponseType.Yes);
				by.UseStock = true;
			}
			dialog.DefaultResponse = DefaultButton.ToGtk(Buttons);
			return dialog;
		}

		DialogResult ToResult(Gtk.ResponseType response)
		{
			var result = response.ToEto();
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

		void CleanupDialog()
		{
			control?.Hide();
#if GTKCORE
			control?.Dispose();
#else
			control?.Destroy();
#endif
			control = null;
		}
	}

}
