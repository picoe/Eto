using System;
using SWF = System.Windows.Forms;
using Eto.Forms;

namespace Eto.Platform.Windows
{
	public abstract class MenuHandler<T, W> : WidgetHandler<T, W>, IWidget
		where T: SWF.ToolStripItem
		where W: InstanceWidget
	{

		public override void AttachEvent (string handler)
		{
			switch (handler) {
			case MenuActionItem.ValidateEvent:
				// handled by parents
				break;
			default:
				base.AttachEvent (handler);
				break;
			}
		}

	}
}
