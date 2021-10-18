using swf = System.Windows.Forms;
using sdp = System.Drawing.Printing;
using Eto.Forms;

namespace Eto.WinForms.Forms.Printing
{
	public class PrintDialogHandler : WidgetHandler<swf.PrintDialog, PrintDialog>, PrintDialog.IHandler
	{
		PrintSettings _printSettings;

		public PrintDialogHandler()
		{
			Control = new swf.PrintDialog
			{
				UseEXDialog = true,
				AllowSomePages = true,
				PrinterSettings = PrintSettingsHandler.DefaultSettings()
			};
		}

		public PrintDocument Document { get; set; }

		public DialogResult ShowDialog(Window parent)
		{
			swf.DialogResult result;

			Control.PrinterSettings = _printSettings.ToSD();

			if (parent != null)
				result = Control.ShowDialog(((IWindowHandler)parent.Handler).Win32Window);
			else
				result = Control.ShowDialog();

			if (result == swf.DialogResult.OK && Document != null)
			{
				Document.PrintSettings = _printSettings;
				Document.Print();
			}

			return result.ToEto();
		}

		public PrintSettings PrintSettings
		{
			get => _printSettings ?? (_printSettings = Control.PrinterSettings.ToEto());
			set
			{
				_printSettings = value;
				Control.PrinterSettings = value.ToSD();
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
