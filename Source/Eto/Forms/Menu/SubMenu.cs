using System;
using System.Collections.Generic;

namespace Eto.Forms
{
	public interface ISubMenu : IMenu
	{
		void AddMenu (int index, MenuItem item);

		void RemoveMenu (MenuItem item);

		void Clear ();
	}
	
	public interface ISubMenuWidget : IControlObjectSource, IHandlerSource, IGeneratorSource
	{
		MenuItemCollection MenuItems { get; }
		
#if MENU_TOOLBAR_REFACTORING
		void GenerateActions (IEnumerable<IActionItem> actionItems);
#endif
	}

	public static class SubMenuWidgetExtensions
	{
		public static ImageMenuItem GetSubmenu(this ISubMenuWidget menu,
			string subMenuText, int order = 500, bool plaintextMatch = false, bool create = true)
		{
			// replace accelerators if plaintextMatch is true
			Func<string, string> convert = s => plaintextMatch ? s.Replace("&", "") : s;

			foreach (var item in menu.MenuItems)
			{
				if (convert(item.Text) == convert(subMenuText))
				{
					return item as ImageMenuItem;
				}
			}
			if (create)
			{
				var subMenu = new ImageMenuItem() { Text = subMenuText };
				subMenu.Order = order;
				menu.MenuItems.Add(subMenu);
				return subMenu;
			}
			return null;
		}
	}
}

