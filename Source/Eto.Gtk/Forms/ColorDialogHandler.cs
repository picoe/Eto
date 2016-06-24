using System;
using Eto.Forms;

namespace Eto.GtkSharp.Forms
{
    public class ColorDialogHandler : WidgetHandler<Gtk.Dialog, ColorDialog, ColorDialog.ICallback>, ColorDialog.IHandler
	{
		public ColorDialogHandler ()
		{
#if GTK3
			Control = new Gtk.ColorChooserDialog("Choose Color", null);
#else
            Control = new Gtk.ColorSelectionDialog(string.Empty);
#endif

		}

		public Eto.Drawing.Color Color {
			get
            {
#if GTK3
				return (Control as Gtk.ColorChooserDialog).Rgba.ToEto();
#else
				return (Control as Gtk.ColorSelectionDialog).ColorSelection.CurrentColor.ToEto ();
#endif
			}
			set
            {
#if GTK3
				(Control as Gtk.ColorChooserDialog).Rgba = value.ToRGBA();
#else
                (Control as Gtk.ColorSelectionDialog).ColorSelection.CurrentColor = value.ToGdk ();
#endif
			}
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

