using System;
using Eto.Drawing;
using Eto.Forms;

namespace Eto.GtkSharp.Forms
{
	public class ColorDialogHandlerGtk34 : WidgetHandler<Gtk.Dialog, ColorDialog, ColorDialog.ICallback>, ColorDialog.IHandler
	{
		public ColorDialogHandlerGtk34()
		{
			Control = new Gtk.Dialog(NativeMethods.gtk_color_chooser_dialog_new("Choose Color", IntPtr.Zero));
			AllowAlpha = false;
		}

		public bool AllowAlpha
		{
			get { return NativeMethods.gtk_color_chooser_get_use_alpha(Control.Handle); }
			set{  NativeMethods.gtk_color_chooser_set_use_alpha(Control.Handle, value); }
		}

		public Color Color
		{
			get
			{
				Gdk.RGBA rgba;
				NativeMethods.gtk_color_chooser_get_rgba(Control.Handle, out rgba);

				return rgba.ToEto();
			}
			set { NativeMethods.gtk_color_chooser_set_rgba(Control.Handle, new double[] { value.R, value.G, value.B, value.A }); }
		}

		public bool SupportsAllowAlpha => true;

		public DialogResult ShowDialog(Window parent)
		{
			if (parent != null)
			{
				Control.TransientFor = ((Gtk.Window)parent.ControlObject);
				Control.Modal = true;
			}

			Control.ShowAll();
			var response = (Gtk.ResponseType)Control.Run();
			Control.Hide();

			if (response == Gtk.ResponseType.Ok)
				Callback.OnColorChanged(Widget, EventArgs.Empty);
			
			return response.ToEto();
		}
	}
}
