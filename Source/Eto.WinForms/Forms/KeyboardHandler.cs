using System;
using Eto.Forms;
using swf = System.Windows.Forms;
using System.Collections.Generic;

namespace Eto.WinForms.Forms
{
	public class KeyboardHandler : Keyboard.IHandler
	{
		public bool IsKeyLocked(Keys key)
		{
			return swf.Control.IsKeyLocked(key.ToSWF());
		}

		public Keys Modifiers
		{
			get { return swf.Control.ModifierKeys.ToEto(); }
		}

		public IEnumerable<Keys> SupportedLockKeys
		{
			get
			{
				yield return Keys.NumberLock;
				yield return Keys.CapsLock;
				yield return Keys.Insert;
				yield return Keys.ScrollLock;
			}
		}
	}
}