using System;
using MonoMac.AppKit;
using Eto.Forms;

namespace Eto.Platform.Mac.Forms.Printing
{
	public class PrintSettingsHandler : WidgetHandler<NSPrintInfo, PrintSettings>, IPrintSettings
	{
		public PrintSettingsHandler ()
		{
			Control = new NSPrintInfo ();
		}

		public int PageCount
		{
			get; set;
		}
	}
}

