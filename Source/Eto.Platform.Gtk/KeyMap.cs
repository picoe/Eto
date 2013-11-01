using Eto.Forms;
using System.Collections.Generic;

namespace Eto.Platform.GtkSharp
{
	public static class KeyMap
	{
		static readonly Dictionary<Gdk.Key, Key> keymap = new Dictionary<Gdk.Key, Key>();
		static readonly Dictionary<Key, Gdk.Key> inversekeymap = new Dictionary<Key, Gdk.Key>();

		public static Key ToEto (this Gdk.Key gkey)
		{
			Key key;
			return keymap.TryGetValue(gkey, out key) ? key : Key.None;
		}
		
		public static Key ToEtoKey (this Gdk.ModifierType modifier)
		{
			Key result = Key.None;
			if ((modifier & Gdk.ModifierType.Mod1Mask) > 0) result |= Key.Alt;
			if ((modifier & Gdk.ModifierType.ControlMask) > 0) result |= Key.Control;
			if ((modifier & Gdk.ModifierType.SuperMask) > 0) result |= Key.Application;
			if ((modifier & Gdk.ModifierType.ShiftMask) > 0) result |= Key.Shift;
			return result;
		}

		public static Gdk.Key ToGdkKey (this Key key)
		{
			Gdk.Key result;
			if (inversekeymap.TryGetValue(key & Key.KeyMask, out result)) return result;
			return (Gdk.Key)0;
		}

		public static Gdk.ModifierType ToGdkModifier (this Key key)
		{
			Gdk.ModifierType result = Gdk.ModifierType.None;
			if ((key & Key.Alt) > 0) result |= Gdk.ModifierType.Mod1Mask;
			if ((key & Key.Control) > 0) result |= Gdk.ModifierType.ControlMask;
			if ((key & Key.Application) > 0) result |= Gdk.ModifierType.SuperMask;
			if ((key & Key.Shift) > 0) result |= Gdk.ModifierType.ShiftMask;
			return result;
		}
		
