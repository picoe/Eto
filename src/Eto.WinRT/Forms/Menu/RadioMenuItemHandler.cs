#if TODO_XAML
using System;
using System.Collections.Generic;
using Eto.Forms;
using swc = Windows.UI.Xaml.Controls;
using swm = Windows.UI.Xaml.Media;
using swi = Windows.UI.Xaml.Input;

namespace Eto.WinRT.Forms.Menu
{
	public class RadioMenuItemHandler : MenuItemHandler<swc.MenuItem, RadioMenuItem>, IRadioMenuItem
	{
		List<RadioMenuItem> group;

		public RadioMenuItemHandler ()
		{
			Control = new swc.MenuItem {
				IsCheckable = true
			};
			Setup ();
		}


		public bool Checked
		{
			get { return Control.IsChecked; }
			set { Control.IsChecked = value; }
		}

		public void Create (RadioMenuItem controller)
		{
			if (controller != null) {
				var controllerInner = (RadioMenuItemHandler)controller.Handler;
				if (controllerInner.group == null) {
					controllerInner.group = new List<RadioMenuItem> ();
					controllerInner.group.Add (controller);
					controllerInner.Control.Click += controllerInner.control_RadioSwitch;
				}
				controllerInner.group.Add(Widget);
				Control.Click += controllerInner.control_RadioSwitch;
			}
		}

		void control_RadioSwitch (object sender, EventArgs e)
		{
			if (group != null) {
				foreach (RadioMenuItem item in group) {
					item.Checked = (item.ControlObject == sender);
				}
			}
		}
	}
}
#endif