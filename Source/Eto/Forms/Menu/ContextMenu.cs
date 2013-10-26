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
		new IContextMenu Handler { get { return (IContextMenu)base.Handler; } }
		readonly MenuItemCollection menuItems;
		
		public ContextMenu () : this (Generator.Current)
		{
		}

		public ContextMenu (Generator g) : this (g, typeof(IContextMenu))
		{
		}

		protected ContextMenu (Generator generator, Type type, bool initialize = true)
			: base (generator, type, initialize)
		{
			menuItems = new MenuItemCollection (this, Handler);
		}

		public ContextMenu (Generator g, IEnumerable<IActionItem> actionItems) : this (g)
		{
			GenerateActions (actionItems);
		}
		
		public void GenerateActions (IEnumerable<IActionItem> actionItems)
		{
			foreach (var ai in actionItems) {
				var mi = ai.Generate (Generator);
				if (mi != null)
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
#endif