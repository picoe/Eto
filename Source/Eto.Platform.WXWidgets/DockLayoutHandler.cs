using System;
using System.Drawing;

namespace Eto.Forms.WXWidgets
{
	internal class DockLayoutHandler : WXLayout, IDockLayout
	{
		wx.BoxSizer sizer;

		public DockLayoutHandler(Widget widget) : base(widget)
		{
			sizer = new wx.BoxSizer(wx.Orientation.wxHORIZONTAL);
		}

		public override object ControlObject
		{
			get { return null; }
		}
		
		public override void AddChild(Control child)
		{
			if (Container.ContainerObject != null) 
			{
				((WXControl)child.InnerControl).CreateControl((Container)Container.Widget);
				sizer.Add((wx.Window)child.ControlObject, 1, wx.Stretch.wxEXPAND);
				sizer.Fit(((wx.Window)child.ControlObject).Parent);
				((wx.Window)child.ControlObject).Parent.Sizer = sizer;
			}
			else Container.Controls.Add(child);
		}

		public override void RemoveChild(Control child)
		{
			if (Container.ContainerObject != null) 
			{
				//((WXControl)child.InnerControl).CreateControl((Container)Container.Widget);
			}
			else Container.Controls.Remove(child);
			//((SWF.Control)Container.ContainerObject).Controls.Remove((SWF.Control)child.ControlObject);
		}
	}
}
