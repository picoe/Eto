using System;

using Eto.Forms;

using SWUC = System.Web.UI.WebControls;
using SWUH = System.Web.UI.HtmlControls;
using SWU = System.Web.UI;

namespace Eto.Platform.Web.Forms
{
	public abstract class MenuHandler : IMenu
	{
		
		public virtual void Clear()
		{
			// TODO: Implement this method
		}
		
		public virtual void SetWidget(Widget widget)
		{
			// TODO: Implement this method
		}
		
		public virtual bool HandleEvent(String handler)
		{
			// TODO: Implement this method
			return false;
		}
		
		Widget widget;
		
		public MenuHandler(Widget widget)
		{
			this.widget = widget;
		}
		
		protected Widget Widget
		{
			get
			{ return widget; }
		}
		
		/*protected SWF.Menu Menu
		 {
		 get { return (SWF.Menu)ControlObject; }
		 }  */
		
		#region IMenu Members
		
		public void AddMenu(int index, MenuItem item)
		{
			//Menu.MenuItems.Add(index, (SWF.MenuItem)item.ControlObject);
		}
		
		public void RemoveMenu(MenuItem item)
		{
			//Menu.MenuItems.Remove((SWF.MenuItem)item.ControlObject);
		}
		#endregion
		
		#region IWidgetBase Members
		
		public void Initialize()
		{
		}
		
		public abstract object ControlObject
		{ get; }
		
		#endregion
	}
}

