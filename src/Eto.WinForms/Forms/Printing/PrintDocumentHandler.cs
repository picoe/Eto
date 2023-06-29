using sdp = System.Drawing.Printing;
namespace Eto.WinForms.Forms.Printing
{
	public class PrintDocumentHandler : WidgetHandler<sdp.PrintDocument, PrintDocument, PrintDocument.ICallback>, PrintDocument.IHandler
	{
		int _currentPage;
		int? _pageCount;
		Control _control;
		sd.Bitmap _controlBitmap;
		PrintSettings printSettings;
		SizeF _preferredSize;

		public PrintDocumentHandler()
		{
			Control = new sdp.PrintDocument
			{
				PrinterSettings = PrintSettingsHandler.DefaultSettings()
			};
		}

		public void Create()
		{
		}

		public void Create(Control control)
		{
			_control = control;
		}

		protected override void Initialize()
		{
			base.Initialize();
			Control.BeginPrint += Control_BeginPrint;
			Control.PrintPage += HandlePrintPage;
			Control.EndPrint += Control_EndPrint;
		}

		private void Control_EndPrint(object sender, sdp.PrintEventArgs e)
		{
			_controlBitmap?.Dispose();
			_controlBitmap = null;
			Callback.OnPrinted(Widget, e);
		}

		public void Print()
		{
			Control.Print();
		}

		public override void AttachEvent(string id)
		{
			switch (id)
			{
				case PrintDocument.PrintingEvent:
				case PrintDocument.PrintPageEvent:
				case PrintDocument.PrintedEvent:
					// handled intrinsically
					break;
				default:
					base.AttachEvent(id);
					break;
			}
		}

		private void Control_BeginPrint(object sender, sdp.PrintEventArgs e)
		{
			_currentPage = 0;
			Callback.OnPrinting(Widget, e);
			
			_preferredSize = _control?.GetPreferredSize() ?? SizeF.Empty;
		}

		protected virtual void HandlePrintPage(object sender, sdp.PrintPageEventArgs e)
		{
			using (var graphics = e.Graphics.ToEto(false))
			{
				var bounds = e.MarginBounds;
				var container = _control.GetContainerControl();
				if (container != null)
				{
					container.SuspendLayout();
					container.Width = (int)Math.Max(bounds.Width, _preferredSize.Width);
					container.Height = (int)Math.Max(bounds.Height, _preferredSize.Height);
					container.ResumeLayout();
					
					// create an offscreen bitmap to paint to
					var bitmapSize = bounds.Size;
					if (_controlBitmap == null || _controlBitmap.Size != bitmapSize)
					{
						_controlBitmap?.Dispose();
						_controlBitmap = new sd.Bitmap(bitmapSize.Width, bitmapSize.Height, sd.Imaging.PixelFormat.Format32bppArgb);
					}
					
					// setup a graphics object to paint to
					sd.Graphics g = sd.Graphics.FromImage(_controlBitmap);
					var hdc = g.GetHdc();
					
					// set the offset for the current page
					var y = _currentPage * bounds.Height;
					var oldOffset = new Win32.POINT();
					Win32.OffsetWindowOrgEx(hdc, 0, y, ref oldOffset);
					
					// send the WM_PRINT message
					Win32.SendMessage(
						container.Handle,
						Win32.WM.PRINT,
						hdc,
						(IntPtr)(Win32.PRF.CHILDREN | Win32.PRF.CLIENT | Win32.PRF.ERASEBKGND | Win32.PRF.NONCLIENT));
					
					// restore offset
					Win32.OffsetWindowOrgEx(hdc, oldOffset.x, oldOffset.y, ref oldOffset);
					
					g.ReleaseHdc(hdc);
					g.Dispose();
					
					// draw the image to the print graphics context
					e.Graphics.DrawImage(_controlBitmap, bounds);
				}

				graphics.TranslateTransform(bounds.Left, bounds.Top);
				var args = new PrintPageEventArgs(graphics, bounds.Size.ToEto(), _currentPage);
				Callback.OnPrintPage(Widget, args);
				_currentPage++;
				e.HasMorePages = _currentPage < PageCount;
			}
		}

		public string Name
		{
			get { return Control.DocumentName; }
			set { Control.DocumentName = value; }
		}

		public int PageCount
		{
			get => _pageCount ?? (_pageCount = GetPageCount()).Value;
			set => _pageCount = value;
		}

		private int GetPageCount()
		{
			if (_control == null)
				return 0;

			var size = Control.DefaultPageSettings.Bounds.Size.ToEto();

			var preferred = _control.GetPreferredSize();
			return (int)Math.Ceiling(preferred.Height / size.Height);
		}

		public PrintSettings PrintSettings
		{
			get
			{
				if (printSettings == null)
					printSettings = Control.PrinterSettings.ToEto();
				return printSettings;
			}
			set
			{
				printSettings = value;
				Control.PrinterSettings = value.ToSD();
			}
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && _controlBitmap != null)
			{
				_controlBitmap.Dispose();
				_controlBitmap = null;
			}
			base.Dispose(disposing);
		}
	}
}
