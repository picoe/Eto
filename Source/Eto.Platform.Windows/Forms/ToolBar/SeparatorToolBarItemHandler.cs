using System;
using System.Collections;
using System.Reflection;
using SD = System.Drawing;
using SWF = System.Windows.Forms;
using Eto.Drawing;
using Eto.Forms;

namespace Eto.Platform.Windows
{
	public class SeparatorToolBarItemHandler : WidgetHandler<SWF.ToolStripSeparator, SeparatorToolBarItem>, ISeparatorToolBarItem, IToolBarItemHandler
	{
		public SeparatorToolBarItemHandler()
		{
			Control = new SWF.ToolStripSeparator();
		}
	
		public SeparatorToolBarItemType Type {
			get {
				if (this.Control.AutoSize) return SeparatorToolBarItemType.FlexibleSpace;
				else return SeparatorToolBarItemType.Divider;
			}
			set {
				switch (value)
				{
					case SeparatorToolBarItemType.FlexibleSpace:
						this.Control.AutoSize = true;
						break;
				default:
					this.Control.AutoSize = false;
					break;
				}
			}
		}
		
		public void CreateControl (ToolBarHandler handler)
		{
			
		}

	}
}
