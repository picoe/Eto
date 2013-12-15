using System;
using Eto.Forms;
using System.Linq;

namespace Eto.Platform.GtkSharp
{
	public abstract class MenuHandler<TControl, TWidget> : WidgetHandler<TControl, TWidget>, IMenu
		where TWidget: InstanceWidget
	{
		protected void ValidateItems()
		{
			var subMenu = Widget as IMenuItemsSource;
			if (subMenu != null) {
				foreach (var item in subMenu.Items) {
					item.OnValidate(EventArgs.Empty);
				}
			}
		}
	}
}
