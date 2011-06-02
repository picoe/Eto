using System;

using Eto.Forms;

using SWUC = System.Web.UI.WebControls;
using SWUH = System.Web.UI.HtmlControls;
using SWU = System.Web.UI;

namespace Eto.Platform.Web.Forms
{
	internal class DockLayoutHandler : WebLayout, IDockLayout
	{
		
		public void Add(Control control)
		{
			SWUC.WebControl ctl = ((WebControl)control.InnerControl).WebControlObject;
			ctl.Style["WIDTH"] = "100%";
			ctl.Style["HEIGHT"] = "100%";
			Container.WebContainerObject.Controls.Add(ctl);
		}
		
		public void Remove(Control control)
		{
			// TODO: Implement this method
		}
		
		SWUC.WebControl control;
		
		public DockLayoutHandler(Widget widget) : base(widget)
		{
			control = new SWUC.WebControl(SWU.HtmlTextWriterTag.Div);
			control.Style["WIDTH"] = "100%";
			control.Style["HEIGHT"] = "100%";
		}
		
		public override object ControlObject
		{
			get
			{ return control; }
		}
		
		public override void Initialize(WebContainer container)
		{
			base.Initialize(container);
			Container.WebContainerObject.Controls.Clear();
		}
	}
}

