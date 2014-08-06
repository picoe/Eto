using Eto.Drawing;
using Eto.Forms;
using MonoMac.AppKit;
using sd = System.Drawing;
using MonoMac.Foundation;
#if Mac64
using CGFloat = System.Double;
using NSInteger = System.Int64;
using NSUInteger = System.UInt64;
#else
using NSSize = System.Drawing.SizeF;
using NSRect = System.Drawing.RectangleF;
using NSPoint = System.Drawing.PointF;
using CGFloat = System.Single;
using NSInteger = System.Int32;
using NSUInteger = System.UInt32;
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
				view.SetFrameOrigin(new NSPoint((superFrame.Width - size.Width) / 2, (superFrame.Height - size.Height) / 2));
			}
		}
	}
}

