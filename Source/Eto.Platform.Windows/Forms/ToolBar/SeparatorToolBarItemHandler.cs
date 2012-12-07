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
                if (this.Control.AutoSize) return SeparatorToolBarItemType.Divider;
                else return SeparatorToolBarItemType.FlexibleSpace;
			}
			set {
				switch (value)
				{
					case SeparatorToolBarItemType.FlexibleSpace:
						this.Control.AutoSize = false;
						break;
                    case SeparatorToolBarItemType.Space:
                        this.Control.AutoSize = false;
                        break;
				default:
					this.Control.AutoSize = true;
					break;
				}
			}
		}
		
		public void CreateControl (ToolBarHandler handler)
		{
			handler.Control.Items.Add(Control);
		}

	}
}
