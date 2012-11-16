using System;
using Eto.Forms;

namespace Eto.Platform.GtkSharp.Forms.Printing
{
	public class PrintDialogHandler : WidgetHandler<Gtk.PrintUnixDialog, PrintDialog>, IPrintDialog
	{
		PrintSettings settings;

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

			var caps = Gtk.PrintCapabilities.Preview
					| Gtk.PrintCapabilities.Collate
					| Gtk.PrintCapabilities.GeneratePdf
					| Gtk.PrintCapabilities.Copies
					| Gtk.PrintCapabilities.GeneratePs
					| Gtk.PrintCapabilities.NumberUp
					| Gtk.PrintCapabilities.Scale
					| Gtk.PrintCapabilities.Reverse;
			if (AllowPageRange)
				caps |= Gtk.PrintCapabilities.PageSet;
			Control.PageSetup = this.PrintSettings.ToGtkPageSetup ();
			Control.PrintSettings = this.PrintSettings.ToGtkPrintSettings ();

			Control.ManualCapabilities = caps;
			Control.ShowAll ();
			var response = (Gtk.ResponseType)Control.Run ();
			Control.Hide ();

			var printSettingsHandler = (PrintSettingsHandler)this.PrintSettings.Handler;
			printSettingsHandler.Set(Control.PrintSettings, Control.PageSetup);
			if (response == Gtk.ResponseType.Apply) {
				printSettingsHandler.ShowPreview = true;
				return DialogResult.Ok;
			}

			return response.ToEto ();
		}
		public PrintSettings PrintSettings {
			get {
				if (settings == null) settings = new PrintSettings(Widget.Generator);
				return settings;
			}
			set {
				settings = value;
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

