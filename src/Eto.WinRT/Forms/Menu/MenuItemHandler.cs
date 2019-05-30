#if TODO_XAML
using System;
using System.Linq;
using Eto.Forms;
using sw = Windows.UI.Xaml;
using swc = Windows.UI.Xaml.Controls;
using swm = Windows.UI.Xaml.Media;
using swi = Windows.UI.Xaml.Input;
using Eto.Drawing;

namespace Eto.WinRT.Forms.Menu
{
	public class MenuItemHandler<TControl, TWidget> : WidgetHandler<TControl, TWidget>, IMenuItem, swi.ICommand
		where TControl : swc.MenuItem
		where TWidget : MenuItem
	{
        Image image;
		readonly swi.RoutedCommand command = new swi.RoutedCommand ();
		bool openingHandled;

		protected void Setup ()
		{
			Control.Click += delegate {
				Widget.OnClick (EventArgs.Empty);
			};
		}

		public Image Image
		{
			get { return image; }
			set
			{
				image = value;
				Control.Icon = image.ToWpfImage (16);
			}
		}

		public string Text
		{
			get { return (Control.Header as string).ToEtoMneumonic(); }
			set { Control.Header = value.ToWpfMneumonic(); }
		}

		public string ToolTip
		{
			get { return Control.ToolTip as string; }
			set { Control.ToolTip = value; }
		}

		public Keys Shortcut
		{
			get
			{
				var keyBinding = Control.InputBindings.OfType<swi.KeyBinding> ().FirstOrDefault ();
				return keyBinding == null ? Keys.None : KeyMap.Convert(keyBinding.Key, keyBinding.Modifiers);
			}
			set
			{
				Control.InputBindings.Clear ();
				if (value != Keys.None) {
					var key = KeyMap.ConvertKey (value);
					var modifier = KeyMap.ConvertModifier (value);
					Control.InputBindings.Add (new swi.KeyBinding { Key = key, Modifiers = modifier, Command = this });
					Control.InputGestureText = value.ToShortcutString ();
				}
				else
					Control.InputGestureText = null;
			}
		}

		public bool Enabled
		{
			get { return Control.IsEnabled; }
			set
			{
				Control.IsEnabled = value;
				OnCanExecuteChanged (EventArgs.Empty);
			}
		}

		public override void AttachEvent (string id)
		{
			switch (id) {
			case MenuItem.ValidateEvent:
				// handled by parent
				break;
			default:
				base.AttachEvent (id);
				break;
			}
		}

		public void AddMenu (int index, MenuItem item)
		{
			Control.Items.Insert (index, item.ControlObject);
			if (!openingHandled) {
				Control.SubmenuOpened += HandleContextMenuOpening;
				openingHandled = true;
			}
		}

		public void RemoveMenu (MenuItem item)
		{
			Control.Items.Remove (item.ControlObject);
		}

		public void Clear ()
		{
			Control.Items.Clear ();
		}

		bool swi.ICommand.CanExecute (object parameter)
		{
			return Enabled;
		}

		void HandleContextMenuOpening (object sender, sw.RoutedEventArgs e)
		{
			var submenu = Widget as ISubMenuWidget;
			if (submenu != null) {
				foreach (var item in submenu.Items) {
					item.OnValidate (EventArgs.Empty);
				}
			}
		}


		public event EventHandler CanExecuteChanged;

		protected virtual void OnCanExecuteChanged (EventArgs e)
		{
			if (CanExecuteChanged != null)
				CanExecuteChanged (this, e);
		}

		void swi.ICommand.Execute (object parameter)
		{
			Widget.OnClick (EventArgs.Empty);
		}

		public void CreateFromCommand(Command command)
		{
		}
	}
}
#endif