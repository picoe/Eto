using System;
using SD = System.Drawing;
using SWF = System.Windows.Forms;
using Eto.Forms;

namespace Eto.Platform.Windows
{
	public class PixelLayoutHandler : WindowsLayout<object, PixelLayout>, IPixelLayout
	{
		public override object Control {
			get {
				return Widget.Container != null ? Widget.Container.ControlObject : null;
			}
			protected set {
				base.Control = value;
			}
		}

		public void Add(Control child, int x, int y)
		{
			SWF.ScrollableControl parent = Widget.Container.ControlObject as SWF.ScrollableControl;
			SWF.Control ctl = (SWF.Control)child.ControlObject;
			SD.Point pt = new SD.Point(x, y);
			if (parent != null) pt.Offset(parent.AutoScrollPosition);
			ctl.Location = pt;
			parent.Controls.Add(ctl);
			ctl.BringToFront();
		}

		public void Move(Control child, int x, int y)
		{
			SWF.ScrollableControl parent = Widget.Container.ControlObject as SWF.ScrollableControl;
			SWF.Control ctl = ((SWF.Control)child.ControlObject);
			SD.Point pt = new SD.Point(x, y);
			if (parent != null) pt.Offset(parent.AutoScrollPosition);
			ctl.Location = pt;
		}
		
		public void Remove (Control child)
		{
			var parent = Widget.Container.ControlObject as SWF.ScrollableControl;
			var ctl = (SWF.Control)child.ControlObject;
			if (parent != null) parent.Controls.Remove(ctl);
			
		}
	}
}
