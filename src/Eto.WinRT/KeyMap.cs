using Eto.Forms;
using System.Collections.Generic;
using swi = Windows.System;

namespace Eto.WinRT
{
	/// <summary>
	/// Xaml platform key map.
	/// </summary>
	/// <copyright>(c) 2014 by Vivek Jhaveri</copyright>
	/// <copyright>(c) 2012-2014 by Curtis Wensley</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	public static class KeyMap
	{
		static readonly Dictionary<swi.VirtualKey, Keys> keymap = new Dictionary<swi.VirtualKey, Keys> ();
		static readonly Dictionary<Keys, swi.VirtualKey> inverse = new Dictionary<Keys, swi.VirtualKey> ();

		public static Keys Find (swi.VirtualKey key)
		{
			Keys mapped;
			return keymap.TryGetValue(key, out mapped) ? mapped : Keys.None;
		}

#if XAML_TODO
		public static Keys Convert (swi.VirtualKey key, swi.ModifierKeys modifier)
		{
			Keys ret = Find(key);

			if (modifier.HasFlag (swi.ModifierKeys.Alt)) ret |= Keys.Alt;
			if (modifier.HasFlag (swi.ModifierKeys.Control)) ret |= Keys.Control;
			if (modifier.HasFlag (swi.ModifierKeys.Shift)) ret |= Keys.Shift;
			if (modifier.HasFlag (swi.ModifierKeys.Windows)) ret |= Keys.Application;

			return ret;
		}
#endif

		public static swi.VirtualKey Find (Keys key)
		{
			swi.VirtualKey mapped;
			return inverse.TryGetValue(key, out mapped) ? mapped : swi.VirtualKey.None;
		}

		public static swi.VirtualKey ConvertKey (Keys key)
		{
			return Find(key & Keys.KeyMask);
		}

#if XAML_TODO
		public static swi.ModifierKeys ConvertModifier (Keys key)
		{
			key &= Keys.ModifierMask;
			swi.ModifierKeys val = swi.ModifierKeys.None;

			if (key.HasFlag (Keys.Alt)) val |= swi.ModifierKeys.Alt;
			if (key.HasFlag (Keys.Control)) val |= swi.ModifierKeys.Control;
			if (key.HasFlag (Keys.Shift)) val |= swi.ModifierKeys.Shift;
			if (key.HasFlag (Keys.Application)) val |= swi.ModifierKeys.Windows;
			return val;
		}
#endif

		static KeyMap ()
		{
			keymap.Add (swi.VirtualKey.A, Keys.A);
			keymap.Add (swi.VirtualKey.B, Keys.B);
			keymap.Add (swi.VirtualKey.C, Keys.C);
			keymap.Add (swi.VirtualKey.D, Keys.D);
			keymap.Add (swi.VirtualKey.E, Keys.E);
			keymap.Add (swi.VirtualKey.F, Keys.F);
			keymap.Add (swi.VirtualKey.G, Keys.G);
			keymap.Add (swi.VirtualKey.H, Keys.H);
			keymap.Add (swi.VirtualKey.I, Keys.I);
			keymap.Add (swi.VirtualKey.J, Keys.J);
			keymap.Add (swi.VirtualKey.K, Keys.K);
			keymap.Add (swi.VirtualKey.L, Keys.L);
			keymap.Add (swi.VirtualKey.M, Keys.M);
			keymap.Add (swi.VirtualKey.N, Keys.N);
			keymap.Add (swi.VirtualKey.O, Keys.O);
			keymap.Add (swi.VirtualKey.P, Keys.P);
			keymap.Add (swi.VirtualKey.Q, Keys.Q);
			keymap.Add (swi.VirtualKey.R, Keys.R);
			keymap.Add (swi.VirtualKey.S, Keys.S);
			keymap.Add (swi.VirtualKey.T, Keys.T);
			keymap.Add (swi.VirtualKey.U, Keys.U);
			keymap.Add (swi.VirtualKey.V, Keys.V);
			keymap.Add (swi.VirtualKey.W, Keys.W);
			keymap.Add (swi.VirtualKey.X, Keys.X);
			keymap.Add (swi.VirtualKey.Y, Keys.Y);
			keymap.Add (swi.VirtualKey.Z, Keys.Z);
			keymap.Add (swi.VirtualKey.F1, Keys.F1);
			keymap.Add (swi.VirtualKey.F2, Keys.F2);
			keymap.Add (swi.VirtualKey.F3, Keys.F3);
			keymap.Add (swi.VirtualKey.F4, Keys.F4);
			keymap.Add (swi.VirtualKey.F5, Keys.F5);
			keymap.Add (swi.VirtualKey.F6, Keys.F6);
			keymap.Add (swi.VirtualKey.F7, Keys.F7);
			keymap.Add (swi.VirtualKey.F8, Keys.F8);
			keymap.Add (swi.VirtualKey.F9, Keys.F9);
			keymap.Add (swi.VirtualKey.F10, Keys.F10);
			keymap.Add (swi.VirtualKey.F11, Keys.F11);
			keymap.Add (swi.VirtualKey.F12, Keys.F12);
			keymap.Add (swi.VirtualKey.Number0, Keys.D0);
			keymap.Add (swi.VirtualKey.Number1, Keys.D1);
			keymap.Add (swi.VirtualKey.Number2, Keys.D2);
			keymap.Add (swi.VirtualKey.Number3, Keys.D3);
			keymap.Add (swi.VirtualKey.Number4, Keys.D4);
			keymap.Add (swi.VirtualKey.Number5, Keys.D5);
			keymap.Add (swi.VirtualKey.Number6, Keys.D6);
			keymap.Add (swi.VirtualKey.Number7, Keys.D7);
			keymap.Add (swi.VirtualKey.Number8, Keys.D8);
			keymap.Add (swi.VirtualKey.Number9, Keys.D9);
			keymap.Add (swi.VirtualKey.Space, Keys.Space);
			keymap.Add (swi.VirtualKey.Up, Keys.Up);
			keymap.Add (swi.VirtualKey.Down, Keys.Down);
			keymap.Add (swi.VirtualKey.Left, Keys.Left);
			keymap.Add (swi.VirtualKey.Right, Keys.Right);
			keymap.Add (swi.VirtualKey.PageDown, Keys.PageDown);
			keymap.Add (swi.VirtualKey.PageUp, Keys.PageUp);
			keymap.Add (swi.VirtualKey.Home, Keys.Home);
			keymap.Add (swi.VirtualKey.End, Keys.End);
			/*
			keymap.Add (swi.VirtualKey.LeftAlt, Keys.Alt);
			keymap.Add (swi.VirtualKey.RightAlt, Keys.Alt);
			keymap.Add (swi.VirtualKey.LeftCtrl, Keys.Control);
			keymap.Add (swi.VirtualKey.LeftCtrl, Keys.Control);
			keymap.Add (swi.VirtualKey.LeftShift, Keys.Shift);
			keymap.Add (swi.VirtualKey.RightShift, Keys.Shift);
			keymap.Add (swi.VirtualKey.LWin, Keys.Menu);
			keymap.Add (swi.VirtualKey.RWin, Keys.Menu);
			 */
			keymap.Add (swi.VirtualKey.Escape, Keys.Escape);
			keymap.Add (swi.VirtualKey.Delete, Keys.Delete);
			keymap.Add (swi.VirtualKey.Back, Keys.Backspace);
			keymap.Add (swi.VirtualKey.Divide, Keys.Divide);
			keymap.Add (swi.VirtualKey.Enter, Keys.Enter);
			keymap.Add (swi.VirtualKey.Insert, Keys.Insert);
			keymap.Add (swi.VirtualKey.Tab, Keys.Tab);
#if TODO_XAML
			keymap.Add (swi.VirtualKey.Apps, Keys.ContextMenu);
#endif
			foreach (var entry in keymap) {
				inverse.Add (entry.Value, entry.Key);
			}
		}
	}
}

