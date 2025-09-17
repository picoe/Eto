
namespace Eto.Wpf.Forms.Menu
{
	public class ButtonMenuItemHandler : MenuItemHandler<swc.MenuItem, ButtonMenuItem, ButtonMenuItem.ICallback>, ButtonMenuItem.IHandler
	{
		public ButtonMenuItemHandler ()
		{
			Control = new swc.MenuItem();
		}

		public ButtonMenuItemHandler(swc.MenuItem mi)
		{
			Control = mi;
		}

	}
}
