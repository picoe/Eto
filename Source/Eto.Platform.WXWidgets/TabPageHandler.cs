using System;
using System.Drawing;

namespace Eto.Forms.WXWidgets
{
	internal class TabPageHandler : WXContainer, ITabPage
	{
		private wx.Panel control = null;
		string text = string.Empty;

		public TabPageHandler(Widget widget) : base(widget)
		{
		}

		public override object ControlObject
		{
			get { return control; }
		}

		public override string Text
		{
			get { return text; }
			set { text = value; }
		}

		public override object CreateControl(Control parent, object container)
		{
			control = new wx.Panel((wx.Window)container);
			//control.ID = ((WXGenerator)Generator).GetNextButtonID();
			CreateChildren();
			
			//wx.BoxSizer sizerPanel = new wx.BoxSizer( wx.Orientation.wxBOTH );
			//sizerPanel.Add( buttonBig, 1, Stretch.wxEXPAND );
			//control.Sizer = sizerPanel;

			//sizer.Layout();
			return control;
		}


		private void control_Click(object sender, EventArgs e)
		{
			//((TabPage)Widget)base.OnClick(e);
		}
	}
}
