using System;
using System.Collections;

namespace Eto.Forms.WXWidgets
{
	/// <summary>
	/// Summary description for MenuBarHandler.
	/// </summary>
	public class MenuBarHandler : MenuHandler, IMenuBar
	{
		wx.MenuBar control = null;
		ArrayList menuItems = new ArrayList();

		public MenuBarHandler(Widget widget) : base(widget)
		{
		}

		public override object ControlObject
		{
			get { return control; }
		}

		public wx.MenuBar WXMenuBar
		{
			get { return control; }
		}

		public wx.MenuBar CreateMenu()
		{
			control = new wx.MenuBar();

			foreach (MenuItem item in menuItems)
			{
				wx.MenuItem mi = ((MenuItemHandler)item.InnerControl).CreateMenu(null);
				if (mi.IsSubMenu) control.Append(mi.SubMenu, item.Text);
			}

			return control;
		}

		public override void AddMenu(int index, MenuItem item)
		{
			if (control == null) menuItems.Add(item);
			else
			{
				wx.MenuItem menu = ((MenuItemHandler)item.InnerControl).CreateMenu(null);
				if (menu.IsSubMenu) control.Insert(index, menu.SubMenu, item.Text);
			}
		}

		public override void RemoveMenu(MenuItem item)
		{
			throw new NotImplementedException("Cannot remove menu items in wxWidgets");
		}


	}
}
