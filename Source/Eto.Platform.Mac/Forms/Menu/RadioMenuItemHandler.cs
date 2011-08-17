using System;
using System.Collections;
using System.Reflection;
using Eto.Drawing;
using Eto.Forms;
using MonoMac.AppKit;
using System.Collections.Generic;

namespace Eto.Platform.Mac
{
	public class RadioMenuItemHandler : MenuHandler<NSMenuItem, RadioMenuItem>, IRadioMenuItem
	{
		List<RadioMenuItem> radioGroup;

		public RadioMenuItemHandler ()
		{
			Control = new NSMenuItem ();
			//control.SetButtonType(NSButtonType.Radio);
			Control.Activated += control_Click;
		}

		private void control_Click (object sender, EventArgs e)
		{
			Widget.OnClick (e);
			
			if (radioGroup != null) {
				foreach (RadioMenuItem item in radioGroup) {
					item.Checked = (item.ControlObject == sender);
				}
			}
		}

		public void Create (RadioMenuItem controller)
		{
			if (controller != null) {
				var controllerInner = (RadioMenuItemHandler)controller.Handler;
				if (controllerInner.radioGroup == null) {
					controllerInner.radioGroup = new List<RadioMenuItem> ();
					controllerInner.radioGroup.Add (controller);
				}
				controllerInner.radioGroup.Add (this.Widget);
				this.radioGroup = controllerInner.radioGroup;
			}
		}

		#region IMenuItem Members

		public bool Enabled {
			get { return Control.Enabled; }
			set { Control.Enabled = value; }
		}

		public string Text {
			get	{ return Control.Title; }
			set { Control.SetTitleWithMnemonic (value); }
		}
		
		public string ToolTip {
			get { return Control.ToolTip; }
			set { Control.ToolTip = value; }
		}

		public Key Shortcut {
			get { return KeyMap.Convert (Control.KeyEquivalent, Control.KeyEquivalentModifierMask); }
			set { 
				this.Control.KeyEquivalent = KeyMap.KeyEquivalent (value);
				this.Control.KeyEquivalentModifierMask = KeyMap.KeyEquivalentModifierMask (value);
			}
		}

		public bool Checked {
			get { return Control.State == NSCellStateValue.On; }
			set { Control.State = value ? NSCellStateValue.On : NSCellStateValue.Off; }
		}

		#endregion

	}
}
