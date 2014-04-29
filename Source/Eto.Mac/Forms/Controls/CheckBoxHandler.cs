using System;
using Eto.Forms;
using MonoMac.AppKit;

namespace Eto.Mac.Forms.Controls
{
	public class CheckBoxHandler : MacButton<NSButton, CheckBox>, ICheckBox
	{
		
		public class EtoCheckBoxButton : NSButton, IMacControl
		{
			public WeakReference WeakHandler { get; set; }

			public object Handler
			{ 
				get { return WeakHandler.Target; }
				set { WeakHandler = new WeakReference(value); } 
			}
		}

		public CheckBoxHandler()
		{
			Control = new EtoCheckBoxButton { Handler = this, Title = string.Empty };
			Control.SetButtonType(NSButtonType.Switch);
			Control.Activated += HandleActivated;
		}

		static void HandleActivated(object sender, EventArgs e)
		{
			var handler = GetHandler(sender) as CheckBoxHandler;
			handler.Widget.OnCheckedChanged(EventArgs.Empty);
		}

		public bool? Checked
		{
			get
			{ 
				switch (Control.State)
				{
					case NSCellStateValue.On:
						return true;
					case NSCellStateValue.Off:
						return false;
					default:
						return null;
				}
			}
			set
			{ 
				if (value == null)
					Control.State = ThreeState ? NSCellStateValue.Mixed : NSCellStateValue.Off;
				else if (value.Value)
					Control.State = NSCellStateValue.On;
				else
					Control.State = NSCellStateValue.Off;
			}
		}

		public bool ThreeState
		{
			get { return Control.AllowsMixedState; }
			set { Control.AllowsMixedState = value; }
		}
	}
}
