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
			Control.Run (Gtk.PrintOperationAction.PrintDialog, null);
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
						var width = args.Context.PageSetup.GetPageWidth(Gtk.Unit.Points);
						var height = args.Context.PageSetup.GetPageHeight(Gtk.Unit.Points);
						var e = new PrintPageEventArgs(graphics, new Size((int)width, (int)height), args.PageNr);
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
			get { return settings; }
			set {
				settings = value;
				if (settings != null)
					Control.PrintSettings = ((PrintSettingsHandler)value.Handler).Control;
				else
					Control.PrintSettings = new Gtk.PrintSettings();
			}
		}

		public int PageCount {
			get { return Control.NPages; }
			set { Control.NPages = value; }
		}
	}
}

