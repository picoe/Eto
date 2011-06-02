using System;

using Eto.Forms;

using SWUC = System.Web.UI.WebControls;
using SWUH = System.Web.UI.HtmlControls;
using SWU = System.Web.UI;

namespace Eto.Platform.Web.Forms
{
	public class TabPageHandler : WebContainer, ITabPage
	{
		//private WebControls.TabItem control = null;
		private SWUC.PlaceHolder container;
		
		public TabPageHandler(Widget widget) : base(widget)
		{
			//control = new WebControls.Tab();
			container = new SWUC.PlaceHolder();
		}
		
		public override object ControlObject
		{
			get { return null; }
		}
		
		public override object ContainerObject
		{
			get { return container; }
		}
		
		public override string Text
		{
			get { return string.Empty; }
			set { }
		}
	}
}

