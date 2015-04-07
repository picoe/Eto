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
	public class CustomLayoutHandler : MacContainer<NSView, CustomLayout, CustomLayout.ICallback>, CustomLayout.IHandler
	{
		readonly Dictionary<Control, RectangleF> points = new Dictionary<Control, RectangleF>();

		public override NSView ContainerControl { get { return Control; } }

		public CustomLayoutHandler()
		{
			Control = new MacEventView { Handler = this };
		}

		public CGRect GetPosition(Control control)
		{
			RectangleF point;
			if (points.TryGetValue(control, out point))
			{
				return point.ToNS();
			}
			return control.GetContainerView().Frame;
		}

		protected override SizeF GetNaturalSize(SizeF availableSize)
		{
			RectangleF size = RectangleF.Empty;
			foreach (var item in points.Where(r => r.Key.Visible))
			{
				
				size = RectangleF.Union(size, item.Value);
			}
			return (SizeF)size.BottomRight;
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

		void SetPosition(Control control, RectangleF rect, float frameHeight, bool flipped)
		{
			var offset = ((IMacViewHandler)control.Handler).PositionOffset;
			var childView = control.GetContainerView();
			

			CGPoint origin;
			if (flipped)
				origin = new CGPoint(
					rect.X + offset.Width,
					rect.Y + offset.Height
				);
			else
				origin = new CGPoint(
					rect.X + offset.Width,
					frameHeight - (rect.Height + rect.Y + offset.Height)
				);

			var frame = new CGRect(origin, rect.Size.ToNS());
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

		public void Add(Control child)
		{
			points[child] = RectangleF.Empty;
			var childView = child.GetContainerView();
			if (Widget.Loaded)
			{
				var frameHeight = Control.Frame.Height;
				SetPosition(child, RectangleF.Empty, (float)frameHeight, Control.IsFlipped);
			}
			Control.AddSubview(childView);
			if (Widget.Loaded)
				LayoutParent();
		}

		public void Move(Control child, Rectangle location)
		{
			//if (points[child] != location)
			{
				points[child] = location;
				//if (Widget.Loaded)
				{
					var frameHeight = Control.Frame.Height;
					SetPosition(child, location, (float)frameHeight, Control.IsFlipped);
					if (Widget.Loaded)
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

		public void RemoveAll()
		{
			foreach (var child in points.Keys)
			{
				var childView = child.GetContainerView();
				childView.RemoveFromSuperview();
			}
			points.Clear();
			if (Widget.Loaded)
				LayoutParent();
		}
	}
}
