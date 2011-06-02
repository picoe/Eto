using System;

using Eto.Forms;

using SWUC = System.Web.UI.WebControls;
using SWUH = System.Web.UI.HtmlControls;
using SWU = System.Web.UI;

namespace Eto.Platform.Web.Forms
{
	public class GroupBoxHandler : WebContainer, IGroupBox
	{
		SWUC.Label control;
		SWUC.Label label;
		
		public GroupBoxHandler(Widget widget) : base(widget)
		{
			control = new SWUC.Label();
			control.Style["border"] = "1px solid";
			control.Style["padding-top"] = "10px";
			
			label = new SWUC.Label();
			label.Style["POSITION"] = "absolute";
			label.Style["LEFT"] = "15px";
			label.Style["TOP"] = "-10px";
			label.Style["background-color"] = "white";
			control.Controls.Add(label);
		}
		
		public override object ContainerObject
		{
			get { return control; }
		}
		
		
		public override object ControlObject
		{
			get { return control; }
		}
		
		public override string Text
		{
			get { return label.Text; }
			set { label.Text = value; }
		}
		
	}
}

