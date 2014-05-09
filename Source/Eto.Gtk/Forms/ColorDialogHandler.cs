using System;
using Eto.Forms;

namespace Eto.GtkSharp.Forms
{
	public class ColorDialogHandler : WidgetHandler<Gtk.ColorSelectionDialog, ColorDialog, ColorDialog.ICallback>, ColorDialog.IHandler
	{
		public ColorDialogHandler ()
		{
			Control = new Gtk.ColorSelectionDialog(string.Empty);
			
		}

		public Eto.Drawing.Color Color {
			get { return Control.ColorSelection.CurrentColor.ToEto (); }
			set { Control.ColorSelection.CurrentColor = value.ToGdk (); }
		}

		public DialogResult ShowDialog (Window parent)
		{
			if (parent != null)
			{
				Control.TransientFor = ((Gtk.Window)parent.ControlObject);
				Control.Modal = true;
			}

			Control.ShowAll ();
			var response = (Gtk.ResponseType)Control.Run ();
			Control.Hide ();
			
			if (response == Gtk.ResponseType.Ok) {
				Callback.OnColorChanged(Widget, EventArgs.Empty);
			}
			return response.ToEto ();
		}
	}
}

