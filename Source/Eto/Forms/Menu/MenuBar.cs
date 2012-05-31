using System;
using System.Collections;

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

		public MenuBar (Generator g, ActionItemCollection actionItems) : this(g)
		{
			GenerateActions (actionItems);
		}
		
		public void GenerateActions (ActionItemCollection actionItems)
		{
			foreach (IActionItem ai in actionItems) {
				ai.Generate (this);
			}
		}

		public MenuItemCollection MenuItems {
			get { return menuItems; }
		}
	}
}
