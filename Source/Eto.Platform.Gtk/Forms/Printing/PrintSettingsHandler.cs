using System;
using Eto.Forms;

namespace Eto.Platform.GtkSharp.Forms.Printing
{
	public class PrintSettingsHandler : WidgetHandler<Gtk.PrintSettings, PrintSettings>, IPrintSettings
	{
		public PrintSettingsHandler ()
		{
			Control = new Gtk.PrintSettings();
		}

		public PrintSettingsHandler (Gtk.PrintSettings settings)
		{
			Control = settings;
		}
	}
}

