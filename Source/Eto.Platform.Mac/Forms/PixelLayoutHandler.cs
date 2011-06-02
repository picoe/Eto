using System;
using Eto.Forms;
using MonoMac.AppKit;
using Eto.Drawing;

namespace Eto.Platform.Mac
{
	public class PixelLayoutHandler : MacLayout<NSView, PixelLayout>, IPixelLayout
	{

		public override NSView Control {
			get {
				return (NSView)Widget.Container.ContainerObject;
			}
			protected set {
				base.Control = value;
			}
		}

		
		public void Add(Control child, int x, int y)
		{
			var parent = ControlObject as NSView;
			var childView = child.ControlObject as NSView;
			var offset = ((IMacView)child.Handler).PositionOffset;
			childView.SetFrameOrigin(new System.Drawing.PointF(x + offset.Width, y + offset.Height));
			parent.AddSubview(childView);
		}

		public void Move(Control child, int x, int y)
		{
			var childView = child.ControlObject as NSView;
			var offset = ((IMacView)child.Handler).PositionOffset;
			childView.SetFrameOrigin(new System.Drawing.PointF(x + offset.Width, y + offset.Height));
		}
		
		public void Remove (Control child)
		{
			var childView = child.ControlObject as NSView;
			childView.RemoveFromSuperview();
			
		}
	}
}
