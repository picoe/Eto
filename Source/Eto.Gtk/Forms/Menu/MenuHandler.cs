using System;
using Eto.Forms;
using System.Linq;

namespace Eto.GtkSharp
{
	public abstract class MenuHandler<TControl, TWidget, TCallback> : WidgetHandler<TControl, TWidget, TCallback>, Menu.IHandler
		where TWidget: Menu
		where TCallback : Menu.ICallback
	{
		protected void ValidateItems()
		{
			var subMenu = Widget as ISubmenu;
			if (subMenu != null)
			{
				foreach (var item in subMenu.Items)
				{
					var handler = item.Handler as IMenuActionItemHandler;
					if (handler != null)
						handler.TriggerValidate();
				}
			}
		}

	}
}
