using System;
using SD = System.Drawing;
using SWF = System.Windows.Forms;
using Eto.Forms;
using Eto.Drawing;

namespace Eto.Platform.Windows
{
	public class PixelLayoutHandler : WindowsLayout<SWF.Control, PixelLayout>, IPixelLayout
	{
		public override SWF.Control Control {
			get {
				return Widget.Container != null ? (SWF.Control)Widget.Container.ControlObject : null;
			}
			protected set {
				base.Control = value;
			}
		}

		public override Size DesiredSize
		{
			get { return Control.PreferredSize.ToEto (); }
		}

		public void Add(Control child, int x, int y)
		{
			SWF.ScrollableControl parent = Widget.Container.ControlObject as SWF.ScrollableControl;
			SWF.Control ctl = child.GetContainerControl ();
			SD.Point pt = new SD.Point(x, y);
			if (parent != null) pt.Offset(parent.AutoScrollPosition);
			ctl.Location = pt;
			parent.Controls.Add(ctl);
			ctl.BringToFront();
		}

		public void Move(Control child, int x, int y)
		{
			SWF.ScrollableControl parent = Widget.Container.ControlObject as SWF.ScrollableControl;
			SWF.Control ctl = child.GetContainerControl ();
			SD.Point pt = new SD.Point(x, y);
			if (parent != null) pt.Offset(parent.AutoScrollPosition);
			ctl.Location = pt;
		}
		
		public void Remove (Control child)
		{
			var parent = Widget.Container.ControlObject as SWF.ScrollableControl;
			var ctl = child.GetContainerControl ();
			if (parent != null) parent.Controls.Remove(ctl);
			
		}
	}
}
