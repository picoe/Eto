using System;
using Eto.Forms;
using MonoMac.AppKit;
using System.Collections.Generic;

namespace Eto.Platform.Mac
{
	public class RadioMenuItemHandler : MenuHandler<NSMenuItem, RadioMenuItem>, IRadioMenuItem, IMenuActionHandler
	{
		List<RadioMenuItem> radioGroup;

		public RadioMenuItemHandler()
		{
			Control = new NSMenuItem();
			Enabled = true;
			//control.SetButtonType(NSButtonType.Radio);
			Control.Target = new MenuActionHandler { Handler = this };
			Control.Action = MenuActionHandler.selActivate;
		}

		public void HandleClick()
		{
			Widget.OnClick(EventArgs.Empty);
			
			if (radioGroup != null)
			{
				foreach (RadioMenuItem item in radioGroup)
				{
					item.Checked = (item.ControlObject == Control);
				}
			}
		}

		public void Create(RadioMenuItem controller)
		{
			if (controller != null)
			{
				var controllerInner = (RadioMenuItemHandler)controller.Handler;
				if (controllerInner.radioGroup == null)
				{
					controllerInner.radioGroup = new List<RadioMenuItem>();
					controllerInner.radioGroup.Add(controller);
				}
				controllerInner.radioGroup.Add(Widget);
				radioGroup = controllerInner.radioGroup;
			}
		}

		public bool Enabled
		{
			get { return Control.Enabled; }
			set { Control.Enabled = value; }
		}

		public string Text
		{
			get	{ return Control.Title; }
			set { Control.SetTitleWithMnemonic(value); }
		}

		public string ToolTip
		{
			get { return Control.ToolTip; }
			set { Control.ToolTip = value; }
		}

		public Key Shortcut
		{
			get { return KeyMap.Convert(Control.KeyEquivalent, Control.KeyEquivalentModifierMask); }
			set
			{ 
				Control.KeyEquivalent = KeyMap.KeyEquivalent(value);
				Control.KeyEquivalentModifierMask = KeyMap.KeyEquivalentModifierMask(value);
			}
		}

		public bool Checked
		{
			get { return Control.State == NSCellStateValue.On; }
			set { Control.State = value ? NSCellStateValue.On : NSCellStateValue.Off; }
		}

		MenuActionItem IMenuActionHandler.Widget
		{
			get { return Widget; }
		}
	}
}
