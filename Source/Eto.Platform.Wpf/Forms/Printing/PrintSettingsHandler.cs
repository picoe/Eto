using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using swd = System.Windows.Documents;
using swc = System.Windows.Controls;
using sp = System.Printing;
using Eto.Forms;

namespace Eto.Platform.Wpf.Forms.Printing
{
	public static class PrintSettingsExtensions
	{
		public static void SetEtoSettings (this swc.PrintDialog dialog, PrintSettings settings)
		{
			if (dialog == null) return;
			if (settings != null) {
				var handler = (PrintSettingsHandler)settings.Handler;
				dialog.PrintQueue = handler.PrintQueue;
				dialog.PrintTicket = handler.Control;
				dialog.MinPage = (uint)handler.PageRange.Location;
				dialog.MaxPage = (uint)(handler.PageRange.Location + handler.PageRange.Length - 1);
				dialog.PageRange = handler.SelectedRange;
				dialog.PageRangeSelection = handler.PageRangeSelection;
			}
			else {
				dialog.PrintQueue = null;
				dialog.PrintTicket = null;
				dialog.MinPage = 1;
				dialog.MaxPage = 1;
			}
		}

		public static PrintSettings GetEtoSettings (this swc.PrintDialog dialog, Eto.Generator generator)
		{
			if (dialog == null) return null;
			return new PrintSettings (generator, new PrintSettingsHandler (dialog));
		}
	}

	public class PrintSettingsHandler : WidgetHandler<sp.PrintTicket, PrintSettings>, IPrintSettings
	{
		public swc.PageRange SelectedRange { get; set; }
		public swc.PageRangeSelection PageRangeSelection { get; set; }
		public sp.PrintQueue PrintQueue { get; set; }

		public PrintSettingsHandler (swc.PrintDialog dialog)
		{
			Control = dialog.PrintTicket;
			PrintQueue = dialog.PrintQueue;
			PageRange = new Range ((int)dialog.MinPage, (int)(dialog.MaxPage - dialog.MinPage) + 1);
			SelectedRange = dialog.PageRange;
			PageRangeSelection = dialog.PageRangeSelection;
		}

		public PrintSettingsHandler ()
		{
			Control = new sp.PrintTicket ();
			PrintQueue = new swc.PrintDialog ().PrintQueue;
			PageRange = new Range (1, 1);
			SelectedRange = new swc.PageRange (1, 1);
		}

		public int Copies
		{
			get { return Control.CopyCount ?? 1; }
			set { Control.CopyCount = value; }
		}

		public Range MaximumPageRange { get; set; }

		public PageOrientation Orientation
		{
			get { return Control.PageOrientation.ToEto (); }
			set { Control.PageOrientation = value.ToSP (); }
		}
	}
}
