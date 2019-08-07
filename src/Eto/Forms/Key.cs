﻿﻿using System;
using System.Text;
using System.Collections.Generic;
using sc = System.ComponentModel;
using System.Diagnostics;

namespace Eto.Forms
{
	/// <summary>
	/// Enumeration of values that correspond to physical keys on a keyboard
	/// </summary>
	[Flags]
	[sc.TypeConverter(typeof(KeysConverter))]
	public enum Keys
	{
		/// <summary>No key</summary>
		None = 0x0000,

		/// <summary>The A key</summary>
		A = 0x0001,
		/// <summary>The B key</summary>
		B = 0x0002,
		/// <summary>The C key</summary>
		C = 0x0003,
		/// <summary>The D key</summary>
		D = 0x0004,
		/// <summary>The E key</summary>
		E = 0x0005,
		/// <summary>The F key</summary>
		F = 0x0006,
		/// <summary>The G key</summary>
		G = 0x0007,
		/// <summary>The H key</summary>
		H = 0x0008,
		/// <summary>The I key</summary>
		I = 0x0009,
		/// <summary>The J key</summary>
		J = 0x000A,
		/// <summary>The K key</summary>
		K = 0x000B,
		/// <summary>The L key</summary>
		L = 0x000C,
		/// <summary>The M key</summary>
		M = 0x000D,
		/// <summary>The N key</summary>
		N = 0x000E,
		/// <summary>The O key</summary>
		O = 0x000F,
		/// <summary>The P key</summary>
		P = 0x0010,
		/// <summary>The Q key</summary>
		Q = 0x0011,
		/// <summary>The R key</summary>
		R = 0x0012,
		/// <summary>The S key</summary>
		S = 0x0013,
		/// <summary>The T key</summary>
		T = 0x0014,
		/// <summary>The U key</summary>
		U = 0x0015,
		/// <summary>The V key</summary>
		V = 0x0016,
		/// <summary>The W key</summary>
		W = 0x0017,
		/// <summary>The X key</summary>
		X = 0x0018,
		/// <summary>The Y key</summary>
		Y = 0x0019,
		/// <summary>The Z key</summary>
		Z = 0x001A,
		/// <summary>The F1 key</summary>
		F1 = 0x001B,
		/// <summary>The F2 key</summary>
		F2 = 0x001C,
		/// <summary>The F3 key</summary>
		F3 = 0x001D,
		/// <summary>The F4 key</summary>
		F4 = 0x001E,
		/// <summary>The F5 key</summary>
		F5 = 0x001F,
		/// <summary>The F6 key</summary>
		F6 = 0x0020,
		/// <summary>The F7 key</summary>
		F7 = 0x0021,
		/// <summary>The F8 key</summary>
		F8 = 0x0022,
		/// <summary>The F9 key</summary>
		F9 = 0x0023,
		/// <summary>The F10 key</summary>
		F10 = 0x0024,
		/// <summary>The F11 key</summary>
		F11 = 0x0025,
		/// <summary>The F12 key</summary>
		F12 = 0x0026,
		/// <summary>The 0 digit key</summary>
		D0 = 0x0027,
		/// <summary>The 1 digit key</summary>
		D1 = 0x0028,
		/// <summary>The 2 digit key</summary>
		D2 = 0x0029,
		/// <summary>The 3 digit key</summary>
		D3 = 0x002A,
		/// <summary>The 4 digit key</summary>
		D4 = 0x002B,
		/// <summary>The 5 digit key</summary>
		D5 = 0x002C,
		/// <summary>The 6 digit key</summary>
		D6 = 0x002D,
		/// <summary>The 7 digit key</summary>
		D7 = 0x002E,
		/// <summary>The 8 digit key</summary>
		D8 = 0x002F,
		/// <summary>The 9 digit key</summary>
		D9 = 0x0030,

