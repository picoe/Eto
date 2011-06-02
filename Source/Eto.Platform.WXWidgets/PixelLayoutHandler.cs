using System;
using System.Drawing;
using SWF = System.Windows.Forms;

namespace Eto.Forms.WXWidgets
{
	internal class PixelLayoutHandler : WXLayout, IPixelLayout
	{
		SWF.Panel control;

		public PixelLayoutHandler(Widget widget) : base(widget)
		{
			control = new SWF.Panel();
		}

		public override object ControlObject
		{
			get { return control; }
		}

		#region IPixelLayout Members

		public void SetLocation(Control child, int x, int y)
		{
			((SWF.Control)child.ControlObject).Location = new Point(x, y);
		}

		#endregion

		public override void AddChild(Control child)
		{
			if (Container.ContainerObject != null) 
			{
				((WXControl)child.InnerControl).CreateControl((Container)Container.Widget);
			}
			else Container.Controls.Add(child);

		}

		public override void RemoveChild(Control child)
		{
			throw new NotImplementedException("Cannot remove controls with wxWidgets");
		}
	}
}
