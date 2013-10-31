#if DESKTOP
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
		
		public MenuBar () : this (Generator.Current)
		{
		}

		public MenuBar (Generator g) : this (g, typeof(IMenuBar))
		{
		}
		
		protected MenuBar (Generator generator, Type type, bool initialize = true)
			: base (generator, type, initialize)
		{
			menuItems = new MenuItemCollection (this, Handler);
		}

		public MenuBar (Generator g, IEnumerable<IActionItem> actionItems) : this(g)
		{
			GenerateActions (actionItems);
		}
		
		public void GenerateActions (IEnumerable<IActionItem> actionItems)
		{
			foreach (IActionItem ai in actionItems) {
				var mi = ai.Generate (Generator);
				if (mi != null)
					MenuItems.Add(mi);
			}
		}

		public MenuItemCollection MenuItems {
			get { return menuItems; }
		}
	}
}
#endif