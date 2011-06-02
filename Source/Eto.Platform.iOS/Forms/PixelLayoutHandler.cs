using System;
using Eto.Forms;
using Eto.Drawing;
using MonoTouch.UIKit;

namespace Eto.Platform.iOS.Forms
{
	public class PixelLayoutHandler : iosLayout<UIView, PixelLayout>, IPixelLayout
	{

		public override UIView Control {
			get {
				return (UIView)Widget.Container.ContainerObject;
			}
			protected set {
				base.Control = value;
			}
		}

		
		public void Add(Control child, int x, int y)
		{
			var parent = ControlObject as UIView;
			var childView = child.ControlObject as UIView;
			var offset = ((IiosView)child.Handler).PositionOffset;
			
			var newposition = new System.Drawing.PointF(x + offset.Width, y + offset.Height);
			//var scrollView = ControlObject as UIScrollView;
			//if (scrollView != null) { newposition.X -= scrollView.ContentOffset.X; newposition.Y -= scrollView.ContentOffset.Y; }
			childView.SetFrameOrigin(newposition);
			parent.AddSubview(childView);
		}

		public void Move(Control child, int x, int y)
		{
			//var parent = ControlObject as UIView;
			var childView = child.ControlObject as UIView;
			var offset = ((IiosView)child.Handler).PositionOffset;
			
			var newposition = new System.Drawing.PointF(x + offset.Width, y + offset.Height);
			//var scrollView = ControlObject as UIScrollView;
			//if (scrollView != null) { newposition.X -= scrollView.ContentOffset.X; newposition.Y -= scrollView.ContentOffset.Y; }
			childView.SetFrameOrigin(newposition);
		}
		
		public void Remove (Control child)
		{
			var childView = child.ControlObject as UIView;
			childView.RemoveFromSuperview();
		}
	}
}
