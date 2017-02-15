using Eto.Drawing;
using Eto.Forms;
using System;
using System.Text.RegularExpressions;


#if XAMMAC2
using AppKit;
using Foundation;
using CoreGraphics;
using ObjCRuntime;
using CoreAnimation;
using CoreText;
#elif OSX
using MonoMac.AppKit;
using MonoMac.Foundation;
using MonoMac.CoreGraphics;
using MonoMac.ObjCRuntime;
using MonoMac.CoreAnimation;
using MonoMac.CoreText;
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

#if IOS
using NSView = UIKit.UIView;
using NSControl = UIKit.UIControl;
using Foundation;
using CoreText;
using CoreGraphics;
using Eto.iOS;
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
			var mh2 = control.GetMacControl2();
			if (mh2 != null)
				return mh2.GetPreferredSize(control, availableSize);
			
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

		public static IMacControlHandler2 GetMacControl2(this Control control)
		{
			if (control == null)
				return null;
			var container = control.Handler as IMacControlHandler2;
			if (container != null)
				return container;
			var child = control.ControlObject as Control;
			return child == null ? null : child.GetMacControl2();
		}

		public static NSView GetContainerView(this Widget control)
		{
			if (control == null)
				return null;
			var containerHandler = control.Handler as IMacControlHandler;
			if (containerHandler != null)
				return containerHandler.ContainerControl;
			var containerHandler2 = control.Handler as IMacControlHandler2;
			if (containerHandler2 != null)
				return containerHandler2.GetContainerControl(control);
			var childControl = control.ControlObject as Control;
			if (childControl != null)
				return childControl.GetContainerView();
			return control.ControlObject as NSView;
		}

		public static NSAttributedString ToAttributedStringWithMnemonic(this string value, NSDictionary attributes = null)
		{
			if (value == null)
				return null;
			var match = Regex.Match(value, @"(?<=([^&](?:[&]{2})*)|^)[&](?![&])");
			if (match.Success)
			{
				value = value.Remove(match.Index, 1);
				value = value.Replace("&&", "&");
				var str = attributes != null ? new NSMutableAttributedString(value, attributes) : new NSMutableAttributedString(value);
				var attr = new CTStringAttributes();
				attr.UnderlineStyle = CTUnderlineStyle.Single;
				str.AddAttributes(attr, new NSRange(match.Index, 1));
				return str;
			}
			else
			{
				value = value.Replace("&&", "&");
				return attributes != null ? new NSAttributedString(value, attributes) : new NSAttributedString(value);
			}
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

