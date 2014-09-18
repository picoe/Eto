using System;
using SD = System.Drawing;
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

namespace Eto.Mac.Forms.Controls
{
	public class MacEventView : NSView, IMacControl
	{
		static readonly NSString CIOutputImage = new NSString("outputImage");
		static readonly Selector selConvertSizeToBacking = new Selector("convertSizeToBacking:");

		public static void Colourize(NSView control, Color color, Action drawAction)
		{
			var size = control.Frame.Size;
			if (size.Width <= 0 || size.Height <= 0)
				return;
			var image = new NSImage(size);
			
			image.LockFocusFlipped(!control.IsFlipped);
			drawAction();
			image.UnlockFocus();

			var ciImage = CIImage.FromCGImage(image.CGImage);

			CGSize realSize;
			if (control.RespondsToSelector(selConvertSizeToBacking))
				realSize = control.ConvertSizeToBacking(size);
			else
				realSize = control.ConvertSizeToBase(size);

			var filter2 = new CIColorControls();
			filter2.SetDefaults();
			filter2.Image = ciImage;
			filter2.Saturation = 0.0f;
			ciImage = (CIImage)filter2.ValueForKey(CIOutputImage);

			var filter3 = new CIColorMatrix();
			filter3.SetDefaults();
			filter3.Image = ciImage;
			filter3.RVector = new CIVector(0, color.R, 0);
			filter3.GVector = new CIVector(color.G, 0, 0);
			filter3.BVector = new CIVector(0, 0, color.B);
			ciImage = (CIImage)filter3.ValueForKey(CIOutputImage);

			// create separate context so we can force using the software renderer, which is more than fast enough for this
			var ciContext = CIContext.FromContext(NSGraphicsContext.CurrentContext.GraphicsPort, new CIContextOptions { UseSoftwareRenderer = true });
			ciContext.DrawImage(ciImage, new CGRect(CGPoint.Empty, size), new CGRect(CGPoint.Empty, realSize));
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
				handler.Callback.OnKeyDown(control, kpea);
				if (!kpea.Handled)
				{
					if (handler != null)
						handler.PostKeyDown(kpea);
				}

				return kpea.Handled;
			}
			return false;
		}

		public static bool KeyUp(Control control, NSEvent theEvent)
		{
			if (control != null)
			{
				var handler = control.Handler as IMacViewHandler;
				var kpea = theEvent.ToEtoKeyEventArgs();
				handler.Callback.OnKeyUp(control, kpea);
				return kpea.Handled;
			}
			return false;
		}

		public override void ResetCursorRects()
		{
			var cursor = Handler.CurrentCursor;
			if (cursor != null)
			{
				AddCursorRect(new CGRect(CGPoint.Empty, Frame.Size), cursor.ControlObject as NSCursor);
			}
		}
	}
}

