using Eto.Drawing;
using Eto.Forms;
using System;
using sd = System.Drawing;
#if XAMMAC2
using AppKit;
using Foundation;
using CoreGraphics;
using ObjCRuntime;
using CoreAnimation;
#else
using MonoMac.AppKit;
using MonoMac.Foundation;
using MonoMac.CoreGraphics;
using MonoMac.ObjCRuntime;
using MonoMac.CoreAnimation;
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

#if IOS
using NSView = MonoTouch.UIKit.UIView;
using NSControl = MonoTouch.UIKit.UIControl;
#endif
namespace Eto.Mac.Forms
{
	public static class MacControlExtensions
	{
		public static SizeF GetPreferredSize(this Control control, SizeF availableSize)
		{
			if (control == null)
				return Size.Empty;
			var mh = control.GetMacControl();
			if (mh != null)
			{
				return mh.GetPreferredSize(availableSize);
			}
			
			var c = control.ControlObject as NSControl;
			if (c != null)
			{
				c.SizeToFit();
				return c.Frame.Size.ToEto();
			}
			var child = control.ControlObject as Control;
			return child == null ? SizeF.Empty : child.GetPreferredSize(availableSize);

		}

		public static IMacContainer GetMacContainer(this Control control)
		{
			if (control == null)
				return null;
			var container = control.Handler as IMacContainer;
			if (container != null)
				return container;
			var child = control.ControlObject as Control;
			return child == null ? null : child.GetMacContainer();
		}

		public static IMacControlHandler GetMacControl(this Control control)
		{
			if (control == null)
				return null;
			var container = control.Handler as IMacControlHandler;
			if (container != null)
				return container;
			var child = control.ControlObject as Control;
			return child == null ? null : child.GetMacControl();
		}

		public static NSView GetContainerView(this Widget control)
		{
			if (control == null)
				return null;
			var containerHandler = control.Handler as IMacControlHandler;
			if (containerHandler != null)
				return containerHandler.ContainerControl;
			var childControl = control.ControlObject as Control;
			if (childControl != null)
				return childControl.GetContainerView();
			return control.ControlObject as NSView;
		}

		public static void CenterInParent(this NSView view)
		{
			var super = view.Superview;
			if (super != null)
			{
				var superFrame = super.Frame;
				var size = view.Frame.Size;
				view.SetFrameOrigin(new CGPoint((nfloat)(superFrame.Width - size.Width) / 2, (nfloat)(superFrame.Height - size.Height) / 2));
			}
		}
	}
}

