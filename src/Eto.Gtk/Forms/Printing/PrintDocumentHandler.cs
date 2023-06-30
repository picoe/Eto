using Eto.GtkSharp.Drawing;

namespace Eto.GtkSharp.Forms.Printing
{
	public class PrintDocumentHandler : WidgetHandler<Gtk.PrintOperation, PrintDocument, PrintDocument.ICallback>, PrintDocument.IHandler
	{
		PrintSettings _settings;
		Control _control;
		SizeF _preferredSize;
		Gtk.Widget _oldParent;
#if GTK3
		Gtk.OffscreenWindow _offscreenWindow;
#endif

		public PrintDocumentHandler()
		{
			Control = new Gtk.PrintOperation();
			Control.BeginPrint += Control_BeginPrint;
			Control.EndPrint += Control_EndPrint;
			Control.DrawPage += Control_DrawPage;
		}

		private void Control_EndPrint(object o, Gtk.EndPrintArgs args)
		{
			Callback.OnPrinted(Widget, EventArgs.Empty);
			if (_oldParent != null && _control != null)
			{
				var gtkWidget = _control.GetContainerWidget();
				gtkWidget.Parent = _oldParent;
				_oldParent = null;
			}
		}

		private void Control_BeginPrint(object o, Gtk.BeginPrintArgs args)
		{
			if (_control != null && PageCount == -1)
			{
				var paperHeight = args.Context.Height;
				var paperWidth = args.Context.Width;

				_preferredSize = _control.GetPreferredSize();
#if GTK3				
				// to draw the widget using Cairo, we need to use an offscreen window
				var gtkWidget = _control.GetContainerWidget();
				_oldParent = gtkWidget.Parent;
				_offscreenWindow = new Gtk.OffscreenWindow();
				_offscreenWindow.Child = gtkWidget;

				_offscreenWindow.SetSizeRequest((int)Math.Max(_preferredSize.Width, paperWidth), (int)Math.Max(_preferredSize.Height, paperHeight));
				_offscreenWindow.ShowAll();
#endif
				
				PageCount = (int)Math.Ceiling(_preferredSize.Height / paperHeight);
			}
			Callback.OnPrinting(Widget, EventArgs.Empty);
		}

		internal void Print(Gtk.Window parentWindow)
		{
			var settingsHandler = (PrintSettingsHandler)PrintSettings.Handler;

			Control.PrintSettings = settingsHandler.Control;
			if (settingsHandler.ShowPreview)
			{
				Control.Run(Gtk.PrintOperationAction.Preview, parentWindow);
			}
			else
			{
				Control.Run(Gtk.PrintOperationAction.PrintDialog, parentWindow);
			}
		}

		public void Create()
		{
		}

		public void Create(Control control)
		{
			_control = control;
		}

		public void Print() => Print(null);

		public override void AttachEvent(string id)
		{
			switch (id)
			{
				case PrintDocument.PrintingEvent:
				case PrintDocument.PrintedEvent:
				case PrintDocument.PrintPageEvent:
					// handled intrinsically
					break;
				default:
					base.AttachEvent(id);
					break;
			}
		}

		private void Control_DrawPage(object o, Gtk.DrawPageArgs args)
		{
			var width = args.Context.Width;
			var height = args.Context.Height;
			var pageNumber = args.PageNr;

			var context = args.Context.CairoContext;

			if (_control != null)
			{
				context.Save();
				context.Rectangle(new Cairo.Rectangle(0, 0, width, height));
				context.Clip();
				var ctl = _control.GetContainerWidget();
				context.Translate(0, -pageNumber * height);
#if !GTK2
				ctl.Draw(context);
#endif
				context.Restore();
			}

			using (var graphics = new Graphics(new GraphicsHandler(context, args.Context.CreatePangoContext())))
			{
				var e = new PrintPageEventArgs(graphics, new SizeF((float)width, (float)height), pageNumber);
				Callback.OnPrintPage(Widget, e);
			}
		}

		public string Name
		{
			get { return Control.JobName; }
			set { Control.JobName = value; }
		}

		public PrintSettings PrintSettings
		{
			get
			{
				if (_settings == null)
					_settings = Control.PrintSettings.ToEto(Control.DefaultPageSetup, false);
				return _settings;
			}
			set
			{
				_settings = value;
				Control.DefaultPageSetup = _settings.ToGtkPageSetup();
				Control.PrintSettings = _settings.ToGtkPrintSettings();
			}
		}

		public int PageCount
		{
			get { return Control.NPages; }
			set { Control.NPages = value; }
		}
	}
}

