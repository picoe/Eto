using System;
using System.Collections;
using System.Collections.Generic;
using Eto.Drawing;
using System.Collections.ObjectModel;

namespace Eto.Forms
{
	public interface IMenuItem : IMenu
	{
	}
	
	public class MenuItem : Menu
	{
		public MenuItem(Generator g, Type type) : base(g, type)
		{
		}
	}

	public class MenuItemCollection : Collection<MenuItem>
	{
		ISubMenu subMenu;
		
		public ISubMenuWidget Parent
		{
			get; private set;
		}
		
		public MenuItemCollection(ISubMenuWidget parent, ISubMenu parentMenu)
		{
			this.Parent = parent;
			this.subMenu = parentMenu;
		}
		
		protected override void InsertItem (int index, MenuItem item)
		{
			base.InsertItem (index, item);
			subMenu.AddMenu(index, item);
		}
		
		protected override void RemoveItem (int index)
		{
			var item = this[index];
			base.RemoveItem (index);
			subMenu.RemoveMenu(item);
		}
		
		protected override void ClearItems ()
		{
			base.ClearItems ();
			subMenu.Clear();
		}
	}
}
