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

namespace Eto.Android
{
	public static class KeyMap
	{
		static Dictionary<av.Keycode, Keys> keymap = new Dictionary<av.Keycode, Keys>();
		static Dictionary<Keys, av.Keycode> inverse = new Dictionary<Keys, av.Keycode>();

		public static Keys ToEto(this av.Keycode keyData)
        {
            // convert the modifiers
			Keys modifiers = Keys.None;

#if TODO
            // Shift
            if ((keyData & av.Keycode.Shift) == av.Keycode.Shift)
                modifiers |= Keys.Shift;

            // Control
            if ((keyData & av.Keycode.Control) == av.Keycode.Control)
                modifiers |= Keys.Control;

            // Alt
            if ((keyData & av.Keycode.Alt) == av.Keycode.Alt)
                modifiers |= Keys.Alt;

            var keyCode =
                Find(keyData & ~(av.Keycode.Shift | av.Keycode.Control | av.Keycode.Alt));
#else
			var keyCode = Find(keyData); // This is incomplete; modifiers are not currently handled
#endif

            return keyCode | modifiers;
        }

		private static Keys Find(av.Keycode key)
		{
			Keys mapped;
			if (keymap.TryGetValue(key, out mapped)) return mapped;
			else return Keys.None;
		}

		public static av.Keycode Find(Keys key)
		{
			av.Keycode mapped;
			if (inverse.TryGetValue(key, out mapped)) return mapped;
			else return av.Keycode.Unknown;
		}

		public static av.Keycode ToSWF(this Keys key)
		{
			var code = key & Keys.KeyMask;
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
			keymap.Add(av.Keycode.A, Keys.A);
			keymap.Add(av.Keycode.B, Keys.B);
			keymap.Add(av.Keycode.C, Keys.C);
			keymap.Add(av.Keycode.D, Keys.D);
			keymap.Add(av.Keycode.E, Keys.E);
			keymap.Add(av.Keycode.F, Keys.F);
			keymap.Add(av.Keycode.G, Keys.G);
			keymap.Add(av.Keycode.H, Keys.H);
			keymap.Add(av.Keycode.I, Keys.I);
			keymap.Add(av.Keycode.J, Keys.J);
			keymap.Add(av.Keycode.K, Keys.K);
			keymap.Add(av.Keycode.L, Keys.L);
			keymap.Add(av.Keycode.M, Keys.M);
			keymap.Add(av.Keycode.N, Keys.N);
			keymap.Add(av.Keycode.O, Keys.O);
			keymap.Add(av.Keycode.P, Keys.P);
			keymap.Add(av.Keycode.Q, Keys.Q);
			keymap.Add(av.Keycode.R, Keys.R);
			keymap.Add(av.Keycode.S, Keys.S);
			keymap.Add(av.Keycode.T, Keys.T);
			keymap.Add(av.Keycode.U, Keys.U);
			keymap.Add(av.Keycode.V, Keys.V);
			keymap.Add(av.Keycode.W, Keys.W);
			keymap.Add(av.Keycode.X, Keys.X);
			keymap.Add(av.Keycode.Y, Keys.Y);
			keymap.Add(av.Keycode.Z, Keys.Z);
			//keymap.Add(av.Keycode.F1, Keys.F1);
			//keymap.Add(av.Keycode.F2, Keys.F2);
			//keymap.Add(av.Keycode.F3, Keys.F3);
			//keymap.Add(av.Keycode.F4, Keys.F4);
			//keymap.Add(av.Keycode.F5, Keys.F5);
			//keymap.Add(av.Keycode.F6, Keys.F6);
			//keymap.Add(av.Keycode.F7, Keys.F7);
			//keymap.Add(av.Keycode.F8, Keys.F8);
			//keymap.Add(av.Keycode.F9, Keys.F9);
			//keymap.Add(av.Keycode.F10, Keys.F10);
			//keymap.Add(av.Keycode.F11, Keys.F11);
			//keymap.Add(av.Keycode.F12, Keys.F12);
			keymap.Add(av.Keycode.Num0, Keys.D0);
			keymap.Add(av.Keycode.Num1, Keys.D1);
			keymap.Add(av.Keycode.Num2, Keys.D2);
			keymap.Add(av.Keycode.Num3, Keys.D3);
			keymap.Add(av.Keycode.Num4, Keys.D4);
			keymap.Add(av.Keycode.Num5, Keys.D5);
			keymap.Add(av.Keycode.Num6, Keys.D6);
			keymap.Add(av.Keycode.Num7, Keys.D7);
			keymap.Add(av.Keycode.Num8, Keys.D8);
			keymap.Add(av.Keycode.Num9, Keys.D9);
			keymap.Add(av.Keycode.Space, Keys.Space);
			//keymap.Add(av.Keycode.Up, Keys.Up);
			//keymap.Add(av.Keycode.Down, Keys.Down);
			//keymap.Add(av.Keycode.Left, Keys.Left);
			//keymap.Add(av.Keycode.Right, Keys.Right);
			//keymap.Add(av.Keycode.PageDown, Keys.PageDown);
			//keymap.Add(av.Keycode.PageUp, Keys.PageUp);
			keymap.Add(av.Keycode.Home, Keys.Home);
			//keymap.Add(av.Keycode.End, Keys.End);
			//keymap.Add(av.Keycode.Alt, Keys.Alt);
			//keymap.Add(av.Keycode.Control, Keys.Control);
			//keymap.Add(av.Keycode.Shift, Keys.Shift);
			keymap.Add(av.Keycode.Menu, Keys.Menu);
			//keymap.Add(av.Keycode.Escape, Keys.Escape);
			keymap.Add(av.Keycode.Del, Keys.Delete);
			keymap.Add(av.Keycode.Back, Keys.Backspace);
			//keymap.Add(av.Keycode.Divide, Keys.Divide);
			keymap.Add(av.Keycode.Enter, Keys.Enter);
			//keymap.Add(av.Keycode.Insert, Keys.Insert);
            keymap.Add(av.Keycode.Period, Keys.Period);
            keymap.Add(av.Keycode.Tab, Keys.Tab);
			//keymap.Add(av.Keycode.Apps, Keys.ContextMenu);
			
			foreach (var entry in keymap)
			{
				inverse.Add(entry.Value, entry.Key);
			}
		}		
	}
}