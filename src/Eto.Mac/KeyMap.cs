using System.Collections.Generic;
using Eto.Forms;
using System.Diagnostics;

#if XAMMAC2
using AppKit;
using Foundation;
using CoreGraphics;
using ObjCRuntime;
using CoreAnimation;
using CoreImage;
#else
using MonoMac.AppKit;
using MonoMac.Foundation;
using MonoMac.CoreGraphics;
using MonoMac.ObjCRuntime;
using MonoMac.CoreAnimation;
using MonoMac.CoreImage;
#if Mac64
using nfloat = System.Double;
using nint = System.Int64;
using nuint = System.UInt64;
#else
using nfloat = System.Single;
using nint = System.Int32;
using nuint = System.UInt32;
#endif
#if SDCOMPAT
using CGSize = System.Drawing.SizeF;
using CGRect = System.Drawing.RectangleF;
using CGPoint = System.Drawing.PointF;
#endif
#endif

namespace Eto.Mac
{
	public static class KeyMap
	{
		static Dictionary<ushort, Keys> _map;
		static Dictionary<Keys, string> _inverseMap;
		static Dictionary<ushort, Keys> Map => _map ?? (_map = GetMap());
		static Dictionary<Keys, string> InverseMap = _inverseMap ?? (_inverseMap = GetInverseMap());

		public static Keys MapKey(ushort key)
		{
			Keys value;
			if (Map.TryGetValue(key, out value))
				return value;
			Debug.WriteLine($"Unknown key '{key}'");
			return Keys.None;
		}

		enum KeyCharacters
		{
			NSParagraphSeparatorCharacter = 0x2029,
			NSLineSeparatorCharacter = 0x2028,
			NSTabCharacter = 0x0009,
			NSFormFeedCharacter = 0x000c,
			NSNewlineCharacter = 0x000a,
			NSCarriageReturnCharacter = 0x000d,
			NSEnterCharacter = 0x0003,
			NSBackspaceCharacter = 0x0008,
			NSBackTabCharacter = 0x0019,
			NSDeleteCharacter = 0x007f
		}

		public static Keys Convert(string keyEquivalent, NSEventModifierMask modifier)
		{
			return Keys.None;
		}

		public static string KeyEquivalent(Keys key)
		{
			string value;
			return InverseMap.TryGetValue(key & Keys.KeyMask, out value) ? value : string.Empty;
		}

		public static NSEventModifierMask KeyEquivalentModifierMask(this Keys key)
		{
			key &= Keys.ModifierMask;
			var mask = (NSEventModifierMask)0;
			if (key.HasFlag(Keys.Shift))
				mask |= NSEventModifierMask.ShiftKeyMask;
			if (key.HasFlag(Keys.Alt))
				mask |= NSEventModifierMask.AlternateKeyMask;
			if (key.HasFlag(Keys.Control))
				mask |= NSEventModifierMask.ControlKeyMask;
			if (key.HasFlag(Keys.Application))
				mask |= NSEventModifierMask.CommandKeyMask;
			if (key == Keys.CapsLock)
				mask |= NSEventModifierMask.AlphaShiftKeyMask;
			if (key == Keys.NumberLock)
				mask |= NSEventModifierMask.NumericPadKeyMask;
			return mask;
		}

		public static NSEventModifierMask ModifierMask(this Keys key)
		{
			var mask = KeyEquivalentModifierMask(key);
			if (key == Keys.CapsLock)
				mask |= NSEventModifierMask.AlphaShiftKeyMask;
			if (key == Keys.NumberLock)
				mask |= NSEventModifierMask.NumericPadKeyMask;
			return mask;
		}

		public static Keys ToEto(this NSEventModifierMask mask)
		{
			Keys key = Keys.None;
			if (mask.HasFlag(NSEventModifierMask.ControlKeyMask))
				key |= Keys.Control;
			if (mask.HasFlag(NSEventModifierMask.CommandKeyMask))
				key |= Keys.Application;
			if (mask.HasFlag(NSEventModifierMask.ShiftKeyMask))
				key |= Keys.Shift;
			if (mask.HasFlag(NSEventModifierMask.AlternateKeyMask))
				key |= Keys.Alt;
			return key;
		}

