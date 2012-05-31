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
	
	public interface ISubMenuWidget
	{
		Generator Generator { get; }
		
		IWidget Handler { get; }
		
		object ControlObject { get; }
		
		MenuItemCollection MenuItems { get; }
		
		void GenerateActions (IEnumerable<IActionItem> actionItems);
	}
}

