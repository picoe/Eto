using System;
using Eto.Forms;
using System.Linq;
using Eto.Drawing;
using System.Collections.Generic;

#if XAMMAC2
using AppKit;
using Foundation;
using CoreGraphics;
using ObjCRuntime;
using CoreAnimation;
using CoreImage;
#elif OSX
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

#if IOS
using Foundation;
using CoreGraphics;
using ObjCRuntime;
using CoreAnimation;
using CoreImage;
using Eto.iOS;

using NSResponder = UIKit.UIResponder;
using NSView = UIKit.UIView;
using Eto.iOS.Forms;
#endif

namespace Eto.Mac.Forms
{
	public abstract class MacContainer<TControl, TWidget, TCallback> : 
		MacView<TControl, TWidget, TCallback>,
		Container.IHandler
		where TControl: NSObject
		where TWidget: Container
		where TCallback: Container.ICallback
	{
		public bool RecurseToChildren { get { return true; } }

		public virtual Size ClientSize { get { return Size; } set { Size = value; } }

		public override IEnumerable<Control> VisualControls => Widget.Controls;

		public virtual void Update()
		{
			InvalidateMeasure();
		}

		protected override bool ControlEnabled
		{
			get => base.ControlEnabled;
			set
			{
				base.ControlEnabled = value;
				foreach (var child in Widget.VisualControls)
				{
					child.GetMacViewHandler()?.SetEnabled(value);
				}
			}
		}

#if OSX
		public bool NeedsQueue => ApplicationHandler.QueueResizing;

		#else
		public bool NeedsQueue => false;

		#endif

		public override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			InvalidateMeasure();
		}

		public override void Invalidate(bool invalidateChildren)
		{
			base.Invalidate(invalidateChildren);
			if (invalidateChildren)
			{
				foreach (var child in Widget.VisualControls)
				{
					child.Invalidate(invalidateChildren);
				}
			}
		}

		public override void Invalidate(Rectangle rect, bool invalidateChildren)
		{
			base.Invalidate(rect, invalidateChildren);
			if (invalidateChildren)
			{
				var screenRect = Widget.RectangleToScreen(rect);
				foreach (var child in Widget.VisualControls)
				{
					child.Invalidate(Rectangle.Round(child.RectangleFromScreen(screenRect)), invalidateChildren);
				}
			}
		}

		public override void RecalculateKeyViewLoop(ref NSView last)
		{
			foreach (var child in Widget.Controls.OrderBy(c => c.TabIndex))
			{
				var handler = child.GetMacControl();
				if (handler != null)
				{
					handler.RecalculateKeyViewLoop(ref last);
					if (last != null)
						last.NextKeyView = handler.FocusControl;
					last = handler.FocusControl;
				}
			}
		}
	}
}
