using Eto.Forms;
using MonoMac.AppKit;

namespace Eto.Platform.Mac
{
	public interface IMenuHandler
	{
		void EnsureSubMenu();
	}

	public abstract class MenuHandler<TControl, TWidget> : WidgetHandler<TControl, TWidget>, IMenu, IMenuHandler
		where TControl: NSMenuItem
		where TWidget: Menu
	{
		public void EnsureSubMenu()
		{
			if (!Control.HasSubmenu)
				Control.Submenu = new NSMenu { AutoEnablesItems = true, ShowsStateColumn = true, Title = Control.Title };
		}

		public virtual void AddMenu(int index, MenuItem item)
		{
			EnsureSubMenu();
			Control.Submenu.InsertItem((NSMenuItem)item.ControlObject, index);
		}

		public virtual void RemoveMenu(MenuItem item)
		{
			if (Control.Submenu == null)
				return;
			Control.Submenu.RemoveItem((NSMenuItem)item.ControlObject);
		}

		public virtual void Clear()
		{
			Control.Submenu = null;
		}
	}
}
