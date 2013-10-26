using System.Collections.Generic;
using Eto.Forms;
using MonoMac.AppKit;

namespace Eto.Platform.Mac
{
	public static class KeyMap
	{
		static Dictionary<ushort, Key> keymap = new Dictionary<ushort, Key> ();
		static Dictionary<Key, string> inverse = new Dictionary<Key, string> ();

		public static Key MapKey (ushort key)
		{
			Key value;
			if (keymap.TryGetValue (key, out value))
				return value;
			return Key.None;
		}
		
		enum KeyCharacters {
			NSParagraphSeparatorCharacter = 0x2029,
			NSLineSeparatorCharacter      = 0x2028,
			NSTabCharacter                = 0x0009,
			NSFormFeedCharacter           = 0x000c,
			NSNewlineCharacter            = 0x000a,
			NSCarriageReturnCharacter     = 0x000d,
			NSEnterCharacter              = 0x0003,
			NSBackspaceCharacter          = 0x0008,
			NSBackTabCharacter            = 0x0019,
			NSDeleteCharacter             = 0x007f
		};

		public static Key Convert (string keyEquivalent, NSEventModifierMask modifier)
		{
			return Key.None;
		}

		public static string KeyEquivalent (Key key)
		{
			string value;
			if (inverse.TryGetValue (key & Key.KeyMask, out value))
				return value;
			return string.Empty;
		}

		public static NSEventModifierMask KeyEquivalentModifierMask (Key key)
		{
			key &= Key.ModifierMask;
			var mask = (NSEventModifierMask)0;
			if ((key & Key.Shift) > 0)
				mask |= NSEventModifierMask.ShiftKeyMask;
			if ((key & Key.Alt) > 0)
				mask |= NSEventModifierMask.AlternateKeyMask;
			if ((key & Key.Control) > 0)
				mask |= NSEventModifierMask.ControlKeyMask;
			if ((key & Key.Application) > 0)
				mask |= NSEventModifierMask.CommandKeyMask;
			return mask;
		}

		public static Key GetModifiers (NSEvent theEvent)
		{
			Key key = Key.None;
			if ((theEvent.ModifierFlags & NSEventModifierMask.ControlKeyMask) != 0)
				key |= Key.Control;
			if ((theEvent.ModifierFlags & NSEventModifierMask.CommandKeyMask) != 0)
				key |= Key.Application;
			if ((theEvent.ModifierFlags & NSEventModifierMask.ShiftKeyMask) != 0)
				key |= Key.Shift;
			if ((theEvent.ModifierFlags & NSEventModifierMask.AlternateKeyMask) != 0)
				key |= Key.Alt;
			return key;
		}
		
