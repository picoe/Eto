using sw = System.Windows;
using swd = System.Windows.Documents;
using swc = System.Windows.Controls;
using swm = System.Windows.Media;
using Eto.Forms;
using Eto.Drawing;
using Eto.Wpf.Drawing;
using System;
using System.Collections;

namespace Eto.Wpf.Forms.Printing
{
	public class PrintDocumentHandler : WidgetHandler<PrintDocumentHandler.Paginator, PrintDocument, PrintDocument.ICallback>, PrintDocument.IHandler
	{
		Control _control;
		int? _pageCount;
		sw.Rect _imageableArea;

		public PrintDocumentHandler()
		{
			Control = new Paginator { Handler = this };
		}

		class Canvas : swc.UserControl
		{
			public PrintDocumentHandler Handler { get; set; }

			int _pageNumber;
			public int PageNumber
			{
				get => _pageNumber;
				set
				{
					_pageNumber = value;
					InvalidateVisual();
				}
			}

			protected override void OnRender(swm.DrawingContext drawingContext)
			{
				base.OnRender(drawingContext);
				var rect = new Rectangle(new Size((int)ActualWidth, (int)ActualHeight));
				var graphicsHandler = new GraphicsHandler(this, drawingContext, new sw.Rect(0, 0, ActualWidth, ActualHeight), rect);
				var graphics = new Graphics(graphicsHandler);
				// needed to set size properly for some reason.. ??
				graphics.DrawRectangle(new Pen(Colors.Transparent), rect);

				var args = new PrintPageEventArgs(graphics, rect.Size, PageNumber);
				Handler.Callback.OnPrintPage(Handler.Widget, args);
			}
		}

		class Combined : sw.FrameworkElement
		{
			Canvas _canvas;
			sw.FrameworkElement _control;
			public PrintDocumentHandler Handler { get; set; }
			public Canvas Canvas
			{
				get => _canvas;
				set
				{
					if (_canvas != value)
					{
						RemoveLogicalChild(_canvas);
						RemoveVisualChild(_canvas);
						_canvas = value;
						AddLogicalChild(_canvas);
						AddVisualChild(_canvas);
						InvalidateMeasure();
					}
				}
			}
			public sw.FrameworkElement Control
			{
				get => _control;
				set
				{
					if (_control != value)
					{
						RemoveLogicalChild(_control);
						RemoveVisualChild(_control);
						_control = value;
						AddLogicalChild(_control);
						AddVisualChild(_control);
						InvalidateMeasure();
					}
				}
			}

			protected override int VisualChildrenCount => 2;

			protected override swm.Visual GetVisualChild(int index)
			{
				if (index == 0)
					return Canvas;
				if (index == 1)
					return Control;
				throw new ArgumentOutOfRangeException(nameof(index));
			}

			protected override IEnumerator LogicalChildren
			{
				get
				{
					yield return Canvas;
					yield return Control;
				}
			}

			int _pageNumber;
			public int PageNumber
			{
				get => _pageNumber;
				set
				{
					_pageNumber = value;
					Canvas.PageNumber = value;
					InvalidateArrange();
				}
			}


			protected override sw.Size ArrangeOverride(sw.Size finalSize)
			{
				var finalRect = new sw.Rect(new sw.Point(), finalSize);
				Canvas?.Arrange(finalRect);

				var desired = Control.DesiredSize;
				var rect = new sw.Rect(0, 0, Math.Max(finalSize.Width, desired.Width), Math.Max(finalSize.Height, desired.Height));
				rect.Y -= PageNumber * Handler._imageableArea.Height;
				Control?.Arrange(rect);
				return finalSize;
			}

			protected override sw.Size MeasureOverride(sw.Size availableSize)
			{
				Canvas?.Measure(availableSize);
				Control?.Measure(new sw.Size(availableSize.Width, double.PositiveInfinity));
				return availableSize;
			}
		}

		public class Paginator : swd.DocumentPaginator
		{
			public PrintDocumentHandler Handler { get; set; }

			public override swd.DocumentPage GetPage(int pageNumber)
			{
				return Handler.GetPage(pageNumber);
			}

			public override bool IsPageCountValid => true;
			public override int PageCount => Handler.PageCount;

			public override sw.Size PageSize { get; set; }

			public override swd.IDocumentPaginatorSource Source => null;
		}
		
