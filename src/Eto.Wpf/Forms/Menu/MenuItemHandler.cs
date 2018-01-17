using System;
using System.Linq;
using Eto.Forms;
using sw = System.Windows;
using swc = System.Windows.Controls;
using swm = System.Windows.Media;
using swi = System.Windows.Input;
using Eto.Drawing;

namespace Eto.Wpf.Forms.Menu
{
	public class MenuItemHandler<TControl, TWidget, TCallback> : MenuHandler<TControl, TWidget, TCallback>, MenuItem.IHandler, swi.ICommand, IWpfValidateBinding
		where TControl : swc.MenuItem
		where TWidget : MenuItem
		where TCallback: MenuItem.ICallback
	{
		Image image;
		bool openingHandled;

		protected override void Initialize()
		{
			base.Initialize();
			Control.Click += (sender, e) => OnClick();
		}

		protected virtual void OnClick()
		{
			Callback.OnClick(Widget, EventArgs.Empty);
		}

		public Image Image
		{
			get { return image; }
			set
			{
				image = value;
				Control.Icon = image.ToWpfImage(Screen.PrimaryScreen.LogicalPixelSize, new Size(16, 16));
			}
		}

		public string Text
		{
			get { return (Control.Header as string).ToEtoMnemonic(); }
			set { Control.Header = value.ToPlatformMnemonic(); }
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
				var keyBinding = Control.InputBindings.OfType<swi.KeyBinding>().FirstOrDefault();
				return keyBinding == null ? Keys.None : keyBinding.Key.ToEtoWithModifier(keyBinding.Modifiers);
			}
			set
			{
				RemoveKeyBindings(Control);
                Control.InputBindings.Clear();
				if (value != Keys.None)
				{
					var key = value.ToWpfKey();
					var modifier = value.ToWpfModifier();
                    Control.InputBindings.Add(new swi.KeyBinding { Key = key, Modifiers = modifier, Command = this });
					Control.InputGestureText = value.ToShortcutString();
					AddKeyBindings(Control);
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
				OnCanExecuteChanged(EventArgs.Empty);
			}
		}

		public override void AttachEvent(string id)
		{
			switch (id)
			{
				case MenuItem.ValidateEvent:
					// handled by parent
					break;
				default:
					base.AttachEvent(id);
					break;
			}
		}

		public void AddMenu(int index, MenuItem item)
		{
			Control.Items.Insert(index, item.ControlObject);
			AddKeyBindings(item.ControlObject as sw.FrameworkElement);
            if (!openingHandled)
			{
				Control.SubmenuOpened += HandleContextMenuOpening;
				openingHandled = true;
			}
		}

		public void RemoveMenu(MenuItem item)
		{
			RemoveKeyBindings(item.ControlObject as sw.FrameworkElement);
			Control.Items.Remove(item.ControlObject);
		}

		public void Clear()
		{
			foreach (var item in Control.Items.OfType<sw.FrameworkElement>())
				RemoveKeyBindings(item);
			Control.Items.Clear();
		}

		bool swi.ICommand.CanExecute(object parameter)
		{
			return Enabled;
		}

		void HandleContextMenuOpening(object sender, sw.RoutedEventArgs e)
		{
			var submenu = Widget as ISubmenu;
			if (submenu != null)
			{
				foreach (var item in submenu.Items)
				{
					var handler = item.Handler as MenuItemHandler<TControl, TWidget, TCallback>;
					if (handler != null)
						handler.Callback.OnValidate(handler.Widget, EventArgs.Empty);
				}
			}
		}


		public event EventHandler CanExecuteChanged;

		protected virtual void OnCanExecuteChanged(EventArgs e)
		{
			if (CanExecuteChanged != null)
				CanExecuteChanged(this, e);
		}

		void swi.ICommand.Execute(object parameter)
		{
			var contextMenu = Control.GetParents().OfType<swc.ContextMenu>().LastOrDefault();
			if (contextMenu != null)
			{
				contextMenu.IsOpen = false;
			}
			var menu = Control.GetParents().OfType<swc.Menu>().LastOrDefault();
			if (menu != null && menu.IsKeyboardFocusWithin)
			{
				swi.Keyboard.ClearFocus();
			}
			Widget.PerformClick();
		}

		public void CreateFromCommand(Command command)
		{
		}

		public void Validate()
		{
			Callback.OnValidate(Widget, EventArgs.Empty);
		}
	}
}
