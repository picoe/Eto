using System;
using SD = System.Drawing;
using Eto.Forms;
using Eto.Drawing;
using MonoTouch.UIKit;

namespace Eto.Platform.iOS.Forms
{
	public class DockLayoutHandler : iosLayout<UIView, DockLayout>, IDockLayout
	{
		Control child;
		Padding padding;
		
		public override UIView Control {
			get {
				return (UIView)Widget.Container.ContainerObject;
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
			
			UIView parent = (UIView)ControlObject;
			
			UIView childControl = (UIView)child.ControlObject;
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
				((UIView)this.child.ControlObject).RemoveFromSuperview(); this.child = null; 
			}
			if (child != null)
			{
				UIView childControl = (UIView)child.ControlObject;
				this.child = child;
				childControl.AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleHeight;
				SetChildFrame();
				UIView parent = (UIView)ControlObject;
				parent.AddSubview(childControl);
			}
		}

		public void Remove(Control child)
		{
			UIView childControl = (UIView)child.ControlObject;
			childControl.RemoveFromSuperview();
			this.child = null;
		}
	}
}
