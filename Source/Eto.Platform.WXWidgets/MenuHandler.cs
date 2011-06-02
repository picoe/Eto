using System;

namespace Eto.Forms.WXWidgets
{
	/// <summary>
	/// Summary description for MenuBarHandler.
	/// </summary>
	public abstract class MenuHandler : IMenu, IWidget
	{
		Widget widget;

		public MenuHandler(Widget widget)
		{
			this.widget = widget;
		}

		protected Widget Widget
		{
			get { return widget; }
		}

		#region IMenu Members

		public abstract void AddMenu(int index, MenuItem item);

		public abstract void RemoveMenu(MenuItem item);
		#endregion

		#region IWidget Members

		public void Initialize()
		{
		}

		public abstract object ControlObject { get; }

		#endregion
	}
}
