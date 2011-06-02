using System;
using Eto.Forms;
using MonoMac.AppKit;

namespace Eto.Platform.Mac
{
	/// <summary>
	/// Summary description for MenuBarHandler.
	/// </summary>
	public abstract class MenuHandler<T, W> : WidgetHandler<T, W>, IWidget, IMenu
		where T: NSMenuItem
		where W: Menu
	{
		
		public MenuHandler()
		{
		}

		
		
		#region IMenu Members

		public virtual void AddMenu(int index, MenuItem item)
		{
			if (!Control.HasSubmenu) Control.Submenu = new NSMenu{ AutoEnablesItems = false, ShowsStateColumn = true, Title = Control.Title };
			Control.Submenu.InsertItematIndex((NSMenuItem)item.ControlObject, index);
			//control.DropDownItems.Insert(index, (SWF.ToolStripItem)item.ControlObject);
		}

		public virtual void RemoveMenu(MenuItem item)
		{
			if (Control.Submenu == null) return;
			Control.Submenu.RemoveItem((NSMenuItem)item.ControlObject);
		}

		public virtual void Clear()
		{
			Control.Submenu = null;
			//control.DropDownItems.Clear();
		}

		#endregion
		
	}
}
