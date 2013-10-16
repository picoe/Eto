using System;
using Eto.Forms;

namespace Eto.Platform.GtkSharp.Forms.Printing
{
	public class PrintDialogHandler : WidgetHandler<Gtk.PrintUnixDialog, PrintDialog>, IPrintDialog
	{
		PrintSettings settings;

		public PrintDialogHandler ()
		{
			AllowPageRange =true;
		}

		public PrintDocument Document { get; set; }

		public class CustomOptions : Gtk.VBox {
			public Gtk.CheckButton SelectionOnly { get; private set; }

			public CustomOptions ()
			{
				this.Spacing = 10;
				SelectionOnly = new Gtk.CheckButton { Label = "Selection Only" };
				this.PackStart (SelectionOnly, false, false, 10);
			}
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
					| Gtk.PrintCapabilities.PageSet
					| Gtk.PrintCapabilities.GeneratePs
					| Gtk.PrintCapabilities.Scale
					| Gtk.PrintCapabilities.NumberUp
					| Gtk.PrintCapabilities.Reverse;
			var printSettingsHandler = (PrintSettingsHandler)this.PrintSettings.Handler;

			Control.PageSetup = this.PrintSettings.ToGtkPageSetup ();
			Control.PrintSettings = this.PrintSettings.ToGtkPrintSettings ();
			var customOptions = new CustomOptions();
			customOptions.SelectionOnly.Active = printSettingsHandler.SelectionOnly;

			if (AllowSelection)
				Control.AddCustomTab (customOptions, new Gtk.Label { Text = "Other Options" });

			Control.ManualCapabilities = caps;
			Control.ShowAll ();
			var response = (Gtk.ResponseType)Control.Run ();
			Control.Hide ();

			printSettingsHandler.Set(Control.PrintSettings, Control.PageSetup, customOptions.SelectionOnly.Active);
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

		// not supported in gtk
		public bool AllowPageRange {
			get; set;
		}

		public bool AllowSelection {
			get; set;
		}
	}
}