		/// <summary>The Minus '-' key</summary>
		Minus = 0x0031,
		/// <summary>The Plus '+' Key, which usually produces an '=' when pressed without shift and is beside the backspace key.</summary>
		[Obsolete("Since 2.4, Use Equal")]
		Plus = Equal,
		/// <summary>The Grave '`' key</summary>
		Grave = 0x0033,
		/// <summary>The Insert key</summary>
		Insert = 0x0034,
		/// <summary>The Home key</summary>
		Home = 0x0035,
		/// <summary>The Page Up key</summary>
		PageUp = 0x0036,
		/// <summary>The Page Down key</summary>
		PageDown = 0x0037,
		/// <summary>The Delete key</summary>
		Delete = 0x0038,
		/// <summary>The End key</summary>
		End = 0x0039,
		/// <summary>The Divide '/' key, usually on the keypad/number pad</summary>
		Divide = 0x003A,
		/// <summary>The Decimal '.' key, usually on the keypad/number pad</summary>
		Decimal = 0x003B,
		/// <summary>The Backspace key</summary>
		Backspace = 0x003C,
		/// <summary>The Up key</summary>
		Up = 0x003D,
		/// <summary>The Down key</summary>
		Down = 0x003E,
		/// <summary>The Left key</summary>
		Left = 0x003F,
		/// <summary>The Right key</summary>
		Right = 0x0040,
		/// <summary>The Tab key</summary>
		Tab = 0x0041,
		/// <summary>The Space key</summary>
		Space = 0x0042,
		/// <summary>The Caps Lock key</summary>
		CapsLock = 0x0043,
		/// <summary>The Scroll Lock key</summary>
		ScrollLock = 0x0044,
		/// <summary>The Print Screen key</summary>
		PrintScreen = 0x0045,
		/// <summary>The Number Lock key</summary>
		NumberLock = 0x0046,
		/// <summary>The Enter key</summary>
		Enter = 0x0047,
		/// <summary>The Escape key</summary>
		Escape = 0x0048,
		/// <summary>The Multiply '*' key, usually on the keypad/number pad</summary>
		Multiply = 0x0049,
		/// <summary>The Add '+' key, usually on the keypad/number pad</summary>
		Add = 0x004A,
		/// <summary>The Subtract '-' key, usually on the keypad/number pad</summary>
		Subtract = 0x004B,
		/// <summary>The Help key</summary>
		Help = 0x004C,
		/// <summary>The Pause key</summary>
		Pause = 0x004D,
		/// <summary>The Clear key</summary>
		Clear = 0x004E,
		/// <summary>The Equal '=' key on the keypad/number pad</summary>
		KeypadEqual = 0x004F,

		/// <summary>The menu (alt) key</summary>
		[Obsolete("Since 2.5. Use LeftAlt and RightAlt")]
		Menu = 0x0050,
		/// <summary>The Backslash '\' key</summary>
		Backslash = 0x0051,
		/// <summary>The Equal '=' key</summary>
		Equal = 0x0055,
		/// <summary>The Semicolon ';' key</summary>
		Semicolon = 0x0056,
		/// <summary>The Quote ''' key</summary>
		Quote = 0x0057,

		/// <summary>The Comma ',' key</summary>
		Comma = 0x0058,
		/// <summary>The Period '.' key</summary>
		Period = 0x0059,
		/// <summary>The Forward Slash '/' key</summary>
		[Obsolete("Since 2.4, Use Slash instead")]
		ForwardSlash = Slash,
		/// <summary>The Slash '/' key</summary>
		Slash = 0x0060,

		/// <summary>The Right Bracket ']' key</summary>
		RightBracket = 0x0061,
		/// <summary>The Left Bracket '['  key</summary>
		LeftBracket = 0x0062,

		/// <summary>The context menu key</summary>
		ContextMenu = 0x0063,

		/// <summary>The keypad/number pad '0' key</summary>
		Keypad0 = 0x0070,
		/// <summary>The keypad/number pad '1' key</summary>
		Keypad1 = 0x0071,
		/// <summary>The keypad/number pad '2' key</summary>
		Keypad2 = 0x0072,
		/// <summary>The keypad/number pad '3' key</summary>
		Keypad3 = 0x0073,
		/// <summary>The keypad/number pad '4' key</summary>
		Keypad4 = 0x0074,
		/// <summary>The keypad/number pad '5' key</summary>
		Keypad5 = 0x0075,
		/// <summary>The keypad/number pad '6' key</summary>
		Keypad6 = 0x0076,
		/// <summary>The keypad/number pad '7' key</summary>
		Keypad7 = 0x0077,
		/// <summary>The keypad/number pad '8' key</summary>
		Keypad8 = 0x0078,
		/// <summary>The keypad/number pad '9' key</summary>
		Keypad9 = 0x0079,

