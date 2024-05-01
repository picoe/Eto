namespace Eto.GtkSharp.Forms
{
	public class KeyboardHandler : Keyboard.IHandler
	{
		EventHandler<EventArgs> _modifiersChanged;

		public event EventHandler<EventArgs> ModifiersChanged
		{
			add
			{
				if (_modifiersChanged == null)
				{
					Gdk.Keymap.Default.StateChanged += Keymap_StateChanged;
				}
				_modifiersChanged += value;
			}
			remove
			{
				_modifiersChanged -= value;
				if (_modifiersChanged == null)
				{
					Gdk.Keymap.Default.StateChanged -= Keymap_StateChanged;
				}
			}
		}
		
		private void Keymap_StateChanged(object sender, EventArgs e)
		{
			_modifiersChanged?.Invoke(null, EventArgs.Empty);
		}

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

		public Keys Modifiers
		{
			get
			{
				
				var ev = Gtk.Application.CurrentEvent;
				if (ev != null)
				{
					Gdk.ModifierType state;
					if (Gdk.EventHelper.GetState(ev, out state))
					{
						return state.ToEtoKey();
					}
				}
				return ((Gdk.ModifierType)Gdk.Keymap.Default.ModifierState).ToEtoKey();
			}
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

