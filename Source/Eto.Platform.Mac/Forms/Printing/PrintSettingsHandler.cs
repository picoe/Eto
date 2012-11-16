using System;
using MonoMac.AppKit;
using Eto.Forms;
using MonoMac.Foundation;

namespace Eto.Platform.Mac.Forms.Printing
{
	public static class PrintSettingsExtensions
	{
		public static PrintSettings ToEto (this NSPrintInfo value, Eto.Generator generator)
		{
			if (value == null) return null;
			return new PrintSettings(generator, new PrintSettingsHandler(value));
		}

		public static NSPrintInfo ToNS (this PrintSettings settings)
		{
			if (settings == null) return null;
			return ((PrintSettingsHandler)settings.Handler).Control;
		}
	}

	public class PrintSettingsHandler : WidgetHandler<NSPrintInfo, PrintSettings>, IPrintSettings
	{
		public PrintSettingsHandler (NSPrintInfo info)
		{
			Control = info;
		}

		public PrintSettingsHandler ()
		{
			Control = new NSPrintInfo ();
			PageRange = new Range(1, 1);
		}
		public int Copies {
			get { return (int)(NSNumber)Control.PrintSettings["com_apple_print_PrintSettings_PMCopies"]; }
			set { Control.PrintSettings["com_apple_print_PrintSettings_PMCopies"] = new NSNumber(value); }
		}

		public Range PageRange {
			get {
				var range = ((NSArray)Control.PrintSettings["com_apple_print_PrintSettings_PMPageRange"]);
				var firstPage = new NSNumber(range.ValueAt (0)).Int32Value;
				var lastPage = new NSNumber(range.ValueAt (1)).Int32Value;
				return new Range(firstPage, lastPage - firstPage + 1);
			}
			set {
				var array = NSArray.FromNSObjects(new NSNumber(value.Location), new NSNumber(value.Location + value.Length - 1));
				Control.PrintSettings["com_apple_print_PrintSettings_PMPageRange"] = array;
			}
		}

		public Range SelectedPageRange {
			get {
				var firstPage = ((NSNumber)Control.PrintSettings["com_apple_print_PrintSettings_PMFirstPage"]).Int32Value;
				var lastPage = ((NSNumber)Control.PrintSettings["com_apple_print_PrintSettings_PMLastPage"]).Int32Value;
				return new Range(firstPage, lastPage - firstPage + 1);
			}
			set {
				var firstPage = ((NSNumber)Control.PrintSettings["com_apple_print_PrintSettings_PMFirstPage"]).Int32Value;

				if (value.Location < firstPage) {
					Control.PrintSettings["com_apple_print_PrintSettings_PMFirstPage"] = new NSNumber (value.Location);
					Control.PrintSettings["com_apple_print_PrintSettings_PMLastPage"] = new NSNumber (value.Location + value.Length - 1);
				}
				else {
					Control.PrintSettings["com_apple_print_PrintSettings_PMLastPage"] = new NSNumber (value.Location + value.Length - 1);
					Control.PrintSettings["com_apple_print_PrintSettings_PMFirstPage"] = new NSNumber (value.Location);
				}
			}
		}

		public PageOrientation Orientation {
			get { return Control.Orientation.ToEto (); }
			set { Control.Orientation = value.ToNS (); }
		}

		public bool Collate {
			get { return ((NSNumber)Control.PrintSettings["com_apple_print_PrintSettings_PMCopyCollate"]).BoolValue; }
			set { Control.PrintSettings["com_apple_print_PrintSettings_PMCopyCollate"] = new NSNumber(value); }
		}
	}
}

