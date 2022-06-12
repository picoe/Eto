#if GTKCORE
using System;
using Eto.Drawing;
using Eto.Forms;

namespace Eto.GtkSharp.Forms
{
	public class ColorDialogHandler : WidgetHandler<Gtk.ColorChooserDialog, ColorDialog, ColorDialog.ICallback>, ColorDialog.IHandler
	{
		public ColorDialogHandler()
		{
			Control = new Gtk.ColorChooserDialog("Choose Color", null);
			AllowAlpha = false;
		}

		public bool AllowAlpha
		{
			get => Control.UseAlpha;
			set => Control.UseAlpha = value;
		}

		public Color Color
		{
			get => Control.Rgba.ToEto();
			set => Control.Rgba = value.ToRGBA();
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
			Control.Unrealize();

			if (response == Gtk.ResponseType.Ok)
				Callback.OnColorChanged(Widget, EventArgs.Empty);
			
			return response.ToEto();
		}
	}
}
#endif