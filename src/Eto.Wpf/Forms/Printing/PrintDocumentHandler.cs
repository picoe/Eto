using sw = System.Windows;
using swd = System.Windows.Documents;
using swc = System.Windows.Controls;
using swm = System.Windows.Media;
using Eto.Forms;
using Eto.Drawing;
using Eto.Wpf.Drawing;

namespace Eto.Wpf.Forms.Printing
{
	public class PrintDocumentHandler : WidgetHandler<PrintDocumentHandler.Paginator, PrintDocument, PrintDocument.ICallback>, PrintDocument.IHandler
	{
		public PrintDocumentHandler ()
		{
			Control = new Paginator { Handler = this };
		}

		class Canvas : swc.UserControl
		{
			public PrintDocumentHandler Handler { get; set; }

			public int PageNumber { get; set; }

			protected override void OnRender (swm.DrawingContext drawingContext)
			{
				base.OnRender (drawingContext);
				var rect = new Rectangle (new Size((int)Width, (int)Height));
				var graphicsHandler = new GraphicsHandler (this, drawingContext, new sw.Rect (0, 0, Width, Height), rect);
				var graphics = new Graphics(graphicsHandler);
				// needed to set size properly for some reason.. ??
				graphics.DrawRectangle (new Pen(Colors.Transparent), rect);

				var args = new PrintPageEventArgs (graphics, rect.Size, PageNumber);
				Handler.Callback.OnPrintPage(Handler.Widget, args);
			}
		}

		public class Paginator : swd.DocumentPaginator
		{
			public PrintDocumentHandler Handler { get; set; }

			public override swd.DocumentPage GetPage (int pageNumber)
			{
				var page = new Canvas { 
					Handler = Handler,
					PageNumber = pageNumber,
					Width = ImageableArea.Width,
					Height = ImageableArea.Height
				};

				page.Measure (ImageableArea.Size);
				page.Arrange (ImageableArea);

				return new swd.DocumentPage(page);
			}

			public override bool IsPageCountValid
			{
				get { return true; }
			}

			public override int PageCount
			{
				get { return Handler.PageCount; }
			}

			public override sw.Size PageSize
			{
				get; set;
			}
			public sw.Rect ImageableArea
			{
				get;
				set;
			}

			public override swd.IDocumentPaginatorSource Source
			{
				get { return null; }
			}
		}

		public void Print ()
		{
			var print = new swc.PrintDialog ();
			print.SetEtoSettings(PrintSettings);

			Control.PageSize = new sw.Size (print.PrintableAreaWidth, print.PrintableAreaHeight);
			var printCapabilities = print.PrintQueue.GetPrintCapabilities(print.PrintTicket);
			var ia = printCapabilities.PageImageableArea;
			Control.ImageableArea = new sw.Rect(ia.OriginWidth, ia.OriginHeight, ia.ExtentWidth, ia.ExtentHeight);
			//printCapabilities.PageImageableArea.OriginWidth, printCapabilities.PageImageableArea.OriginHeight
			print.PrintDocument(Control, Name);
		}


		public override void AttachEvent (string id)
		{
			switch (id) {
			case PrintDocument.PrintingEvent:
			case PrintDocument.PrintedEvent:
			case PrintDocument.PrintPageEvent:
				// handled by paginator
				break;
			default:
				base.AttachEvent (id);
				break;
			}
		}

		public string Name
		{
			get; set;
		}

		public int PageCount { get; set; }

		public PrintSettings PrintSettings
		{
			get; set;
		}
	}
}
