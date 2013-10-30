using System;
using MonoMac.AppKit;
using Eto.Forms;
using MonoMac.Foundation;

namespace Eto.Platform.Mac.Forms.Printing
{
	public class PrintSettingsHandler : WidgetHandler<NSPrintInfo, PrintSettings>, IPrintSettings
	{
		int lastSelectedPage;
		int firstSelectedPage;

		public PrintSettingsHandler(NSPrintInfo info)
		{
			Control = info;
		}

		public PrintSettingsHandler()
		{
			Control = new NSPrintInfo();
			MaximumPageRange = new Range(1, 1);
			SelectedPageRange = new Range(1, 1);
		}

		public int Copies
		{
			get { return (int)(NSNumber)Control.PrintSettings["com_apple_print_PrintSettings_PMCopies"]; }
			set { Control.PrintSettings["com_apple_print_PrintSettings_PMCopies"] = new NSNumber(value); }
		}

		public Range MaximumPageRange
		{
			get
			{
				var range = ((NSArray)Control.PrintSettings["com_apple_print_PrintSettings_PMPageRange"]);
				var firstPage = new NSNumber(range.ValueAt(0)).Int32Value;
				var lastPage = new NSNumber(range.ValueAt(1)).Int32Value;
				return new Range(firstPage, lastPage - firstPage + 1);
			}
			set
			{
				var array = NSArray.FromNSObjects(new NSNumber(value.Start), new NSNumber(value.End));
				Control.PrintSettings["com_apple_print_PrintSettings_PMPageRange"] = array;
			}
		}

		public Range SelectedPageRange
		{
			get
			{
				if (IsAllPages)
				{
					return new Range(firstSelectedPage, lastSelectedPage - firstSelectedPage + 1);
				}
				else
				{
					var firstPage = ((NSNumber)Control.PrintSettings["com_apple_print_PrintSettings_PMFirstPage"]).Int32Value;
					var lastPage = ((NSNumber)Control.PrintSettings["com_apple_print_PrintSettings_PMLastPage"]).Int32Value;
					return new Range(firstPage, lastPage - firstPage + 1);
				}
			}
			set
			{
				lastSelectedPage = value.End;
				firstSelectedPage = value.Start;

				if (!IsAllPages)
				{
					var firstPage = ((NSNumber)Control.PrintSettings["com_apple_print_PrintSettings_PMFirstPage"]).Int32Value;
					if (value.Start < firstPage)
					{
						Control.PrintSettings["com_apple_print_PrintSettings_PMFirstPage"] = new NSNumber(value.Start);
						Control.PrintSettings["com_apple_print_PrintSettings_PMLastPage"] = new NSNumber(value.End);
					}
					else
					{
						Control.PrintSettings["com_apple_print_PrintSettings_PMLastPage"] = new NSNumber(value.End);
						Control.PrintSettings["com_apple_print_PrintSettings_PMFirstPage"] = new NSNumber(value.Start);
					}
				}
			}
		}

		public PageOrientation Orientation
		{
			get { return Control.Orientation.ToEto(); }
			set { Control.Orientation = value.ToNS(); }
		}

		public bool Collate
		{
			get { return ((NSNumber)Control.PrintSettings["com_apple_print_PrintSettings_PMCopyCollate"]).BoolValue; }
			set { Control.PrintSettings["com_apple_print_PrintSettings_PMCopyCollate"] = new NSNumber(value); }
		}

		bool IsAllPages
		{
			get
			{
				var lastPage = ((NSNumber)Control.PrintSettings["com_apple_print_PrintSettings_PMLastPage"]).Int32Value;
				var firstPage = ((NSNumber)Control.PrintSettings["com_apple_print_PrintSettings_PMFirstPage"]).Int32Value;
				return firstPage == 1 && lastPage == Int32.MaxValue;
			}
		}

		public PrintSelection PrintSelection
		{
			get
			{
				if (Control.SelectionOnly)
					return PrintSelection.Selection;
				return IsAllPages ? PrintSelection.AllPages : PrintSelection.SelectedPages;
			}
			set
			{
				switch (value)
				{
					case Eto.Forms.PrintSelection.AllPages:
						Control.PrintSettings["com_apple_print_PrintSettings_PMLastPage"] = new NSNumber(Int32.MaxValue);
						Control.PrintSettings["com_apple_print_PrintSettings_PMFirstPage"] = new NSNumber(1);
						Control.SelectionOnly = false;
						break;
					case Eto.Forms.PrintSelection.SelectedPages:
						Control.PrintSettings["com_apple_print_PrintSettings_PMLastPage"] = new NSNumber(lastSelectedPage);
						Control.PrintSettings["com_apple_print_PrintSettings_PMFirstPage"] = new NSNumber(firstSelectedPage);
						Control.SelectionOnly = false;
						break;
					case Eto.Forms.PrintSelection.Selection:
						Control.SelectionOnly = true;
						break;
				}
			}
		}

		public bool Reverse
		{
			get
			{ 
				var order = (NSString)Control.PrintSettings["OutputOrder"];
				return order != null && order == "Reverse";
			}
			set { Control.PrintSettings["OutputOrder"] = new NSString(value ? "Reverse" : "Normal"); }
		}
	}
}

