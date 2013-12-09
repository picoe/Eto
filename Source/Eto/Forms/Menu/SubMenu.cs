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
		void Add(MenuItem menuItem);
		void Remove(MenuItem menuItem);
		IEnumerable<MenuItem> MenuItems { get; }		
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
				menu.Add(subMenu);
				return subMenu;
			}
			return null;
		}
	}
}

