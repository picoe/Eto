using Eto.Drawing;
using Eto.Forms;

namespace Eto.Test.Sections.Controls
{
	[Section("Controls", typeof(CheckBox))]
	public class CheckBoxSection : Scrollable
	{
		public CheckBoxSection()
		{
			var layout = new DynamicLayout { DefaultSpacing = new Size(5, 5), Padding = new Padding(10) };

			layout.Add(Default());

			layout.Add(Disabled());

			layout.Add(SetInitialValue());

			layout.Add(ThreeState());

			layout.Add(ThreeStateInitialValue());

			layout.Add(new CheckBox { Text = "With Larger Font", Font = SystemFonts.Label(40) });
			layout.Add(new CheckBox { Text = "With Smaller Font", Font = SystemFonts.Label(6) });

			layout.Add(null, false, true);


			Content = layout;
		}

		Control Default()
		{
			var control = new CheckBox
			{
				Text = "Default"
			};
			LogEvents(control);
			return control;
		}

		Control Disabled()
		{
			var control = new CheckBox
			{
				Text = "Disabled",
				Enabled = false
			};
			LogEvents(control);
			return control;
		}

		Control SetInitialValue()
		{
			var control = new CheckBox
			{
				Text = "Set initial value",
				Checked = true
			};
			LogEvents(control);

			return control;
		}

		Control ThreeState()
		{
			var control = new CheckBox
			{
				Text = "Three State",
				ThreeState = true
			};
			LogEvents(control);
			return control;
		}

		Control ThreeStateInitialValue()
		{
			var control = new CheckBox
			{
				Text = "Three State with Initial Value",
				ThreeState = true,
				Checked = null
			};
			LogEvents(control);
			return control;
		}

		void LogEvents(CheckBox control)
		{
			control.CheckedChanged += delegate
			{
				Log.Write(control, "CheckedChanged, Value: {0}", control.Checked);
			};
		}
	}
}

