using System;
using Eto.Forms;

namespace Eto.GtkSharp.Forms
{
    public class ColorDialogHandler : WidgetHandler<Gtk.Dialog, ColorDialog, ColorDialog.ICallback>, ColorDialog.IHandler
	{
		public ColorDialogHandler ()
		{
#if GTK3
            if (Gtk.Global.MinorVersion >= 4)
                Control = new Gtk.Dialog(NativeMethods.gtk_color_chooser_dialog_new("Choose Color", IntPtr.Zero));
            else
#endif
                Control = new Gtk.ColorSelectionDialog(string.Empty);
			
		}

		public Eto.Drawing.Color Color {
			get
            {
#if GTK3
                if (Gtk.Global.MinorVersion >= 4)
                    return NativeMethods.gtk_color_chooser_get_rgba(Control.Handle).ToEto();
                else
#endif 
                    return (Control as Gtk.ColorSelectionDialog).ColorSelection.CurrentColor.ToEto ();
            }
			set
            {
#if GTK3
                if (Gtk.Global.MinorVersion >= 4)
                    NativeMethods.gtk_color_chooser_set_rgba(Control.Handle, new double[] { value.R, value.G, value.B, value.A });
                else
#endif
                    (Control as Gtk.ColorSelectionDialog).ColorSelection.CurrentColor = value.ToGdk ();
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

