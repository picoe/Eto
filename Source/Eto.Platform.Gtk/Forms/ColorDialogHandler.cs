using System;
using Eto.Forms;

namespace Eto.Platform.GtkSharp.Forms
{
	public class ColorDialogHandler : WidgetHandler<Gtk.ColorSelectionDialog, ColorDialog>, IColorDialog
	{
		public ColorDialogHandler ()
		{
			Control = new Gtk.ColorSelectionDialog(string.Empty);
			
		}

		#region IColorDialog implementation
		public Eto.Drawing.Color Color {
			get {
				return Generator.Convert (Control.ColorSelection.CurrentColor);
			}
			set {
				Control.ColorSelection.CurrentColor = Generator.Convert (value);
			}
		}
		#endregion

		#region ICommonDialog implementation
		public DialogResult ShowDialog (Window parent)
		{
			if (parent != null)
			{
				Control.TransientFor = ((Gtk.Window)parent.ControlObject);
				Control.Modal = true;
			}

			Control.ShowAll ();
			var response = (Gtk.ResponseType)Control.Run ();
			Control.HideAll ();
			
			if (response == Gtk.ResponseType.Ok) {
				Widget.OnColorChanged(EventArgs.Empty);
			}
			return Generator.Convert (response);
		}
		#endregion

	}
}

