#if DESKTOP
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
		IContextMenu handler;
		MenuItemCollection menuItems;
		
		public ContextMenu () : this (Generator.Current)
		{
		}

		public ContextMenu (Generator g) : this (g, typeof(IContextMenu))
		{
		}

		protected ContextMenu (Generator generator, Type type, bool initialize = true)
			: base (generator, type, initialize)
		{
			handler = (IContextMenu)this.Handler;
			menuItems = new MenuItemCollection (this, handler);
		}

		public ContextMenu (Generator g, IEnumerable<IActionItem> actionItems) : this (g)
		{
			GenerateActions (actionItems);
		}
		
		public void GenerateActions (IEnumerable<IActionItem> actionItems)
		{
			foreach (var ai in actionItems) {
				ai.Generate (this);
			}
		}

		public MenuItemCollection MenuItems {
			get { return menuItems; }
		}
		
		public void Show (Control relativeTo)
		{
			handler.Show (relativeTo);
		}
	}
}
#endif