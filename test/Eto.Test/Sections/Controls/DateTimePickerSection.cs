using System;
using Eto.Forms;
using Eto.Drawing;

namespace Eto.Test.Sections.Controls
{
	[Section("Controls", typeof(DateTimePicker))]
	public class DateTimePickerSection : Panel
	{
		public DateTimePickerSection()
		{
			var layout = new DynamicLayout { DefaultSpacing = new Size(5, 5), Padding = new Padding(10) };

			layout.BeginVertical();
			layout.AddRow("Default", Default(), "Default with Value", DefaultWithValue(), null);
			layout.AddRow("Date", DateControl(), "Date with Value", DateControlWithValue());
			layout.AddRow("Time", TimeControl(), "Time with Value", TimeControlWithValue());
			layout.AddRow("Date/Time", DateTimeControl(), "Date/Time with Value", DateTimeControlWithValue());
			layout.EndVertical();

			layout.AddCentered(TestProperties());

			// growing space at end is blank!
			layout.Add(null);

			Content = layout;
		}

		Control TestProperties()
		{
			var layout = new DynamicLayout { DefaultSpacing = new Size(5, 5) };
			DateTimePicker min, max, current, setValue;
			Button setButton;

			layout.AddRow("Min Value", min = new DateTimePicker());
			layout.AddRow("Max Value", max = new DateTimePicker());
			layout.BeginHorizontal();
			layout.Add("Set to value");
			layout.BeginVertical(Padding.Empty);
			layout.BeginHorizontal();
			layout.AddAutoSized(setValue = new DateTimePicker());
			layout.Add(setButton = new Button { Text = "Set" });
			layout.EndHorizontal();
			layout.EndVertical();
			layout.EndHorizontal();
			layout.AddRow("Value", current = new DateTimePicker());

			min.ValueChanged += (sender, e) => current.MinDate = min.Value ?? DateTime.MinValue;
			max.ValueChanged += (sender, e) => current.MaxDate = max.Value ?? DateTime.MaxValue;
			setButton.Click += (sender, e) => current.Value = setValue.Value;
			LogEvents(current);

			return layout;
		}

		DateTimePicker Default()
		{
			var control = new DateTimePicker();
			LogEvents(control);
			return control;
		}

		Control DefaultWithValue()
		{
			var control = Default();
			control.Value = DateTime.Now;
			return control;
		}

		DateTimePicker DateControl()
		{
			var control = new DateTimePicker { Mode = DateTimePickerMode.Date };
			LogEvents(control);
			return control;
		}

		Control DateControlWithValue()
		{
			var control = DateControl();
			control.Value = DateTime.Now;
			return control;
		}

		DateTimePicker TimeControl()
		{
			var control = new DateTimePicker { Mode = DateTimePickerMode.Time };
			LogEvents(control);
			return control;
		}

		Control TimeControlWithValue()
		{
			var control = TimeControl();
			control.Value = DateTime.Now;
			return control;
		}

		DateTimePicker DateTimeControl()
		{
			var control = new DateTimePicker { Mode = DateTimePickerMode.DateTime };
			LogEvents(control);
			return control;
		}

		Control DateTimeControlWithValue()
		{
			var control = DateTimeControl();
			control.Value = DateTime.Now;
			return control;
		}

		void LogEvents(DateTimePicker control)
		{
			control.ValueChanged += delegate
			{
				Log.Write(control, "ValueChanged, Value: {0}", control.Value);
			};
		}
	}
}

