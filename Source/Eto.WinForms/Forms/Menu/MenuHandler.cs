using SWF = System.Windows.Forms;
using Eto.Forms;

namespace Eto.WinForms
{
	public abstract class MenuHandler<TControl, TWidget> : WidgetHandler<TControl, TWidget>, IMenu
		where TControl: SWF.ToolStripItem
		where TWidget: InstanceWidget
	{

		public override void AttachEvent (string id)
		{
			switch (id) {
			case MenuItem.ValidateEvent:
				// handled by parents
				break;
			default:
				base.AttachEvent (id);
				break;
			}
		}

		public void CreateFromCommand(Command command)
		{
		}
	}
}
