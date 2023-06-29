namespace Eto.GtkSharp.Forms.Printing
{
	public class PrintPreviewDialogHandler : WidgetHandler<object, PrintPreviewDialog>, PrintPreviewDialog.IHandler
	{
		PrintSettings settings;

		public PrintDocument Document { get; set; }

		public DialogResult ShowDialog(Window parent)
		{
			var parentWindow = parent?.ControlObject as Gtk.Window;

			var printSettingsHandler = (PrintSettingsHandler)PrintSettings.Handler;

			Document.PrintSettings = PrintSettings;

			printSettingsHandler.ShowPreview = true;
			((PrintDocumentHandler)Document.Handler).Print(parentWindow);
			return DialogResult.Ok;
		}
		
		public PrintSettings PrintSettings
		{
			get => settings ?? (settings = new PrintSettings());
			set => settings = value;
		}

	}
}