		static Dictionary<ushort, Keys> GetMap()
		{
			var keymap = new Dictionary<ushort, Keys>();
			// keep in same order as in Keys
			keymap.Add(0, Keys.A);
			keymap.Add(11, Keys.B);
			keymap.Add(8, Keys.C);
			keymap.Add(2, Keys.D);
			keymap.Add(14, Keys.E);
			keymap.Add(3, Keys.F);
			keymap.Add(5, Keys.G);
			keymap.Add(4, Keys.H);
			keymap.Add(34, Keys.I);
			keymap.Add(38, Keys.J);
			keymap.Add(40, Keys.K);
			keymap.Add(37, Keys.L);
			keymap.Add(46, Keys.M);
			keymap.Add(45, Keys.N);
			keymap.Add(31, Keys.O);
			keymap.Add(35, Keys.P);
			keymap.Add(12, Keys.Q);
			keymap.Add(15, Keys.R);
			keymap.Add(1, Keys.S);
			keymap.Add(17, Keys.T);
			keymap.Add(32, Keys.U);
			keymap.Add(9, Keys.V);
			keymap.Add(13, Keys.W);
			keymap.Add(7, Keys.X);
			keymap.Add(16, Keys.Y);
			keymap.Add(6, Keys.Z);
			keymap.Add(122, Keys.F1);
			keymap.Add(120, Keys.F2);
			keymap.Add(99, Keys.F3);
			keymap.Add(118, Keys.F4);
			keymap.Add(96, Keys.F5);
			keymap.Add(97, Keys.F6);
			keymap.Add(98, Keys.F7);
			keymap.Add(100, Keys.F8);
			keymap.Add(101, Keys.F9);
			keymap.Add(109, Keys.F10);
			keymap.Add(103, Keys.F11);
			keymap.Add(111, Keys.F12);
			keymap.Add(18, Keys.D1);
			keymap.Add(19, Keys.D2);
			keymap.Add(20, Keys.D3);
			keymap.Add(21, Keys.D4);
			keymap.Add(23, Keys.D5);
			keymap.Add(22, Keys.D6);
			keymap.Add(26, Keys.D7);
			keymap.Add(28, Keys.D8);
			keymap.Add(25, Keys.D9);
			keymap.Add(29, Keys.D0);
			keymap.Add(27, Keys.Minus);
			keymap.Add(50, Keys.Grave);
			keymap.Add(76, Keys.Insert);
			keymap.Add(115, Keys.Home);
			keymap.Add(121, Keys.PageDown);
			keymap.Add(116, Keys.PageUp);
			keymap.Add(117, Keys.Delete);
			keymap.Add(119, Keys.End);
			keymap.Add(75, Keys.Divide);
			keymap.Add(65, Keys.Decimal);
			keymap.Add(51, Keys.Backspace);
			keymap.Add(126, Keys.Up);
			keymap.Add(125, Keys.Down);
			keymap.Add(123, Keys.Left);
			keymap.Add(124, Keys.Right);
			keymap.Add(48, Keys.Tab);
			keymap.Add(49, Keys.Space);
			//keymap.Add(, Keys.CapsLock);
			//keymap.Add(, Keys.ScrollLock);
			//keymap.Add(, Keys.PrintScreen);
			//keymap.Add(, Keys.NumberLock);
			keymap.Add(36, Keys.Enter);
			keymap.Add(53, Keys.Escape);
			keymap.Add(67, Keys.Multiply);
			keymap.Add(69, Keys.Add);
			keymap.Add(78, Keys.Subtract);
			keymap.Add(114, Keys.Help);
			//keymap.Add(, Keys.Pause);
			keymap.Add(71, Keys.Clear);
			keymap.Add(81, Keys.KeypadEqual);
			//keymap.Add(, Keys.Menu);
			keymap.Add(42, Keys.Backslash);
			keymap.Add(24, Keys.Equal);
			keymap.Add(41, Keys.Semicolon);
			keymap.Add(39, Keys.Quote);
			keymap.Add(43, Keys.Comma);
			keymap.Add(47, Keys.Period);
			keymap.Add(44, Keys.Slash);
			keymap.Add(30, Keys.RightBracket);
			keymap.Add(33, Keys.LeftBracket);
			keymap.Add(110, Keys.ContextMenu);
			keymap.Add(82, Keys.Keypad0);
			keymap.Add(83, Keys.Keypad1);
			keymap.Add(84, Keys.Keypad2);
			keymap.Add(85, Keys.Keypad3);
			keymap.Add(86, Keys.Keypad4);
			keymap.Add(87, Keys.Keypad5);
			keymap.Add(88, Keys.Keypad6);
			keymap.Add(89, Keys.Keypad7);
			keymap.Add(91, Keys.Keypad8);
			keymap.Add(92, Keys.Keypad9);

			return keymap;
		}

