using System;
using System.Collections.Generic;

namespace Eto.Forms
{
	public interface IContextMenu : ISubMenu
	{
		void Show (Control relativeTo);	
	}
	
	public class ContextMenu : Menu, ISubMenuWidget
	{
		new IContextMenu Handler { get { return (IContextMenu)base.Handler; } }
		readonly MenuItemCollection menuItems;
		
		public ContextMenu()
			: this((Generator)null)
		{
		}

		public ContextMenu (Generator generator) : this (generator, typeof(IContextMenu))
		{
		}

		protected ContextMenu (Generator generator, Type type, bool initialize = true)
			: base (generator, type, initialize)
		{
			menuItems = new MenuItemCollection (this, Handler);
		}

		public ContextMenu (Generator g, IEnumerable<MenuItem> actionItems) : this (g)
		{
			GenerateActions (actionItems);
		}
		
		public void GenerateActions (IEnumerable<MenuItem> actionItems)
		{
			foreach (var mi in actionItems) {
				menuItems.Add(mi);
			}
		}

		public MenuItemCollection MenuItems {
			get { return menuItems; }
		}
		
		public void Show (Control relativeTo)
		{
			Handler.Show (relativeTo);
		}
	}
}