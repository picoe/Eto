using System;
using Eto.Forms;
using MonoMac.AppKit;

namespace Eto.Platform.Mac
{
	public class MenuBarHandler : WidgetHandler<NSMenu, Menu>, IMenuBar, IMenu
	{

		public MenuBarHandler ()
		{
			Control = new NSMenu ();
			Control.AutoEnablesItems = true;
			Control.ShowsStateColumn = true;
		}

		public void AddMenu (int index, MenuItem item)
		{
			Control.InsertItematIndex ((NSMenuItem)item.ControlObject, index);
		}

		public void RemoveMenu (MenuItem item)
		{
			Control.RemoveItem ((NSMenuItem)item.ControlObject);
		}

		public void Clear ()
		{
			Control.RemoveAllItems ();
		}

	}
}
