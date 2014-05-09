using System;
using Eto.Forms;
using MonoMac.AppKit;

namespace Eto.Mac
{
	public class CheckMenuItemHandler : MenuHandler<NSMenuItem, CheckMenuItem>, CheckMenuItem.IHandler, IMenuActionHandler
	{
		public CheckMenuItemHandler ()
		{
			Control = new NSMenuItem ();
			Enabled = true;
			Control.Target = new MenuActionHandler{ Handler = this };
			Control.Action = MenuActionHandler.selActivate;
		}
		
		public void HandleClick ()
		{
			Widget.OnClick (EventArgs.Empty);
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
			set { Control.ToolTip = value ?? string.Empty; }
		}

		public Keys Shortcut {
			get { return KeyMap.Convert (Control.KeyEquivalent, Control.KeyEquivalentModifierMask); }
			set { 
				Control.KeyEquivalent = KeyMap.KeyEquivalent (value);
				Control.KeyEquivalentModifierMask = KeyMap.KeyEquivalentModifierMask (value);
			}
		}

		public bool Checked {
			get { return Control.State == NSCellStateValue.On; }
			set { Control.State = value ? NSCellStateValue.On : NSCellStateValue.Off; }
		}

		#endregion

		MenuItem IMenuActionHandler.Widget {
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
