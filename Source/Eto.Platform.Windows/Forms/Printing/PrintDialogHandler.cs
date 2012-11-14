using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using swf = System.Windows.Forms;
using Eto.Forms;

namespace Eto.Platform.Windows.Forms.Printing
{
	public class PrintDialogHandler : WidgetHandler<swf.PrintDialog, PrintDialog>, IPrintDialog
	{
		PrintSettings printSettings;

		public PrintDialogHandler ()
		{
			Control = new swf.PrintDialog {
				UseEXDialog = true,
				AllowSomePages = true
			};
		}

		public DialogResult ShowDialog (Window parent)
		{
			swf.DialogResult result;

			if (parent != null)
				result = Control.ShowDialog (((IWindowHandler)parent.Handler).Win32Window);
			else
				result = Control.ShowDialog ();

			return result.ToEto ();
		}

		public PrintSettings PrintSettings
		{
			get { return printSettings; }
			set
			{
				printSettings = value;
				Control.PrinterSettings = printSettings == null ? null : ((PrintSettingsHandler)printSettings.Handler).Control;
			}
		}

		public bool AllowPageRange
		{
			get { return Control.AllowSomePages; }
			set { Control.AllowSomePages = value; }
		}

		public bool AllowSelection
		{
			get { return Control.AllowSelection; }
			set { Control.AllowSelection = value; }
		}
	}
}
