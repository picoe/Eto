﻿using System;
using Eto.Forms;
using System.Collections.Generic;

namespace Eto.Mac.Forms
{
	public class KeyboardHandler : Keyboard.IHandler
	{
		public bool IsKeyLocked(Keys key)
		{
			return NSEvent.CurrentModifierFlags == key.ModifierMask();
		}

		public Keys Modifiers
		{
			get { return NSEvent.CurrentModifierFlags.ToEto(); }
		}

		public IEnumerable<Keys> SupportedLockKeys
		{
			get
			{
				yield return Keys.CapsLock;
				yield return Keys.NumberLock;
			}
		}
	}
}