		static KeyMap ()
		{
			keymap.Add(0, Key.A);
			keymap.Add(11, Key.B);
			keymap.Add(8, Key.C);
			keymap.Add(2, Key.D);
			keymap.Add(14, Key.E);
			keymap.Add(3, Key.F);
			keymap.Add(5, Key.G);
			keymap.Add(4, Key.H);
			keymap.Add(34, Key.I);
			keymap.Add(38, Key.J);
			keymap.Add(40, Key.K);
			keymap.Add(37, Key.L);
			keymap.Add(46, Key.M);
			keymap.Add(45, Key.N);
			keymap.Add(31, Key.O);
			keymap.Add(35, Key.P);
			keymap.Add(12, Key.Q);
			keymap.Add(15, Key.R);
			keymap.Add(1, Key.S);
			keymap.Add(17, Key.T);
			keymap.Add(32, Key.U);
			keymap.Add(9, Key.V);
			keymap.Add(13, Key.W);
			keymap.Add(7, Key.X);
			keymap.Add(16, Key.Y);
			keymap.Add(6, Key.Z);
			keymap.Add(18, Key.D1);
			keymap.Add(19, Key.D2);
			keymap.Add(20, Key.D3);
			keymap.Add(21, Key.D4);
			keymap.Add(23, Key.D5);
			keymap.Add(22, Key.D6);
			keymap.Add(26, Key.D7);
			keymap.Add(28, Key.D8);
			keymap.Add(25, Key.D9);
			keymap.Add(29, Key.D0);
			keymap.Add(122, Key.F1);
			keymap.Add(120, Key.F2);
			keymap.Add(99, Key.F3);
			keymap.Add(118, Key.F4);
			keymap.Add(96, Key.F5);
			keymap.Add(97, Key.F6);
			keymap.Add(98, Key.F7);
			keymap.Add(100, Key.F8);
			keymap.Add(101, Key.F9);
			keymap.Add(109, Key.F10);
			keymap.Add(103, Key.F11);
			keymap.Add(111, Key.F12);
			keymap.Add(50, Key.Grave);
			keymap.Add(27, Key.Minus);
			keymap.Add(24, Key.Equal);
			keymap.Add(42, Key.Backslash);
			keymap.Add(49, Key.Space);
			//keymap.Add(30, Key.]);
			//keymap.Add(33, Key.[);
			keymap.Add(39, Key.Quote);
			keymap.Add(41, Key.Semicolon);
			keymap.Add(44, Key.ForwardSlash);
			keymap.Add(47, Key.Period);
			keymap.Add(43, Key.Comma);
			keymap.Add(36, Key.Enter);
			keymap.Add(48, Key.Tab);
			//keymap.Add(76, Key.Return);
			keymap.Add(53, Key.Escape);

			keymap.Add(76, Key.Insert);
			keymap.Add(51, Key.Backspace);
			keymap.Add(117, Key.Delete);
			
			keymap.Add(125, Key.Down);
			keymap.Add(126, Key.Up);
			keymap.Add(123, Key.Left);
			keymap.Add(124, Key.Right);
			
			keymap.Add(116, Key.PageUp);
			keymap.Add(121, Key.PageDown);
			keymap.Add(119, Key.End);
			keymap.Add(115, Key.Home);
			keymap.Add(110, Key.ContextMenu);

			
			inverse.Add (Key.A, "a");
			inverse.Add (Key.B, "b");
			inverse.Add (Key.C, "c");
			inverse.Add (Key.D, "d");
			inverse.Add (Key.E, "e");
			inverse.Add (Key.F, "f");
			inverse.Add (Key.G, "g");
			inverse.Add (Key.H, "h");
			inverse.Add (Key.I, "i");
			inverse.Add (Key.J, "j");
			inverse.Add (Key.K, "k");
			inverse.Add (Key.L, "l");
			inverse.Add (Key.M, "m");
			inverse.Add (Key.N, "n");
			inverse.Add (Key.O, "o");
			inverse.Add (Key.P, "p");
			inverse.Add (Key.Q, "q");
			inverse.Add (Key.R, "r");
			inverse.Add (Key.S, "s");
			inverse.Add (Key.T, "t");
			inverse.Add (Key.U, "u");
			inverse.Add (Key.V, "v");
			inverse.Add (Key.W, "w");
			inverse.Add (Key.X, "x");
			inverse.Add (Key.Y, "y");
			inverse.Add (Key.Z, "z");
			inverse.Add (Key.Period, ".");
			inverse.Add (Key.Comma, ",");
			inverse.Add (Key.Space, " ");
			inverse.Add (Key.Backslash, "\\");
			inverse.Add (Key.ForwardSlash, "/");
			inverse.Add (Key.Equal, "=");
			inverse.Add (Key.Grave, "`");
			inverse.Add (Key.Minus, "-");
			inverse.Add (Key.Semicolon, ";");
			inverse.Add (Key.Up, ((char)NSKey.UpArrow).ToString());
			inverse.Add (Key.Down, ((char)NSKey.DownArrow).ToString());
			inverse.Add (Key.Right, ((char)NSKey.RightArrow).ToString());
			inverse.Add (Key.Left, ((char)NSKey.LeftArrow).ToString());
			inverse.Add (Key.Home, ((char)NSKey.Home).ToString());
			inverse.Add (Key.End, ((char)NSKey.End).ToString());
			inverse.Add (Key.Insert, ((char)NSKey.Insert).ToString());
			inverse.Add (Key.Delete, ((char)KeyCharacters.NSDeleteCharacter).ToString());
			inverse.Add (Key.Backspace, ((char)KeyCharacters.NSBackspaceCharacter).ToString());
			inverse.Add (Key.Tab, ((char)KeyCharacters.NSTabCharacter).ToString());
			inverse.Add (Key.D0, "0");
			inverse.Add (Key.D1, "1");
			inverse.Add (Key.D2, "2");
			inverse.Add (Key.D3, "3");
			inverse.Add (Key.D4, "4");
			inverse.Add (Key.D5, "5");
			inverse.Add (Key.D6, "6");
			inverse.Add (Key.D7, "7");
			inverse.Add (Key.D8, "8");
			inverse.Add (Key.D9, "9");
			inverse.Add (Key.F1, ((char)NSKey.F1).ToString());
			inverse.Add (Key.F2, ((char)NSKey.F2).ToString());
			inverse.Add (Key.F3, ((char)NSKey.F3).ToString());
			inverse.Add (Key.F4, ((char)NSKey.F4).ToString());
			inverse.Add (Key.F5, ((char)NSKey.F5).ToString());
			inverse.Add (Key.F6, ((char)NSKey.F6).ToString());
			inverse.Add (Key.F7, ((char)NSKey.F7).ToString());
			inverse.Add (Key.F8, ((char)NSKey.F8).ToString());
			inverse.Add (Key.F9, ((char)NSKey.F9).ToString());
			inverse.Add (Key.F10, ((char)NSKey.F10).ToString());
			inverse.Add (Key.F11, ((char)NSKey.F11).ToString());
			inverse.Add (Key.F12, ((char)NSKey.F12).ToString());
		}
	}
}

