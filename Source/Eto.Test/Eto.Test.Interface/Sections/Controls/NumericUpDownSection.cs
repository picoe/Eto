using System;
using Eto.Forms;
using Eto.Drawing;
using Eto.Test.Interface.Controls;

namespace Eto.Test.Interface.Sections.Controls
{
	public class NumericUpDownSection : SectionBase
	{
		public NumericUpDownSection ()
		{
			var layout = new DynamicLayout (this);
			
			layout.AddRow (new Label{ Text = "Default" }, Default ());
			
			layout.AddRow (new Label{ Text = "Disabled" }, Disabled ());
			
			layout.AddRow (new Label{ Text = "Set Min/Max" }, SetMinMax ());
			
			
			// growing space at end is blank!
			layout.Add (null);
		}
		
		Control Default ()
		{
			var control = new NumericUpDown ();
			LogEvents (control);
			return control;
		}

		Control Disabled ()
		{
			var control = SetMinMax ();
			control.Enabled = false;
			return control;
		}

		Control SetMinMax ()
		{
			var control = new NumericUpDown{
				Value = 24,
				MinValue = 20,
				MaxValue = 2000
			};
			LogEvents (control);
			return control;
		}
		
		void LogEvents (NumericUpDown control)
		{
			control.ValueChanged += delegate {
				Log.Write (control, "ValueChanged, Value: {0}", control.Value);
			};
		}
	}
}

