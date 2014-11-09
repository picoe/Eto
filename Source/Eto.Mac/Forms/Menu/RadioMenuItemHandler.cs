using System;
using Eto.Forms;
using System.Collections.Generic;
#if XAMMAC2
using AppKit;
using Foundation;
using CoreGraphics;
using ObjCRuntime;
using CoreAnimation;
#else
using MonoMac.AppKit;
using MonoMac.Foundation;
using MonoMac.CoreGraphics;
using MonoMac.ObjCRuntime;
using MonoMac.CoreAnimation;
#endif

namespace Eto.Mac.Forms.Menu
{
	public class RadioMenuItemHandler : MenuHandler<NSMenuItem, RadioMenuItem, RadioMenuItem.ICallback>, RadioMenuItem.IHandler, IMenuActionHandler
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
			Callback.OnClick(Widget, EventArgs.Empty);
			
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
			set { Control.ToolTip = value ?? string.Empty; }
		}

		public Keys Shortcut
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

		MenuItem IMenuActionHandler.Widget
		{
			get { return Widget; }
		}

		public override void AttachEvent(string id)
		{
			switch (id)
			{
				case MenuItem.ValidateEvent:
					// handled in MenuActionHandler
					break;
				default:
					base.AttachEvent(id);
					break;
			}
		}
	}
}
