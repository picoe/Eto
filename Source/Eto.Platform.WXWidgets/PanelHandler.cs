using System;
using System.Drawing;

namespace Eto.Forms.WXWidgets
{
	internal class PanelHandler : WXContainer, IPanel
	{
		wx.Panel control = null;

		public PanelHandler(Widget widget) : base(widget)
		{
			
		}

		public override object ControlObject
		{
			get { return control; }
		}

		public override object CreateControl(Control parent, object container)
		{
			control = new wx.Panel((wx.Window)container, ((WXGenerator)Generator).GetNextButtonID(), Location, Size);
			CreateChildren();
			return control;
		}

	}
}
