using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Eto.Forms;
using sw = System.Windows;
using swc = System.Windows.Controls;

namespace Eto.Platform.Wpf.Forms.Printing
{
	public class PrintDialogHandler : WidgetHandler<swc.PrintDialog, PrintDialog>, IPrintDialog
	{
		public PrintDialogHandler ()
		{
			Control = new swc.PrintDialog ();
		}

		public DialogResult ShowDialog (Window parent)
		{
			var result = Control.ShowDialog ();
			return result == true ? DialogResult.Ok : DialogResult.Cancel;
		}

		public PrintSettings PrintSettings
		{
			get
			{
				return null;
			}
			set
			{
			}
		}

		public bool AllowPageRange
		{
			get { return Control.UserPageRangeEnabled; }
			set { Control.UserPageRangeEnabled = value; }
		}

		public bool AllowSelection
		{
			get; set;
		}
	}
}
