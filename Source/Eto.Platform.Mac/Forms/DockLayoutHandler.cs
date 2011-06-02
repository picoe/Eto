using System;
using SD = System.Drawing;
using Eto.Forms;
using Eto.Drawing;
using MonoMac.AppKit;
using MonoMac.Foundation;

namespace Eto.Platform.Mac
{
	public class DockLayoutHandler : MacLayout<NSView, DockLayout>, IDockLayout
	{
		Control child;
		Padding padding;
		
		public override NSView Control {
			get {
				return (NSView)Widget.Container.ContainerObject;
			}
			protected set {
				base.Control = value;
			}
		}
		
		public Eto.Drawing.Padding Padding {
			get { return padding; }
			set {
				padding = value;
				SetChildFrame();
			}
		}
		
		void SetChildFrame()
		{
			if (child == null) return;
			
			NSView parent = (NSView)ControlObject;
			
			NSView childControl = (NSView)child.ControlObject;
			var frame = parent.Frame;
			
			frame.Y = padding.Top;
			frame.X = padding.Left;
			frame.Width -= padding.Horizontal;
			frame.Height -= padding.Vertical;
			
			childControl.Frame = frame;
		}
				
		public void Add(Control child)
		{
			if (this.child != null) { 
				((NSView)this.child.ControlObject).RemoveFromSuperview(); this.child = null; 
			}
			if (child != null)
			{
				NSView childControl = (NSView)child.ControlObject;
				childControl.AutoresizingMask = NSViewResizingMask.WidthSizable | NSViewResizingMask.HeightSizable;
				this.child = child;
				SetChildFrame();
				NSView parent = (NSView)ControlObject;
				parent.AddSubview(childControl);
			}
		}

		public void Remove(Control child)
		{
			NSView childControl = (NSView)child.ControlObject;
			childControl.RemoveFromSuperview();
			this.child = null;
		}
	}
}
