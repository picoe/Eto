using System;
using Eto.Forms;

namespace Eto.Test.Sections.Controls
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
			LogEvents(control);
			return control;
		}
		
		Control Disabled ()
		{
			var control = new CheckBox {
				Text = "Disabled",
				Enabled = false
			};
			LogEvents(control);
			return control;
		}
		
		Control Events ()
		{
			var control = new CheckBox{
				Text = "Set initial value",
				Checked = true
			};
			LogEvents (control);
			
			return control;
		}
		
		void LogEvents(CheckBox control)
		{
			control.CheckedChanged += delegate {
				Log.Write (control, "CheckedChanged, Value: {0}", control.Checked);
			};
		}
	}
}

