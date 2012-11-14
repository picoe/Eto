using System;
using Eto.Forms;

namespace Eto.Platform.GtkSharp.Forms.Printing
{
	public class PrintDialogHandler : WidgetHandler<Gtk.PrintUnixDialog, PrintDialog>, IPrintDialog
	{
		public PrintDialogHandler ()
		{

		}

		public DialogResult ShowDialog (Window parent)
		{
			var parentWindow = parent != null ? (Gtk.Window)parent.ControlObject : null;
			Control = new Gtk.PrintUnixDialog(string.Empty, parentWindow);

			if (parent != null)
			{
				Control.TransientFor = ((Gtk.Window)parent.ControlObject);
				Control.Modal = true;
			}

			var caps = Gtk.PrintCapabilities.Preview | Gtk.PrintCapabilities.Collate | Gtk.PrintCapabilities.GeneratePdf | Gtk.PrintCapabilities.Copies;
			if (AllowPageRange)
				caps |= Gtk.PrintCapabilities.PageSet;

			Control.ManualCapabilities = caps;
			Control.ShowAll ();
			var response = (Gtk.ResponseType)Control.Run ();
			Control.HideAll ();
			
			return response.ToEto ();
		}
		public PrintSettings PrintSettings {
			get {
				throw new NotImplementedException ();
			}
			set {
				throw new NotImplementedException ();
			}
		}

		public bool AllowPageRange {
			get; set;
		}

		public bool AllowSelection {
			get; set;
		}
	}
}

