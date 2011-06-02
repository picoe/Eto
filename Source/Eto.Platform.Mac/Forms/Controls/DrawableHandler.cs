using System;
using SD = System.Drawing;
using Eto.Drawing;
using Eto.Forms;
using Eto.Platform.Mac.Drawing;
using MonoMac.AppKit;
using MonoMac.Foundation;
using MonoMac;

namespace Eto.Platform.Mac
{
	public class DrawableHandler : MacView<DrawableHandler.MyView, Drawable>, IDrawable
	{
		public class MyView : NSView
		{
			public DrawableHandler Handler { get; set; }

			public override void DrawRect (System.Drawing.RectangleF dirtyRect)
			{
				dirtyRect.Y = this.Frame.Height - dirtyRect.Y - dirtyRect.Height;
				Handler.Update (Generator.ConvertF (dirtyRect));
			}

			public override void KeyDown (NSEvent theEvent)
			{
				char keyChar = !string.IsNullOrEmpty (theEvent.Characters) ? theEvent.Characters [0] : '\0';
				Key key = KeyMap.MapKey (theEvent.KeyCode);
				KeyPressEventArgs kpea;
				Key modifiers = KeyMap.GetModifiers (theEvent);
				key |= modifiers;
				//Console.WriteLine("\t\tkeymap.Add({2}, Key.{0}({1})); {3}", theEvent.Characters, (int)keyChar, theEvent.KeyCode, theEvent.ModifierFlags);
				//Console.WriteLine("\t\t{0} {1} {2}", key & Key.ModifierMask, key & Key.KeyMask, (NSKey)keyChar);
				if (key != Key.None) {
					if ((int)keyChar < 127 && (int)keyChar >= 32 && ((modifiers & ~Key.Shift) == 0))
						kpea = new KeyPressEventArgs (key, keyChar);
					else
						kpea = new KeyPressEventArgs (key);
				} else {
					kpea = new KeyPressEventArgs (key, keyChar);
				}
				Handler.Widget.OnKeyDown (kpea);
				if (!kpea.Handled)
					base.KeyDown (theEvent);
			}
			
			MouseEventArgs CreateMouseArgs (NSEvent theEvent)
			{
				var pt = Generator.GetLocation (this, theEvent);
				Key modifiers = KeyMap.GetModifiers (theEvent);
				MouseButtons buttons = KeyMap.GetMouseButtons (theEvent);
				return new MouseEventArgs (buttons, modifiers, pt);
			}
			
			public override void MouseDragged (NSEvent theEvent)
			{
				var args = CreateMouseArgs (theEvent);
				Handler.Widget.OnMouseMove (args);
				if (!args.Handled)
					base.MouseDragged (theEvent);
			}
			
			public override void MouseUp (NSEvent theEvent)
			{
				var args = CreateMouseArgs (theEvent);
				Handler.Widget.OnMouseUp (args);
				if (!args.Handled)
					base.MouseUp (theEvent);
			}

			public override void MouseDown (NSEvent theEvent)
			{
				var args = CreateMouseArgs (theEvent);
				Handler.Widget.OnMouseDown (args);
				if (!args.Handled)
					base.MouseDown (theEvent);
			}
			
			public override void RightMouseDown (NSEvent theEvent)
			{
				var args = CreateMouseArgs (theEvent);
				Handler.Widget.OnMouseDown (args);
				if (!args.Handled)
					base.RightMouseDown (theEvent);
			}
			
			public override void RightMouseUp (NSEvent theEvent)
			{
				var args = CreateMouseArgs (theEvent);
				Handler.Widget.OnMouseUp (args);
				if (!args.Handled)
					base.RightMouseUp (theEvent);
			}
			
			public override void RightMouseDragged (NSEvent theEvent)
			{
				var args = CreateMouseArgs (theEvent);
				Handler.Widget.OnMouseMove (args);
				if (!args.Handled)
					base.RightMouseDragged (theEvent);
			}
			
			public bool CanFocus { get; set; }

			public override bool AcceptsFirstResponder ()
			{
				return CanFocus;
			}

			public override bool AcceptsFirstMouse (NSEvent theEvent)
			{
				return CanFocus;
			}
			
			public override bool BecomeFirstResponder ()
			{
				Handler.Widget.OnGotFocus (EventArgs.Empty);
				return base.BecomeFirstResponder ();
			}
			
			public override bool ResignFirstResponder ()
			{
				Handler.Widget.OnLostFocus (EventArgs.Empty);
				return base.ResignFirstResponder ();
			}
			
			
		}

		public DrawableHandler ()
		{
			Control = new MyView{ Handler = this };
		}
		
		public bool CanFocus {
			get { return Control.CanFocus; }
			set { Control.CanFocus = value; }
		}
		
		public void Update (Rectangle rect)
		{
			var context = NSGraphicsContext.CurrentContext;
			if (context != null) {
				var graphics = new Graphics (Widget.Generator, new GraphicsHandler (context, Control.Frame.Height));
				Widget.OnPaint (new PaintEventArgs (graphics, rect));
			}
		}

	}
}
