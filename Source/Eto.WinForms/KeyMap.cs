using swf = System.Windows.Forms;
using Eto.Forms;
using System.Collections.Generic;
using System.Diagnostics;

#if WPF
namespace Eto.Wpf
#else
namespace Eto.WinForms
#endif
{
#if WPF
	public static class KeyMapWinForms
#else
	public static class KeyMap
#endif
	{
		static Dictionary<swf.Keys, Keys> _map;
		static Dictionary<Keys, swf.Keys> _inverseMap;

		static Dictionary<swf.Keys, Keys> Map => _map ?? (_map = GetMap());
		static Dictionary<Keys, swf.Keys> InverseMap = _inverseMap ?? (_inverseMap = GetInverseMap());

        public static Keys ToEto (this swf.Keys keyData)
        {
            // convert the modifiers
            Keys modifiers = Keys.None;

            // Shift
            if ((keyData & swf.Keys.Shift) == swf.Keys.Shift)
                modifiers |= Keys.Shift;

            // Control
            if ((keyData & swf.Keys.Control) == swf.Keys.Control)
                modifiers |= Keys.Control;

            // Alt
            if ((keyData & swf.Keys.Alt) == swf.Keys.Alt)
                modifiers |= Keys.Alt;

            var keyCode = Find(keyData & swf.Keys.KeyCode);

            return keyCode | modifiers;
        }

		static Keys Find(swf.Keys key)
		{
			Keys mapped;
			if (Map.TryGetValue(key, out mapped))
				return mapped;
			Debug.WriteLine($"Unknown key {key}");
			return Keys.None;
		}
		
		public static swf.Keys Find(Keys key)
		{
			swf.Keys mapped;
			return InverseMap.TryGetValue(key, out mapped) ? mapped : swf.Keys.None;
		}
		
		public static swf.Keys ToSWF (this Keys key)
		{
			var code = key & Keys.KeyMask;
			swf.Keys modifiers = swf.Keys.None;
			
			// convert the modifiers
			// Shift
			if (key.HasFlag(Keys.Shift))
				modifiers |= swf.Keys.Shift;
	
			// Control
			if (key.HasFlag(Keys.Control))
				modifiers |= swf.Keys.Control;

			// Alt
			if (key.HasFlag(Keys.Alt))
				modifiers |= swf.Keys.Alt;

			return Find (code) | modifiers;
		}

		static Dictionary<swf.Keys, Keys> GetMap()
		{
			var keymap = new Dictionary<swf.Keys, Keys>();
			// keep in same order as in Keys
			keymap.Add(swf.Keys.A, Keys.A);
			keymap.Add(swf.Keys.B, Keys.B);
			keymap.Add(swf.Keys.C, Keys.C);
			keymap.Add(swf.Keys.D, Keys.D);
			keymap.Add(swf.Keys.E, Keys.E);
			keymap.Add(swf.Keys.F, Keys.F);
			keymap.Add(swf.Keys.G, Keys.G);
			keymap.Add(swf.Keys.H, Keys.H);
			keymap.Add(swf.Keys.I, Keys.I);
			keymap.Add(swf.Keys.J, Keys.J);
			keymap.Add(swf.Keys.K, Keys.K);
			keymap.Add(swf.Keys.L, Keys.L);
			keymap.Add(swf.Keys.M, Keys.M);
			keymap.Add(swf.Keys.N, Keys.N);
			keymap.Add(swf.Keys.O, Keys.O);
			keymap.Add(swf.Keys.P, Keys.P);
			keymap.Add(swf.Keys.Q, Keys.Q);
			keymap.Add(swf.Keys.R, Keys.R);
			keymap.Add(swf.Keys.S, Keys.S);
			keymap.Add(swf.Keys.T, Keys.T);
			keymap.Add(swf.Keys.U, Keys.U);
			keymap.Add(swf.Keys.V, Keys.V);
			keymap.Add(swf.Keys.W, Keys.W);
			keymap.Add(swf.Keys.X, Keys.X);
			keymap.Add(swf.Keys.Y, Keys.Y);
			keymap.Add(swf.Keys.Z, Keys.Z);
			keymap.Add(swf.Keys.F1, Keys.F1);
			keymap.Add(swf.Keys.F2, Keys.F2);
			keymap.Add(swf.Keys.F3, Keys.F3);
			keymap.Add(swf.Keys.F4, Keys.F4);
			keymap.Add(swf.Keys.F5, Keys.F5);
			keymap.Add(swf.Keys.F6, Keys.F6);
			keymap.Add(swf.Keys.F7, Keys.F7);
			keymap.Add(swf.Keys.F8, Keys.F8);
			keymap.Add(swf.Keys.F9, Keys.F9);
			keymap.Add(swf.Keys.F10, Keys.F10);
			keymap.Add(swf.Keys.F11, Keys.F11);
			keymap.Add(swf.Keys.F12, Keys.F12);
			keymap.Add(swf.Keys.D0, Keys.D0);
			keymap.Add(swf.Keys.D1, Keys.D1);
			keymap.Add(swf.Keys.D2, Keys.D2);
			keymap.Add(swf.Keys.D3, Keys.D3);
			keymap.Add(swf.Keys.D4, Keys.D4);
			keymap.Add(swf.Keys.D5, Keys.D5);
			keymap.Add(swf.Keys.D6, Keys.D6);
			keymap.Add(swf.Keys.D7, Keys.D7);
			keymap.Add(swf.Keys.D8, Keys.D8);
			keymap.Add(swf.Keys.D9, Keys.D9);
			keymap.Add(swf.Keys.OemMinus, Keys.Minus);
			keymap.Add(swf.Keys.Oemtilde, Keys.Grave);
			keymap.Add(swf.Keys.Insert, Keys.Insert);
			keymap.Add(swf.Keys.Home, Keys.Home);
			keymap.Add(swf.Keys.PageDown, Keys.PageDown);
			keymap.Add(swf.Keys.PageUp, Keys.PageUp);
			keymap.Add(swf.Keys.Delete, Keys.Delete);
			keymap.Add(swf.Keys.End, Keys.End);
			keymap.Add(swf.Keys.Divide, Keys.Divide);
			keymap.Add(swf.Keys.Decimal, Keys.Decimal);
			keymap.Add(swf.Keys.Back, Keys.Backspace);
			keymap.Add(swf.Keys.Up, Keys.Up);
			keymap.Add(swf.Keys.Down, Keys.Down);
			keymap.Add(swf.Keys.Left, Keys.Left);
			keymap.Add(swf.Keys.Right, Keys.Right);
			keymap.Add(swf.Keys.Tab, Keys.Tab);
			keymap.Add(swf.Keys.Space, Keys.Space);
			keymap.Add(swf.Keys.CapsLock, Keys.CapsLock);
			keymap.Add(swf.Keys.Scroll, Keys.ScrollLock);
			keymap.Add(swf.Keys.PrintScreen, Keys.PrintScreen);
			keymap.Add(swf.Keys.NumLock, Keys.NumberLock);
			keymap.Add(swf.Keys.Enter, Keys.Enter);
			keymap.Add(swf.Keys.Escape, Keys.Escape);
			keymap.Add(swf.Keys.Multiply, Keys.Multiply);
			keymap.Add(swf.Keys.Add, Keys.Add);
			keymap.Add(swf.Keys.Subtract, Keys.Subtract);
			keymap.Add(swf.Keys.Help, Keys.Help);
			keymap.Add(swf.Keys.Pause, Keys.Pause);
			keymap.Add(swf.Keys.Clear, Keys.Clear);
			//keymap.Add(swf.Keys., Keys.KeypadEqual);
			keymap.Add(swf.Keys.Menu, Keys.Menu);
			keymap.Add(swf.Keys.OemPipe, Keys.Backslash);
			keymap.Add(swf.Keys.Oemplus, Keys.Equal);
			keymap.Add(swf.Keys.OemSemicolon, Keys.Semicolon);
			keymap.Add(swf.Keys.OemQuotes, Keys.Quote);
			keymap.Add(swf.Keys.Oemcomma, Keys.Comma);
			keymap.Add(swf.Keys.OemPeriod, Keys.Period);
			keymap.Add(swf.Keys.OemQuestion, Keys.Slash);
			keymap.Add(swf.Keys.OemCloseBrackets, Keys.RightBracket);
			keymap.Add(swf.Keys.OemOpenBrackets, Keys.LeftBracket);
			keymap.Add(swf.Keys.Apps, Keys.ContextMenu);
			keymap.Add(swf.Keys.NumPad0, Keys.Keypad0);
			keymap.Add(swf.Keys.NumPad1, Keys.Keypad1);
			keymap.Add(swf.Keys.NumPad2, Keys.Keypad2);
			keymap.Add(swf.Keys.NumPad3, Keys.Keypad3);
			keymap.Add(swf.Keys.NumPad4, Keys.Keypad4);
			keymap.Add(swf.Keys.NumPad5, Keys.Keypad5);
			keymap.Add(swf.Keys.NumPad6, Keys.Keypad6);
			keymap.Add(swf.Keys.NumPad7, Keys.Keypad7);
			keymap.Add(swf.Keys.NumPad8, Keys.Keypad8);
			keymap.Add(swf.Keys.NumPad9, Keys.Keypad9);
			return keymap;
		}

		static Dictionary<Keys, swf.Keys> GetInverseMap()
		{
			var inverse = new Dictionary<Keys, swf.Keys>();
			foreach (var entry in Map)
			{
				inverse[entry.Value] = entry.Key;
			}
			return inverse;
		}
	}
}

