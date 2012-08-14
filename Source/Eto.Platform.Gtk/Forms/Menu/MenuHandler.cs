using System;
using Eto.Forms;
using System.Linq;

namespace Eto.Platform.GtkSharp
{
	public abstract class MenuHandler<T, W> : WidgetHandler<T, W>, IMenu, IWidget
		where W: InstanceWidget
	{
		protected void ValidateItems()
		{
			var subMenu = Widget as ISubMenuWidget;
			if (subMenu != null) {
				foreach (var item in subMenu.MenuItems.OfType<MenuActionItem>()) {
					item.OnValidate(EventArgs.Empty);
				}
			}
		}
		

		#region IMenu Members

		public abstract void AddMenu(int index, MenuItem item);

		public abstract void RemoveMenu(MenuItem item);

		public abstract void Clear();
		
		#endregion
	}
}
