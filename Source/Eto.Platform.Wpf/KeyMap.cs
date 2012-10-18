using System;
using Eto.Forms;
using System.Collections.Generic;
using System.Linq;
using swi = System.Windows.Input;

namespace Eto.Platform.Wpf
{
	public static class KeyMap
	{
		static Dictionary<swi.Key, Key> keymap = new Dictionary<swi.Key, Key> ();
		static Dictionary<Key, swi.Key> inverse = new Dictionary<Key, swi.Key> ();

		public static Key Find (swi.Key key)
		{
			Key mapped;
			if (keymap.TryGetValue (key, out mapped)) return mapped;
			else return Key.None;
		}

		public static Key Convert (swi.Key key, swi.ModifierKeys modifier)
		{
			var keys = key.ToString ()
				  .Split (new[] { ", " }, StringSplitOptions.None)
				  .Select (v => (swi.Key)Enum.Parse (typeof (swi.Key), v));
			Key ret = Key.None;
			foreach (var val in keys) {
				ret |= Find (val);
			}

			if (modifier.HasFlag (swi.ModifierKeys.Alt)) ret |= Key.Alt;
			if (modifier.HasFlag (swi.ModifierKeys.Control)) ret |= Key.Control;
			if (modifier.HasFlag (swi.ModifierKeys.Shift)) ret |= Key.Shift;
			if (modifier.HasFlag (swi.ModifierKeys.Windows)) ret |= Key.Application;

			return ret;
		}


		public static swi.Key Find (Key key)
		{
			swi.Key mapped;
			if (inverse.TryGetValue (key, out mapped)) return mapped;
			else return swi.Key.None;
		}

		public static swi.Key ConvertKey (Key key)
		{
			key &= Key.KeyMask;
			var keys = key.ToString ()
				  .Split (new[] { ", " }, StringSplitOptions.None)
				  .Select (v => (Key)Enum.Parse (typeof (Key), v));
			var ret = swi.Key.None;
			foreach (var val in keys) {
				ret |= Find (val);
			}
			return ret;
		}

		public static swi.ModifierKeys ConvertModifier (Key key)
		{
			key &= Key.ModifierMask;
			swi.ModifierKeys val = swi.ModifierKeys.None;

			if (key.HasFlag (Key.Alt)) val |= swi.ModifierKeys.Alt;
			if (key.HasFlag (Key.Control)) val |= swi.ModifierKeys.Control;
			if (key.HasFlag (Key.Shift)) val |= swi.ModifierKeys.Shift;
			if (key.HasFlag (Key.Application)) val |= swi.ModifierKeys.Windows;
			return val;
		}

		public static string KeyToString (Key key)
		{
			if (key != Key.None) {
				string val = string.Empty;
				Key modifier = (key & Key.ModifierMask);
				if (modifier != Key.None) val += modifier.ToString ();
				Key mainKey = (key & Key.KeyMask);
				if (mainKey != Key.None) {
					if (val.Length > 0) val += "+";
					val += mainKey.ToString ();
				}
				return val;
			}
			return string.Empty;
		}


		static KeyMap ()
		{
			keymap.Add (swi.Key.A, Key.A);
			keymap.Add (swi.Key.B, Key.B);
			keymap.Add (swi.Key.C, Key.C);
			keymap.Add (swi.Key.D, Key.D);
			keymap.Add (swi.Key.E, Key.E);
			keymap.Add (swi.Key.F, Key.F);
			keymap.Add (swi.Key.G, Key.G);
			keymap.Add (swi.Key.H, Key.H);
			keymap.Add (swi.Key.I, Key.I);
			keymap.Add (swi.Key.J, Key.J);
			keymap.Add (swi.Key.K, Key.K);
			keymap.Add (swi.Key.L, Key.L);
			keymap.Add (swi.Key.M, Key.M);
			keymap.Add (swi.Key.N, Key.N);
			keymap.Add (swi.Key.O, Key.O);
			keymap.Add (swi.Key.P, Key.P);
			keymap.Add (swi.Key.Q, Key.Q);
			keymap.Add (swi.Key.R, Key.R);
			keymap.Add (swi.Key.S, Key.S);
			keymap.Add (swi.Key.T, Key.T);
			keymap.Add (swi.Key.U, Key.U);
			keymap.Add (swi.Key.V, Key.V);
			keymap.Add (swi.Key.W, Key.W);
			keymap.Add (swi.Key.X, Key.X);
			keymap.Add (swi.Key.Y, Key.Y);
			keymap.Add (swi.Key.Z, Key.Z);
			keymap.Add (swi.Key.F1, Key.F1);
			keymap.Add (swi.Key.F2, Key.F2);
			keymap.Add (swi.Key.F3, Key.F3);
			keymap.Add (swi.Key.F4, Key.F4);
			keymap.Add (swi.Key.F5, Key.F5);
			keymap.Add (swi.Key.F6, Key.F6);
			keymap.Add (swi.Key.F7, Key.F7);
			keymap.Add (swi.Key.F8, Key.F8);
			keymap.Add (swi.Key.F9, Key.F9);
			keymap.Add (swi.Key.F10, Key.F10);
			keymap.Add (swi.Key.F11, Key.F11);
			keymap.Add (swi.Key.F12, Key.F12);
			keymap.Add (swi.Key.D0, Key.D0);
			keymap.Add (swi.Key.D1, Key.D1);
			keymap.Add (swi.Key.D2, Key.D2);
			keymap.Add (swi.Key.D3, Key.D3);
			keymap.Add (swi.Key.D4, Key.D4);
			keymap.Add (swi.Key.D5, Key.D5);
			keymap.Add (swi.Key.D6, Key.D6);
			keymap.Add (swi.Key.D7, Key.D7);
			keymap.Add (swi.Key.D8, Key.D8);
			keymap.Add (swi.Key.D9, Key.D9);
			keymap.Add (swi.Key.Space, Key.Space);
			keymap.Add (swi.Key.Up, Key.Up);
			keymap.Add (swi.Key.Down, Key.Down);
			keymap.Add (swi.Key.Left, Key.Left);
			keymap.Add (swi.Key.Right, Key.Right);
			keymap.Add (swi.Key.PageDown, Key.PageDown);
			keymap.Add (swi.Key.PageUp, Key.PageUp);
			keymap.Add (swi.Key.Home, Key.Home);
			keymap.Add (swi.Key.End, Key.End);
			/*
			keymap.Add (swi.Key.LeftAlt, Key.Alt);
			keymap.Add (swi.Key.RightAlt, Key.Alt);
			keymap.Add (swi.Key.LeftCtrl, Key.Control);
			keymap.Add (swi.Key.LeftCtrl, Key.Control);
			keymap.Add (swi.Key.LeftShift, Key.Shift);
			keymap.Add (swi.Key.RightShift, Key.Shift);
			keymap.Add (swi.Key.LWin, Key.Menu);
			keymap.Add (swi.Key.RWin, Key.Menu);
			 */
			keymap.Add (swi.Key.Escape, Key.Escape);
			keymap.Add (swi.Key.Delete, Key.Delete);
			keymap.Add (swi.Key.Back, Key.Backspace);
			keymap.Add (swi.Key.Divide, Key.Divide);
			keymap.Add (swi.Key.Enter, Key.Enter);
			keymap.Add (swi.Key.Insert, Key.Insert);
			keymap.Add (swi.Key.Tab, Key.Tab);

			foreach (var entry in keymap) {
				inverse.Add (entry.Value, entry.Key);
			}
		}
	}
}

