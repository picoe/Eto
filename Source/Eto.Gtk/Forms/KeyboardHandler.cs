using System;
using Eto.Forms;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Eto.GtkSharp.Forms
{
	public class KeyboardHandler : Keyboard.IHandler
	{
		public bool IsKeyLocked(Keys key)
		{
			#if GTK3
			switch (key)
			{
				case Keys.CapsLock:
					return Gdk.Keymap.Default.CapsLockState;
				case Keys.NumberLock:
					return Gdk.Keymap.Default.NumLockState;
				default:
					return false;
			}
			#else
			return false;
			#endif
		}

		public IEnumerable<Keys> SupportedLockKeys
		{
			get
			{
				#if GTK3
				yield return Keys.CapsLock;
				yield return Keys.NumberLock;
				#else
				yield break;
				#endif
			}
		}
	}
}

