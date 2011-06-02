using System;
using System.Drawing;

namespace Eto.Forms.WXWidgets
{
	internal class CheckBoxHandler : WXControl, ICheckBox
	{
		private wx.CheckBox control = null;
		private string label = string.Empty;
		private bool check = false;

		public CheckBoxHandler(Widget widget) : base(widget)
		{
		}

		public override string Text
		{
			get { return (control == null) ? label : control.Label; }
			set { if (control == null) label = value; else control.Label = value;}
		}

		public bool Checked
		{
			get { return (control == null) ? check : control.IsChecked; }
			set { if (control == null) check = value; else control.Value = value;}
		}

		public override object ControlObject
		{
			get { return control; }
		}

		public override object CreateControl(Control parent, object container)
		{
			control = new wx.CheckBox((wx.Window)container, ((WXGenerator)Generator).GetNextButtonID(), label, Location, Size);
			control.Value = check;
			return control;
		}

		private void control_CheckedChanged(object sender, EventArgs e)
		{
			((CheckBox)Widget).OnCheckedChanged(e);
		}
	}
}
