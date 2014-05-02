using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Eto.Forms;

using aa = Android.App;
using ac = Android.Content;
using ao = Android.OS;
using ar = Android.Runtime;
using av = Android.Views;
using aw = Android.Widget;

namespace Eto.Platform.Android
{
	public static class KeyMap
	{
		static Dictionary<av.Keycode, Key> keymap = new Dictionary<av.Keycode, Key>();
		static Dictionary<Key, av.Keycode> inverse = new Dictionary<Key, av.Keycode>();

        public static Key ToEto (this av.Keycode keyData)
        {
            // convert the modifiers
            Key modifiers = Key.None;

#if TODO
            // Shift
            if ((keyData & av.Keycode.Shift) == av.Keycode.Shift)
                modifiers |= Key.Shift;

            // Control
            if ((keyData & av.Keycode.Control) == av.Keycode.Control)
                modifiers |= Key.Control;

            // Alt
            if ((keyData & av.Keycode.Alt) == av.Keycode.Alt)
                modifiers |= Key.Alt;

            var keyCode =
                Find(keyData & ~(av.Keycode.Shift | av.Keycode.Control | av.Keycode.Alt));
#else
            var keyCode = Find(keyData); // This is incomplete; modifiers are not currently handled
#endif

            return keyCode | modifiers;
        }

		private static Key Find(av.Keycode key)
		{
			Key mapped;
			if (keymap.TryGetValue(key, out mapped)) return mapped;
			else return Key.None;
		}
		
		public static av.Keycode Find(Key key)
		{
			av.Keycode mapped;
			if (inverse.TryGetValue(key, out mapped)) return mapped;
			else return av.Keycode.Unknown;
		}
		
		public static av.Keycode ToSWF (this Key key)
		{
			var code = key & Key.KeyMask;
			av.Keycode modifiers = av.Keycode.Unknown;
			
#if TODO
			// convert the modifiers
			// Shift
			if ((key & Key.Shift) == Key.Shift)
				modifiers |= av.Keycode.Shift;
	
			// Control
			if ((key & Key.Control) == Key.Control)
				modifiers |= av.Keycode.Control;

			// Alt
			if ((key & Key.Alt) == Key.Alt)
				modifiers |= av.Keycode.Alt;
#endif

			return Find (code) | modifiers;
		}
		
		static KeyMap()
		{
			keymap.Add(av.Keycode.A, Key.A);
			keymap.Add(av.Keycode.B, Key.B);
			keymap.Add(av.Keycode.C, Key.C);
			keymap.Add(av.Keycode.D, Key.D);
			keymap.Add(av.Keycode.E, Key.E);
			keymap.Add(av.Keycode.F, Key.F);
			keymap.Add(av.Keycode.G, Key.G);
			keymap.Add(av.Keycode.H, Key.H);
			keymap.Add(av.Keycode.I, Key.I);
			keymap.Add(av.Keycode.J, Key.J);
			keymap.Add(av.Keycode.K, Key.K);
			keymap.Add(av.Keycode.L, Key.L);
			keymap.Add(av.Keycode.M, Key.M);
			keymap.Add(av.Keycode.N, Key.N);
			keymap.Add(av.Keycode.O, Key.O);
			keymap.Add(av.Keycode.P, Key.P);
			keymap.Add(av.Keycode.Q, Key.Q);
			keymap.Add(av.Keycode.R, Key.R);
			keymap.Add(av.Keycode.S, Key.S);
			keymap.Add(av.Keycode.T, Key.T);
			keymap.Add(av.Keycode.U, Key.U);
			keymap.Add(av.Keycode.V, Key.V);
			keymap.Add(av.Keycode.W, Key.W);
			keymap.Add(av.Keycode.X, Key.X);
			keymap.Add(av.Keycode.Y, Key.Y);
			keymap.Add(av.Keycode.Z, Key.Z);
			//keymap.Add(av.Keycode.F1, Key.F1);
			//keymap.Add(av.Keycode.F2, Key.F2);
			//keymap.Add(av.Keycode.F3, Key.F3);
			//keymap.Add(av.Keycode.F4, Key.F4);
			//keymap.Add(av.Keycode.F5, Key.F5);
			//keymap.Add(av.Keycode.F6, Key.F6);
			//keymap.Add(av.Keycode.F7, Key.F7);
			//keymap.Add(av.Keycode.F8, Key.F8);
			//keymap.Add(av.Keycode.F9, Key.F9);
			//keymap.Add(av.Keycode.F10, Key.F10);
			//keymap.Add(av.Keycode.F11, Key.F11);
			//keymap.Add(av.Keycode.F12, Key.F12);
			keymap.Add(av.Keycode.Num0, Key.D0);
			keymap.Add(av.Keycode.Num1, Key.D1);
			keymap.Add(av.Keycode.Num2, Key.D2);
			keymap.Add(av.Keycode.Num3, Key.D3);
			keymap.Add(av.Keycode.Num4, Key.D4);
			keymap.Add(av.Keycode.Num5, Key.D5);
			keymap.Add(av.Keycode.Num6, Key.D6);
			keymap.Add(av.Keycode.Num7, Key.D7);
			keymap.Add(av.Keycode.Num8, Key.D8);
			keymap.Add(av.Keycode.Num9, Key.D9);
			keymap.Add(av.Keycode.Space, Key.Space);
			//keymap.Add(av.Keycode.Up, Key.Up);
			//keymap.Add(av.Keycode.Down, Key.Down);
			//keymap.Add(av.Keycode.Left, Key.Left);
			//keymap.Add(av.Keycode.Right, Key.Right);
			//keymap.Add(av.Keycode.PageDown, Key.PageDown);
			//keymap.Add(av.Keycode.PageUp, Key.PageUp);
			keymap.Add(av.Keycode.Home, Key.Home);
			//keymap.Add(av.Keycode.End, Key.End);
			//keymap.Add(av.Keycode.Alt, Key.Alt);
			//keymap.Add(av.Keycode.Control, Key.Control);
			//keymap.Add(av.Keycode.Shift, Key.Shift);
			keymap.Add(av.Keycode.Menu, Key.Menu);
			//keymap.Add(av.Keycode.Escape, Key.Escape);
			keymap.Add(av.Keycode.Del, Key.Delete);
			keymap.Add(av.Keycode.Back, Key.Backspace);
			//keymap.Add(av.Keycode.Divide, Key.Divide);
			keymap.Add(av.Keycode.Enter, Key.Enter);
			//keymap.Add(av.Keycode.Insert, Key.Insert);
            keymap.Add(av.Keycode.Period, Key.Period);
            keymap.Add(av.Keycode.Tab, Key.Tab);
			//keymap.Add(av.Keycode.Apps, Key.ContextMenu);
			
			foreach (var entry in keymap)
			{
				inverse.Add(entry.Value, entry.Key);
			}
		}		
	}
}