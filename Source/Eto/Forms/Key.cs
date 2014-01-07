using System;
using System.Text;
using System.Collections.Generic;

namespace Eto.Forms
{
	/// <summary>
	/// Enumeration of values that correspond to physical keys on a keyboard
	/// </summary>
	[Flags]
	public enum Keys
	{
		/// <summary>No key</summary>
		None = 0x0000,

		/// <summary>The A Key</summary>
		A = 0x0001,
		/// <summary>The B Key</summary>
		B = 0x0002,
		/// <summary>The C Key</summary>
		C = 0x0003,
		/// <summary>The D Key</summary>
		D = 0x0004,
		/// <summary>The E Key</summary>
		E = 0x0005,
		/// <summary>The F Key</summary>
		F = 0x0006,
		/// <summary>The G Key</summary>
		G = 0x0007,
		/// <summary>The H Key</summary>
		H = 0x0008,
		/// <summary>The I Key</summary>
		I = 0x0009,
		/// <summary>The J Key</summary>
		J = 0x000A,
		/// <summary>The K Key</summary>
		K = 0x000B,
		/// <summary>The L Key</summary>
		L = 0x000C,
		/// <summary>The M Key</summary>
		M = 0x000D,
		/// <summary>The N Key</summary>
		N = 0x000E,
		/// <summary>The O Key</summary>
		O = 0x000F,
		/// <summary>The P Key</summary>
		P = 0x0010,
		/// <summary>The Q Key</summary>
		Q = 0x0011,
		/// <summary>The R Key</summary>
		R = 0x0012,
		/// <summary>The S Key</summary>
		S = 0x0013,
		/// <summary>The T Key</summary>
		T = 0x0014,
		/// <summary>The U Key</summary>
		U = 0x0015,
		/// <summary>The V Key</summary>
		V = 0x0016,
		/// <summary>The W Key</summary>
		W = 0x0017,
		/// <summary>The X Key</summary>
		X = 0x0018,
		/// <summary>The Y Key</summary>
		Y = 0x0019,
		/// <summary>The Z Key</summary>
		Z = 0x001A,
		/// <summary>The F1 Key</summary>
		F1 = 0x001B,
		/// <summary>The F2 Key</summary>
		F2 = 0x001C,
		/// <summary>The F3 Key</summary>
		F3 = 0x001D,
		/// <summary>The F4 Key</summary>
		F4 = 0x001E,
		/// <summary>The F5 Key</summary>
		F5 = 0x001F,
		/// <summary>The F6 Key</summary>
		F6 = 0x0020,
		/// <summary>The F7 Key</summary>
		F7 = 0x0021,
		/// <summary>The F8 Key</summary>
		F8 = 0x0022,
		/// <summary>The F9 Key</summary>
		F9 = 0x0023,
		/// <summary>The F10 Key</summary>
		F10 = 0x0024,
		/// <summary>The F11 Key</summary>
		F11 = 0x0025,
		/// <summary>The F12 Key</summary>
		F12 = 0x0026,
		/// <summary>The 0 digit Key</summary>
		D0 = 0x0027,
		/// <summary>The 1 digit Key</summary>
		D1 = 0x0028,
		/// <summary>The 2 digit Key</summary>
		D2 = 0x0029,
		/// <summary>The 3 digit Key</summary>
		D3 = 0x002A,
		/// <summary>The 4 digit Key</summary>
		D4 = 0x002B,
		/// <summary>The 5 digit Key</summary>
		D5 = 0x002C,
		/// <summary>The 6 digit Key</summary>
		D6 = 0x002D,
		/// <summary>The 7 digit Key</summary>
		D7 = 0x002E,
		/// <summary>The 8 digit Key</summary>
		D8 = 0x002F,
		/// <summary>The 9 digit Key</summary>
		D9 = 0x0030,

