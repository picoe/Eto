using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using swf = System.Windows.Forms;
using sdp = System.Drawing.Printing;
using Eto.Forms;
using Eto.Drawing;
using Eto.Platform.Windows.Drawing;

namespace Eto.Platform.Windows.Forms.Printing
{
	public class PrintDocumentHandler : WidgetHandler<sdp.PrintDocument, PrintDocument>, IPrintDocument
	{
		int currentPage;
		PrintSettings printSettings;

		public PrintDocumentHandler ()
		{
			Control = new sdp.PrintDocument ();
		}

		public void Print ()
		{
			var dialog = new PrintDialog (Widget.Generator);
			dialog.PrintSettings = this.PrintSettings;
			if (dialog.ShowDialog (null) == DialogResult.Ok)
				Control.Print ();
		}

		public override void AttachEvent (string id)
		{
			switch (id) {
			case PrintDocument.BeginPrintEvent:
				Control.BeginPrint += (sender, e) => {
					currentPage = 0;
					Widget.OnBeginPrint (e);
				};
				break;
			case PrintDocument.EndPrintEvent:
				Control.EndPrint += (sender, e) => {
					Widget.OnEndPrint (e);
				};
				break;
			case PrintDocument.PrintPageEvent:
				Control.PrintPage += (sender, e) => {
					var graphics = new Graphics (Widget.Generator, new GraphicsHandler (e.Graphics));
					
					var args = new PrintPageEventArgs (graphics, this.PageSize, currentPage);
					Widget.OnPrintPage (args);
					currentPage++;
					e.HasMorePages = currentPage < PageCount;
				};
				break;
			default:
				base.AttachEvent (id);
				break;
			}
		}

		public string Name
		{
			get { return Control.DocumentName; }
			set { Control.DocumentName = value; }
		}

		public int PageCount
		{
			get; set;
		}

		public Size PageSize
		{
			get; set;
		}

		public PrintSettings PrintSettings
		{
			get { return printSettings; }
			set {
				printSettings = value;
				Control.PrinterSettings = printSettings == null ? null : ((PrintSettingsHandler)printSettings.Handler).Control;
			}
		}
	}
}
