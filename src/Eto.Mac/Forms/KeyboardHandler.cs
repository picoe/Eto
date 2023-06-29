namespace Eto.Mac.Forms
{
	public class KeyboardHandler : Keyboard.IHandler
	{
		EventHandler<EventArgs> _modifiersChanged;
		NSObject _monitor;
		public event EventHandler<EventArgs> ModifiersChanged
		{
			add
			{
				if (_modifiersChanged == null)
				{
					_monitor = NSEvent.AddLocalMonitorForEventsMatchingMask(NSEventMask.FlagsChanged, HandleFlagsChanged);
				}
				_modifiersChanged += value;
			}
			remove
			{
				_modifiersChanged -= value;
				if (_modifiersChanged == null && _monitor != null)
				{
					NSEvent.RemoveMonitor(_monitor);
					_monitor = null;
				}
			}
		}

		private NSEvent HandleFlagsChanged(NSEvent theEvent)
		{
			_modifiersChanged?.Invoke(null, EventArgs.Empty);
			return theEvent;
		}

		public bool IsKeyLocked(Keys key)
		{
			var modifier = key.ModifierMask();
			return (ModifierFlags & modifier) == modifier;
		}

		public Keys Modifiers => ModifierFlags.ToEto();
		
		NSEventModifierMask ModifierFlags => NSEvent.CurrentModifierFlags;
		// NSEventModifierMask ModifierFlags => NSApplication.SharedApplication.CurrentEvent?.ModifierFlags ?? NSEvent.CurrentModifierFlags;

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

