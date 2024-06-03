namespace Eto.GtkSharp.Forms.Printing
{
	public class PrintDialogHandler : WidgetHandler<Gtk.PrintUnixDialog, PrintDialog>, PrintDialog.IHandler
	{
		PrintSettings settings;

		public PrintDocument Document { get; set; }

		public class CustomOptions : Gtk.Box
		{
			public Gtk.CheckButton SelectionOnly { get; private set; }

			public CustomOptions() : base(Gtk.Orientation.Vertical, 10)
			{
				SelectionOnly = new Gtk.CheckButton { Label = "Selection Only" };
				this.PackStart(SelectionOnly, false, false, 10);
			}
		}

		public DialogResult ShowDialog(Window parent)
		{
			var parentWindow = parent?.ControlObject as Gtk.Window;
			Control = new Gtk.PrintUnixDialog(string.Empty, parentWindow);

			if (parentWindow != null)
			{
				Control.TransientFor = parentWindow;
				Control.Modal = true;
			}


			const Gtk.PrintCapabilities caps = Gtk.PrintCapabilities.Preview
					| Gtk.PrintCapabilities.Collate
					| Gtk.PrintCapabilities.GeneratePdf
					| Gtk.PrintCapabilities.Copies
					| Gtk.PrintCapabilities.PageSet
					| Gtk.PrintCapabilities.GeneratePs
					| Gtk.PrintCapabilities.Scale
					| Gtk.PrintCapabilities.NumberUp
					| Gtk.PrintCapabilities.Reverse;
			var printSettingsHandler = (PrintSettingsHandler)PrintSettings.Handler;

			Control.PageSetup = PrintSettings.ToGtkPageSetup();
			Control.PrintSettings = PrintSettings.ToGtkPrintSettings();
			var customOptions = new CustomOptions();
			customOptions.SelectionOnly.Active = printSettingsHandler.SelectionOnly;

			if (AllowSelection)
				Control.AddCustomTab(customOptions, new Gtk.Label { Text = "Other Options" });


			NativeMethods.gtk_print_unix_dialog_set_embed_page_setup(Control.Handle, true);

			Control.ManualCapabilities = caps;
			Control.ShowAll();
			var response = (Gtk.ResponseType)Control.Run();
			Control.Hide();
			Control.Unrealize();

			printSettingsHandler.Set(Control.PrintSettings, Control.PageSetup, customOptions.SelectionOnly.Active);
			if (Document != null)
				Document.PrintSettings = PrintSettings;

			if (response == Gtk.ResponseType.Apply)
			{
				printSettingsHandler.ShowPreview = true;
				(Document?.Handler as PrintDocumentHandler)?.Print(parentWindow);
				return DialogResult.Ok;
			}
			if (response == Gtk.ResponseType.Ok)
			{
				(Document?.Handler as PrintDocumentHandler)?.Print(parentWindow);
			}

			return response.ToEto();
		}
		
		public PrintSettings PrintSettings
		{
			get => settings ?? (settings = new PrintSettings());
			set => settings = value;
		}

		// not supported in gtk
		public bool AllowPageRange { get; set; } = true;

		public bool AllowSelection { get; set; }
	}
}

