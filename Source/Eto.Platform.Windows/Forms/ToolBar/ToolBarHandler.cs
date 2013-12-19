using SD = System.Drawing;
using SWF = System.Windows.Forms;
using Eto.Forms;
using System;

namespace Eto.Platform.Windows
{
	public class ToolBarHandler : WidgetHandler<SWF.ToolStrip, ToolBar>, IToolBar
	{
		ToolBarDock dock = ToolBarDock.Top;

		public ToolBarHandler()
		{
			Control = new SWF.ToolStrip();
			Control.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.StackWithOverflow;
			Control.AutoSize = true;
		}

		public ToolBarDock Dock
		{
			get { return dock; }
			set { dock = value; }
		}

		public void AddButton(ToolItem item)
		{
			((IToolBarItemHandler)item.Handler).CreateControl(this);
		}

		public void RemoveButton(ToolItem item)
		{
			Control.Items.Remove((SWF.ToolStripItem)item.ControlObject);
		}

		public ToolBarTextAlign TextAlign
		{
			get
			{
				/*switch (control.TextAlign)
				{
					case SWF.ToolBarTextAlign.Right:
						return ToolBarTextAlign.Right;
					default:
					case SWF.ToolBarTextAlign.Underneath:
						return ToolBarTextAlign.Underneath;
				}
				 */
				return ToolBarTextAlign.Underneath;
			}
			set
			{
				switch (value)
				{
					case ToolBarTextAlign.Right:
						//control.TextAlign = SWF.ToolBarTextAlign.Right;
						break;
					case ToolBarTextAlign.Underneath:
						//control.TextAlign = SWF.ToolBarTextAlign.Underneath;
						break;
					default:
						throw new NotSupportedException();
				}
			}
		}

		public void Clear()
		{
			Control.Items.Clear();
		}
	}
}
