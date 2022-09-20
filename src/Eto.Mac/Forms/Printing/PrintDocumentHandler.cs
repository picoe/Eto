using System;
using Eto.Forms;
using Eto.Drawing;
using Eto.Mac.Drawing;

namespace Eto.Mac.Forms.Printing
{
	public class PrintDocumentHandler : WidgetHandler<NSView, PrintDocument, PrintDocument.ICallback>, PrintDocument.IHandler
	{
		PrintSettings printSettings;
		int? _pageCount;
		private Control _control;

		public class DrawnPrintView : PrintView
		{
			public override CGRect RectForPage(nint pageNumber)
			{
				var operation = NSPrintOperation.CurrentOperation;
				if (Frame.Size != operation.PrintInfo.PaperSize)
					SetFrameSize(operation.PrintInfo.PaperSize);
				return new CGRect(new CGPoint(0, 0), operation.PrintInfo.PaperSize);
				//return this.Frame;
			}
			public override bool KnowsPageRange(ref NSRange aRange)
			{
				aRange = new NSRange(1, Handler.PageCount);
				return true;
			}
		} 

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

			public override string PrintJobTitle => Handler.Name ?? string.Empty;


			static readonly IntPtr selCurrentContext = Selector.GetHandle("currentContext");
			static readonly IntPtr classNSGraphicsContext = Class.GetHandle("NSGraphicsContext");

			public override void DrawRect(CGRect dirtyRect)
			{
				var operation = NSPrintOperation.CurrentOperation;

				var context = Messaging.GetNSObject<NSGraphicsContext>(Messaging.IntPtr_objc_msgSend(classNSGraphicsContext, selCurrentContext));
				// this causes monomac to hang for some reason:
				//var context = NSGraphicsContext.CurrentContext;
				base.DrawRect(dirtyRect);
				var height = Frame.Height;
				var pageRect = RectForPage(operation.CurrentPage);

				using (var graphics = new Graphics(new GraphicsHandler(this, context, (float)height)))
				{
					graphics.TranslateTransform((float)pageRect.X, (float)(height-pageRect.Y-pageRect.Height));
					Handler.Callback.OnPrintPage(Handler.Widget, new PrintPageEventArgs(graphics, operation.PrintInfo.ImageablePageBounds.Size.ToEto(), (int)operation.CurrentPage - 1));
				}
			}
			
			public virtual void PrepareForPrint(NSPrintOperation operation)
			{
			}

		}

		public void Print() => Print(false, null, null);

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
			var op = NSPrintOperation.FromView(Control, PrintSettings.ToNS());
				
			(Control as PrintView)?.PrepareForPrint(op);
			
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

		public int PageCount
		{
			get => _pageCount ?? (_pageCount = CalculatePageCount()).Value;
			set => _pageCount = value;
		}

		int CalculatePageCount()
		{
			if (_control == null)
				return 0;

			var printInfo = printSettings.ToNS();
			if (printInfo == null)
			{
				printInfo = new NSPrintInfo();
				PrintSettingsHandler.SetDefaults(printInfo);
			}

			// todo: calculate if width is greater than one page??
			var preferredSize = _control.GetPreferredSize();
			var paperSize = printInfo.ImageablePageBounds.Size;
			return (int)Math.Ceiling(preferredSize.Height / paperSize.Height);
		}

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

		public void Create()
		{
			Control = new DrawnPrintView { Handler = this };
		}

		public class ChildPrintView : PrintView
		{
			Control _child;
			NSView _subView;
			SizeF _preferredSize;
			int _numPages;
			public ChildPrintView(Control child)
			{
				_child = child;
				_preferredSize = _child.GetPreferredSize();
				SetFrameSize(_preferredSize.ToNS());
				_subView = child.GetContainerView();
				_subView.Frame = Bounds;
				_subView.AutoresizingMask = NSViewResizingMask.WidthSizable | NSViewResizingMask.HeightSizable;
				_subView.TranslatesAutoresizingMaskIntoConstraints = true;
				AddSubview(_subView);
			}
			
			public override void PrepareForPrint(NSPrintOperation operation)
			{
				base.PrepareForPrint(operation);
				
				var paperSize = operation.PrintInfo.ImageablePageBounds.Size;
				var size = _preferredSize.ToNS();

				// todo: take into account if width > page size and multiply number of pages as needed.
				var scale = (nfloat)Math.Min(1.0, paperSize.Width / size.Width);
				
				operation.PrintInfo.ScalingFactor = scale;
				
				_numPages = (int)Math.Ceiling(_preferredSize.Height / (paperSize.Height / scale));
				
				size.Width = (nfloat)(Math.Max(size.Width, paperSize.Width / scale));
				size.Height = (nfloat)(Math.Max(size.Height, paperSize.Height / scale * _numPages));
				SetFrameSize(size);
				UpdateConstraintsForSubtreeIfNeeded();
				LayoutSubtreeIfNeeded();

				// scale to fit
				if (scale < 1)
					ScaleUnitSquareToSize(new CGSize(scale, scale));
				
			}

			public override CGRect RectForPage(nint pageNumber)
			{
				var operation = NSPrintOperation.CurrentOperation;
				var paperSize = operation.PrintInfo.ImageablePageBounds.Size;
				var frame = Frame;
				var scale = operation.PrintInfo.ScalingFactor;
				
				var rect = new CGRect();
				rect.Y = (nfloat)Math.Max(0, frame.Height - (paperSize.Height * pageNumber / scale));
				rect.Width = (nfloat)(Math.Min(frame.Width, paperSize.Width / scale));
				rect.Height = (nfloat)(Math.Min(frame.Height, paperSize.Height / scale));
				
				return rect;
			}

			public override bool KnowsPageRange(ref NSRange aRange)
			{
				aRange = new NSRange(1, _numPages);
				return true;
			}

		}
		
		bool _dispose = true;

		protected override bool DisposeControl => _dispose;

		public void Create(Control control)
		{
			_control = control;
			
			if (_control.Loaded || _control.Parent != null)
			{
				Control = _control.GetContainerView();
				_dispose = false;
			}
			else
				Control = new ChildPrintView(control) { Handler = this };
		}
	}
}

