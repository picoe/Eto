using System;

using Eto.Forms;

using SWC = System.Web.UI.WebControls;

namespace Eto.Platform.Web.Forms
{
	public class TextBoxHandler : WebControl, ITextBox
	{
		SWC.TextBox control;
		
		public TextBoxHandler(Widget widget) : base(widget)
		{
			control = new SWC.TextBox();
		}
		
		public override object ControlObject
		{
			get
			{ return control; }
		}
		
		public override string Text
		{
			get
			{ return control.Text; }
			set
			{ control.Text = value; }
		}
		
		public bool ReadOnly
		{
			get
			{ return control.ReadOnly; }
			set
			{ control.ReadOnly = value; }
		}
		
		
		public bool Multiline
		{
			get
			{ return true; }
			set
			{ }
		}
	}
}

