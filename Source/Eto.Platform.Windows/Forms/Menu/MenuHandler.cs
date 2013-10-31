using SWF = System.Windows.Forms;
using Eto.Forms;

namespace Eto.Platform.Windows
{
	public abstract class MenuHandler<TControl, TWidget> : WidgetHandler<TControl, TWidget>, IMenu
		where TControl: SWF.ToolStripItem
		where TWidget: InstanceWidget
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
