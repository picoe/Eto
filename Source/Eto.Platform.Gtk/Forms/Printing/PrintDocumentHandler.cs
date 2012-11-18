using System;
using Eto.Forms;
using Eto.Drawing;
using Eto.Platform.GtkSharp.Drawing;

namespace Eto.Platform.GtkSharp.Forms.Printing
{
	public class PrintDocumentHandler : WidgetHandler<Gtk.PrintOperation, PrintDocument>, IPrintDocument
	{
		PrintSettings settings;

		public PrintDocumentHandler ()
		{
			Control = new Gtk.PrintOperation();
		}

		public void Print ()
		{
			var settingsHandler = (PrintSettingsHandler)this.PrintSettings.Handler;
			Control.PrintSettings = settingsHandler.Control;
				if (settingsHandler.ShowPreview)
				Control.Run (Gtk.PrintOperationAction.Preview, null);
			else
				Control.Run (Gtk.PrintOperationAction.Print, null);
		}

		public override void AttachEvent (string id)
		{
			switch (id) {
			case PrintDocument.BeginPrintEvent:
				Control.BeginPrint += (o, args) => {
					Widget.OnBeginPrint (EventArgs.Empty);
				};
				break;
			case PrintDocument.EndPrintEvent:
				Control.EndPrint += (o, args) => {
					Widget.OnEndPrint (EventArgs.Empty);
				};
				break;

			case PrintDocument.PrintPageEvent:
				Control.DrawPage += (o, args) => {
					using (var graphics = new Graphics(Widget.Generator, new GraphicsHandler(args.Context.CairoContext, args.Context.CreatePangoContext ()))) {
						var width = args.Context.Width; //.PageSetup.GetPageWidth(Gtk.Unit.Points);
						var height = args.Context.Height; //.PageSetup.GetPageHeight(Gtk.Unit.Points);
						var e = new PrintPageEventArgs(graphics, new SizeF((float)width, (float)height), args.PageNr);
						Widget.OnPrintPage (e);
					}
				};
				break;
			default:
				base.AttachEvent (id);
				break;
			}
		}

		public string Name {
			get { return Control.JobName; }
			set { Control.JobName = value; }
		}

		public PrintSettings PrintSettings {
			get {
				if (settings == null)
					settings = Control.PrintSettings.ToEto (Control.DefaultPageSetup, false, Widget.Generator);
				return settings;
			}
			set {
				settings = value;
				Control.DefaultPageSetup = settings.ToGtkPageSetup ();
				Control.PrintSettings = settings.ToGtkPrintSettings ();
			}
		}

		public int PageCount {
			get { return Control.NPages; }
			set { Control.NPages = value; }
		}
	}
}

