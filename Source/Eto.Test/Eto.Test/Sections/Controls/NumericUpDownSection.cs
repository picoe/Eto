using Eto.Drawing;
using Eto.Forms;

namespace Eto.Test.Sections.Controls
{
	[Section("Controls", typeof(NumericUpDown))]
	public class NumericUpDownSection : Panel
	{
		public NumericUpDownSection()
		{
			var layout = new DynamicLayout { DefaultSpacing = new Size(5, 5), Padding = new Padding(10) };

			layout.AddRow(new Label { Text = "Default" }, Default());

			layout.AddRow(new Label { Text = "Disabled" }, Disabled());

			layout.AddRow(new Label { Text = "Set Min/Max" }, SetMinMax());

			layout.AddRow(new Label { Text = "Decimal Places" }, GetWithDecimalPlaces());

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
		Control GetWithDecimalPlaces()
		{
			var control = new NumericUpDown
			{
				Value = 24,
				DecimalPlaces = 5,
				Increment = 0.1
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

