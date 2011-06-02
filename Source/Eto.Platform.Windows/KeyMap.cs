using System;
using SWF = System.Windows.Forms;
using Eto.Forms;
using System.Collections.Generic;
using System.Linq;

namespace Eto.Platform.Windows
{
	public static class KeyMap
	{
		static Dictionary<SWF.Keys, Key> keymap = new Dictionary<SWF.Keys, Key>();
		static Dictionary<Key, SWF.Keys> inverse = new Dictionary<Key, SWF.Keys>();
		
		public static Key Find(SWF.Keys key)
		{
			Key mapped;
			if (keymap.TryGetValue(key, out mapped)) return mapped;
			else return Key.None;
		}
		
		public static Key Convert(SWF.Keys key)
		{
			var keys = key.ToString()
                  .Split(new[] { ", " }, StringSplitOptions.None)
                  .Select(v => (SWF.Keys)Enum.Parse(typeof(SWF.Keys), v));
			Key ret = Key.None;
			foreach (var val in keys)
			{
				ret |= Find(val);
			}
			return ret;
		}
		

		public static SWF.Keys Find(Key key)
		{
			SWF.Keys mapped;
			if (inverse.TryGetValue(key, out mapped)) return mapped;
			else return SWF.Keys.None;
		}
		
		public static SWF.Keys Convert(Key key)
		{
			var keys = key.ToString()
                  .Split(new[] { ", " }, StringSplitOptions.None)
                  .Select(v => (Key)Enum.Parse(typeof(Key), v));
			SWF.Keys ret = SWF.Keys.None;
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
			keymap.Add(SWF.Keys.A, Key.A);
			keymap.Add(SWF.Keys.B, Key.B);
			keymap.Add(SWF.Keys.C, Key.C);
			keymap.Add(SWF.Keys.D, Key.D);
			keymap.Add(SWF.Keys.E, Key.E);
			keymap.Add(SWF.Keys.F, Key.F);
			keymap.Add(SWF.Keys.G, Key.G);
			keymap.Add(SWF.Keys.H, Key.H);
			keymap.Add(SWF.Keys.I, Key.I);
			keymap.Add(SWF.Keys.J, Key.J);
			keymap.Add(SWF.Keys.K, Key.K);
			keymap.Add(SWF.Keys.L, Key.L);
			keymap.Add(SWF.Keys.M, Key.M);
			keymap.Add(SWF.Keys.N, Key.N);
			keymap.Add(SWF.Keys.O, Key.O);
			keymap.Add(SWF.Keys.P, Key.P);
			keymap.Add(SWF.Keys.Q, Key.Q);
			keymap.Add(SWF.Keys.R, Key.R);
			keymap.Add(SWF.Keys.S, Key.S);
			keymap.Add(SWF.Keys.T, Key.T);
			keymap.Add(SWF.Keys.U, Key.U);
			keymap.Add(SWF.Keys.V, Key.V);
			keymap.Add(SWF.Keys.W, Key.W);
			keymap.Add(SWF.Keys.X, Key.X);
			keymap.Add(SWF.Keys.Y, Key.Y);
			keymap.Add(SWF.Keys.Z, Key.Z);
			keymap.Add(SWF.Keys.F1, Key.F1);
			keymap.Add(SWF.Keys.F2, Key.F2);
			keymap.Add(SWF.Keys.F3, Key.F3);
			keymap.Add(SWF.Keys.F4, Key.F4);
			keymap.Add(SWF.Keys.F5, Key.F5);
			keymap.Add(SWF.Keys.F6, Key.F6);
			keymap.Add(SWF.Keys.F7, Key.F7);
			keymap.Add(SWF.Keys.F8, Key.F8);
			keymap.Add(SWF.Keys.F9, Key.F9);
			keymap.Add(SWF.Keys.F10, Key.F10);
			keymap.Add(SWF.Keys.F11, Key.F11);
			keymap.Add(SWF.Keys.F12, Key.F12);
			keymap.Add(SWF.Keys.D0, Key.D0);
			keymap.Add(SWF.Keys.D1, Key.D1);
			keymap.Add(SWF.Keys.D2, Key.D2);
			keymap.Add(SWF.Keys.D3, Key.D3);
			keymap.Add(SWF.Keys.D4, Key.D4);
			keymap.Add(SWF.Keys.D5, Key.D5);
			keymap.Add(SWF.Keys.D6, Key.D6);
			keymap.Add(SWF.Keys.D7, Key.D7);
			keymap.Add(SWF.Keys.D8, Key.D8);
			keymap.Add(SWF.Keys.D9, Key.D9);
			keymap.Add(SWF.Keys.Space, Key.Space);
			keymap.Add(SWF.Keys.Up, Key.Up);
			keymap.Add(SWF.Keys.Down, Key.Down);
			keymap.Add(SWF.Keys.Left, Key.Left);
			keymap.Add(SWF.Keys.Right, Key.Right);
			keymap.Add(SWF.Keys.PageDown, Key.PageDown);
			keymap.Add(SWF.Keys.PageUp, Key.PageUp);
			keymap.Add(SWF.Keys.Home, Key.Home);
			keymap.Add(SWF.Keys.End, Key.End);
			keymap.Add(SWF.Keys.Alt, Key.Alt);
			keymap.Add(SWF.Keys.Control, Key.Control);
			keymap.Add(SWF.Keys.Shift, Key.Shift);
			keymap.Add(SWF.Keys.Menu, Key.Menu);
			keymap.Add(SWF.Keys.Escape, Key.Escape);
			keymap.Add(SWF.Keys.Delete, Key.Delete);
			keymap.Add(SWF.Keys.Back, Key.Backspace);
			keymap.Add(SWF.Keys.Divide, Key.Divide);
			keymap.Add(SWF.Keys.Enter, Key.Enter);
			keymap.Add(SWF.Keys.Insert, Key.Insert);
			
			foreach (var entry in keymap)
			{
				inverse.Add(entry.Value, entry.Key);
			}
		}
	}
}

