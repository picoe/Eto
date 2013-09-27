using System;
using Eto.Forms;
using Eto.Drawing;
using MonoTouch.UIKit;
using System.Collections.Generic;
using sd = System.Drawing;
using System.Linq;
using Eto.Platform.Mac.Forms;

namespace Eto.Platform.iOS.Forms
{
	public class PixelLayoutHandler : iosLayout<UIView, PixelLayout>, IPixelLayout
	{
		Dictionary<Control, Point> points = new Dictionary<Control, Point>();

		public PixelLayoutHandler()
		{
			Control = new UIView();
		}
		/*
		public sd.RectangleF GetPosition (Control control)
		{
			Point point;
			if (points.TryGetValue (control, out point)) {
				var frameSize = ((UIView)control.ControlObject).Frame.Size;
				return new sd.RectangleF (Generator.ConvertF (point), frameSize);
			}
			return base.GetPosition (control);
		}*/

		public override Size GetPreferredSize(Size availableSize)
		{
			Size size = Size.Empty;
			foreach (var item in points)
			{
				var frameSize = item.Key.GetPreferredSize(availableSize);
				size = Size.Max(size, frameSize + new Size(item.Value));
			}
			return size;
		}

		public override void OnLoadComplete(EventArgs e)
		{
			base.OnLoadComplete(e);
		}

		void SetPosition(Control control, Point point, float frameHeight, bool flipped)
		{
			var offset = ((IiosView)control.Handler).PositionOffset;
			var childView = control.GetContainerView();
			
			var preferredSize = control.GetPreferredSize(Size.MaxValue);
			
			sd.PointF origin;
			if (flipped)
				origin = new sd.PointF(
					point.X + offset.Width,
					point.Y + offset.Height
				);
			else
				origin = new sd.PointF(
					point.X + offset.Width,
					frameHeight - (preferredSize.Height + point.Y + offset.Height)
				);
			
			var frame = new sd.RectangleF(origin, preferredSize.ToSDSizeF());
			if (frame != childView.Frame)
			{
				childView.Frame = frame;
			}
		}

		public override void LayoutChildren()
		{
			var controlPoints = points.ToArray();
			var frameHeight = Control.Frame.Height;
			var flipped = !Control.Layer.GeometryFlipped;
			foreach (var item in controlPoints)
			{
				SetPosition(item.Key, item.Value, frameHeight, flipped);
			}
		}

		public void Add(Control child, int x, int y)
		{
			var location = new Point(x, y);
			points[child] = location;
			var childView = child.GetContainerView();
			if (Widget.Loaded)
			{
				var frameHeight = Control.Frame.Height;
				SetPosition(child, location, frameHeight, false);
			}
			Control.AddSubview(childView);
			if (Widget.Loaded)
				LayoutParent();
		}

		public void Move(Control child, int x, int y)
		{
			var location = new Point(x, y);
			if (points[child] != location)
			{
				points[child] = location;
				if (Widget.Loaded)
				{
					var frameHeight = Control.Frame.Height;
					SetPosition(child, location, frameHeight, false);
					LayoutParent();
				}
			}
		}

		public void Remove(Control child)
		{
			var childView = child.GetContainerView();
			points.Remove(child);
			childView.RemoveFromSuperview();
			if (Widget.Loaded)
				LayoutParent();
		}
		/*
		public void Add(Control child, int x, int y)
		{
			var parent = this.Control;
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
		*/
	}
}
