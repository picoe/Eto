namespace Eto.Wpf.Forms.Printing
{
	public class PrintDialogHandler : WidgetHandler<swc.PrintDialog, PrintDialog>, PrintDialog.IHandler
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
			if (parent?.HasFocus == false)
				parent.Focus();

			Control.SetEtoSettings(settings);
			var result = Control.ShowDialog();
			WpfFrameworkElementHelper.ShouldCaptureMouse = false;
			if (result == true)
			{
				settings.SetFromDialog(Control);
				Document?.Print();
				return DialogResult.Ok;
			}
			return DialogResult.Cancel;
		}

		public PrintSettings PrintSettings
		{
			get
			{
				if (settings == null)
					settings = Control.GetEtoSettings();
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
