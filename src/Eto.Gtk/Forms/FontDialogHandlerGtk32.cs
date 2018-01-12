using System;
using System.Text;
using Eto.Drawing;
using Eto.Forms;
using Eto.GtkSharp.Drawing;

namespace Eto.GtkSharp.Forms
{
	public class FontDialogHandlerGtk32 : WidgetHandler<Gtk.Dialog, FontDialog, FontDialog.ICallback>, FontDialog.IHandler
	{
		public FontDialogHandlerGtk32()
		{
			Control = new Gtk.Dialog(NativeMethods.gtk_font_chooser_dialog_new("Choose Font", IntPtr.Zero));
		}

		public Font Font { get; set; }

		public override void AttachEvent(string id)
		{
			switch (id)
			{
				case FontDialog.FontChangedEvent:
					// handled in ShowDialog
					break;
				default:
					base.AttachEvent(id);
					break;
			}
		}

		public DialogResult ShowDialog(Window parent)
		{
			if (parent != null)
			{
				Control.TransientFor = ((Gtk.Window)parent.ControlObject);
				Control.Modal = true;
			}

			if (Font != null)
			{
				var handler = Font.Handler as FontHandler;
				NativeMethods.gtk_font_chooser_set_font(Control.Handle, handler.Control.ToString());
			}
			else
				NativeMethods.gtk_font_chooser_set_font(Control.Handle, string.Empty);

			Control.ShowAll();
			var response = (Gtk.ResponseType)Control.Run();
			Control.Hide();

			if (response == Gtk.ResponseType.Ok)
			{
				Font = new Font(new FontHandler(NativeMethods.gtk_font_chooser_get_font(Control.Handle)));
				Callback.OnFontChanged(Widget, EventArgs.Empty);
			}

			return response.ToEto();
		}
	}
}
