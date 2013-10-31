using SD = System.Drawing;
using SWF = System.Windows.Forms;
using Eto.Forms;

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

		#region IToolBar Members

		public ToolBarDock Dock
		{
			get { return dock; }
			set { dock = value; }
		}
		
		public void AddButton(ToolBarItem item)
		{
			((IToolBarItemHandler)item.Handler).CreateControl(this);
		}

		public void RemoveButton(ToolBarItem item)
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
					default:
					case ToolBarTextAlign.Underneath:
						//control.TextAlign = SWF.ToolBarTextAlign.Underneath;
						break;
				}
			}
		}

		public void Clear()
		{
			Control.Items.Clear();
		}
		#endregion

	}
}
