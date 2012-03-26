using System;
using System.Text;
using System.Collections.Generic;

namespace Eto.Forms
{
	[Flags]
	public enum Key
	{
		A = 0x0001,
		B = 0x0002,
		C = 0x0003,
		D = 0x0004,
		E = 0x0005,
		F = 0x0006,
		G = 0x0007,
		H = 0x0008,
		I = 0x0009,
		J = 0x000A,
		K = 0x000B,
		L = 0x000C,
		M = 0x000D,
		N = 0x000E,
		O = 0x000F,
		P = 0x0010,
		Q = 0x0011,
		R = 0x0012,
		S = 0x0013,
		T = 0x0014,
		U = 0x0015,
		V = 0x0016,
		W = 0x0017,
		X = 0x0018,
		Y = 0x0019,
		Z = 0x001A,
		F1 = 0x001B,
		F2 = 0x001C,
		F3 = 0x001D,
		F4 = 0x001E,
		F5 = 0x001F,
		F6 = 0x0020,
		F7 = 0x0021,
		F8 = 0x0022,
		F9 = 0x0023,
		F10 = 0x0024,
		F11 = 0x0025,
		F12 = 0x0026,
		D0 = 0x0027,
		D1 = 0x0028,
		D2 = 0x0029,
		D3 = 0x002A,
		D4 = 0x002B,
		D5 = 0x002C,
		D6 = 0x002D,
		D7 = 0x002E,
		D8 = 0x002F,
		D9 = 0x0030,

		Minus = 0x0031,
		Plus = 0x0032,
		Grave = 0x0033, // `
		Insert = 0x0034,
		Home = 0x0035,
		PageUp = 0x0036,
		PageDown = 0x0037,
		Delete = 0x0038,
		End = 0x0039,
		Divide = 0x003A, // '/'
		Decimal = 0x003B, // '.'
		Backspace = 0x003C,
		Up = 0x003D,
		Down = 0x003E,
		Left = 0x003F,
		Right = 0x0040,
		Tab = 0x0041,
		Space = 0x0042,
		CapsLock = 0x0043,
		ScrollLock = 0x0044,
		PrintScreen = 0x0045,
		NumberLock = 0x0046,
		Enter = 0x0047,
		Escape = 0x0048,
		/// <summary>The menu key</summary>
		Menu = 0x0050,
		Backslash = 0x0051, // '\'
		Equal = 0x0055, // '='
		
		SemiColon = 0x0056,
		Quote = 0x0057,
		
		Comma = 0x0058,
		Period = 0x0059,
		ForwardSlash = 0x0060,
		
		RightBracket = 0x0061,
		LeftBracket = 0x0062,
		
		
		Shift		= 0x1000,
		Alt			= 0x2000,
		Control		= 0x4000,
		Application = 0x8000,  // windows/command key

		ModifierMask = 0xF000,
		KeyMask = 0x0FFF,

		None = 0x0000,

	}
	
	public static class KeyExtensions
	{
		static void AppendSeparator (StringBuilder sb, string separator, string value)
		{
			if (sb.Length > 0)
				sb.Append (separator);
			sb.Append (value);
		}
		
		static Dictionary<Key, string> keymap = new Dictionary<Key, string> () {
			{ Key.D0, "0" },
			{ Key.D1, "1" },
			{ Key.D2, "2" },
			{ Key.D3, "3" },
			{ Key.D4, "4" },
			{ Key.D5, "5" },
			{ Key.D6, "6" },
			{ Key.D7, "7" },
			{ Key.D8, "8" },
			{ Key.D9, "9" },

			{ Key.Minus, "-" },
			{ Key.Plus, "+" },
			{ Key.Grave, "`" },
			{ Key.Divide, "/" },
			{ Key.Decimal, "." },
			{ Key.Backslash, "\\" },
			{ Key.Equal, "=" },
		
			{ Key.SemiColon, ";" },
			{ Key.Quote, "'" },
		
			{ Key.Comma, "," },
			{ Key.Period, "." },
			{ Key.ForwardSlash, "/" },
		
			{ Key.RightBracket, "(" },
			{ Key.LeftBracket, ")" }
		};
		
		public static string ToShortcutString (this Key key, string separator = "+")
		{
			var sb = new StringBuilder ();
			if (key.HasFlag (Key.Control))
				AppendSeparator (sb, separator, "Ctrl");
			if (key.HasFlag (Key.Shift))
				AppendSeparator (sb, separator, "Shift");
			if (key.HasFlag (Key.Alt))
				AppendSeparator (sb, separator, "Alt");
			
			var mainKey = key & Key.KeyMask;
			string val;
			if (keymap.TryGetValue (mainKey, out val))
				AppendSeparator (sb, separator, val);
			else
				AppendSeparator (sb, separator, mainKey.ToString ());
			
			return sb.ToString ();
		}
	}
}
