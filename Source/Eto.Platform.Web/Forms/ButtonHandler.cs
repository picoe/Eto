using System;

using Eto.Forms;

using SWC = System.Web.UI.WebControls;

namespace Eto.Platform.Web.Forms
{
	public class ButtonHandler : WebControl, IButton
	{
		private SWC.Button control = null;
		
		public ButtonHandler(Widget widget) : base(widget)
		{
			control = new SWC.Button();
			control.Click += new EventHandler(control_Click);
		}
		
		public override string Text
		{
			get	{ return control.Text; }
			set	{ control.Text = value; }
		}
		
		public override object ControlObject
		{
			get	{ return control; }
		}
		
		private void control_Click(object sender, EventArgs e)
		{
			((Button)Widget).OnClick(e);
		}
	}
}

