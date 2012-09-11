using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sdp = System.Drawing.Printing;
using swf = System.Windows.Forms;
using Eto.Forms;

namespace Eto.Platform.Windows.Forms.Printing
{
	public class PrintSettingsHandler : WidgetHandler<sdp.PrinterSettings, PrintSettings>, IPrintSettings
	{
		public PrintSettingsHandler ()
		{
			Control = new sdp.PrinterSettings ();
		}
	}
}