		/// <summary>The left shift key</summary>
		LeftShift = 0x007A,
		/// <summary>The right shift key</summary>
		RightShift = 0x007B,
		/// <summary>The left control key</summary>
		LeftControl = 0x007C,
		/// <summary>The right control key</summary>
		RightControl = 0x007D,
		/// <summary>The left alt/option key</summary>
		LeftAlt = 0x007E,
		/// <summary>The right alt/option key</summary>
		RightAlt = 0x007F,
		/// <summary>The right application/windows key</summary>
		LeftApplication = 0x0080,
		/// <summary>The right application/windows key</summary>
		RightApplication = 0x0081,

		/// <summary>The Shift Key Modifier</summary>
		Shift = 0x1000,
		/// <summary>The Alt Key Modifier</summary>
		Alt = 0x2000,
		/// <summary>The Control Key Modifier</summary>
		Control = 0x4000,
		/// <summary>The Application/Windows Key Modifier</summary>
		Application = 0x8000,  // windows/command key

		/// <summary>The mask for the modifiers (<see cref="Shift"/>/<see cref="Alt"/>/<see cref="Control"/>/<see cref="Application"/>)</summary>
		ModifierMask = 0xF000,
		/// <summary>The mask for the key value without modifiers</summary>
		KeyMask = 0x0FFF

	}

	/// <summary>
	/// Extensions for the <see cref="Keys"/> enumeration
	/// </summary>
	public static class KeysExtensions
	{
		static void AppendSeparator (StringBuilder sb, string separator, string value)
		{
			if (sb.Length > 0)
				sb.Append (separator);
			sb.Append (value);
		}

		static readonly Dictionary<Keys, string> keymap = new Dictionary<Keys,string> {
			{ Keys.D0, "0" },
			{ Keys.D1, "1" },
			{ Keys.D2, "2" },
			{ Keys.D3, "3" },
			{ Keys.D4, "4" },
			{ Keys.D5, "5" },
			{ Keys.D6, "6" },
			{ Keys.D7, "7" },
			{ Keys.D8, "8" },
			{ Keys.D9, "9" },

			{ Keys.Minus, "-" },
			{ Keys.Equal, "=" },
			{ Keys.Grave, "`" },
			{ Keys.Divide, "/" },
			{ Keys.Decimal, "." },
			{ Keys.Backslash, "\\" },
			{ Keys.KeypadEqual, "=" },
			{ Keys.Multiply, "*" },
			{ Keys.Add, "+" },
			{ Keys.Subtract, "-" },
			{ Keys.Tab, "\x21E5" },
			{ Keys.Enter, "\x23ce" },
			{ Keys.Delete, EtoEnvironment.Platform.IsMac ? "\x232b" : "Del" },
			{ Keys.Escape, EtoEnvironment.Platform.IsMac ? "\x238b" : "Esc" },

			{ Keys.Semicolon, ";" },
			{ Keys.Quote, "'" },
		
			{ Keys.Comma, "," },
			{ Keys.Period, "." },
			{ Keys.Slash, "/" },
		
			{ Keys.RightBracket, "[" },
			{ Keys.LeftBracket, "]" }
		};

		/// <summary>
		/// Converts the specified key to a shortcut string such as Ctrl+Alt+Z
		/// </summary>
		/// <param name="key">Key to convert</param>
		/// <param name="separator">Separator between each modifier and key</param>
		/// <returns>A human-readable string representing the key combination including modifiers</returns>
		public static string ToShortcutString(this Keys key, string separator = "+")
		{
			var sb = new StringBuilder();
			if (key.HasFlag(Keys.Application))
				AppendSeparator(sb, separator, 
					EtoEnvironment.Platform.IsMac ? "\x2318" : 
					EtoEnvironment.Platform.IsWindows ? "Win" :
					"App");
			if (key.HasFlag(Keys.Control))
				AppendSeparator(sb, separator, EtoEnvironment.Platform.IsMac ? "^" : "Ctrl");
			if (key.HasFlag(Keys.Shift))
				AppendSeparator(sb, separator, EtoEnvironment.Platform.IsMac ? "\x21e7" : "Shift");
			if (key.HasFlag(Keys.Alt))
				AppendSeparator(sb, separator, EtoEnvironment.Platform.IsMac ? "\x2325" : "Alt");

			var mainKey = key & Keys.KeyMask;
			string val;
			if (keymap.TryGetValue(mainKey, out val))
				AppendSeparator(sb, separator, val);
			else
				AppendSeparator(sb, separator, mainKey.ToString());

			return sb.ToString();
		}
	}
}
