using System;
using MonoMac.AppKit;
using Eto.Forms;
using MonoMac.Foundation;
using Eto.Drawing;
using Eto.Platform.Mac.Drawing;
using MonoMac.ObjCRuntime;

namespace Eto.Platform.Mac.Forms.Printing
{
	public class PrintDocumentHandler : WidgetHandler<PrintDocumentHandler.PrintView, PrintDocument>, IPrintDocument
	{
		PrintSettings printSettings;

		[Register ("PrintView")]
		public class PrintView : NSView
		{
			public PrintDocumentHandler Handler { get; set; }

			public override string PrintJobTitle {
				get { return Handler.Name ?? string.Empty; }
			}

			public override System.Drawing.RectangleF RectForPage (int pageNumber)
			{
				var operation = NSPrintOperation.CurrentOperation;
				if (this.Frame.Size != operation.PrintInfo.PaperSize)
					this.SetFrameSize (operation.PrintInfo.PaperSize);
				return new System.Drawing.RectangleF(new System.Drawing.PointF(0, 0), operation.PrintInfo.PaperSize);
				//return this.Frame;
			}
			static IntPtr selCurrentContext = Selector.GetHandle ("currentContext");
			static IntPtr classNSGraphicsContext = Class.GetHandle ("NSGraphicsContext");

			public override void DrawRect (System.Drawing.RectangleF dirtyRect)
			{
				var operation = NSPrintOperation.CurrentOperation;

				var context = new NSGraphicsContext(Messaging.IntPtr_objc_msgSend (classNSGraphicsContext, selCurrentContext));
				// this causes monomac to hang for some reason:
				//var context = NSGraphicsContext.CurrentContext;

				using (var graphics = new Graphics(Handler.Widget.Generator, new GraphicsHandler(context, this.Frame.Height))) {
					Handler.Widget.OnPrintPage (new PrintPageEventArgs (graphics, Generator.ConvertF (operation.PrintInfo.PaperSize), operation.CurrentPage - 1));
				}
			}

			public override bool KnowsPageRange (ref NSRange aRange)
			{
				aRange = new NSRange(1, Handler.PageCount);
				return true;
			}
		}

		public PrintDocumentHandler ()
		{
			Control = new PrintView { Handler = this };
		}

		public class ModalReceiver : NSObject
		{
			public PrintDocumentHandler Handler { get; set; }

			[Export ("printOperationDidRun:success:contextInfo:")]
			void PrintOperationDidRun (NSPrintOperation operation, bool success, IntPtr contextInfo)
			{
				//NSApplication.SharedApplication.StopModalWithCode (success ? 1 : 0);
			}
		}


		public void Print ()
		{
			var op = NSPrintOperation.FromView(Control);
			if (printSettings != null)
				op.PrintInfo = ((PrintSettingsHandler)printSettings.Handler).Control;
			//var window = NSApplication.SharedApplication.KeyWindow;
			//Control.RunOperationModal(window, new ModalReceiver { Handler = this }, new Selector("printOperationDidRun:success:contextInfo:"), IntPtr.Zero);
			op.RunOperation ();
		}

		public string Name { get; set; }

		public int PageCount { get; set; }

		public Size PageSize
		{
			get { return Generator.ConvertF (Control.Frame.Size); }
			set { Control.SetFrameSize (Generator.Convert (value)); }
		}

		public PrintSettings PrintSettings {
			get { return printSettings; }
			set {
				printSettings = value;
				//Control.PrintInfo = printSettings == null ? null : ((PrintSettingsHandler)printSettings.Handler).Control;
			}
		}

		public override void AttachEvent (string id)
		{
			switch (id) {
			case PrintDocument.PrintPageEvent:
				break;
			default:
				base.AttachEvent (id);
				break;
			}
		}
	}
}