		private swd.DocumentPage GetPage(int pageNumber)
		{
			sw.UIElement page;
			bool measure = true;
			if (_control != null)
			{
				var control = _control.GetContainerControl();
				
					
				if (control.Parent is Combined parentCombined)
				{
					parentCombined.PageNumber = pageNumber;
					page = parentCombined;
				}
				else if (control.Parent == null)
				{
					var combined = new Combined
					{
						Handler = this,
						Width = _imageableArea.Width,
						Height = _imageableArea.Height,
						Control = control,
						Canvas = new Canvas { Handler = this }
					};
					
					// force all children to load
					combined.Measure(new sw.Size(double.PositiveInfinity, double.PositiveInfinity));
					combined.Arrange(new sw.Rect(combined.DesiredSize));
					// set the scale after the control is loaded
					// note we need to do the additional measure/arrange below so controls get to the right spot.
					_control?.GetWpfFrameworkElement()?.SetScale(true, true);
					page = combined;
				}
				else
				{
					page = control;
					measure = false;
				}
			}
			else
			{
				page = new Canvas
				{
					Handler = this,
					PageNumber = pageNumber,
					Width = _imageableArea.Width,
					Height = _imageableArea.Height
				};
			}
			
			if (measure)
			{
				page.Measure(_imageableArea.Size);
				page.Arrange(_imageableArea);
			}
			
			return new swd.DocumentPage(page);
		}

		public void Print()
		{
			var print = new swc.PrintDialog();
			print.SetEtoSettings(PrintSettings);


			Control.PageSize = new sw.Size(print.PrintableAreaWidth, print.PrintableAreaHeight);
			var printCapabilities = print.PrintQueue.GetPrintCapabilities(print.PrintTicket);
			var ia = printCapabilities.PageImageableArea;
			_imageableArea = new sw.Rect(ia.OriginWidth, ia.OriginHeight, ia.ExtentWidth, ia.ExtentHeight);

			print.PrintDocument(Control, Name);
		}
		
		public sw.Xps.Packaging.XpsDocument GetPrintPreview(string tempFileName)
		{
			var print = new swc.PrintDialog();
			print.SetEtoSettings(PrintSettings);

			Control.PageSize = new sw.Size(print.PrintableAreaWidth, print.PrintableAreaHeight);
			var printCapabilities = print.PrintQueue.GetPrintCapabilities(print.PrintTicket);
			var ia = printCapabilities.PageImageableArea;
			_imageableArea = new sw.Rect(ia.OriginWidth, ia.OriginHeight, ia.ExtentWidth, ia.ExtentHeight);

			var xpsDocument = new sw.Xps.Packaging.XpsDocument(tempFileName, System.IO.FileAccess.ReadWrite);
			var writer = sw.Xps.Packaging.XpsDocument.CreateXpsDocumentWriter(xpsDocument);
			writer.WritingPrintTicketRequired += (sender, e) => e.CurrentPrintTicket = print.PrintTicket;
			writer.Write(Control);
			return xpsDocument;
		}

		public override void AttachEvent(string id)
		{
			switch (id)
			{
				case PrintDocument.PrintingEvent:
				case PrintDocument.PrintedEvent:
				case PrintDocument.PrintPageEvent:
					// handled by paginator
					break;
				default:
					base.AttachEvent(id);
					break;
			}
		}

		public void Create()
		{
		}

		public void Create(Control control)
		{
			_control = control;
		}

		public string Name
		{
			get; set;
		}

		public int PageCount
		{
			get => _pageCount ?? (_pageCount = GetPageCount()) ?? 0;
			set => _pageCount = value;
		}

		int GetPageCount()
		{
			if (_control == null)
				return 0;

			var settings = PrintSettings.Handler as PrintSettingsHandler;

			var printCapabilities = settings.PrintQueue.GetPrintCapabilities(settings.Control);
			var ia = printCapabilities.PageImageableArea;

			// var ctl = _control.GetContainerControl();
			// ctl.Measure(WpfConversions.PositiveInfinitySize);
			// var preferredSize = ctl.DesiredSize;

			var preferredSize = _control.GetPreferredSize();
			return (int)Math.Ceiling(preferredSize.Height / ia.ExtentHeight);
		}

		PrintSettings _printSettings;
		public PrintSettings PrintSettings
		{
			get
			{
				if (_printSettings == null)
					_printSettings = new PrintSettings();
				return _printSettings;
			}
			set => _printSettings = value;
		}
	}
}
