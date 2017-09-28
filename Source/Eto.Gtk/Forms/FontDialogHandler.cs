using System;
using System.Text;
using Eto.Drawing;
using Eto.Forms;
using Eto.GtkSharp.Drawing;

namespace Eto.GtkSharp.Forms
{
	public class FontDialogHandler : WidgetHandler<Gtk.FontSelectionDialog, FontDialog, FontDialog.ICallback>, FontDialog.IHandler
	{
		public FontDialogHandler()
		{
			Control = new Gtk.FontSelectionDialog(null);
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

			#if GTK3
			Pango.FontDescription OrigFontDesc = Control.GetFont();
			if (Font != null) {
				var handler = Font.Handler as FontHandler;
				OrigFontDesc.Family  = handler.Control.ToString ();
				Control.SetFont(OrigFontDesc);
			} else {
				OrigFontDesc.Family  = string.Empty;
				Control.SetFont(OrigFontDesc);
			}
			#else
			if (Font != null)
			{
				var handler = Font.Handler as FontHandler;				
				Control.SetFontName(handler.Control.ToString());				
			}
			else
				Control.SetFontName(string.Empty);
			#endif

			Control.ShowAll();
			var response = (Gtk.ResponseType)Control.Run();
			Control.Hide();

			if (response == Gtk.ResponseType.Apply || response == Gtk.ResponseType.Ok)
			{
				#if GTK3
				Pango.FontDescription FDesc = Control.GetFont();
				Font = new Font(new FontHandler(FDesc.Family));
				#else
				Font = new Font(new FontHandler(Control.FontName));
				#endif
				Callback.OnFontChanged(Widget, EventArgs.Empty);
				return DialogResult.Ok;
			}
			return DialogResult.Cancel;
		}
	}
}