		static Dictionary<Keys, string> GetInverseMap()
		{
			var inverse = new Dictionary<Keys, string>();
			inverse.Add(Keys.A, "a");
			inverse.Add(Keys.B, "b");
			inverse.Add(Keys.C, "c");
			inverse.Add(Keys.D, "d");
			inverse.Add(Keys.E, "e");
			inverse.Add(Keys.F, "f");
			inverse.Add(Keys.G, "g");
			inverse.Add(Keys.H, "h");
			inverse.Add(Keys.I, "i");
			inverse.Add(Keys.J, "j");
			inverse.Add(Keys.K, "k");
			inverse.Add(Keys.L, "l");
			inverse.Add(Keys.M, "m");
			inverse.Add(Keys.N, "n");
			inverse.Add(Keys.O, "o");
			inverse.Add(Keys.P, "p");
			inverse.Add(Keys.Q, "q");
			inverse.Add(Keys.R, "r");
			inverse.Add(Keys.S, "s");
			inverse.Add(Keys.T, "t");
			inverse.Add(Keys.U, "u");
			inverse.Add(Keys.V, "v");
			inverse.Add(Keys.W, "w");
			inverse.Add(Keys.X, "x");
			inverse.Add(Keys.Y, "y");
			inverse.Add(Keys.Z, "z");
			inverse.Add(Keys.Period, ".");
			inverse.Add(Keys.Comma, ",");
			inverse.Add(Keys.Space, " ");
			inverse.Add(Keys.Backslash, "\\");
			inverse.Add(Keys.Slash, "/");
			inverse.Add(Keys.Equal, "=");
			inverse.Add(Keys.Grave, "`");
			inverse.Add(Keys.Minus, "-");
			inverse.Add(Keys.Semicolon, ";");
			inverse.Add(Keys.Up, "\xF700");
			inverse.Add(Keys.Down, "\xF701");
			inverse.Add(Keys.Left, "\xF702");
			inverse.Add(Keys.Right, "\xF703");
			inverse.Add(Keys.Home, "\xF729");
			inverse.Add(Keys.End, "\xF72B");
			inverse.Add(Keys.Insert, "\xF727");
			inverse.Add(Keys.Delete, "\x007f");
			inverse.Add(Keys.Backspace, "\x0008");
			inverse.Add(Keys.Tab, "\x0009");
			inverse.Add(Keys.D0, "0");
			inverse.Add(Keys.D1, "1");
			inverse.Add(Keys.D2, "2");
			inverse.Add(Keys.D3, "3");
			inverse.Add(Keys.D4, "4");
			inverse.Add(Keys.D5, "5");
			inverse.Add(Keys.D6, "6");
			inverse.Add(Keys.D7, "7");
			inverse.Add(Keys.D8, "8");
			inverse.Add(Keys.D9, "9");
			inverse.Add(Keys.F1, "\xF704");
			inverse.Add(Keys.F2, "\xF705");
			inverse.Add(Keys.F3, "\xF706");
			inverse.Add(Keys.F4, "\xF707");
			inverse.Add(Keys.F5, "\xF708");
			inverse.Add(Keys.F6, "\xF709");
			inverse.Add(Keys.F7, "\xF70A");
			inverse.Add(Keys.F8, "\xF70B");
			inverse.Add(Keys.F9, "\xF70C");
			inverse.Add(Keys.F10, "\xF70D");
			inverse.Add(Keys.F11, "\xF70E");
			inverse.Add(Keys.F12, "\xF70F");
			return inverse;
		}
	}
}

