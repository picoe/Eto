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
		static NSString CIInputImage = new NSString ("inputImage");
		static NSString CIInputTransform = new NSString ("inputTransform");
		static NSString CIInputSaturation = new NSString ("inputSaturation");
		static NSString CIOutputImage = new NSString ("outputImage");
		static NSString CIInputRVector = new NSString ("inputRVector");
		static NSString CIInputGVector = new NSString ("inputGVector");
		static NSString CIInputBVector = new NSString ("inputBVector");

		static Selector selConvertSizeToBacking = new Selector("convertSizeToBacking:");

		public static void Colourize (NSView control, Color color, Action drawAction)
		{
			var size = control.Frame.Size;
			var image = new NSImage (size);
			
			image.LockFocusFlipped (control.IsFlipped);
			drawAction ();
			image.UnlockFocus ();
			
			var ciImage = CIImage.FromData (image.AsTiff ());
			image.Dispose();
			
			if (control.IsFlipped) {
				SD.SizeF realSize;
				if (control.RespondsToSelector (selConvertSizeToBacking))
					realSize = control.ConvertSizeToBacking (size);
				else
					realSize = control.ConvertSizeToBase (size);
				var affineTransform = new NSAffineTransform ();
				affineTransform.Translate (0, realSize.Height);
				affineTransform.Scale (1, -1);
				var filter1 = CIFilter.FromName ("CIAffineTransform");
				filter1.SetValueForKey (ciImage, CIInputImage);
				filter1.SetValueForKey (affineTransform, CIInputTransform);
				ciImage = filter1.ValueForKey (CIOutputImage) as CIImage;
			}
			
			var filter2 = CIFilter.FromName ("CIColorControls");
			filter2.SetDefaults ();
			filter2.SetValueForKey (ciImage, CIInputImage);
			filter2.SetValueForKey (new NSNumber (0.0f), CIInputSaturation);
			ciImage = filter2.ValueForKey (CIOutputImage) as CIImage;
			
			var filter3 = CIFilter.FromName ("CIColorMatrix");
			filter3.SetDefaults ();
			filter3.SetValueForKey (ciImage, CIInputImage);
			filter3.SetValueForKey (new CIVector (0, color.R, 0), CIInputRVector);
			filter3.SetValueForKey (new CIVector (color.G, 0, 0), CIInputGVector);
			filter3.SetValueForKey (new CIVector (0, 0, color.B), CIInputBVector);
			ciImage = filter3.ValueForKey (CIOutputImage) as CIImage;
			
			image = new NSImage (size);
			var rep = NSCIImageRep.FromCIImage (ciImage);
			image.AddRepresentation (rep);
			image.Draw (SD.PointF.Empty, new SD.RectangleF (SD.PointF.Empty, size), NSCompositingOperation.SourceOver, 1);
			image.Dispose();
			/* Use this when implemented in maccore:
			ciImage.Draw (SD.PointF.Empty, new SD.RectangleF (SD.PointF.Empty, size), NSCompositingOperation.SourceOver, 1);
			 */
		}

		public WeakReference WeakHandler { get; set; }

		public IMacViewHandler Handler
		{ 
			get { return (IMacViewHandler)WeakHandler.Target; }
			set { WeakHandler = new WeakReference(value); } 
		}

		public Control Widget {
			get { return Handler != null ? Handler.Widget : null; }
		}
		
		public static bool KeyDown (Control control, NSEvent theEvent)
		{
			if (control != null) {
				var kpea = theEvent.ToEtoKeyPressEventArgs ();
				control.OnKeyDown (kpea);
				if (!kpea.Handled) {
					var handler = control.Handler as IMacViewHandler;
					if (handler != null)
						handler.PostKeyDown (kpea);
				}

				return kpea.Handled;
			}
			return false;
		}

		public static bool KeyUp (Control control, NSEvent theEvent)
		{
			if (control != null) {
				var kpea = theEvent.ToEtoKeyPressEventArgs ();
				control.OnKeyUp (kpea);
				return kpea.Handled;
			}
			return false;
		}

		public override void ResetCursorRects ()
		{
			var cursor = Handler.Cursor;
			if (cursor != null) {
				this.AddCursorRect (new SD.RectangleF(SD.PointF.Empty, this.Frame.Size), cursor.ControlObject as NSCursor);
			}
		}
	}
}

