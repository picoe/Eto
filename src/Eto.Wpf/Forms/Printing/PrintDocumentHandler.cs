using swd = System.Windows.Documents;
using Eto.Wpf.Drawing;
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
			double _scale = 1;
			public PrintDocumentHandler Handler { get; set; }
			
			public double Scale
			{
				get => _scale;
				set
				{
					_scale = value;
					LayoutTransform = new swm.ScaleTransform(_scale, _scale);
				}
			}
			
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
				var pageSize = Handler._imageableArea;
				var pageCount = Handler._pageCount ?? 1;
				var rect = new sw.Rect(0, -PageNumber * pageSize.Height / Scale, pageSize.Width / Scale, pageSize.Height * pageCount / Scale);
				Control?.Arrange(rect);
				return finalSize;
			}

			protected override sw.Size MeasureOverride(sw.Size availableSize)
			{
				base.MeasureOverride(availableSize);
				
				Canvas?.Measure(availableSize);
				Control?.Measure(new sw.Size(availableSize.Width, double.PositiveInfinity));
				var canvasDesired = Canvas?.DesiredSize.ToEto() ?? SizeF.Empty;
				var controlDesired = Control?.DesiredSize.ToEto() ?? SizeF.Empty;
				return SizeF.Max(canvasDesired, controlDesired).ToWpf();
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
				SetupControl();
				var control = _control.GetContainerControl();

				if (control.Parent is Combined parentCombined)
				{
					parentCombined.PageNumber = pageNumber;
					page = parentCombined;
				}
				else
				{
					page = control;
					var scale = Math.Min(1, Math.Min(_imageableArea.Width / page.DesiredSize.Width, _imageableArea.Height / page.DesiredSize.Height));
					if (_lastTransform == null)
						_lastTransform = control.LayoutTransform;
					control.LayoutTransform = new swm.ScaleTransform(scale, scale);
					measure = true;
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

			return new swd.DocumentPage(page, Control.PageSize, sw.Rect.Empty, _imageableArea);
		}

		private void SetupControl()
		{
			var control = _control?.GetContainerControl();
			if (control == null || control.Parent != null)
				return;
				
			var combined = new Combined
			{
				Handler = this,
				Control = control,
				Canvas = new Canvas { Handler = this }
			};

			// force all children to load
			combined.Measure(new sw.Size(double.PositiveInfinity, double.PositiveInfinity));
			combined.Arrange(new sw.Rect(combined.DesiredSize));
			// set the scale after the control is loaded
			// note we need to do the additional measure/arrange below so controls get to the right spot.
			_control?.GetWpfFrameworkElement()?.SetScale(true, true);

			// limit to at most one page wide, so scale the desired size to fit..
			// todo: allow spanning multiple pages wide somehow?
			combined.Scale = Math.Min(1, _imageableArea.Width / combined.DesiredSize.Width);
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
			Cleanup();
		}

		private void Cleanup()
		{
			if (_control != null && _lastTransform != null)
			{
				var element = _control.GetContainerControl();
				element.LayoutTransform = _lastTransform;
			}
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
			Cleanup();
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
				
			// can't span pages if printing directly from screen
			if (_control.Parent != null)
				return 1;

			// ensure the WPF control isn't attached
			var wpfControl = _control.GetContainerControl();
			if (wpfControl == null || (wpfControl.Parent != null && !(wpfControl.Parent is Combined)))
				return 1;

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
		swm.Transform _lastTransform;

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
