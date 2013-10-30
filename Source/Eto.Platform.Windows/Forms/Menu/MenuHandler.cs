using SWF = System.Windows.Forms;
using Eto.Forms;

namespace Eto.Platform.Windows
{
	public abstract class MenuHandler<T, W> : WidgetHandler<T, W>, IMenu
		where T: SWF.ToolStripItem
		where W: InstanceWidget
	{

		public override void AttachEvent (string id)
		{
			switch (id) {
			case MenuActionItem.ValidateEvent:
				// handled by parents
				break;
			default:
				base.AttachEvent (id);
				break;
			}
		}

	}
}
