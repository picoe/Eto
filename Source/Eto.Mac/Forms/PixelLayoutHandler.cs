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
using nfloat = System.Double;
using nint = System.Int64;
using nuint = System.UInt64;
#else
using nfloat = System.Single;
using nint = System.Int32;
using nuint = System.UInt32;
#endif
#if SDCOMPAT
using CGSize = System.Drawing.SizeF;
using CGRect = System.Drawing.RectangleF;
using CGPoint = System.Drawing.PointF;
#endif
#endif

namespace Eto.Mac.Forms
{
	public class PixelLayoutHandler : MacContainer<NSView, PixelLayout, PixelLayout.ICallback>, PixelLayout.IHandler
	{
		public override NSView ContainerControl { get { return Control; } }

		protected override NSView CreateControl()
		{
			return new MacEventView();
		}

		protected override SizeF GetNaturalSize(SizeF availableSize)
		{
			base.GetNaturalSize(availableSize);
			SizeF size = SizeF.Empty;
			foreach (var item in Widget.Controls)
			{
				var frameSize = item.GetPreferredSize(availableSize);
				size = SizeF.Max(size, frameSize + new SizeF(item.Location));
			}
			return size;
		}

		void SetPosition(Control control, PointF point)
		{
			var offset = ((IMacViewHandler)control.Handler).PositionOffset;
			var childView = control.GetContainerView();
			
			var preferredSize = control.GetPreferredSize(Control.Frame.Size.ToEtoSize());

			var origin = new CGPoint(point.X + offset.Width, point.Y + offset.Height);
			if (!Control.IsFlipped)
			{
				origin.Y = Control.Frame.Height - origin.Y - preferredSize.Height;
			}

			var frame = new CGRect(origin, preferredSize.ToNS());
			if (frame != childView.Frame)
			{
				childView.Frame = frame;
			}
		}

		public void Add(Control child, int x, int y)
		{
			var location = new Point(x, y);
			var childView = child.GetContainerView();
			childView.AutoresizingMask = NSViewResizingMask.MinYMargin;
			SetPosition(child, location);
			Control.AddSubview(childView);
			if (Widget.Loaded)
				LayoutParent();
		}

		public void Move(Control child, int x, int y)
		{
			var location = new Point(x, y);
			SetPosition(child, location);
			if (Widget.Loaded)
				LayoutParent();
		}

		public void Remove(Control child)
		{
			var childView = child.GetContainerView();
			childView.RemoveFromSuperview();
			if (Widget.Loaded)
				LayoutParent();
		}
	}
}
