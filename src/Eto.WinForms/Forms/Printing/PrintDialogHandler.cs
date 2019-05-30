using swf = System.Windows.Forms;
using sdp = System.Drawing.Printing;
using Eto.Forms;

namespace Eto.WinForms.Forms.Printing
{
	public class PrintDialogHandler : WidgetHandler<swf.PrintDialog, PrintDialog>, PrintDialog.IHandler
	{
		PrintSettings printSettings;

		public PrintDialogHandler ()
		{
			Control = new swf.PrintDialog {
				UseEXDialog = true,
				AllowSomePages = true,
				PrinterSettings = PrintSettingsHandler.DefaultSettings ()
			};
		}

		public PrintDocument Document { get; set; }

		public DialogResult ShowDialog (Window parent)
		{
			swf.DialogResult result;

			Control.PrinterSettings = printSettings.ToSD ();

			if (parent != null)
				result = Control.ShowDialog (((IWindowHandler)parent.Handler).Win32Window);
			else
				result = Control.ShowDialog ();

			return result.ToEto ();
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
				Control.PrinterSettings = value.ToSD ();
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
