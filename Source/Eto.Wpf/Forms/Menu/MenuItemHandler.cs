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
	public class MenuItemHandler<TControl, TWidget, TCallback> : WidgetHandler<TControl, TWidget, TCallback>, MenuItem.IHandler, swi.ICommand
		where TControl : swc.MenuItem
		where TWidget : MenuItem
		where TCallback: MenuItem.ICallback
	{
		Image image;
		readonly swi.RoutedCommand command = new swi.RoutedCommand();
		bool openingHandled;

		protected void Setup()
		{
			Control.Click += (sender, e) => Callback.OnClick(Widget, EventArgs.Empty);
		}

		public Image Image
		{
			get { return image; }
			set
			{
				image = value;
				Control.Icon = image.ToWpfImage(16);
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
				Control.InputBindings.Clear();
				if (value != Keys.None)
				{
					var key = value.ToWpfKey();
					var modifier = value.ToWpfModifier();
					Control.InputBindings.Add(new swi.KeyBinding { Key = key, Modifiers = modifier, Command = this });
					Control.InputGestureText = value.ToShortcutString();
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
			if (!openingHandled)
			{
				Control.SubmenuOpened += HandleContextMenuOpening;
				openingHandled = true;
			}
		}

		public void RemoveMenu(MenuItem item)
		{
			Control.Items.Remove(item.ControlObject);
		}

		public void Clear()
		{
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
			Callback.OnClick(Widget, EventArgs.Empty);
		}

		public void CreateFromCommand(Command command)
		{
		}
	}
}
