using System;
using System.Collections;
using System.Collections.Generic;

namespace Eto.Forms
{
	public interface IMenuBar : IMenu, ISubMenu
	{

	}
	
	public class MenuBar : Menu, ISubMenuWidget
	{
		IMenuBar handler;
		MenuItemCollection menuItems;
		
		public MenuBar () : this (Generator.Current)
		{
		}

		public MenuBar (Generator g) : this (g, typeof(IMenuBar))
		{
		}
		
		protected MenuBar (Generator generator, Type type, bool initialize = true)
			: base (generator, type, initialize)
		{
			handler = (IMenuBar)base.Handler;
			menuItems = new MenuItemCollection (this, handler);
		}

		public MenuBar (Generator g, IEnumerable<IActionItem> actionItems) : this(g)
		{
			GenerateActions (actionItems);
		}
		
		public void GenerateActions (IEnumerable<IActionItem> actionItems)
		{
			foreach (IActionItem ai in actionItems) {
				ai.Generate (this);
			}
		}

		public MenuItemCollection MenuItems {
			get { return menuItems; }
		}

		IWidget ISubMenuWidget.Handler { get { return this.Handler; } }

	}
}
