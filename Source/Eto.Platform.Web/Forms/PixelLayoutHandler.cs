using System;

using Eto.Forms;

using SWUC = System.Web.UI.WebControls;
using SWUH = System.Web.UI.HtmlControls;
using SWU = System.Web.UI;

namespace Eto.Platform.Web.Forms
{
	internal class PixelLayoutHandler : WebLayout, IPixelLayout
	{
		
		SWUC.WebControl control;
		
		public PixelLayoutHandler(Widget widget) : base(widget)
		{
			control = new SWUC.WebControl(SWU.HtmlTextWriterTag.Div);
			control.Style["POSITION"] = "relative";
			control.Style["OVERFLOW"] = "auto";
			control.Style["WIDTH"] = "100%";
			control.Style["HEIGHT"] = "100%";
		}
		
		public override object ControlObject
		{
			get { return control; }
		}
		
		#region IPixelLayout Members
		
		
		#endregion
		
		public void Add(Control child, int x, int y)
		{
			SWUC.WebControl control = ((WebControl)child.InnerControl).WebControlObject;
			control.Style["POSITION"] = "absolute";
			control.Style["TOP"] = (y > 0) ? y + "px" : null;
			control.Style["LEFT"] = (x > 0) ? x + "px" : null;
			this.control.Controls.Add(control);
		}
		
		public void Move(Control child, int x, int y)
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

		
		
	}
}

