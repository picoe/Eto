using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Eto.Forms;
using swc = System.Windows.Controls;

namespace Eto.Platform.Wpf.Forms
{
	public class SeparatorToolBarItemHandler : ToolBarItemHandler<swc.ContentControl, SeparatorToolBarItem>, ISeparatorToolBarItem
	{
		class EtoSpaceSeparator : swc.Control
		{
			public EtoSpaceSeparator ()
			{
				this.Width = this.Height = 16;
			}
		}

		public SeparatorToolBarItemHandler ()
		{
			Control = new swc.ContentControl();
			Type = SeparatorToolBarItemType.Divider;
		}

		public SeparatorToolBarItemType Type
		{
			get
			{
				var control = Control.Content;
				if (control is swc.Separator) return SeparatorToolBarItemType.Divider;
				else if (control is EtoSpaceSeparator) return SeparatorToolBarItemType.Space;
				else return SeparatorToolBarItemType.FlexibleSpace;
			}
			set
			{
				swc.Control control;
				switch (value) {
					case SeparatorToolBarItemType.Divider:
						control = new swc.Separator ();
						break;
					case SeparatorToolBarItemType.FlexibleSpace:
					case SeparatorToolBarItemType.Space:
						control = new EtoSpaceSeparator ();
						break;
					default:
						throw new NotSupportedException();
				}
				Control.Content = control;
			}
		}
	}
}
