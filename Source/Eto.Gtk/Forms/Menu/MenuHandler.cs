using System;
using Eto.Forms;
using System.Linq;

namespace Eto.GtkSharp
{
	public abstract class MenuHandler<TControl, TWidget> : WidgetHandler<TControl, TWidget>, IMenu
		where TWidget: Widget
	{
		protected void ValidateItems()
		{
			var subMenu = Widget as ISubMenuWidget;
			if (subMenu != null)
			{
				foreach (var item in subMenu.Items)
				{
					item.OnValidate(EventArgs.Empty);
				}
			}
		}

	}
}
