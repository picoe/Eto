using System;

using Eto.Forms;

using SWUC = System.Web.UI.WebControls;
using SWU = System.Web.UI;

namespace Eto.Platform.Web.Forms
{
	public class PanelHandler : WebContainer, IPanel
	{
		SWUC.WebControl control;
		
		public PanelHandler(Widget widget) : base(widget)
		{
			control = new SWUC.WebControl(SWU.HtmlTextWriterTag.Div);
			control.Style["OVERFLOW"] = "auto";
		}
		
		public override object ControlObject
		{
			get
			{ return control; }
		}
		
	}
}

