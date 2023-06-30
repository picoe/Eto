namespace Eto.Wpf.Forms
{
	public class KeyboardHandler : Keyboard.IHandler
	{
		public bool IsKeyLocked(Keys key)
		{
			return swi.Keyboard.IsKeyToggled(key.ToWpfKey());
		}

		public IEnumerable<Keys> SupportedLockKeys
		{
			get
			{
				yield return Keys.CapsLock;
				yield return Keys.NumberLock;
				yield return Keys.ScrollLock;
				yield return Keys.Insert;
			}
		}

		public Keys Modifiers
		{
			get { return swi.Keyboard.Modifiers.ToEto(); }
		}
	}
}
