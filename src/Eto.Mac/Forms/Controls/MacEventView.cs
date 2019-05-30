using System;
using Eto.Forms;
using Eto.Drawing;
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

namespace Eto.Mac.Forms.Controls
{
	public class MacEventView : NSBox, IMacControl
	{
		public MacEventView()
		{
			BoxType = NSBoxType.NSBoxCustom;
			Transparent = true;
			BorderWidth = 0;
			BorderType = NSBorderType.NoBorder;
			ContentViewMargins = CGSize.Empty;
		}

		public MacEventView(IntPtr handle)
			: base(handle)
		{
		}

		public WeakReference WeakHandler { get; set; }

		public IMacViewHandler Handler
		{ 
			get { return (IMacViewHandler)WeakHandler.Target; }
			set { WeakHandler = new WeakReference(value); } 
		}

		public Control Widget
		{
			get { return Handler == null ? null : Handler.Widget; }
		}

		public static bool KeyDown(Control control, NSEvent theEvent)
		{
			if (control != null)
			{
				var handler = control.Handler as IMacViewHandler;
				var kpea = theEvent.ToEtoKeyEventArgs();
				handler.OnKeyDown(kpea);
				return kpea.Handled;
			}
			return false;
		}

		public static bool KeyUp(Control control, NSEvent theEvent)
		{
			if (control != null)
			{
				var handler = control.Handler as IMacViewHandler;
				if (handler != null)
				{
					var kpea = theEvent.ToEtoKeyEventArgs ();
					handler.Callback.OnKeyUp (control, kpea);
					return kpea.Handled;
				}
			}
			return false;
		}

		public override void ResetCursorRects()
		{
			var handler = Handler;
			if (handler == null)
				return;
			var cursor = handler.CurrentCursor;
			if (cursor != null)
			{
				AddCursorRect(new CGRect(CGPoint.Empty, Frame.Size), cursor.ControlObject as NSCursor);
			}
		}
	}
}

