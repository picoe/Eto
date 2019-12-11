using System;
using Eto.Forms;
using Eto.Drawing;
using System.Diagnostics;
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
			var handler = control?.Handler as IMacViewHandler;
			if (handler == null)
				return false;

			var kpea = theEvent.ToEtoKeyEventArgs();
			handler.OnKeyDown(kpea);
			return kpea.Handled;
		}

		public static bool FlagsChanged(Control control, NSEvent theEvent)
		{
			var handler = control?.Handler as IMacViewHandler;
			if (handler == null)
				return false;

			Keys key;
			NSEventModifierMask requiredModifier;

			// check which key is being pressed currently and figure out correct modifier mask for that key alone
			switch (theEvent.KeyCode)
			{
				case 56:
					key = Keys.LeftShift;
					requiredModifier = (NSEventModifierMask)0x20002;
					break;
				case 60:
					key = Keys.RightShift;
					requiredModifier = (NSEventModifierMask)0x20004;
					break;
				case 59:
					key = Keys.LeftControl;
					requiredModifier = (NSEventModifierMask)0x40001;
					break;
				case 57:
					key = Keys.RightControl;
					requiredModifier = (NSEventModifierMask)0x40002; // correct?  I don't have a keyboard with right control key.
					break;
				case 58:
					key = Keys.LeftAlt;
					requiredModifier = (NSEventModifierMask)0x80020;
					break;
				case 61:
					key = Keys.RightAlt;
					requiredModifier = (NSEventModifierMask)0x80040;
					break;
				case 55:
					key = Keys.LeftApplication;
					requiredModifier = (NSEventModifierMask)0x100008;
					break;
				case 54:
					key = Keys.RightApplication;
					requiredModifier = (NSEventModifierMask)0x100010;
					break;
				default:
					Debug.WriteLine($"Unknown FlagsChanged Key: {theEvent.KeyCode}, Modifiers: {theEvent.ModifierFlags}");
					return false;
			}
			// test the modifier to see if the key was pressed or released
			var modifierFlags = theEvent.ModifierFlags;
			var type = modifierFlags.HasFlag(requiredModifier) ? KeyEventType.KeyDown : KeyEventType.KeyUp;

			key |= modifierFlags.ToEto();
			var kpea = new KeyEventArgs(key, type);
			if (type == KeyEventType.KeyDown)
				handler.OnKeyDown(kpea);
			else
				handler.OnKeyUp(kpea);
			return kpea.Handled;
		}

		public static bool KeyUp(Control control, NSEvent theEvent)
		{
			var handler = control?.Handler as IMacViewHandler;
			if (handler == null)
				return false;

			var kpea = theEvent.ToEtoKeyEventArgs();
			handler.Callback.OnKeyUp(control, kpea);
			return kpea.Handled;
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

