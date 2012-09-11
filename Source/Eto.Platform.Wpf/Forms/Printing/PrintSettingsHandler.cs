using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using swd = System.Windows.Documents;
using swc = System.Windows.Controls;
using Eto.Forms;

namespace Eto.Platform.Wpf.Forms.Printing
{
	public class PrintSettingsHandler : WidgetHandler<object, PrintSettings>, IPrintSettings
	{
		public int PageCount
		{
			get; set;
		}
	}
}
