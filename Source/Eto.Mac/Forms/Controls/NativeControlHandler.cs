using System;
using System.Globalization;
using Eto.Forms;
using Eto.Drawing;

#if XAMMAC2
using AppKit;
using Foundation;
using CoreGraphics;
using ObjCRuntime;
using CoreAnimation;
using nnint = System.nint;
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
using nnint = System.UInt64;
#else
using CGSize = System.Drawing.SizeF;
using CGRect = System.Drawing.RectangleF;
using CGPoint = System.Drawing.PointF;
using nfloat = System.Single;
using nint = System.Int32;
using nuint = System.UInt32;
using nnint = System.Int32;
#endif
#endif

namespace Eto.Mac.Forms.Controls
{
	public class NativeControlHandler : MacView<NSView, Control, Control.ICallback>
	{
		NSViewController controller;

		public NativeControlHandler(NSView nativeControl)
		{
			Control = nativeControl;
		}

		protected override void Initialize()
		{
			base.Initialize();
			AutoSize = false;
		}

		public override SizeF GetPreferredSize(SizeF availableSize)
		{
			return Control.Frame.Size.ToEto();
		}

		public NativeControlHandler(NSViewController nativeControl)
		{
			controller = nativeControl;
			Control = controller.View;
		}

		public override NSView ContainerControl { get { return Control; } }

		public override bool Enabled
		{
			get
			{
				throw new NotSupportedException(string.Format(CultureInfo.CurrentCulture, "You cannot get the enabled state of a native control"));
			}
			set
			{
				throw new NotSupportedException(string.Format(CultureInfo.CurrentCulture, "You cannot set the enabled state of a native control"));
			}
		}
	}
}

