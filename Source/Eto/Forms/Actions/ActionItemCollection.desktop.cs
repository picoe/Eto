#if DESKTOP
using System;
using System.Collections.Generic;

namespace Eto.Forms
{
	public partial class ActionItemCollection
	{
		
		public MenuBar GenerateMenuBar()
		{
			var menu = new MenuBar(Generator);
			Generate (menu);
			return menu;
		}
		
		public void Generate(ISubMenuWidget menu)
		{
			var list = new List<IActionItem>(this);
			list.Sort(Compare);
			var lastSeparator = false;
			for (int i = 0; i < list.Count; i++)
			{
				var ai = list[i];
				var isSeparator = (ai is ActionItemSeparator);
				
				if ((lastSeparator && isSeparator) || (isSeparator && (i == 0 || i == list.Count - 1)))
					continue;
				var mi = ai.Generate(menu.Generator);
				if (mi != null)
					menu.Add(mi);
				lastSeparator = isSeparator;	
			}
		}
	}
}
#endif
