using SD = System.Drawing;
using SWF = System.Windows.Forms;
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
					case SeparatorToolBarItemType.Divider:
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
			handler.Control.Items.Add(Control);
		}

	}
}
