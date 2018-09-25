using System;
using Eto.Forms;
using Eto.Mac.Forms.Controls;

#if XAMMAC2
using AppKit;
using Foundation;
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

namespace Eto.Mac.Forms
{
	public class MacFieldEditor : NSTextView, IMacControl
	{
		public MacFieldEditor()
		{
			FieldEditor = true;
		}

		public MacFieldEditor(IntPtr handle)
			: base(handle)
		{
		}

		public IMacControl MacControl => WeakDelegate as IMacControl;
		public object Handler => MacControl?.WeakHandler?.Target;

		WeakReference IMacControl.WeakHandler
		{
			get => MacControl?.WeakHandler;
			set { }
		}

		public override void KeyDown(NSEvent theEvent)
		{
			var handler = Handler as IMacViewHandler;
			if (handler != null && MacEventView.KeyDown(handler.Widget, theEvent))
				return;

			base.KeyDown(theEvent);
		}

		public override bool ShouldChangeText(NSRange affectedCharRange, string replacementString)
		{
			var handler = Handler as IMacTextBoxHandler;
			if (handler != null)
			{
				var args = new TextChangingEventArgs(replacementString, affectedCharRange.ToEto(), true);
				handler.Callback.OnTextChanging(handler.Widget, args);
				if (args.Cancel)
					return false;
			}

			return base.ShouldChangeText(affectedCharRange, replacementString);
		}

		public override bool ResignFirstResponder()
		{
			var handler = Handler as IMacViewHandler;
			if (handler != null && handler.ShouldHaveFocus == null)
			{
				handler.ShouldHaveFocus = false;
				// for some reason calling base.ResignFirstResponder calls this method again???
				var result = base.ResignFirstResponder();
				handler.Callback.OnLostFocus(handler.Widget, EventArgs.Empty);
				handler.ShouldHaveFocus = null;
				return result;
			}
			return base.ResignFirstResponder();
		}

		public override void InsertText(NSObject insertString)
		{
			var handler = Handler as IMacViewHandler;
			if (handler != null && insertString is NSString text)
			{
				var args = new TextInputEventArgs(text);
				handler.Callback.OnTextInput(handler.Widget, args);
				if (args.Cancel)
					return;
			}

			base.InsertText(insertString);
		}
	}
}
