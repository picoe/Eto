using Eto.Forms;
using System.Collections.Generic;
using swi = System.Windows.Input;

namespace Eto.Wpf
{
	public static class KeyMap
	{
		static readonly Dictionary<swi.Key, Keys> keymap = new Dictionary<swi.Key, Keys>();
		static readonly Dictionary<Keys, swi.Key> inverse = new Dictionary<Keys, swi.Key>();

		public static Keys ToEto(this swi.Key key)
		{
			Keys mapped;
			return keymap.TryGetValue(key, out mapped) ? mapped : Keys.None;
		}

		public static Keys ToEto(this swi.ModifierKeys modifier)
		{
			Keys ret = Keys.None;

			if (modifier.HasFlag(swi.ModifierKeys.Alt)) ret |= Keys.Alt;
			if (modifier.HasFlag(swi.ModifierKeys.Control)) ret |= Keys.Control;
			if (modifier.HasFlag(swi.ModifierKeys.Shift)) ret |= Keys.Shift;
			if (modifier.HasFlag(swi.ModifierKeys.Windows)) ret |= Keys.Application;

			return ret;
		}

		public static Keys ToEtoWithModifier(this swi.Key key, swi.ModifierKeys modifier)
		{
			return key.ToEto() | modifier.ToEto();
		}

		public static swi.Key ToWpf(this Keys key)
		{
			swi.Key mapped;
			return inverse.TryGetValue(key, out mapped) ? mapped : swi.Key.None;
		}

		public static swi.Key ToWpfKey(this Keys key)
		{
			return ToWpf(key & Keys.KeyMask);
		}

		public static swi.ModifierKeys ToWpfModifier(this Keys key)
		{
			key &= Keys.ModifierMask;
			swi.ModifierKeys val = swi.ModifierKeys.None;

			if (key.HasFlag(Keys.Alt)) val |= swi.ModifierKeys.Alt;
			if (key.HasFlag(Keys.Control)) val |= swi.ModifierKeys.Control;
			if (key.HasFlag(Keys.Shift)) val |= swi.ModifierKeys.Shift;
			if (key.HasFlag(Keys.Application)) val |= swi.ModifierKeys.Windows;
			return val;
		}

		static KeyMap()
		{
			keymap.Add(swi.Key.A, Keys.A);
			keymap.Add(swi.Key.B, Keys.B);
			keymap.Add(swi.Key.C, Keys.C);
			keymap.Add(swi.Key.D, Keys.D);
			keymap.Add(swi.Key.E, Keys.E);
			keymap.Add(swi.Key.F, Keys.F);
			keymap.Add(swi.Key.G, Keys.G);
			keymap.Add(swi.Key.H, Keys.H);
			keymap.Add(swi.Key.I, Keys.I);
			keymap.Add(swi.Key.J, Keys.J);
			keymap.Add(swi.Key.K, Keys.K);
			keymap.Add(swi.Key.L, Keys.L);
			keymap.Add(swi.Key.M, Keys.M);
			keymap.Add(swi.Key.N, Keys.N);
			keymap.Add(swi.Key.O, Keys.O);
			keymap.Add(swi.Key.P, Keys.P);
			keymap.Add(swi.Key.Q, Keys.Q);
			keymap.Add(swi.Key.R, Keys.R);
			keymap.Add(swi.Key.S, Keys.S);
			keymap.Add(swi.Key.T, Keys.T);
			keymap.Add(swi.Key.U, Keys.U);
			keymap.Add(swi.Key.V, Keys.V);
			keymap.Add(swi.Key.W, Keys.W);
			keymap.Add(swi.Key.X, Keys.X);
			keymap.Add(swi.Key.Y, Keys.Y);
			keymap.Add(swi.Key.Z, Keys.Z);
			keymap.Add(swi.Key.F1, Keys.F1);
			keymap.Add(swi.Key.F2, Keys.F2);
			keymap.Add(swi.Key.F3, Keys.F3);
			keymap.Add(swi.Key.F4, Keys.F4);
			keymap.Add(swi.Key.F5, Keys.F5);
			keymap.Add(swi.Key.F6, Keys.F6);
			keymap.Add(swi.Key.F7, Keys.F7);
			keymap.Add(swi.Key.F8, Keys.F8);
			keymap.Add(swi.Key.F9, Keys.F9);
			keymap.Add(swi.Key.F10, Keys.F10);
			keymap.Add(swi.Key.F11, Keys.F11);
			keymap.Add(swi.Key.F12, Keys.F12);
			keymap.Add(swi.Key.D0, Keys.D0);
			keymap.Add(swi.Key.D1, Keys.D1);
			keymap.Add(swi.Key.D2, Keys.D2);
			keymap.Add(swi.Key.D3, Keys.D3);
			keymap.Add(swi.Key.D4, Keys.D4);
			keymap.Add(swi.Key.D5, Keys.D5);
			keymap.Add(swi.Key.D6, Keys.D6);
			keymap.Add(swi.Key.D7, Keys.D7);
			keymap.Add(swi.Key.D8, Keys.D8);
			keymap.Add(swi.Key.D9, Keys.D9);
			keymap.Add(swi.Key.Space, Keys.Space);
			keymap.Add(swi.Key.Up, Keys.Up);
			keymap.Add(swi.Key.Down, Keys.Down);
			keymap.Add(swi.Key.Left, Keys.Left);
			keymap.Add(swi.Key.Right, Keys.Right);
			keymap.Add(swi.Key.PageDown, Keys.PageDown);
			keymap.Add(swi.Key.PageUp, Keys.PageUp);
			keymap.Add(swi.Key.Home, Keys.Home);
			keymap.Add(swi.Key.End, Keys.End);
			/*
			keymap.Add (swi.Key.LeftAlt, Keys.Alt);
			keymap.Add (swi.Key.RightAlt, Keys.Alt);
			keymap.Add (swi.Key.LeftCtrl, Keys.Control);
			keymap.Add (swi.Key.LeftCtrl, Keys.Control);
			keymap.Add (swi.Key.LeftShift, Keys.Shift);
			keymap.Add (swi.Key.RightShift, Keys.Shift);
			keymap.Add (swi.Key.LWin, Keys.Menu);
			keymap.Add (swi.Key.RWin, Keys.Menu);
			 */
			keymap.Add(swi.Key.Escape, Keys.Escape);
			keymap.Add(swi.Key.Delete, Keys.Delete);
			keymap.Add(swi.Key.Back, Keys.Backspace);
			keymap.Add(swi.Key.Divide, Keys.Divide);
			keymap.Add(swi.Key.Enter, Keys.Enter);
			keymap.Add(swi.Key.Insert, Keys.Insert);
			keymap.Add(swi.Key.Tab, Keys.Tab);
			keymap.Add(swi.Key.Apps, Keys.ContextMenu);

			foreach (var entry in keymap)
			{
				inverse.Add(entry.Value, entry.Key);
			}
		}
	}
}

