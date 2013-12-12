using System;
using Eto.Forms;
using MonoMac.AppKit;
using Eto.Drawing;
using SD = System.Drawing;
using System.Collections.Generic;
using System.Linq;
using Eto.Platform.Mac.Forms.Controls;

namespace Eto.Platform.Mac.Forms
{
	public class PixelLayoutHandler : MacContainer<NSView, PixelLayout>, IPixelLayout
	{
		readonly Dictionary<Control, PointF> points = new Dictionary<Control, PointF>();

		public override NSView ContainerControl { get { return Control; } }

		public PixelLayoutHandler()
		{
			Control = new MacEventView { Handler = this };
		}

		public SD.RectangleF GetPosition(Control control)
		{
			PointF point;
			if (points.TryGetValue(control, out point))
			{
				var frameSize = ((NSView)control.ControlObject).Frame.Size;
				return new SD.RectangleF(point.ToSD(), frameSize);
			}
			return control.GetContainerView().Frame;
		}

		protected override SizeF GetNaturalSize(SizeF availableSize)
		{
			SizeF size = SizeF.Empty;
			foreach (var item in points)
			{
				var frameSize = item.Key.GetPreferredSize(availableSize);
				size = SizeF.Max(size, frameSize + new SizeF(item.Value));
			}
			return size;
		}

		public override void OnLoadComplete(EventArgs e)
		{
			base.OnLoadComplete(e);
			LayoutChildren();
			Widget.SizeChanged += HandleSizeChanged;
		}

		public override void OnUnLoad(EventArgs e)
		{
			base.OnUnLoad(e);
			Widget.SizeChanged -= HandleSizeChanged;
		}

		void HandleSizeChanged (object sender, EventArgs e)
		{
			LayoutChildren();
		}

		void SetPosition(Control control, PointF point, float frameHeight, bool flipped)
		{
			var offset = ((IMacViewHandler)control.Handler).PositionOffset;
			var childView = control.GetContainerView();
			
			var preferredSize = control.GetPreferredSize(Control.Frame.Size.ToEtoSize());

			SD.PointF origin;
			if (flipped)
				origin = new System.Drawing.PointF(
					point.X + offset.Width,
					point.Y + offset.Height
				);
			else
				origin = new System.Drawing.PointF(
					point.X + offset.Width,
					frameHeight - (preferredSize.Height + point.Y + offset.Height)
				);
			
			var frame = new SD.RectangleF(origin, preferredSize.ToSD());
			if (frame != childView.Frame)
			{
				childView.Frame = frame;
			}
		}

		public override void LayoutChildren()
		{
			if (NeedsQueue())
				return;
			var controlPoints = points.ToArray();
			var frameHeight = Control.Frame.Height;
			var flipped = Control.IsFlipped;
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
				SetPosition(child, location, frameHeight, Control.IsFlipped);
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
					SetPosition(child, location, frameHeight, Control.IsFlipped);
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
	}
}