		/// <summary>The Minus '-' Key</summary>
		Minus = 0x0031,
		/// <summary>The Plus '+' Key</summary>
		Plus = 0x0032,
		/// <summary>The Grave '`' Key</summary>
		Grave = 0x0033,
		/// <summary>The Insert Key</summary>
		Insert = 0x0034,
		/// <summary>The Home Key</summary>
		Home = 0x0035,
		/// <summary>The Page Up Key</summary>
		PageUp = 0x0036,
		/// <summary>The Page Down Key</summary>
		PageDown = 0x0037,
		/// <summary>The Delete Key</summary>
		Delete = 0x0038,
		/// <summary>The End Key</summary>
		End = 0x0039,
		/// <summary>The Divide '/' Key</summary>
		Divide = 0x003A,
		/// <summary>The Decimal '.' Key</summary>
		Decimal = 0x003B,
		/// <summary>The Backspace Key</summary>
		Backspace = 0x003C,
		/// <summary>The Up Key</summary>
		Up = 0x003D,
		/// <summary>The Down Key</summary>
		Down = 0x003E,
		/// <summary>The Left Key</summary>
		Left = 0x003F,
		/// <summary>The Right Key</summary>
		Right = 0x0040,
		/// <summary>The Tab Key</summary>
		Tab = 0x0041,
		/// <summary>The Space Key</summary>
		Space = 0x0042,
		/// <summary>The Caps Lock Key</summary>
		CapsLock = 0x0043,
		/// <summary>The Scroll Lock Key</summary>
		ScrollLock = 0x0044,
		/// <summary>The Print Screen Key</summary>
		PrintScreen = 0x0045,
		/// <summary>The Number Lock Key</summary>
		NumberLock = 0x0046,
		/// <summary>The Enter Key</summary>
		Enter = 0x0047,
		/// <summary>The Escape Key</summary>
		Escape = 0x0048,
		/// <summary>The menu key</summary>
		Menu = 0x0050,
		/// <summary>The Bacslash '\' Key</summary>
		Backslash = 0x0051,
		/// <summary>The Equal '=' Key</summary>
		Equal = 0x0055,

		/// <summary>The Semicolon ';' Key</summary>
		[Obsolete("Use Semicolon instead"), CLSCompliant(false)]
		SemiColon = 0x0056,

		/// <summary>The Semicolon ';' Key</summary>
		Semicolon = 0x0056,
		/// <summary>The Quote ''' Key</summary>
		Quote = 0x0057,

		/// <summary>The Comma ',' Key</summary>
		Comma = 0x0058,
		/// <summary>The Period '.' Key</summary>
		Period = 0x0059,
		/// <summary>The Forward Slash '/' Key</summary>
		ForwardSlash = 0x0060,

		/// <summary>The Right Bracket ']' Key</summary>
		RightBracket = 0x0061,
		/// <summary>The Left Bracket '['  Key</summary>
		LeftBracket = 0x0062,

		/// <summary> /// The context menu Key /// </summary>
		ContextMenu = 0x0063,

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

	[Obsolete("Use Keys instead")]
	public struct Key
	{
		readonly Keys keys;
		public Key(Keys keys)
		{
			this.keys = keys;
		}

		public const Keys None = Keys.None;
		public const Keys A = Keys.A;
		public const Keys B = Keys.B;
		public const Keys C = Keys.C;
		public const Keys D = Keys.D;
		public const Keys E = Keys.E;
		public const Keys F = Keys.F;
		public const Keys G = Keys.G;
		public const Keys H = Keys.H;
		public const Keys I = Keys.I;
		public const Keys J = Keys.J;
		public const Keys K = Keys.K;
		public const Keys L = Keys.L;
		public const Keys M = Keys.M;
		public const Keys N = Keys.N;
		public const Keys O = Keys.O;
		public const Keys P = Keys.P;
		public const Keys Q = Keys.Q;
		public const Keys R = Keys.R;
		public const Keys S = Keys.S;
		public const Keys T = Keys.T;
		public const Keys U = Keys.U;
		public const Keys V = Keys.V;
		public const Keys W = Keys.W;
		public const Keys X = Keys.X;
		public const Keys Y = Keys.Y;
		public const Keys Z = Keys.Z;
		public const Keys F1 = Keys.F1;
		public const Keys F2 = Keys.F2;
		public const Keys F3 = Keys.F3;
		public const Keys F4 = Keys.F4;
		public const Keys F5 = Keys.F5;
		public const Keys F6 = Keys.F6;
		public const Keys F7 = Keys.F7;
		public const Keys F8 = Keys.F8;
		public const Keys F9 = Keys.F9;
		public const Keys F10 = Keys.F10;
		public const Keys F11 = Keys.F11;
		public const Keys F12 = Keys.F12;
		public const Keys D0 = Keys.D0;
		public const Keys D1 = Keys.D1;
		public const Keys D2 = Keys.D2;
		public const Keys D3 = Keys.D3;
		public const Keys D4 = Keys.D4;
		public const Keys D5 = Keys.D5;
		public const Keys D6 = Keys.D6;
		public const Keys D7 = Keys.D7;
		public const Keys D8 = Keys.D8;
		public const Keys D9 = Keys.D9;

