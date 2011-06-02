using System;

using Eto.Forms;

using SWUC = System.Web.UI.WebControls;
using SWUH = System.Web.UI.HtmlControls;
using SWU = System.Web.UI;

namespace Eto.Platform.Web.Forms
{
	public class MenuBarHandler : MenuHandler, IMenuBar
	{
		
		public override void Clear()
		{
			// TODO: Implement this method
		}
		
		public override void SetWidget(Widget widget)
		{
			// TODO: Implement this method
		}
		
		public override bool HandleEvent(String handler)
		{
			// TODO: Implement this method
			return false;
		}
		
		SWUC.WebControl control = null;
		
		public MenuBarHandler(Widget widget) : base(widget)
		{
			//control = new SWF.MainMenu();
		}
		
		public override object ControlObject
		{
			get
			{ return control; }
		}
		
	}
}

