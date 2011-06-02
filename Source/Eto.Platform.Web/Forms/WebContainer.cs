using System;

using Eto.Forms;

using SWUC = System.Web.UI.WebControls;
using SWUH = System.Web.UI.HtmlControls;
using SWU = System.Web.UI;

namespace Eto.Platform.Web.Forms
{
	public abstract class WebContainer : WebControl, IContainer
	{
		Layout layout = null;
		
		public WebContainer(Widget widget) : base(widget)
		{
		}
		
		public SWU.Control WebContainerObject
		{
			get
			{ return (SWU.Control)ContainerObject; }
		}
		
		#region IContainer Members
		
		public virtual object ContainerObject
		{
			get
			{ return ControlObject; }
		}
		
		public Layout Layout
		{
			get
			{ return layout; }
			set
			{
				layout = value;
				((WebLayout)layout.InnerControl).Initialize(this);
			}
		}
		
		#endregion
		
	}
}