		public const Keys Minus = Keys.Minus;
		public const Keys Plus = Keys.Plus;
		public const Keys Grave = Keys.Grave;
		public const Keys Insert = Keys.Insert;
		public const Keys Home = Keys.Home;
		public const Keys PageUp = Keys.PageUp;
		public const Keys PageDown = Keys.PageDown;
		public const Keys Delete = Keys.Delete;
		public const Keys End = Keys.End;
		public const Keys Divide = Keys.Divide;
		public const Keys Decimal = Keys.Decimal;
		public const Keys Backspace = Keys.Backspace;
		public const Keys Up = Keys.Up;
		public const Keys Down = Keys.Down;
		public const Keys Left = Keys.Left;
		public const Keys Right = Keys.Right;
		public const Keys Tab = Keys.Tab;
		public const Keys Space = Keys.Space;
		public const Keys CapsLock = Keys.CapsLock;
		public const Keys ScrollLock = Keys.ScrollLock;
		public const Keys PrintScreen = Keys.PrintScreen;
		public const Keys NumberLock = Keys.NumberLock;
		public const Keys Enter = Keys.Enter;
		public const Keys Escape = Keys.Escape;
		public const Keys Menu = Keys.Menu;
		public const Keys Backslash = Keys.Backslash;
		public const Keys Equal = Keys.Equal;

		[Obsolete("Use Semicolon instead"), CLSCompliant(false)]
		public const Keys SemiColon = Keys.Semicolon;

		public const Keys Semicolon = Keys.Semicolon;
		public const Keys Quote = Keys.Quote;

		public const Keys Comma = Keys.Comma;
		public const Keys Period = Keys.Period;
		public const Keys ForwardSlash = Keys.ForwardSlash;

		public const Keys RightBracket = Keys.RightBracket;
		public const Keys LeftBracket = Keys.LeftBracket;

		public const Keys ContextMenu = Keys.ContextMenu;

		public const Keys Shift = Keys.Shift;
		public const Keys Alt = Keys.Alt;
		public const Keys Control = Keys.Control;
		public const Keys Application = Keys.Application;  // windows/command key

		public const Keys ModifierMask = Keys.ModifierMask;
		public const Keys KeyMask = Keys.KeyMask;

		public bool HasFlag(Key key)
		{
			return keys.HasFlag(key.keys);
		}

		public static Key operator &(Key key1, Key key2)
		{
			return new Key(key1.keys & key2.keys);
		}

		public static Key operator ~(Key key1)
		{
			return new Key(~key1.keys);
		}

		public static bool operator ==(Key key1, Key key2)
		{
			return key1.keys == key2.keys;
		}

		public static bool operator !=(Key key1, Key key2)
		{
			return key1.keys != key2.keys;
		}

		public static implicit operator Keys(Key key)
		{
			return key.keys;
		}

		public static implicit operator Key(Keys key)
		{
			return new Key(key);
		}

		public static implicit operator int(Key key)
		{
			return (int)key.keys;
		}

		public static Key operator |(Key key1, Key key2)
		{
			return new Key(key1.keys | key2.keys);
		}

		public override bool Equals(object obj)
		{
			return obj is Key && (Key)obj == this;
		}

		public override int GetHashCode()
		{
			return keys.GetHashCode();
		}
	}


	/// <summary>
	/// Extensions for the <see cref="Key"/> enumeration
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
			{ Keys.Plus, "+" },
			{ Keys.Grave, "`" },
			{ Keys.Divide, "/" },
			{ Keys.Decimal, "." },
			{ Keys.Backslash, "\\" },
			{ Keys.Equal, "=" },
		
			{ Keys.Semicolon, ";" },
			{ Keys.Quote, "'" },
		
			{ Keys.Comma, "," },
			{ Keys.Period, "." },
			{ Keys.ForwardSlash, "/" },
		
			{ Keys.RightBracket, "(" },
			{ Keys.LeftBracket, ")" }
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
			if (key.HasFlag(Keys.Control))
				AppendSeparator(sb, separator, "Ctrl");
			if (key.HasFlag(Keys.Shift))
				AppendSeparator(sb, separator, "Shift");
			if (key.HasFlag(Keys.Alt))
				AppendSeparator(sb, separator, "Alt");

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
