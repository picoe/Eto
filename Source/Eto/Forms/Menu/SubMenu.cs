using System;
using System.Collections.Generic;
using System.Linq;

namespace Eto.Forms
{
	public interface ISubMenu : IMenu
	{
		void AddMenu (int index, MenuItem item);

		void RemoveMenu (MenuItem item);

		void Clear ();
	}
	
	public interface ISubMenuWidget
	{
		MenuItemCollection Items { get; }
	}
}

