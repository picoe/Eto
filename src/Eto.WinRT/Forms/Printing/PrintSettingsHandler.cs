#if TODO_XAML
using swd = Windows.UI.Xaml.Documents;
using swc = Windows.UI.Xaml.Controls;
using sp = System.Printing;
using Eto.Forms;

namespace Eto.WinRT.Forms.Printing
{
	public static class PrintSettingsExtensions
	{
		public static void SetEtoSettings(this swc.PrintDialog dialog, PrintSettings settings)
		{
			if (dialog == null) return;
			if (settings != null)
			{
				var handler = (PrintSettingsHandler)settings.Handler;
				dialog.PrintQueue = handler.PrintQueue;
				dialog.PrintTicket = handler.Control;
				var maxPageRange = handler.MaximumPageRange;
				dialog.MinPage = (uint)maxPageRange.Start;
				dialog.MaxPage = (uint)maxPageRange.End;
				dialog.PageRange = handler.SelectedPageRange.ToPageRange();
				dialog.PageRangeSelection = handler.PrintSelection.ToSWC();
			}
			else
			{
				dialog.PrintQueue = null;
				dialog.PrintTicket = null;
				dialog.MinPage = 1;
				dialog.MaxPage = 1;
				dialog.PageRangeSelection = swc.PageRangeSelection.AllPages;
				dialog.PageRange = new swc.PageRange(1, 1);
			}
		}

		public static PrintSettings GetEtoSettings(this swc.PrintDialog dialog, Eto.Generator generator)
		{
			return dialog == null ? null : new PrintSettings(generator, new PrintSettingsHandler(dialog));
		}

		public static void SetFromDialog(this PrintSettings settings, swc.PrintDialog dialog)
		{
			if (dialog == null) return;
			if (settings != null)
			{
				var handler = (PrintSettingsHandler)settings.Handler;
				handler.PrintQueue = dialog.PrintQueue;
				handler.MaximumPageRange = new Range((int)dialog.MinPage, (int)dialog.MaxPage);
				handler.SelectedPageRange = dialog.PageRange.ToEto();
				handler.PrintSelection = dialog.PageRangeSelection.ToEto();
			}
		}

	}

	public class PrintSettingsHandler : WidgetHandler<sp.PrintTicket, PrintSettings>, IPrintSettings
	{
		PrintSelection printSelection;

		public sp.PrintQueue PrintQueue { get; set; }

		public PrintSettingsHandler(swc.PrintDialog dialog)
		{
			Control = dialog.PrintTicket;
			PrintQueue = dialog.PrintQueue;
			MaximumPageRange = new Range((int)dialog.MinPage, (int)(dialog.MaxPage - dialog.MinPage) + 1);
			SelectedPageRange = dialog.PageRange.ToEto();
			PrintSelection = dialog.PageRangeSelection.ToEto();
		}

		public PrintSettingsHandler()
		{
			Control = new sp.PrintTicket();
			PrintQueue = new swc.PrintDialog().PrintQueue;
			MaximumPageRange = new Range(1, 1);
			SelectedPageRange = new Range(1, 1);
			Collate = true;
			PrintSelection = PrintSelection.AllPages;
		}

		public int Copies
		{
			get { return Control.CopyCount ?? 1; }
			set { Control.CopyCount = value; }
		}

		public Range MaximumPageRange { get; set; }

		public Range SelectedPageRange { get; set; }

		public bool Collate
		{
			get { return Control.Collation == sp.Collation.Collated; }
			set { Control.Collation = value ? sp.Collation.Collated : sp.Collation.Uncollated; }
		}

		public PrintSelection PrintSelection
		{
			get { return printSelection; }
			set
			{
				if (value != Eto.Forms.PrintSelection.Selection)
					printSelection = value;
			}
		}

		public PageOrientation Orientation
		{
			get { return Control.PageOrientation.ToEto(); }
			set { Control.PageOrientation = value.ToSP(); }
		}

		public bool Reverse
		{
			get { return Control.PageOrder == sp.PageOrder.Reverse; }
			set { Control.PageOrder = value ? sp.PageOrder.Reverse : sp.PageOrder.Standard; }
		}
	}
}
#endif