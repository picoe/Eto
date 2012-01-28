using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Eto.Forms;
using swc = System.Windows.Controls;
using swm = System.Windows.Media;
using swi = System.Windows.Input;
using Eto.Platform.Wpf.Drawing;

namespace Eto.Platform.Wpf.Forms.Menu
{
	public class WpfMenuItem<C,W> : WidgetHandler<C, W>, IMenuActionItem
		where C: swc.MenuItem
		where W: MenuActionItem
	{
		Eto.Drawing.Icon icon;
		swi.RoutedCommand command = new swi.RoutedCommand();

		protected void Setup ()
		{
			Control.Click += delegate {
				Widget.OnClick (EventArgs.Empty);
			};
		}

		public Eto.Drawing.Icon Icon
		{
			get { return icon; }
			set
			{
				icon = value;
				if (icon != null)
					Control.Icon = new swc.Image { 
						Source = ((IconHandler)icon.Handler).GetIconClosestToSize(16), 
						MaxWidth = 16, MaxHeight = 16 };
				else
					Control.Icon = null;
			}
		}

		public string Text
		{
			get { return Generator.ConvertMneumonicFromWPF(Control.Header); }
			set { Control.Header = Generator.ConvertMneumonicToWPF(value); }
		}

		public string ToolTip
		{
			get { return Control.ToolTip as string; }
			set { Control.ToolTip = value; }
		}

		public Key Shortcut
		{
			get { 
				var keyBinding = Control.InputBindings.OfType<swi.KeyBinding>().FirstOrDefault();
				if (keyBinding != null)
					return KeyMap.Convert(keyBinding.Key, keyBinding.Modifiers);
				return Key.None;
			}
			set {
				Control.InputBindings.Clear ();
				if (value != Key.None) {
					var key = KeyMap.ConvertKey (value);
					var modifier = KeyMap.ConvertModifier (value);
					Control.InputBindings.Add (new swi.KeyBinding { Key = key, Modifiers = modifier });
					Control.InputGestureText = KeyMap.KeyToString (value);
				}
				else 
					Control.InputGestureText = null;
			}
		}

		public bool Enabled
		{
			get { return Control.IsEnabled; }
			set { Control.IsEnabled = value; }
		}

		public void AddMenu (int index, MenuItem item)
		{
			Control.Items.Insert (index, item.ControlObject);
		}

		public void RemoveMenu (MenuItem item)
		{
			Control.Items.Remove (item.ControlObject);
		}

		public void Clear ()
		{
			Control.Items.Clear ();	
		}
	}
}
