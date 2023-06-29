#if WINFORMS

namespace Eto.WinForms.Forms
#else

namespace Eto.Wpf.Forms
#endif
{
	public class KeyboardHandler : Keyboard.IHandler
	{
		EventHandler<EventArgs> _modifiersChanged;
		Keys _modifiers;
		List<Keys> _oldLockedKeys = new List<Keys>();

		Win32.HookProc _hookProc;
		IntPtr _hookId;
		Keys? _downKeys;
		Keys? _upKeys;

		IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
		{
			// only trigger event when the application is active
			if (nCode == 0 && Application.Instance.IsActive)
			{
				if (wParam == (IntPtr)Win32.WM.KEYDOWN || wParam == (IntPtr)Win32.WM.KEYUP
				|| wParam == (IntPtr)Win32.WM.SYSKEYDOWN || wParam == (IntPtr)Win32.WM.SYSKEYUP)
				{
					var kb = Marshal.PtrToStructure<Win32.KeyboardLowLevelHook>(lParam);
					// Console.WriteLine($"Callback: {wParam:x}, {lParam}, {kb.VirtualKeyCode:x}, {kb.ScanCode:x}");
					
					// this event happens before the state is updated, so we check which key was pressed.
					var keys = Keys.None;
					switch (kb.VirtualKeyCode)
					{
						case (int)Win32.VK.RMENU:
						case (int)Win32.VK.LMENU:
						case (int)Win32.VK.MENU:
							keys = Keys.Alt;
							break;
						case (int)Win32.VK.RCONTROL:
						case (int)Win32.VK.LCONTROL:
						case (int)Win32.VK.CONTROL:
							keys = Keys.Control;
							break;
						case (int)Win32.VK.RSHIFT:
						case (int)Win32.VK.LSHIFT:
						case (int)Win32.VK.SHIFT:
							keys = Keys.Shift;
							break;
						case (int)Win32.VK.RWIN:
						case (int)Win32.VK.LWIN:
							keys = Keys.Application;
							break;
					}
					if (wParam == (IntPtr)Win32.WM.KEYDOWN || wParam == (IntPtr)Win32.WM.SYSKEYDOWN)
						_downKeys = keys;
					else if (wParam == (IntPtr)Win32.WM.KEYUP || wParam == (IntPtr)Win32.WM.SYSKEYUP)
						_upKeys = keys;
					TriggerChanged();
					_downKeys = null;
					_upKeys = null;
				}
			}
			return Win32.CallNextHookEx(_hookId, nCode, wParam, lParam);
		}

		public event EventHandler<EventArgs> ModifiersChanged
		{
			add
			{
				if (_modifiersChanged == null)
				{
					_hookProc = new Win32.HookProc(HookCallback);
					_hookId = Win32.SetHook(Win32.WH.KEYBOARD_LL, _hookProc);
					_modifiers = Modifiers;
					_oldLockedKeys.Clear();
					_oldLockedKeys.AddRange(SupportedLockKeys.Where(IsKeyLocked));
				}
				_modifiersChanged += value;
			}
			remove
			{
				_modifiersChanged -= value;
				if (_modifiersChanged == null && _hookId != IntPtr.Zero)
				{
					Win32.UnhookWindowsHookEx(_hookId);
					_hookProc = null;
					_hookId = IntPtr.Zero;
					_modifiers = Keys.None;
					_oldLockedKeys.Clear();
				}
			}
		}

		private void TriggerChanged()
		{
			var newModifiers = Modifiers;
			var newLockedKeys = SupportedLockKeys.Where(IsKeyLocked).ToList();

			if (_modifiers != newModifiers || !_oldLockedKeys.SequenceEqual(newLockedKeys))
			{
				_modifiers = newModifiers;
				_oldLockedKeys = newLockedKeys;
				_modifiersChanged?.Invoke(null, EventArgs.Empty);
			}
		}

#if WINFORMS
		public bool IsKeyLocked(Keys key) => swf.Control.IsKeyLocked(key.ToSWF());
#else
		public bool IsKeyLocked(Keys key) => swi.Keyboard.IsKeyToggled(key.ToWpfKey());
#endif

		static readonly Keys[] _supportedLockKeys = new[] {
			Keys.CapsLock,
			Keys.NumberLock,
			Keys.ScrollLock,
			Keys.Insert
		};

		public IEnumerable<Keys> SupportedLockKeys => _supportedLockKeys;

		public Keys Modifiers
		{
			get
			{
#if WINFORMS
				var modifiers = swf.Control.ModifierKeys.ToEto();
#else
				var modifiers = swi.Keyboard.Modifiers.ToEto();
#endif
				if (_downKeys != null)
					modifiers |= _downKeys.Value;
				if (_upKeys != null)
					modifiers &= ~_upKeys.Value;
				return modifiers;
			}
		}
	}
}
