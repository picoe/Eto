using Eto.Forms;
using System.Collections.Generic;

namespace Eto.GtkSharp
{
	public static class KeyMap
	{
		static Dictionary<Gdk.Key, Keys> _map;
		static Dictionary<Keys, Gdk.Key> _inverseMap;

		static Dictionary<Gdk.Key, Keys> Map => _map ?? (_map = GetMap());
		static Dictionary<Keys, Gdk.Key> InverseMap => _inverseMap ?? (_inverseMap = GetInverseMap());

		public static Keys ToEto (this Gdk.Key gkey)
		{
			Keys key;
			return Map.TryGetValue(gkey, out key) ? key : Keys.None;
		}
		
		public static Keys ToEtoKey (this Gdk.ModifierType modifier)
		{
			Keys result = Keys.None;
			if (modifier.HasFlag(Gdk.ModifierType.Mod1Mask)) result |= Keys.Alt;
			if (modifier.HasFlag(Gdk.ModifierType.ControlMask)) result |= Keys.Control;
			if (modifier.HasFlag(Gdk.ModifierType.SuperMask)) result |= Keys.Application;
			if (modifier.HasFlag(Gdk.ModifierType.ShiftMask)) result |= Keys.Shift;
			return result;
		}

		public static Gdk.Key ToGdkKey (this Keys key)
		{
			Gdk.Key result;
			if (InverseMap.TryGetValue(key & Keys.KeyMask, out result)) return result;
			return (Gdk.Key)0;
		}

		public static Gdk.ModifierType ToGdkModifier (this Keys key)
		{
			Gdk.ModifierType result = Gdk.ModifierType.None;
			if (key.HasFlag(Keys.Alt)) result |= Gdk.ModifierType.Mod1Mask;
			if (key.HasFlag(Keys.Control)) result |= Gdk.ModifierType.ControlMask;
			if (key.HasFlag(Keys.Application)) result |= Gdk.ModifierType.SuperMask;
			if (key.HasFlag(Keys.Shift)) result |= Gdk.ModifierType.ShiftMask;
			return result;
		}

