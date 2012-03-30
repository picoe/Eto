using System;
using System.Reflection;
using Eto.Forms;
using MonoTouch.UIKit;

namespace Eto.Platform.iOS.Forms.Controls
{
	public class CheckBoxHandler : iosControl<UISwitch, CheckBox>, ICheckBox
	{

		public CheckBoxHandler()
		{
			Control = new UISwitch();
			Control.ValueChanged += delegate {
				Widget.OnCheckedChanged(EventArgs.Empty);
			};
		}
		
		public string Text {
			get {
				return null;
			}
			set {
				
			}
		}
		
		public bool ThreeState {
			get {
				throw new NotImplementedException ();
			}
			set {
				throw new NotImplementedException ();
			}
		}

		public bool? Checked
		{
			get { return Control.On; }
			set { Control.On = value ?? false; }
		}
	}
}
