using System;
using Eto.Forms;
using System.ComponentModel;
using sw = System.Windows;
using swc = System.Windows.Controls;
using swm = System.Windows.Media;
using swi = System.Windows.Input;

namespace Eto.Wpf.Forms.Menu
{
	public class CheckMenuItemHandler : MenuItemHandler<swc.MenuItem, CheckMenuItem, CheckMenuItem.ICallback>, CheckMenuItem.IHandler
	{
		public CheckMenuItemHandler()
		{
			Control = new swc.MenuItem
			{
				IsCheckable = true
			};
		}

		public bool Checked
		{
			get { return Control.IsChecked; }
			set { Control.IsChecked = value; }
		}

		public override void AttachEvent(string id)
		{
			switch (id)
			{
				case CheckMenuItem.CheckedChangedEvent:
					Widget.Properties.Set(swc.MenuItem.IsCheckedProperty, PropertyChangeNotifier.Register(swc.MenuItem.IsCheckedProperty, HandleIsCheckedChanged, Control));
                    break;
				default:
					base.AttachEvent(id);
					break;
			}
		}

		void HandleIsCheckedChanged(object sender, EventArgs e)
		{
			Callback.OnCheckedChanged(Widget, EventArgs.Empty);
		}
	}
}
