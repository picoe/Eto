using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
#if XAML
using System.Windows.Markup;
#endif

namespace Eto.Forms
{
#if XAML
	[ContentProperty("Control")]
#endif
	public class DynamicControl : DynamicItem
	{
		public override Control Generate (DynamicLayout layout)
		{
			return Control;
		}

		public Control Control { get; set; }
	}
}
