using System;
using Eto.Forms;
using MonoMac.AppKit;
using Eto.Drawing;
using SD = System.Drawing;

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

		
		public override void SizeToFit ()
		{
			foreach (var c in Widget.Container.Controls)
			{
				AutoSize (c);
			}
			
			SD.SizeF size = new SD.SizeF (0, 0);
			foreach (var c in Control.Subviews) {
				var frame = c.Frame;
				if (size.Width < frame.Right)
					size.Width = frame.Right;
				if (size.Height < frame.Bottom)
					size.Height = frame.Bottom;
			}
			
			if (size != Control.Frame.Size) {
				SetContainerSize(size);
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
