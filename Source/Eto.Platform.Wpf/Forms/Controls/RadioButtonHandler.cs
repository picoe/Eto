using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using swc = System.Windows.Controls;
using Eto.Forms;

namespace Eto.Platform.Wpf.Forms.Controls
{
	public class RadioButtonHandler : WpfControl<swc.RadioButton, RadioButton>, IRadioButton
	{
		public void Create (RadioButton controller)
		{
			Control = new swc.RadioButton ();
			if (controller != null) {
				var parent = controller.ControlObject as swc.RadioButton;
				Control.GroupName = parent.GroupName;
			}
			else
				Control.GroupName = Guid.NewGuid ().ToString ();

			Control.Checked += delegate {
				Widget.OnCheckedChanged (EventArgs.Empty);
			};
			Control.Unchecked += delegate {
				Widget.OnCheckedChanged (EventArgs.Empty);
			};
		}

		public bool Checked
		{
			get { return Control.IsChecked ?? false; }
			set { Control.IsChecked = value; }
		}

		public string Text
		{
			get { return Control.Content as string; }
			set { Control.Content = value; }
		}
	}
}
