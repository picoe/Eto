#if !GTKCORE
using System;
using Eto.Drawing;
using Eto.Forms;

namespace Eto.GtkSharp.Forms
{
	public class ColorDialogHandler : WidgetHandler<Gtk.ColorSelectionDialog, ColorDialog, ColorDialog.ICallback>, ColorDialog.IHandler
	{
		public ColorDialogHandler()
		{
			Control = new Gtk.ColorSelectionDialog(string.Empty);
		}

		public bool AllowAlpha
		{
			get { return Control.ColorSelection.HasOpacityControl; }
			set { Control.ColorSelection.HasOpacityControl = value; }
		}

		public Color Color
		{
			get
			{
				return Control.ColorSelection.CurrentColor.ToEto(Control.ColorSelection.CurrentAlpha);
			}
			set
			{
				Control.ColorSelection.CurrentColor = value.ToGdk();
				Control.ColorSelection.CurrentAlpha = (ushort)(value.A * ushort.MaxValue);
			}
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

#endif