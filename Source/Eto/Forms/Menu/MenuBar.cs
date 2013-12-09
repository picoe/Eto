using System;
using System.Collections.Generic;

namespace Eto.Forms
{
	public interface IMenuBar : IMenu, ISubMenu
	{

	}
	
	public class MenuBar : Menu, ISubMenuWidget
	{
		new IMenuBar Handler { get { return (IMenuBar)base.Handler; } }

		readonly MenuItemCollection menuItems;
		
		public MenuBar()
			: this((Generator)null)
		{
		}

		public MenuBar (Generator generator) : this (generator, typeof(IMenuBar))
		{
		}
		
		protected MenuBar (Generator generator, Type type, bool initialize = true)
			: base (generator, type, initialize)
		{
			menuItems = new MenuItemCollection (this, Handler);
		}

		public MenuBar (Generator g, IEnumerable<MenuItem> actionItems) : this(g)
		{
			GenerateActions (actionItems);
		}

		public void GenerateActions(IEnumerable<MenuItem> actionItems)
		{
			foreach (var mi in actionItems) {
				this.Add(mi);
			}
		}

		/// <summary>
		/// Adds a menu item based on its Order.
		/// </summary>
		/// <param name="menuItem"></param>
		public void Add(MenuItem menuItem)
		{
			ImageMenuItem.AddMenuItem(this.menuItems, menuItem);
		}

		public void Remove(MenuItem menuItem)
		{
			this.menuItems.Remove(menuItem);
		}

		public IEnumerable<MenuItem> MenuItems
		{
			get
			{
				foreach (var m in this.menuItems)
					yield return m;
			}
		}
	}
}