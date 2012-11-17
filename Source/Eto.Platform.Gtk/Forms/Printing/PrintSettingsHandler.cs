using System;
using Eto.Forms;

namespace Eto.Platform.GtkSharp.Forms.Printing
{
	public static class PrintSettingsExtensions
	{
		public static PrintSettings ToEto (this Gtk.PrintSettings settings, Gtk.PageSetup setup, Eto.Generator generator)
		{
			if (settings == null)
				return null;
			return new PrintSettings(generator, new PrintSettingsHandler (settings, setup));
		}

		public static Gtk.PrintSettings ToGtkPrintSettings (this PrintSettings settings)
		{
			if (settings == null)
				return null;
			return ((PrintSettingsHandler)settings.Handler).Control;
		}

		public static Gtk.PageSetup ToGtkPageSetup (this PrintSettings settings)
		{
			if (settings == null)
				return null;
			return ((PrintSettingsHandler)settings.Handler).PageSetup;
		}
	}

	public class PrintSettingsHandler : WidgetHandler<Gtk.PrintSettings, PrintSettings>, IPrintSettings
	{
		public Gtk.PageSetup PageSetup { get; set; }

		public bool ShowPreview { get; set; }

		public PrintSettingsHandler ()
		{
			Control = new Gtk.PrintSettings();
			PageSetup = new Gtk.PageSetup();
			MaximumPageRange = new Range(1, 1);
		}

		public PrintSettingsHandler (Gtk.PrintSettings settings, Gtk.PageSetup setup)
		{
			MaximumPageRange = new Range(1, 1);
			Set (settings, setup);
		}

		public void Set (Gtk.PrintSettings settings, Gtk.PageSetup setup)
		{
			Control = settings;
			PageSetup = setup;
		}

		public int Copies {
			get { return Control.NCopies; }
			set { Control.NCopies = value; }
		}

		public Range MaximumPageRange {
			get; set;
		}

		public Range SelectedPageRange {
			get {
				int num_ranges;
				return Control.GetPageRanges (out num_ranges).ToEto ();
			}
			set {
				Control.SetPageRanges(value.ToGtkPageRange (), 1);
			}
		}

		public PageOrientation Orientation {
			get { return PageSetup.Orientation.ToEto (); }
			set { PageSetup.Orientation = value.ToGtk (); }
		}

		public bool Collate {
			get { return Control.Collate; }
			set { Control.Collate = value; }
		}
	}
}

