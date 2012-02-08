using System;
using System.Reflection;
using Eto.Forms;
using MonoMac.AppKit;

namespace Eto.Platform.Mac
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
				Widget.OnCheckedChanged(EventArgs.Empty);
			};
		}

		
		#region ICheckBox Members

		public bool Checked {
			get { return Control.State == NSCellStateValue.On; }
			set { Control.State = value ? NSCellStateValue.On : NSCellStateValue.Off; }
		}

		#endregion
	}
}
