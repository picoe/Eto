#if GTKCORE
using System;
using System.Text;
using Eto.Drawing;
using Eto.Forms;
using Eto.GtkSharp.Drawing;

namespace Eto.GtkSharp.Forms
{
	public class FontDialogHandler : WidgetHandler<Gtk.FontChooserDialog, FontDialog, FontDialog.ICallback>, FontDialog.IHandler
	{
		public FontDialogHandler()
		{
			Control = new Gtk.FontChooserDialog("Choose Font", null); 
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
				Control.Font = handler.Control.ToString();
			}
			else
				Control.Font = string.Empty;

			Control.ShowAll();
			var response = (Gtk.ResponseType)Control.Run();
			Control.Hide();
			Control.Unrealize();

			if (response == Gtk.ResponseType.Ok)
			{
				Font = new Font(new FontHandler(Control.Font));
				Callback.OnFontChanged(Widget, EventArgs.Empty);
			}

			return response.ToEto();
		}
	}
}
#endif