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
			Control = new sdp.PrintDocument {
				PrinterSettings = new sdp.PrinterSettings { MinimumPage = 1, MaximumPage = 1, FromPage = 1, ToPage = 1 }
			};
		}

		public void Print ()
		{
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
				Control.EndPrint += (sender, e) => Widget.OnEndPrint(e);
				break;
			case PrintDocument.PrintPageEvent:
				Control.PrintPage += (sender, e) => {
					var graphics = new Graphics (Widget.Generator, new GraphicsHandler (e.Graphics));
					
					var args = new PrintPageEventArgs (graphics, e.PageBounds.Size.ToEto (), currentPage);
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

		public PrintSettings PrintSettings
		{
			get {
				if (printSettings == null)
					printSettings = Control.PrinterSettings.ToEto (Widget.Generator);
				return printSettings;
			}
			set {
				printSettings = value;
				Control.PrinterSettings = value.ToSD ();
			}
		}
	}
}
