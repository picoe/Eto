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

		[Register("PrintView")]
		public class PrintView : NSView
		{
			public PrintView()
			{
			}

			public PrintView(IntPtr handle)
				: base(handle)
			{
			}

			public PrintView(NSCoder coder)
				: base(coder)
			{
			}

			public PrintDocumentHandler Handler { get; set; }

			public override string PrintJobTitle
			{
				get { return Handler.Name ?? string.Empty; }
			}

			public override System.Drawing.RectangleF RectForPage(int pageNumber)
			{
				var operation = NSPrintOperation.CurrentOperation;
				if (this.Frame.Size != operation.PrintInfo.PaperSize)
					this.SetFrameSize(operation.PrintInfo.PaperSize);
				return new System.Drawing.RectangleF(new System.Drawing.PointF(0, 0), operation.PrintInfo.PaperSize);
				//return this.Frame;
			}

			static readonly IntPtr selCurrentContext = Selector.GetHandle("currentContext");
			static readonly IntPtr classNSGraphicsContext = Class.GetHandle("NSGraphicsContext");

			public override void DrawRect(System.Drawing.RectangleF dirtyRect)
			{
				var operation = NSPrintOperation.CurrentOperation;

				var context = new NSGraphicsContext(Messaging.IntPtr_objc_msgSend(classNSGraphicsContext, selCurrentContext));
				// this causes monomac to hang for some reason:
				//var context = NSGraphicsContext.CurrentContext;

				using (var graphics = new Graphics(Handler.Widget.Generator, new GraphicsHandler(this, context, this.Frame.Height, this.IsFlipped)))
				{
					Handler.Widget.OnPrintPage(new PrintPageEventArgs(graphics, Platform.Conversions.ToEto(operation.PrintInfo.PaperSize), operation.CurrentPage - 1));
				}
			}

			public override bool KnowsPageRange(ref NSRange aRange)
			{
				aRange = new NSRange(1, Handler.PageCount);
				return true;
			}
		}

		public PrintDocumentHandler()
		{
			Control = new PrintView { Handler = this };
		}

		public void Print()
		{
			Print(false, null, null);
		}

		class SheetHelper : NSObject
		{
			public bool Success { get; set; }

			[Export("printOperationDidRun:success:contextInfo:")]
			public void PrintOperationDidRun(IntPtr printOperation, bool success, IntPtr contextInfo)
			{
				Success = success;
				NSApplication.SharedApplication.StopModalWithCode(success ? 1 : 0); 
			}
		}

		public bool Print(bool showPanel, Window parent, NSPrintPanel panel)
		{
			var op = NSPrintOperation.FromView(Control);
			if (printSettings != null)
				op.PrintInfo = printSettings.ToNS();
			if (panel != null)
				op.PrintPanel = panel;
			op.ShowsPrintPanel = showPanel;
			if (parent != null)
			{
				var parentHandler = (IMacWindow)parent.Handler;
				var closeSheet = new SheetHelper();
				op.RunOperationModal(parentHandler.Control, closeSheet, new Selector("printOperationDidRun:success:contextInfo:"), IntPtr.Zero);
				NSApplication.SharedApplication.RunModalForWindow(parentHandler.Control);
				return closeSheet.Success;
			}
			else
				return op.RunOperation();
		}

		public string Name { get; set; }

		public int PageCount { get; set; }

		public PrintSettings PrintSettings
		{
			get
			{
				if (printSettings == null)
					printSettings = new PrintSettings(Widget.Generator);
				return printSettings;
			}
			set
			{
				printSettings = value;
				//Control.PrintInfo = printSettings == null ? null : ((PrintSettingsHandler)printSettings.Handler).Control;
			}
		}

		public override void AttachEvent(string id)
		{
			switch (id)
			{
				case PrintDocument.PrintPageEvent:
					break;
				default:
					base.AttachEvent(id);
					break;
			}
		}
	}
}

