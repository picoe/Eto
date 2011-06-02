using System;
using System.Collections;
using System.Reflection;

namespace Eto.Forms.WXWidgets
{
	/// <summary>
	/// Summary description for MenuBarHandler.
	/// </summary>
	public class MenuItemHandler : MenuHandler, IMenuItem
	{
		string text = string.Empty;
		ArrayList menuItems = null;
		wx.MenuItem control = null;

		public MenuItemHandler(Widget widget) : base(widget) 
		{
		}

		public override object ControlObject
		{
			get { return control; }
		}

		private void control_Click(object sender, EventArgs e)
		{
			((MenuItem)Widget).OnClick(e);
		}

		public wx.MenuItem CreateMenu(wx.Menu parent)
		{
			wx.Menu subMenu = null;
			if (menuItems != null && menuItems.Count > 0)
			{
				subMenu = new wx.Menu();
				foreach (MenuItem mi in menuItems)
				{
					((MenuItemHandler)mi.InnerControl).CreateMenu(subMenu);
				}
			}

			if (text == "-") 
			{
				if (parent != null) parent.AppendSeparator();
			}
			else 
			{
				control = new wx.MenuItem(parent, ((WXGenerator)Widget.Generator).GetNextButtonID(), text, string.Empty, wx.ItemKind.wxITEM_NORMAL, subMenu);
				if (parent != null) parent.Append(control, new wx.EventListener(item_Click));
			}

			return control;
		}

		public override void AddMenu(int index, MenuItem item)
		{
			if (menuItems == null) menuItems = new ArrayList();
			if (control == null) this.menuItems.Add(item);
			else
			{
				wx.MenuItem mi = ((MenuItemHandler)item.InnerControl).CreateMenu(control.SubMenu);
				control.SubMenu.Append(mi);
			}
		}

		public override void RemoveMenu(MenuItem item)
		{
			// TODO: implement removing a sub menu item
		}

		private void item_Click(object sender, wx.Event e)
		{
			((MenuItem)Widget).OnClick(EventArgs.Empty);
		}

		#region IMenuItem Members

		public string Text
		{
			get	{ return text; }
			set { text = value; }
		}

		#endregion
	}
}
