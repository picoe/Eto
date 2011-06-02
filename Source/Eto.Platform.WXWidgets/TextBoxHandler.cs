using System;
using System.Drawing;

namespace Eto.Forms.WXWidgets
{
	internal class TextBoxHandler : WXControl, ITextBox
	{
		private wx.TextCtrl control = null;
		private string label = string.Empty;
		private bool enabled = true;

		public TextBoxHandler(Widget widget) : base(widget)
		{
		}

		public override string Text
		{
			get { return (control == null) ? label : control.Title; }
			set { if (control == null) label = value; else control.Title = value;}
		}

		public bool ReadOnly
		{
			get { return (control == null) ? enabled : control.Enabled; }
			set { if (control == null) enabled = value; else control.Enabled = value; }
		}


		public bool Multiline
		{
			get { return false; }
			set {  }
		}

		public override object ControlObject
		{
			get { return control; }
		}

		public override object CreateControl(Control parent, object container)
		{
			control = new wx.TextCtrl((wx.Window)container, ((WXGenerator)Generator).GetNextButtonID(), label, Location, Size);
			control.EVT_TEXT_ENTER(control.ID, new wx.EventListener(control_TextChanged));
			control.Enabled = enabled;
			return control;
		}

		private void control_TextChanged(object sender, wx.Event e)
		{
			((TextBox)Widget).OnTextChanged(EventArgs.Empty);
		}
	}
}
