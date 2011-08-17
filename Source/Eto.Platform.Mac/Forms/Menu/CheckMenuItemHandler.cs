using System;
using System.Reflection;
using Eto.Drawing;
using Eto.Forms;
using MonoMac.AppKit;
using MonoMac.Foundation;

namespace Eto.Platform.Mac
{
	public class CheckMenuItemHandler : MenuHandler<NSMenuItem, CheckMenuItem>, ICheckMenuItem
	{

		public CheckMenuItemHandler ()
		{
			Control = new NSMenuItem ();
			Control.Activated += delegate {
				Widget.OnClick (EventArgs.Empty);
			};
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
