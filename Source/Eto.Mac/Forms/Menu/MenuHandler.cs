using Eto.Forms;
using MonoMac.AppKit;
using Eto.Mac.Forms.Actions;

namespace Eto.Mac
{
	public interface IMenuHandler
	{
		void EnsureSubMenu();
	}

	public abstract class MenuHandler<TControl, TWidget, TCallback> : WidgetHandler<TControl, TWidget, TCallback>, Menu.IHandler, IMenuHandler
		where TControl: NSMenuItem
		where TWidget: Menu
		where TCallback: Menu.ICallback
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

		public void CreateFromCommand(Command command)
		{
			var m = command as MacCommand;
			if (m != null)
			{
				Control.Target = null;
				Control.Action = m.Selector;
			}
		}
	}
}
