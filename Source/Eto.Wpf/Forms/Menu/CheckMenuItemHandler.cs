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

		static DependencyPropertyDescriptor dpdIsChecked = DependencyPropertyDescriptor.FromProperty(swc.MenuItem.IsCheckedProperty, typeof(swc.MenuItem));

		public override void AttachEvent(string id)
		{
			switch (id)
			{
				case CheckMenuItem.CheckedChangedEvent:
					dpdIsChecked.AddValueChanged(Control, (sender, e) => Callback.OnCheckedChanged(Widget, EventArgs.Empty));
                    break;
				default:
					base.AttachEvent(id);
					break;
			}
		}
	}
}
