using Eto.Forms;
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

namespace Eto.Mac
{
	public class MenuBarHandler : WidgetHandler<NSMenu, Menu>, MenuBar.IHandler
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
