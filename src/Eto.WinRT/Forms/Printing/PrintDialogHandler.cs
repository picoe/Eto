#if TODO_XAML
using Eto.Forms;
using sw = Windows.UI.Xaml;
using swc = Windows.UI.Xaml.Controls;

namespace Eto.WinRT.Forms.Printing
{
	public class PrintDialogHandler : WidgetHandler<swc.PrintDialog, PrintDialog>, IPrintDialog
	{
		PrintSettings settings;
		public PrintDialogHandler()
		{
			Control = new swc.PrintDialog
			{
				UserPageRangeEnabled = true
			};
		}

		public PrintDocument Document { get; set; }

		public DialogResult ShowDialog(Window parent)
		{
			Control.SetEtoSettings(settings);
			var result = Control.ShowDialog();
			if (result == true)
			{
				settings.SetFromDialog(Control);
				return DialogResult.Ok;
			}
			return DialogResult.Cancel;
		}

		public PrintSettings PrintSettings
		{
			get
			{
				if (settings == null)
					settings = Control.GetEtoSettings(Widget.Generator);
				return settings;
			}
			set
			{
				settings = value;
				Control.SetEtoSettings(settings);
			}
		}

		public bool AllowPageRange
		{
			get { return Control.UserPageRangeEnabled; }
			set { Control.UserPageRangeEnabled = value; }
		}

		// not supported in wpf
		public bool AllowSelection
		{
			get;
			set;
		}
	}
}
#endif