		static KeyMap()
		{
			keymap.Add(Gdk.Key.A, Key.A);
			keymap.Add(Gdk.Key.B, Key.B);
			keymap.Add(Gdk.Key.C, Key.C);
			keymap.Add(Gdk.Key.D, Key.D);
			keymap.Add(Gdk.Key.E, Key.E);
			keymap.Add(Gdk.Key.F, Key.F);
			keymap.Add(Gdk.Key.G, Key.G);
			keymap.Add(Gdk.Key.H, Key.H);
			keymap.Add(Gdk.Key.I, Key.I);
			keymap.Add(Gdk.Key.J, Key.J);
			keymap.Add(Gdk.Key.K, Key.K);
			keymap.Add(Gdk.Key.L, Key.L);
			keymap.Add(Gdk.Key.M, Key.M);
			keymap.Add(Gdk.Key.N, Key.N);
			keymap.Add(Gdk.Key.O, Key.O);
			keymap.Add(Gdk.Key.P, Key.P);
			keymap.Add(Gdk.Key.Q, Key.Q);
			keymap.Add(Gdk.Key.R, Key.R);
			keymap.Add(Gdk.Key.S, Key.S);
			keymap.Add(Gdk.Key.T, Key.T);
			keymap.Add(Gdk.Key.U, Key.U);
			keymap.Add(Gdk.Key.V, Key.V);
			keymap.Add(Gdk.Key.W, Key.W);
			keymap.Add(Gdk.Key.X, Key.X);
			keymap.Add(Gdk.Key.Y, Key.Y);
			keymap.Add(Gdk.Key.Z, Key.Z);
			keymap.Add(Gdk.Key.F1, Key.F1);
			keymap.Add(Gdk.Key.F2, Key.F2);
			keymap.Add(Gdk.Key.F3, Key.F3);
			keymap.Add(Gdk.Key.F4, Key.F4);
			keymap.Add(Gdk.Key.F5, Key.F5);
			keymap.Add(Gdk.Key.F6, Key.F6);
			keymap.Add(Gdk.Key.F7, Key.F7);
			keymap.Add(Gdk.Key.F8, Key.F8);
			keymap.Add(Gdk.Key.F9, Key.F9);
			keymap.Add(Gdk.Key.F10, Key.F10);
			keymap.Add(Gdk.Key.F11, Key.F11);
			keymap.Add(Gdk.Key.F12, Key.F12);
			keymap.Add(Gdk.Key.Key_0, Key.D0);
			keymap.Add(Gdk.Key.Key_1, Key.D1);
			keymap.Add(Gdk.Key.Key_2, Key.D2);
			keymap.Add(Gdk.Key.Key_3, Key.D3);
			keymap.Add(Gdk.Key.Key_4, Key.D4);
			keymap.Add(Gdk.Key.Key_5, Key.D5);
			keymap.Add(Gdk.Key.Key_6, Key.D6);
			keymap.Add(Gdk.Key.Key_7, Key.D7);
			keymap.Add(Gdk.Key.Key_8, Key.D8);
			keymap.Add(Gdk.Key.Key_9, Key.D9);
			keymap.Add(Gdk.Key.Up, Key.Up);
			keymap.Add(Gdk.Key.Down, Key.Down);
			keymap.Add(Gdk.Key.Left, Key.Left);
			keymap.Add(Gdk.Key.Right, Key.Right);
			keymap.Add(Gdk.Key.Page_Down, Key.PageDown);
			keymap.Add(Gdk.Key.Page_Up, Key.PageUp);
			keymap.Add(Gdk.Key.Home, Key.Home);
			keymap.Add(Gdk.Key.End, Key.End);
			keymap.Add(Gdk.Key.space, Key.Space);
			keymap.Add(Gdk.Key.Delete, Key.Delete);
			keymap.Add(Gdk.Key.BackSpace, Key.Backspace);
			keymap.Add(Gdk.Key.Insert, Key.Insert);
			keymap.Add(Gdk.Key.Tab, Key.Tab);
			keymap.Add(Gdk.Key.Escape, Key.Escape);
			keymap.Add(Gdk.Key.Return, Key.Enter);
			
			keymap.Add(Gdk.Key.period, Key.Decimal);
			keymap.Add(Gdk.Key.comma, Key.Comma);
			keymap.Add(Gdk.Key.equal, Key.Equal);
			keymap.Add(Gdk.Key.minus, Key.Minus);
			keymap.Add(Gdk.Key.backslash, Key.Backslash);
			keymap.Add(Gdk.Key.slash, Key.ForwardSlash);
			keymap.Add(Gdk.Key.division, Key.Divide);
			//keymap.Add(Gdk.Key.dollar, Key.Dollar);
			keymap.Add(Gdk.Key.Menu, Key.ContextMenu);

			foreach (var val in keymap)
			{
				inversekeymap.Add(val.Value, val.Key);
			}

			keymap.Add((Gdk.Key)0x1000010, Key.ContextMenu); // os x
			keymap.Add(Gdk.Key.a, Key.A);
			keymap.Add(Gdk.Key.b, Key.B);
			keymap.Add(Gdk.Key.c, Key.C);
			keymap.Add(Gdk.Key.d, Key.D);
			keymap.Add(Gdk.Key.e, Key.E);
			keymap.Add(Gdk.Key.f, Key.F);
			keymap.Add(Gdk.Key.g, Key.G);
			keymap.Add(Gdk.Key.h, Key.H);
			keymap.Add(Gdk.Key.i, Key.I);
			keymap.Add(Gdk.Key.j, Key.J);
			keymap.Add(Gdk.Key.k, Key.K);
			keymap.Add(Gdk.Key.l, Key.L);
			keymap.Add(Gdk.Key.m, Key.M);
			keymap.Add(Gdk.Key.n, Key.N);
			keymap.Add(Gdk.Key.o, Key.O);
			keymap.Add(Gdk.Key.p, Key.P);
			keymap.Add(Gdk.Key.q, Key.Q);
			keymap.Add(Gdk.Key.r, Key.R);
			keymap.Add(Gdk.Key.s, Key.S);
			keymap.Add(Gdk.Key.t, Key.T);
			keymap.Add(Gdk.Key.u, Key.U);
			keymap.Add(Gdk.Key.v, Key.V);
			keymap.Add(Gdk.Key.w, Key.W);
			keymap.Add(Gdk.Key.x, Key.X);
			keymap.Add(Gdk.Key.y, Key.Y);
			keymap.Add(Gdk.Key.z, Key.Z);
			
		}
		
	}
}

