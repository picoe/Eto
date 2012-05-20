using System;
using System.Reflection;
using Eto.Forms;
using MonoMac.AppKit;

namespace Eto.Platform.Mac.Forms.Controls
{
	public class CheckBoxHandler : MacButton<NSButton, CheckBox>, ICheckBox
	{
		
		public class EtoCheckBoxButton : NSButton, IMacControl
		{
			public object Handler { get; set; }
		}

		public CheckBoxHandler ()
		{
			Control = new EtoCheckBoxButton { Handler = this };
			Control.SetButtonType (NSButtonType.Switch);
			Control.Activated += delegate {
				Widget.OnCheckedChanged (EventArgs.Empty);
			};
		}

		public bool? Checked {
			get { 
				switch (Control.State) {
				case NSCellStateValue.On:
					return true;
				case NSCellStateValue.Off:
					return false;
				default:
				case NSCellStateValue.Mixed:
					return null;
				}
			}
			set { 
				if (value == null)
					Control.State = NSCellStateValue.Mixed;
				else if (value.Value)
					Control.State = NSCellStateValue.On;
				else
					Control.State = NSCellStateValue.Off;
			}
		}
		
		public bool ThreeState {
			get { return Control.AllowsMixedState; }
			set { Control.AllowsMixedState = value; }
		}
	}
}
