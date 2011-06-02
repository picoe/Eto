using System;
using System.Collections;
using System.Collections.Generic;

namespace Eto.Forms
{
	public partial class ActionItemCollection : List<IActionItem>
	{
		public void Generate(Menu menu)
		{
			var list = new List<IActionItem>(this);
			list.Sort(Compare);
			foreach (IActionItem ai in list)
			{
				ai.Generate(menu);
			}
		}
	}
}
