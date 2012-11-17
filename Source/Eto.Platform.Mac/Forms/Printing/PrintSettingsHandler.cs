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
			MaximumPageRange = new Range(1, 1);
		}
		public int Copies {
			get { return (int)(NSNumber)Control.PrintSettings["com_apple_print_PrintSettings_PMCopies"]; }
			set { Control.PrintSettings["com_apple_print_PrintSettings_PMCopies"] = new NSNumber(value); }
		}

		public Range MaximumPageRange {
			get {
				var range = ((NSArray)Control.PrintSettings["com_apple_print_PrintSettings_PMPageRange"]);
				var firstPage = new NSNumber(range.ValueAt (0)).Int32Value;
				var lastPage = new NSNumber(range.ValueAt (1)).Int32Value;
				return new Range(firstPage, lastPage - firstPage + 1);
			}
			set {
				var array = NSArray.FromNSObjects(new NSNumber(value.Start), new NSNumber(value.End));
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

				if (value.Start < firstPage) {
					Control.PrintSettings["com_apple_print_PrintSettings_PMFirstPage"] = new NSNumber (value.Start);
					Control.PrintSettings["com_apple_print_PrintSettings_PMLastPage"] = new NSNumber (value.End);
				}
				else {
					Control.PrintSettings["com_apple_print_PrintSettings_PMLastPage"] = new NSNumber (value.End);
					Control.PrintSettings["com_apple_print_PrintSettings_PMFirstPage"] = new NSNumber (value.Start);
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

		public PrintSelection PrintSelection {
			get; set;
		}

		public bool Reverse {
			get { 
				var order = (NSString)Control.PrintSettings["OutputOrder"];
				return order != null && order == "Reverse";
			}
			set { Control.PrintSettings["OutputOrder"] = new NSString(value ? "Reverse" : "Normal"); }
		}
	}
}

