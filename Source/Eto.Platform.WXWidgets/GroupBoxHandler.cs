using System;
using System.Drawing;

namespace Eto.Forms.WXWidgets
{
	internal class GroupBoxHandler : WXContainer, IGroupBox
	{
		wx.StaticBox control = null;
		wx.Panel panel = null;
		string label = string.Empty;

		public GroupBoxHandler(Widget widget) : base(widget)
		{
			
		}

		public override object ControlObject
		{
			get { return control; }
		}

		public override object ContainerObject
		{
			get { return panel; }
		}


		public override string Text
		{
			get { return (control == null) ? label : control.Label; }
			set { if (control == null) label = value; else control.Label = value;}
		}

		public override object CreateControl(Control parent, object container)
		{
			control = new wx.StaticBox((wx.Window)container, ((WXGenerator)Generator).GetNextButtonID(), label, Location, Size, 0, string.Empty);
			control.EVT_SIZE(new wx.EventListener(control_Size));

			panel = new wx.Panel(control, -1, new Point(4,17), new Size(control.Size.Width-7, control.Size.Height-20));
			CreateChildren();
			
			return control;
		}

		private void control_Size(object sender, wx.Event evt)
		{
			panel.Size = new Size(control.Size.Width-7, control.Size.Height-20);
		}

	}
}
