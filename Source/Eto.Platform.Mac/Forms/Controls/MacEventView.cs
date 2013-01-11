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
			/* Use this when implemented in maccore:
			ciImage.Draw (SD.PointF.Empty, new SD.RectangleF (SD.PointF.Empty, size), NSCompositingOperation.SourceOver, 1);
			 */
		}

		WeakReference handler;
		
		public IMacViewHandler Handler {
			get { return handler.Target as IMacViewHandler; }
			set { handler = new WeakReference (value); }
		}
		
		object IMacControl.Handler { get { return Handler; } }
		
		public Control Widget {
			get { return Handler != null ? Handler.Widget : null; }
		}
		
		public static bool KeyDown (Control control, NSEvent theEvent)
		{
			if (control != null) {
				char keyChar = !string.IsNullOrEmpty (theEvent.Characters) ? theEvent.Characters [0] : '\0';
				Key key = KeyMap.MapKey (theEvent.KeyCode);
				KeyPressEventArgs kpea;
				Key modifiers = KeyMap.GetModifiers (theEvent);
				key |= modifiers;
				//Console.WriteLine("\t\tkeymap.Add({2}, Key.{0}({1})); {3}", theEvent.Characters, (int)keyChar, theEvent.KeyCode, theEvent.ModifierFlags);
				//Console.WriteLine("\t\t{0} {1} {2}", key & Key.ModifierMask, key & Key.KeyMask, (NSKey)keyChar);
				if (key != Key.None) {
					if (((modifiers & ~(Key.Shift | Key.Alt)) == 0))
                        kpea = new KeyPressEventArgs(key, KeyType.KeyDown, keyChar);
					else
						kpea = new KeyPressEventArgs (key, KeyType.KeyDown);
				} else {
                    kpea = new KeyPressEventArgs(key, KeyType.KeyDown, keyChar);
				}
				control.OnKeyDown (kpea);
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

		/*
		public override void KeyDown (NSEvent theEvent)
		{
			//base.InterpretKeyEvents (new NSEvent [] { theEvent });
			if (!KeyDown (Widget, theEvent))
				base.KeyDown (theEvent);
		}
		*/
		
		/*
		public override void MouseDragged (NSEvent theEvent)
		{
			if (Widget != null) {
				var args = CreateMouseArgs (theEvent);
				Widget.OnMouseMove (args);
				if (!args.Handled)
					base.MouseDragged (theEvent);
			} else
				base.MouseDragged (theEvent);
		}
		
		public override void MouseUp (NSEvent theEvent)
		{
			if (Widget != null) {
				var args = CreateMouseArgs (theEvent);
				Widget.OnMouseUp (args);
				if (!args.Handled)
					base.MouseUp (theEvent);
			} else
				base.MouseUp (theEvent);
		}

		public override void MouseDown (NSEvent theEvent)
		{
			if (Widget != null) {
				var args = CreateMouseArgs (theEvent);
				if (theEvent.ClickCount >= 2)
					Widget.OnMouseDoubleClick (args);
				
				if (!args.Handled) {
					Widget.OnMouseDown (args);
				}
					
				if (!args.Handled)
					base.MouseDown (theEvent);
			} else
				base.MouseDown (theEvent);
		}
		
		public override void RightMouseDown (NSEvent theEvent)
		{
			if (Widget != null) {
				var args = CreateMouseArgs (theEvent);
				if (theEvent.ClickCount >= 2)
					Widget.OnMouseDoubleClick (args);
				
				if (!args.Handled) {
					Widget.OnMouseDown (args);
				}
				if (!args.Handled)
					base.RightMouseDown (theEvent);
			} else
				base.RightMouseDown (theEvent);
		}
			
		public override void RightMouseUp (NSEvent theEvent)
		{
			if (Widget != null) {
				var args = CreateMouseArgs (theEvent);
				Widget.OnMouseUp (args);
				if (!args.Handled)
					base.RightMouseUp (theEvent);
			} else
				base.RightMouseUp (theEvent);
		}
			
		public override void RightMouseDragged (NSEvent theEvent)
		{
			if (Widget != null) {
				var args = CreateMouseArgs (theEvent);
				Widget.OnMouseMove (args);
				if (!args.Handled)
					base.RightMouseDragged (theEvent);
			} else
				base.RightMouseDragged (theEvent);
		}
		*/
	}
}

