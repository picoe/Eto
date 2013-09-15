using System;
using Eto.Forms;
using Eto.Drawing;

namespace Eto.Test.Sections.Controls
{
	public class NumericUpDownSection : Panel
	{
		public NumericUpDownSection()
		{
			var layout = new DynamicLayout();
			
			layout.AddRow(new Label { Text = "Default" }, Default());
			
			layout.AddRow(new Label { Text = "Disabled" }, Disabled());
			
			layout.AddRow(new Label { Text = "Set Min/Max" }, SetMinMax());

			// growing space at end is blank!
			layout.Add(null);

			Content = layout;
		}

		Control Default()
		{
			var control = new NumericUpDown();
			LogEvents(control);
			return control;
		}

		Control Disabled()
		{
			var control = SetMinMax();
			control.Enabled = false;
			return control;
		}

		Control SetMinMax()
		{
			var control = new NumericUpDown
			{
				Value = 24,
				MinValue = 20,
				MaxValue = 2000
			};
			LogEvents(control);
			return control;
		}

		void LogEvents(NumericUpDown control)
		{
			control.ValueChanged += delegate
			{
				Log.Write(control, "ValueChanged, Value: {0}", control.Value);
			};
		}
	}
}

