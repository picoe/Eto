using Eto.Drawing;
using Eto.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using Eto.Platform.GtkSharp.Drawing;

namespace Eto.Platform.GtkSharp.Forms
{
	public class FontDialogHandler : WidgetHandler<Gtk.FontSelectionDialog, FontDialog>, IFontDialog
	{
		public override Gtk.FontSelectionDialog CreateControl ()
		{
			return new Gtk.FontSelectionDialog(null);
		}

		public Font Font
		{
			get; set;
		}

		public override void AttachEvent (string id)
		{
			switch (id) {
			case FontDialog.FontChangedEvent:
				// handled in ShowDialog
				break;
			default:
				base.AttachEvent (id);
				break;
			}
		}

		public DialogResult ShowDialog (Window parent)
		{
			if (parent != null) {
				Control.TransientFor = ((Gtk.Window)parent.ControlObject);
				Control.Modal = true;
			}
			if (Font != null) {
				var fontHandler = Font.Handler as FontHandler;
				Control.SetFontName (fontHandler.Control.ToString ());
			}
			else
				Control.SetFontName (string.Empty);

			Control.ShowAll ();
			var response = (Gtk.ResponseType)Control.Run ();
			Control.Hide ();

			if (response == Gtk.ResponseType.Apply || response == Gtk.ResponseType.Ok) {
				Font = new Font(Widget.Generator, new FontHandler(Control.FontName));
				Widget.OnFontChanged (EventArgs.Empty);
				return DialogResult.Ok;
			}
			else
				return DialogResult.Cancel;
		}
	}
}
