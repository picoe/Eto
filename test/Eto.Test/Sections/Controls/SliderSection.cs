using Eto.Forms;
using Eto.Drawing;

namespace Eto.Test.Sections.Controls
{
	[Section("Controls", typeof(Slider))]
	public class SliderSection : Panel
	{
		public SliderSection()
		{
			var layout = new DynamicLayout { DefaultSpacing = new Size(5, 5), Padding = new Padding(10) };

			layout.AddRow(new Label { Text = "Default" }, Default());
			layout.AddRow(new Label { Text = "SetInitialValue" }, SetInitialValue());
			layout.AddRow(new Label { Text = "Snap To Tick" }, SnapToTick());
			layout.AddRow(new Label { Text = "Disabled" }, Disabled());
			layout.AddRow(new Label { Text = "Vertical" }, Vertical());

			layout.Add(null, null, true);

			Content = layout;
		}

		Control Default()
		{
			var control = new Slider();
			LogEvents(control);
			return control;
		}

		Slider SetInitialValue()
		{
			var control = new Slider
			{
				MinValue = 0,
				MaxValue = 1000,
				TickFrequency = 100,
				Value = 500
			};
			LogEvents(control);
			return control;
		}

		Control SnapToTick()
		{
			var control = SetInitialValue();
			control.SnapToTick = true;
			return control;
		}

		Control Disabled()
		{
			var control = SetInitialValue();
			control.Enabled = false;
			return control;
		}

		Control Vertical()
		{
			var control = SetInitialValue();
			control.Size = new Size(-1, 150);
			control.Orientation = Orientation.Vertical;
			var layout = new DynamicLayout();
			layout.AddCentered(control);
			return layout;
		}

		void LogEvents(Slider control)
		{
			control.ValueChanged += delegate
			{
				Log.Write(control, "ValueChanged, Value: {0}", control.Value);
			};
		}
	}
}

