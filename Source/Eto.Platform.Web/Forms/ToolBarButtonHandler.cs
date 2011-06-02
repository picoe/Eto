using System;

using Eto.Forms;
using Eto.Drawing;

using SWU = System.Web.UI;
using SWUC = System.Web.UI.WebControls;

namespace Eto.Platform.Web.Forms
{
	public class ToolBarButtonHandler : WebControl, IToolBarButton
	{
		SWUC.LinkButton control;
		
		public ToolBarButtonHandler(Widget widget)
		: base(widget)
		{
			control = new SWUC.LinkButton();
			control.Click += new EventHandler(control_Click);
		}
		
		private void control_Click(object sender, EventArgs e)
		{
			((ToolBarButton)Widget).OnClick(e);
		}
		
		public Icon Icon
		{
			get { return null; }
			set { }
		}
		
		public override string Text
		{
			get { return control.Text; }
			set { control.Text = value; }
		}
		
		public override Object ControlObject
		{
			get { return control; }
		}
		
	}
}
