using System;

using Eto.Forms;

using SWC = System.Web.UI.WebControls;

namespace Eto.Platform.Web.Forms
{
	public class CheckBoxHandler : WebControl, ICheckBox
	{
		private SWC.CheckBox control = null;
		
		public CheckBoxHandler(Widget widget) : base(widget)
		{
			control = new SWC.CheckBox();
			control.CheckedChanged += new EventHandler(control_CheckedChanged);
		}
		
		public override string Text
		{
			get
			{ return control.Text; }
			set
			{ control.Text = value; }
		}
		
		public bool Checked
		{
			get
			{ return control.Checked; }
			set
			{ control.Checked = value; }
		}
		
		
		public override object ControlObject
		{
			get
			{ return control; }
		}
		
		private void control_CheckedChanged(object sender, EventArgs e)
		{
			((CheckBox)Widget).OnCheckedChanged(e);
		}
	}
}

