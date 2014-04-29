using Eto.Forms;
using MonoMac.AppKit;

namespace Eto.Mac
{
	public class MenuBarHandler : WidgetHandler<NSMenu, Menu>, IMenuBar, IMenu
	{

		public MenuBarHandler()
		{
			Control = new NSMenu();
			Control.AutoEnablesItems = false;
			Control.ShowsStateColumn = true;
		}

		public void AddMenu(int index, MenuItem item)
		{
			var itemHandler = item.Handler as IMenuHandler;
			if (itemHandler != null)
				itemHandler.EnsureSubMenu();
			Control.InsertItem((NSMenuItem)item.ControlObject, index);
		}

		public void RemoveMenu(MenuItem item)
		{
			Control.RemoveItem((NSMenuItem)item.ControlObject);
		}

		public void Clear()
		{
			Control.RemoveAllItems();
		}
	}
}
