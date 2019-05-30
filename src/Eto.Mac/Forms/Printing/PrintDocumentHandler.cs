using System;
using Eto.Forms;
using Eto.Drawing;
using Eto.Mac.Drawing;
#if XAMMAC2
using AppKit;
using Foundation;
using CoreGraphics;
using ObjCRuntime;
using CoreAnimation;
#else
using MonoMac.AppKit;
using MonoMac.Foundation;
using MonoMac.CoreGraphics;
using MonoMac.ObjCRuntime;
using MonoMac.CoreAnimation;
#if Mac64
using nfloat = System.Double;
using nint = System.Int64;
using nuint = System.UInt64;
#else
using nfloat = System.Single;
using nint = System.Int32;
using nuint = System.UInt32;
#endif
#if SDCOMPAT
using CGSize = System.Drawing.SizeF;
using CGRect = System.Drawing.RectangleF;
using CGPoint = System.Drawing.PointF;
#endif
#endif

namespace Eto.Mac.Forms.Printing
{
	public class PrintDocumentHandler : WidgetHandler<PrintDocumentHandler.PrintView, PrintDocument, PrintDocument.ICallback>, PrintDocument.IHandler
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

			WeakReference handler;

			public PrintDocumentHandler Handler
			{
				get => handler?.Target as PrintDocumentHandler;
				set => handler = new WeakReference(value);
			}

			public override string PrintJobTitle
			{
				get { return Handler.Name ?? string.Empty; }
			}

			public override CGRect RectForPage(nint pageNumber)
			{
				var operation = NSPrintOperation.CurrentOperation;
				if (Frame.Size != operation.PrintInfo.PaperSize)
					SetFrameSize(operation.PrintInfo.PaperSize);
				return new CGRect(new CGPoint(0, 0), operation.PrintInfo.PaperSize);
				//return this.Frame;
			}

			static readonly IntPtr selCurrentContext = Selector.GetHandle("currentContext");
			static readonly IntPtr classNSGraphicsContext = Class.GetHandle("NSGraphicsContext");

			public override void DrawRect(CGRect dirtyRect)
			{
				var operation = NSPrintOperation.CurrentOperation;

				var context = Messaging.GetNSObject<NSGraphicsContext>(Messaging.IntPtr_objc_msgSend(classNSGraphicsContext, selCurrentContext));
				// this causes monomac to hang for some reason:
				//var context = NSGraphicsContext.CurrentContext;

				using (var graphics = new Graphics(new GraphicsHandler(this, context, (float)Frame.Height, IsFlipped)))
				{
					Handler.Callback.OnPrintPage(Handler.Widget, new PrintPageEventArgs(graphics, operation.PrintInfo.PaperSize.ToEto(), (int)operation.CurrentPage - 1));
				}
			}

			public override bool KnowsPageRange(ref NSRange aRange)
			{
				aRange = new NSRange(1, Handler.PageCount);
				return true;
			}
		}

		protected override PrintView CreateControl()
		{
			return new PrintView();
		}

		protected override void Initialize()
		{
			Control.Handler = this;
			base.Initialize();
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
			return op.RunOperation();
		}

		public string Name { get; set; }

		public int PageCount { get; set; }

		public PrintSettings PrintSettings
		{
			get
			{
				if (printSettings == null)
					printSettings = new PrintSettings();
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

