using System;
using Eto.Forms;

namespace Eto.Platform.GtkSharp
{
	/// <summary>
	/// Summary description for MenuBarHandler.
	/// </summary>
	public abstract class MenuHandler<T, W> : WidgetHandler<T, W>, IMenu, IWidget
		where W: Widget
	{

		#region IMenu Members

		public abstract void AddMenu(int index, MenuItem item);

		public abstract void RemoveMenu(MenuItem item);

		public abstract void Clear();
		
		#endregion
	}
}
