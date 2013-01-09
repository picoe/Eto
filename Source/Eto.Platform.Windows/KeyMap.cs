using System;
using swf = System.Windows.Forms;
using Eto.Forms;
using System.Collections.Generic;
using System.Linq;

namespace Eto.Platform.Windows
{
	public static class KeyMap
	{
		static Dictionary<swf.Keys, Key> keymap = new Dictionary<swf.Keys, Key>();
		static Dictionary<Key, swf.Keys> inverse = new Dictionary<Key, swf.Keys>();

        public static Key Convert(swf.Keys keyData)
        {
            // convert the modifiers
            Key modifiers = Key.None;

            // Shift
            if ((keyData & swf.Keys.Shift) == swf.Keys.Shift)
                modifiers |= Key.Shift;

            // Control
            if ((keyData & swf.Keys.Control) == swf.Keys.Control)
                modifiers |= Key.Control;

            // Alt
            if ((keyData & swf.Keys.Alt) == swf.Keys.Alt)
                modifiers |= Key.Alt;

            var keyCode =
                Find(keyData & ~(swf.Keys.Shift | swf.Keys.Control | swf.Keys.Alt));

            return keyCode | modifiers;
        }

		private static Key Find(swf.Keys key)
		{
			Key mapped;
			if (keymap.TryGetValue(key, out mapped)) return mapped;
			else return Key.None;
		}
		
		public static swf.Keys Find(Key key)
		{
			swf.Keys mapped;
			if (inverse.TryGetValue(key, out mapped)) return mapped;
			else return swf.Keys.None;
		}
		
		public static swf.Keys Convert(Key key)
		{
			var keys = key.ToString()
                  .Split(new[] { ", " }, StringSplitOptions.None)
                  .Select(v => (Key)Enum.Parse(typeof(Key), v));
			swf.Keys ret = swf.Keys.None;
			foreach (var val in keys)
			{
				ret |= Find(val);
			}
			return ret;
		}
		
		public static string KeyToString(Key key)
		{
			if (key != Key.None)
			{
				string val = string.Empty;
				Key modifier = (key & Key.ModifierMask);
				if (modifier != Key.None) val += modifier.ToString();
				Key mainKey = (key & Key.KeyMask);
				if (mainKey != Key.None)
				{
					if (val.Length > 0) val += "+";
					val += mainKey.ToString();
				}
				return val;
			}
			return string.Empty;
		}
		

		static KeyMap()
		{
			keymap.Add(swf.Keys.A, Key.A);
			keymap.Add(swf.Keys.B, Key.B);
			keymap.Add(swf.Keys.C, Key.C);
			keymap.Add(swf.Keys.D, Key.D);
			keymap.Add(swf.Keys.E, Key.E);
			keymap.Add(swf.Keys.F, Key.F);
			keymap.Add(swf.Keys.G, Key.G);
			keymap.Add(swf.Keys.H, Key.H);
			keymap.Add(swf.Keys.I, Key.I);
			keymap.Add(swf.Keys.J, Key.J);
			keymap.Add(swf.Keys.K, Key.K);
			keymap.Add(swf.Keys.L, Key.L);
			keymap.Add(swf.Keys.M, Key.M);
			keymap.Add(swf.Keys.N, Key.N);
			keymap.Add(swf.Keys.O, Key.O);
			keymap.Add(swf.Keys.P, Key.P);
			keymap.Add(swf.Keys.Q, Key.Q);
			keymap.Add(swf.Keys.R, Key.R);
			keymap.Add(swf.Keys.S, Key.S);
			keymap.Add(swf.Keys.T, Key.T);
			keymap.Add(swf.Keys.U, Key.U);
			keymap.Add(swf.Keys.V, Key.V);
			keymap.Add(swf.Keys.W, Key.W);
			keymap.Add(swf.Keys.X, Key.X);
			keymap.Add(swf.Keys.Y, Key.Y);
			keymap.Add(swf.Keys.Z, Key.Z);
			keymap.Add(swf.Keys.F1, Key.F1);
			keymap.Add(swf.Keys.F2, Key.F2);
			keymap.Add(swf.Keys.F3, Key.F3);
			keymap.Add(swf.Keys.F4, Key.F4);
			keymap.Add(swf.Keys.F5, Key.F5);
			keymap.Add(swf.Keys.F6, Key.F6);
			keymap.Add(swf.Keys.F7, Key.F7);
			keymap.Add(swf.Keys.F8, Key.F8);
			keymap.Add(swf.Keys.F9, Key.F9);
			keymap.Add(swf.Keys.F10, Key.F10);
			keymap.Add(swf.Keys.F11, Key.F11);
			keymap.Add(swf.Keys.F12, Key.F12);
			keymap.Add(swf.Keys.D0, Key.D0);
			keymap.Add(swf.Keys.D1, Key.D1);
			keymap.Add(swf.Keys.D2, Key.D2);
			keymap.Add(swf.Keys.D3, Key.D3);
			keymap.Add(swf.Keys.D4, Key.D4);
			keymap.Add(swf.Keys.D5, Key.D5);
			keymap.Add(swf.Keys.D6, Key.D6);
			keymap.Add(swf.Keys.D7, Key.D7);
			keymap.Add(swf.Keys.D8, Key.D8);
			keymap.Add(swf.Keys.D9, Key.D9);
			keymap.Add(swf.Keys.Space, Key.Space);
			keymap.Add(swf.Keys.Up, Key.Up);
			keymap.Add(swf.Keys.Down, Key.Down);
			keymap.Add(swf.Keys.Left, Key.Left);
			keymap.Add(swf.Keys.Right, Key.Right);
			keymap.Add(swf.Keys.PageDown, Key.PageDown);
			keymap.Add(swf.Keys.PageUp, Key.PageUp);
			keymap.Add(swf.Keys.Home, Key.Home);
			keymap.Add(swf.Keys.End, Key.End);
			keymap.Add(swf.Keys.Alt, Key.Alt);
			keymap.Add(swf.Keys.Control, Key.Control);
			keymap.Add(swf.Keys.Shift, Key.Shift);
			keymap.Add(swf.Keys.Menu, Key.Menu);
			keymap.Add(swf.Keys.Escape, Key.Escape);
			keymap.Add(swf.Keys.Delete, Key.Delete);
			keymap.Add(swf.Keys.Back, Key.Backspace);
			keymap.Add(swf.Keys.Divide, Key.Divide);
			keymap.Add(swf.Keys.Enter, Key.Enter);
			keymap.Add(swf.Keys.Insert, Key.Insert);
            keymap.Add(swf.Keys.OemPeriod, Key.Period);
            keymap.Add(swf.Keys.Tab, Key.Tab);
			
			foreach (var entry in keymap)
			{
				inverse.Add(entry.Value, entry.Key);
			}
		}
	}
}

