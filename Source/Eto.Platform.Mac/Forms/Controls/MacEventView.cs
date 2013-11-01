using System;
using SD = System.Drawing;
using Eto.Forms;
using MonoMac.AppKit;
using MonoMac.Foundation;
using MonoMac.CoreImage;
using Eto.Drawing;
using MonoMac.ObjCRuntime;

namespace Eto.Platform.Mac.Forms.Controls
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

			SD.SizeF realSize;
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
			ciContext.DrawImage(ciImage, new SD.RectangleF(SD.PointF.Empty, size), new SD.RectangleF(SD.PointF.Empty, realSize));
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
				var kpea = theEvent.ToEtoKeyPressEventArgs();
				control.OnKeyDown(kpea);
				if (!kpea.Handled)
				{
					var handler = control.Handler as IMacViewHandler;
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
				var kpea = theEvent.ToEtoKeyPressEventArgs();
				control.OnKeyUp(kpea);
				return kpea.Handled;
			}
			return false;
		}

		public override void ResetCursorRects()
		{
			var cursor = Handler.Cursor;
			if (cursor != null)
			{
				AddCursorRect(new SD.RectangleF(SD.PointF.Empty, Frame.Size), cursor.ControlObject as NSCursor);
			}
		}
	}
}

