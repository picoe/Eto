using System;
using Eto.Forms;

namespace Eto.GtkSharp.Forms.Printing
{
	public static class PrintSettingsExtensions
	{
		public static PrintSettings ToEto (this Gtk.PrintSettings settings, Gtk.PageSetup setup, bool selectionOnly)
		{
			return settings == null ? null : new PrintSettings(new PrintSettingsHandler(settings, setup, selectionOnly));
		}

		public static Gtk.PrintSettings ToGtkPrintSettings (this PrintSettings settings)
		{
			return settings == null ? null : ((PrintSettingsHandler)settings.Handler).Control;
		}

		public static Gtk.PageSetup ToGtkPageSetup (this PrintSettings settings)
		{
			return settings == null ? null : ((PrintSettingsHandler)settings.Handler).PageSetup;
		}
	}

	public class PrintSettingsHandler : WidgetHandler<Gtk.PrintSettings, PrintSettings>, PrintSettings.IHandler
	{
		public bool SelectionOnly { get; set; }

		public Gtk.PageSetup PageSetup { get; set; }

		public bool ShowPreview { get; set; }

		public PrintSettingsHandler ()
		{
			Control = new Gtk.PrintSettings();
			PageSetup = new Gtk.PageSetup();
			MaximumPageRange = new Range<int>(1, 1);
			Collate = true;
		}

		public PrintSettingsHandler (Gtk.PrintSettings settings, Gtk.PageSetup setup, bool selectionOnly)
		{
			MaximumPageRange = new Range<int>(1, 1);
			Set (settings, setup, selectionOnly);
		}

		public void Set (Gtk.PrintSettings settings, Gtk.PageSetup setup, bool selectionOnly)
		{
			Control = settings;
			PageSetup = setup;
			SelectionOnly = selectionOnly;
		}

		public int Copies {
			get { return Control.NCopies; }
			set { Control.NCopies = value; }
		}

		public Range<int> MaximumPageRange
		{
			get; set;
		}

		public Range<int> SelectedPageRange
		{
			get
			{
				int num_pages;

				var pointer = NativeMethods.gtk_print_settings_get_page_ranges(Control.Handle, out num_pages);
				var array = new Gtk.PageRange[num_pages];

				for (int i = 0; i < num_pages; i++)
					array[i] = Gtk.PageRange.New(pointer + i * IntPtr.Size);
				GLib.Marshaller.Free(pointer);

				return array.ToEto();
			}
			set { Control.SetPageRanges(value.ToGtkPageRange (), 1); }
		}

		public PrintSelection PrintSelection {
			get {
				if (SelectionOnly)
					return PrintSelection.Selection;
				return Control.PrintPages.ToEto ();
			}
			set {
				if (value == Eto.Forms.PrintSelection.Selection)
					SelectionOnly = true;
				else {
					Control.PrintPages = value.ToGtk ();
					SelectionOnly = false;
				}
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

		public bool Reverse {
			get { return Control.Reverse; }
			set { Control.Reverse = value; }
		}
	}
}

