using Eto.Forms;
using Eto.Mac.Forms.Actions;
#if XAMMAC2
using AppKit;
using Foundation;
using CoreGraphics;
using ObjCRuntime;
using CoreAnimation;
#else
using MonoMac.AppKit;
using MonoMac.Foundation;
using MonoMac.CoreGraphics;
using MonoMac.ObjCRuntime;
using MonoMac.CoreAnimation;
#endif

namespace Eto.Mac.Forms.Menu
{
	public interface IMenuHandler
	{
		void EnsureSubMenu();
	}

	public abstract class MenuHandler<TControl, TWidget, TCallback> : WidgetHandler<TControl, TWidget, TCallback>, Eto.Forms.Menu.IHandler, IMenuHandler
		where TControl: NSMenuItem
		where TWidget: Eto.Forms.Menu
		where TCallback: Eto.Forms.Menu.ICallback
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