		static Dictionary<Gdk.Key, Keys> GetMap()
		{
			var keymap = new Dictionary<Gdk.Key, Keys>();
			keymap.Add(Gdk.Key.A, Keys.A);
			keymap.Add(Gdk.Key.B, Keys.B);
			keymap.Add(Gdk.Key.C, Keys.C);
			keymap.Add(Gdk.Key.D, Keys.D);
			keymap.Add(Gdk.Key.E, Keys.E);
			keymap.Add(Gdk.Key.F, Keys.F);
			keymap.Add(Gdk.Key.G, Keys.G);
			keymap.Add(Gdk.Key.H, Keys.H);
			keymap.Add(Gdk.Key.I, Keys.I);
			keymap.Add(Gdk.Key.J, Keys.J);
			keymap.Add(Gdk.Key.K, Keys.K);
			keymap.Add(Gdk.Key.L, Keys.L);
			keymap.Add(Gdk.Key.M, Keys.M);
			keymap.Add(Gdk.Key.N, Keys.N);
			keymap.Add(Gdk.Key.O, Keys.O);
			keymap.Add(Gdk.Key.P, Keys.P);
			keymap.Add(Gdk.Key.Q, Keys.Q);
			keymap.Add(Gdk.Key.R, Keys.R);
			keymap.Add(Gdk.Key.S, Keys.S);
			keymap.Add(Gdk.Key.T, Keys.T);
			keymap.Add(Gdk.Key.U, Keys.U);
			keymap.Add(Gdk.Key.V, Keys.V);
			keymap.Add(Gdk.Key.W, Keys.W);
			keymap.Add(Gdk.Key.X, Keys.X);
			keymap.Add(Gdk.Key.Y, Keys.Y);
			keymap.Add(Gdk.Key.Z, Keys.Z);
			keymap.Add(Gdk.Key.F1, Keys.F1);
			keymap.Add(Gdk.Key.F2, Keys.F2);
			keymap.Add(Gdk.Key.F3, Keys.F3);
			keymap.Add(Gdk.Key.F4, Keys.F4);
			keymap.Add(Gdk.Key.F5, Keys.F5);
			keymap.Add(Gdk.Key.F6, Keys.F6);
			keymap.Add(Gdk.Key.F7, Keys.F7);
			keymap.Add(Gdk.Key.F8, Keys.F8);
			keymap.Add(Gdk.Key.F9, Keys.F9);
			keymap.Add(Gdk.Key.F10, Keys.F10);
			keymap.Add(Gdk.Key.F11, Keys.F11);
			keymap.Add(Gdk.Key.F12, Keys.F12);
			keymap.Add(Gdk.Key.Key_0, Keys.D0);
			keymap.Add(Gdk.Key.Key_1, Keys.D1);
			keymap.Add(Gdk.Key.Key_2, Keys.D2);
			keymap.Add(Gdk.Key.Key_3, Keys.D3);
			keymap.Add(Gdk.Key.Key_4, Keys.D4);
			keymap.Add(Gdk.Key.Key_5, Keys.D5);
			keymap.Add(Gdk.Key.Key_6, Keys.D6);
			keymap.Add(Gdk.Key.Key_7, Keys.D7);
			keymap.Add(Gdk.Key.Key_8, Keys.D8);
			keymap.Add(Gdk.Key.Key_9, Keys.D9);
			keymap.Add(Gdk.Key.Up, Keys.Up);
			keymap.Add(Gdk.Key.Down, Keys.Down);
			keymap.Add(Gdk.Key.Left, Keys.Left);
			keymap.Add(Gdk.Key.Right, Keys.Right);
			keymap.Add(Gdk.Key.Page_Down, Keys.PageDown);
			keymap.Add(Gdk.Key.Page_Up, Keys.PageUp);
			keymap.Add(Gdk.Key.Home, Keys.Home);
			keymap.Add(Gdk.Key.End, Keys.End);
			keymap.Add(Gdk.Key.space, Keys.Space);
			keymap.Add(Gdk.Key.Delete, Keys.Delete);
			keymap.Add(Gdk.Key.BackSpace, Keys.Backspace);
			keymap.Add(Gdk.Key.Insert, Keys.Insert);
			keymap.Add(Gdk.Key.Tab, Keys.Tab);
			keymap.Add(Gdk.Key.Escape, Keys.Escape);
			keymap.Add(Gdk.Key.Return, Keys.Enter);
			
			keymap.Add(Gdk.Key.period, Keys.Decimal);
			keymap.Add(Gdk.Key.comma, Keys.Comma);
			keymap.Add(Gdk.Key.equal, Keys.Equal);
			keymap.Add(Gdk.Key.minus, Keys.Minus);
			keymap.Add(Gdk.Key.backslash, Keys.Backslash);
			keymap.Add(Gdk.Key.slash, Keys.ForwardSlash);
			keymap.Add(Gdk.Key.division, Keys.Divide);
			//keymap.Add(Gdk.Key.dollar, Keys.Dollar);
			keymap.Add(Gdk.Key.Menu, Keys.ContextMenu);

			if (EtoEnvironment.Platform.IsMac)
			{
				// os x
				keymap.Add(Gdk.Key.KP_Enter, Keys.Insert);
				keymap.Add((Gdk.Key)0x1000010, Keys.ContextMenu);
			}
			keymap.Add(Gdk.Key.a, Keys.A);
			keymap.Add(Gdk.Key.b, Keys.B);
			keymap.Add(Gdk.Key.c, Keys.C);
			keymap.Add(Gdk.Key.d, Keys.D);
			keymap.Add(Gdk.Key.e, Keys.E);
			keymap.Add(Gdk.Key.f, Keys.F);
			keymap.Add(Gdk.Key.g, Keys.G);
			keymap.Add(Gdk.Key.h, Keys.H);
			keymap.Add(Gdk.Key.i, Keys.I);
			keymap.Add(Gdk.Key.j, Keys.J);
			keymap.Add(Gdk.Key.k, Keys.K);
			keymap.Add(Gdk.Key.l, Keys.L);
			keymap.Add(Gdk.Key.m, Keys.M);
			keymap.Add(Gdk.Key.n, Keys.N);
			keymap.Add(Gdk.Key.o, Keys.O);
			keymap.Add(Gdk.Key.p, Keys.P);
			keymap.Add(Gdk.Key.q, Keys.Q);
			keymap.Add(Gdk.Key.r, Keys.R);
			keymap.Add(Gdk.Key.s, Keys.S);
			keymap.Add(Gdk.Key.t, Keys.T);
			keymap.Add(Gdk.Key.u, Keys.U);
			keymap.Add(Gdk.Key.v, Keys.V);
			keymap.Add(Gdk.Key.w, Keys.W);
			keymap.Add(Gdk.Key.x, Keys.X);
			keymap.Add(Gdk.Key.y, Keys.Y);
			keymap.Add(Gdk.Key.z, Keys.Z);
			return keymap;
		}
		
		static Dictionary<Keys, Gdk.Key> GetInverseMap()
		{
			var inversekeymap = new Dictionary<Keys, Gdk.Key>();
			foreach (var val in Map)
			{
				if (!inversekeymap.ContainsKey(val.Value))
					inversekeymap.Add(val.Value, val.Key);
			}
			return inversekeymap;
		}
	}
}

