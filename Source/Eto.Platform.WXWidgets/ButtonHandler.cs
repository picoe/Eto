using System;
using System.Drawing;

namespace Eto.Forms.WXWidgets
{
	internal class ButtonHandler : WXControl, IButton
	{
		private wx.Button control = null;
		private string label = string.Empty;

		public ButtonHandler(Widget widget) : base(widget)
		{
		}

		public override string Text
		{
			get { return (control == null) ? label : control.Label; }
			set { if (control == null) label = value; else control.Label = value;}
		}

		public override object ControlObject
		{
			get { return control; }
		}

		public override object CreateControl(Control parent, object container)
		{
			control = new wx.Button((wx.Window)container, ((WXGenerator)Generator).GetNextButtonID(), label, Location, Size);
			control.AutoLayout = true;
			control.EVT_BUTTON(control.ID, new wx.EventListener(control_Click));
			return control;
		}


		private void control_Click(object sender, wx.Event e)
		{
			((Button)Widget).OnClick(EventArgs.Empty);
		}
	}
}
