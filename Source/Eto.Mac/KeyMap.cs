using System.Collections.Generic;
using Eto.Forms;

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
using CGSize = MonoMac.Foundation.NSSize;
using CGRect = MonoMac.Foundation.NSRect;
using CGPoint = MonoMac.Foundation.NSPoint;
using nfloat = System.Double;
using nint = System.Int64;
using nuint = System.UInt64;
#else
using CGSize = System.Drawing.SizeF;
using CGRect = System.Drawing.RectangleF;
using CGPoint = System.Drawing.PointF;
using nfloat = System.Single;
using nint = System.Int32;
using nuint = System.UInt32;
#endif
#endif

namespace Eto.Mac
{
	public static class KeyMap
	{
		static readonly Dictionary<ushort, Keys> keymap = new Dictionary<ushort, Keys>();
		static readonly Dictionary<Keys, string> inverse = new Dictionary<Keys, string>();

		public static Keys MapKey(ushort key)
		{
			Keys value;
			return keymap.TryGetValue(key, out value) ? value : Keys.None;
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
			return inverse.TryGetValue(key & Keys.KeyMask, out value) ? value : string.Empty;
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

		static KeyMap()
		{
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
			keymap.Add(50, Keys.Grave);
			keymap.Add(27, Keys.Minus);
			keymap.Add(24, Keys.Equal);
			keymap.Add(42, Keys.Backslash);
			keymap.Add(49, Keys.Space);
			//keymap.Add(30, Keys.]);
			//keymap.Add(33, Keys.[);
			keymap.Add(39, Keys.Quote);
			keymap.Add(41, Keys.Semicolon);
			keymap.Add(44, Keys.ForwardSlash);
			keymap.Add(47, Keys.Period);
			keymap.Add(43, Keys.Comma);
			keymap.Add(36, Keys.Enter);
			keymap.Add(48, Keys.Tab);
			//keymap.Add(76, Keys.Return);
			keymap.Add(53, Keys.Escape);

			keymap.Add(76, Keys.Insert);
			keymap.Add(51, Keys.Backspace);
			keymap.Add(117, Keys.Delete);
			
			keymap.Add(125, Keys.Down);
			keymap.Add(126, Keys.Up);
			keymap.Add(123, Keys.Left);
			keymap.Add(124, Keys.Right);
			
			keymap.Add(116, Keys.PageUp);
			keymap.Add(121, Keys.PageDown);
			keymap.Add(119, Keys.End);
			keymap.Add(115, Keys.Home);
			keymap.Add(110, Keys.ContextMenu);

			
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
			inverse.Add(Keys.ForwardSlash, "/");
			inverse.Add(Keys.Equal, "=");
			inverse.Add(Keys.Grave, "`");
			inverse.Add(Keys.Minus, "-");
			inverse.Add(Keys.Semicolon, ";");
			inverse.Add(Keys.Up, ((char)NSKey.UpArrow).ToString());
			inverse.Add(Keys.Down, ((char)NSKey.DownArrow).ToString());
			inverse.Add(Keys.Right, ((char)NSKey.RightArrow).ToString());
			inverse.Add(Keys.Left, ((char)NSKey.LeftArrow).ToString());
			inverse.Add(Keys.Home, ((char)NSKey.Home).ToString());
			inverse.Add(Keys.End, ((char)NSKey.End).ToString());
			#if !__UNIFIED__
			inverse.Add(Keys.Insert, ((char)NSKey.Insert).ToString());
			#endif
			inverse.Add(Keys.Delete, ((char)KeyCharacters.NSDeleteCharacter).ToString());
			inverse.Add(Keys.Backspace, ((char)KeyCharacters.NSBackspaceCharacter).ToString());
			inverse.Add(Keys.Tab, ((char)KeyCharacters.NSTabCharacter).ToString());
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
			inverse.Add(Keys.F1, ((char)NSKey.F1).ToString());
			inverse.Add(Keys.F2, ((char)NSKey.F2).ToString());
			inverse.Add(Keys.F3, ((char)NSKey.F3).ToString());
			inverse.Add(Keys.F4, ((char)NSKey.F4).ToString());
			inverse.Add(Keys.F5, ((char)NSKey.F5).ToString());
			inverse.Add(Keys.F6, ((char)NSKey.F6).ToString());
			inverse.Add(Keys.F7, ((char)NSKey.F7).ToString());
			inverse.Add(Keys.F8, ((char)NSKey.F8).ToString());
			inverse.Add(Keys.F9, ((char)NSKey.F9).ToString());
			inverse.Add(Keys.F10, ((char)NSKey.F10).ToString());
			inverse.Add(Keys.F11, ((char)NSKey.F11).ToString());
			inverse.Add(Keys.F12, ((char)NSKey.F12).ToString());
		}
	}
}

