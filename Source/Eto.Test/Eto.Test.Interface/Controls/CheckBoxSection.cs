using System;
using Eto.Forms;

namespace Eto.Test.Interface.Controls
{
	public class CheckBoxSection : Panel
	{
		public CheckBoxSection ()
		{
			var layout = new DynamicLayout(this);
			
			layout.Add (Default());

			layout.Add (Disabled());
			
			layout.Add (Events());
			
			layout.Add (null, false, true);
		}
		
		Control Default ()
		{
			var control = new CheckBox {
				Text = "Default"
			};
			return control;
		}
		
		Control Disabled ()
		{
			var control = new CheckBox {
				Text = "Disabled",
				Enabled = false
			};
			return control;
		}
		
		Control Events ()
		{
			var control = new CheckBox{
				Text = "Set and get state",
				Checked = true
			};
			
			control.CheckedChanged += delegate {
				MessageBox.Show (this, string.Format ("Checked was changed to {0}", control.Checked));
			};
			
			return control;
		}
	}
}

