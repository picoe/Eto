using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using swc = System.Windows.Controls;
using Eto.Forms;

namespace Eto.Platform.Wpf.Forms.Menu
{
	public class SeparatorMenuItemHandler : WidgetHandler<swc.Separator, SeparatorMenuItem>, ISeparatorMenuItem
	{
		public SeparatorMenuItemHandler ()
		{
			Control = new swc.Separator ();
		}
	}
}
