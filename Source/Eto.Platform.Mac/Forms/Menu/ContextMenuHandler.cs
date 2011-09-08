using System;
using Eto.Forms;
using MonoMac.AppKit;

namespace Eto.Platform.Mac.Forms.Menu
{
	public class ContextMenuHandler : WidgetHandler<NSMenu, ContextMenu>, IContextMenu
	{
		public ContextMenuHandler ()
		{
			Control = new NSMenu ();
			Control.AutoEnablesItems = false;
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

