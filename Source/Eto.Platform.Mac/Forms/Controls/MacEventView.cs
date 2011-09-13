using System;
using Eto.Forms;
using MonoMac.AppKit;

namespace Eto.Platform.Mac.Forms.Controls
{
	public class MacEventView : NSView
	{
		WeakReference handler;
		
		public IMacView Handler {
			get { return handler.Target as IMacView; }
			set { handler = new WeakReference (value); }
		}
		
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
					if ((int)keyChar < 127 && (int)keyChar >= 32 && ((modifiers & ~(Key.Shift | Key.Alt)) == 0))
						kpea = new KeyPressEventArgs (key, keyChar);
					else
						kpea = new KeyPressEventArgs (key);
				} else {
					kpea = new KeyPressEventArgs (key, keyChar);
				}
				control.OnKeyDown (kpea);
				return kpea.Handled;
			}
			return false;
		}
		
		public override void KeyDown (NSEvent theEvent)
		{
			if (!KeyDown (Widget, theEvent))
				base.KeyDown (theEvent);
		}
		
		/*
		public override bool PerformKeyEquivalent (NSEvent theEvent)
		{
			return false;
		}*/

		MouseEventArgs CreateMouseArgs (NSEvent theEvent)
		{
			var pt = Generator.GetLocation (this, theEvent);
			Key modifiers = KeyMap.GetModifiers (theEvent);
			MouseButtons buttons = KeyMap.GetMouseButtons (theEvent);
			return new MouseEventArgs (buttons, modifiers, pt);
		}
			
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
	}
}

