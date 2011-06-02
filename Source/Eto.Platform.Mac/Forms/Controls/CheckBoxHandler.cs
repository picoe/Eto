using System;
using System.Reflection;
using Eto.Forms;
using MonoMac.AppKit;

namespace Eto.Platform.Mac
{
	public class CheckBoxHandler : MacButton<NSButton, CheckBox>, ICheckBox
	{

		public CheckBoxHandler ()
		{
			Control = new NSButton ();
			Control.SetButtonType (NSButtonType.OnOff);
			//control.CheckedChanged += control_CheckedChanged;
		}

		private void control_CheckedChanged (object sender, EventArgs e)
		{
			this.Widget.OnCheckedChanged (e);
		}
		
		#region ICheckBox Members

		public bool Checked {
			get { return Control.State == NSCellStateValue.On; }
			set { Control.State = value ? NSCellStateValue.On : NSCellStateValue.Off; }
		}

		#endregion
	}
}
