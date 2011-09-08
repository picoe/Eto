using System;
using System.Collections;
using System.Collections.Generic;
using Eto.Drawing;
using Eto.Collections;

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


	public class MenuItemCollection : BaseList<MenuItem>
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

		protected override void OnAdded (ListEventArgs<MenuItem> e)
		{
			base.OnAdded (e);
			subMenu.AddMenu(IndexOf(e.Item), e.Item);
		}
		
		protected override void OnRemoved (ListEventArgs<MenuItem> e)
		{
			base.OnRemoved (e);
			subMenu.RemoveMenu(e.Item);
		}
		
		public override void Clear()
		{
			base.Clear ();
			subMenu.Clear();
		}
	}
}
