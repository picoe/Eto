using System;
using Eto.Forms;
using Eto.Drawing;
using System.Collections.Generic;
using System.Linq;
using Eto.Mac.Forms.Controls;

#if XAMMAC2
using AppKit;
using Foundation;
using CoreGraphics;
using ObjCRuntime;
using CoreAnimation;
using CoreImage;
#else
using MonoMac.AppKit;
using MonoMac.Foundation;
using MonoMac.CoreGraphics;
using MonoMac.ObjCRuntime;
using MonoMac.CoreAnimation;
using MonoMac.CoreImage;
#if Mac64
using CGSize = MonoMac.Foundation.NSSize;
using CGRect = MonoMac.Foundation.NSRect;
using CGPoint = MonoMac.Foundation.NSPoint;
using nfloat = System.Double;
using nint = System.Int64;
using nuint = System.UInt64;
#else
using CGSize = System.Drawing.SizeF;
using CGRect = System.Drawing.RectangleF;
using CGPoint = System.Drawing.PointF;
using nfloat = System.Single;
using nint = System.Int32;
using nuint = System.UInt32;
#endif
#endif

namespace Eto.Mac.Forms
{
	public class PixelLayoutHandler : MacContainer<NSView, PixelLayout, PixelLayout.ICallback>, PixelLayout.IHandler
	{
		readonly Dictionary<Control, PointF> points = new Dictionary<Control, PointF>();

		public override NSView ContainerControl { get { return Control; } }

		protected override NSView CreateControl()
		{
			return new MacEventView();
		}

		public CGRect GetPosition(Control control)
		{
			PointF point;
			if (points.TryGetValue(control, out point))
			{
				var frameSize = ((NSView)control.ControlObject).Frame.Size;
				return new CGRect(point.ToNS(), frameSize);
			}
			return control.GetContainerView().Frame;
		}

		protected override SizeF GetNaturalSize(SizeF availableSize)
		{
			SizeF size = SizeF.Empty;
			foreach (var item in points.Where(r => r.Key.Visible))
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

			CGPoint origin;
			if (flipped)
				origin = new CGPoint(
					point.X + offset.Width,
					point.Y + offset.Height
				);
			else
				origin = new CGPoint(
					point.X + offset.Width,
					frameHeight - (preferredSize.Height + point.Y + offset.Height)
				);

			var frame = new CGRect(origin, preferredSize.ToNS());
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
				SetPosition(item.Key, item.Value, (float)frameHeight, flipped);
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
				SetPosition(child, location, (float)frameHeight, Control.IsFlipped);
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
					SetPosition(child, location, (float)frameHeight, Control.IsFlipped);
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
