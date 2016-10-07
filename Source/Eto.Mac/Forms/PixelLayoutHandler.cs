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

		public class FlippedMacEventView : MacEventView 
		{
			public override bool IsFlipped
			{
				get { return true; }
			}
		}

		protected override NSView CreateControl()
		{
			return new FlippedMacEventView();
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
			var macControl = control.GetMacControl();
			var offset = macControl.PositionOffset;
			var childView = macControl.ContainerControl;
			var availableSize = Widget.Loaded ? Size.MaxValue : Control.Frame.Size.ToEtoSize();
			var preferredSize = macControl.GetPreferredSize(availableSize);

			var origin = new CGPoint(point.X + offset.Width, point.Y + offset.Height);
			if (!Control.IsFlipped)
			{
				origin.Y = Control.Frame.Height - origin.Y - preferredSize.Height;
			}

			childView.Frame = new CGRect(origin, preferredSize.ToNS());;
		}

		public override void LayoutChildren()
		{
			base.LayoutChildren();

			// set sizes of controls when resizing since available size changes, 
			// it may change the preferred size of the children.
			var availableSize = Widget.Loaded ? Size.MaxValue : Control.Frame.Size.ToEtoSize();
			foreach (var control in Widget.Controls.Select(r => r.GetMacControl()))
			{
				if (control == null)
					continue;

				var preferredSize = control.GetPreferredSize(availableSize);
				control.ContainerControl.SetFrameSize(preferredSize.ToNS());
			}
		}

		public override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			LayoutChildren();
			Widget.SizeChanged += Widget_SizeChanged;;
		}

		public override void OnUnLoad(EventArgs e)
		{
			base.OnUnLoad(e);
			Widget.SizeChanged -= Widget_SizeChanged;;
		}

		void Widget_SizeChanged (object sender, EventArgs e)
		{
			LayoutChildren();
		}

		public void Add(Control child, int x, int y)
		{
			var location = new Point(x, y);
			var childView = child.GetContainerView();
			childView.AutoresizingMask = NSViewResizingMask.NotSizable;
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
