using System;
using Eto.Forms;
using Eto.Drawing;

namespace Eto.Test.Sections.Controls
{
	[Section("Controls", typeof(Calendar))]
	public class CalendarSection : Scrollable
	{
		public CalendarSection()
		{
			Content = TestProperties();
		}

		Control TestProperties()
		{
			var min = new DateTimePicker();
			var max = new DateTimePicker();
			var setValue = new DateTimePicker();
			var toValue = new DateTimePicker();
			var modeSelect = new EnumRadioButtonList<CalendarMode>();
			var current = new Calendar();
			var setButton = new Button { Text = "Set" };
			var toValueSection = new StackLayout
			{
				Orientation = Orientation.Horizontal,
				Visible = false,
				Spacing = 5,
				Items = { " to ", toValue }
			};

			var layout = new TableLayout
			{
				Spacing = new Size(5, 5),
				Padding = new Padding(10),
				Rows = 
				{
					new TableRow("Min Value", min),
					new TableRow("Max Value", max),
					new TableRow("Mode", modeSelect),
					new TableRow("Set to value",
						new StackLayout
						{
							Orientation = Orientation.Horizontal,
							Spacing = 5,
							Items = { setValue, toValueSection, setButton }
						}
					),
					new TableRow("Value", TableLayout.AutoSized(current), null),
					null
				}
			};

			modeSelect.SelectedValueBinding.Bind(() => current.Mode, v =>
			{
				toValueSection.Visible = v == CalendarMode.Range;
				current.Mode = v;
			});
			min.ValueChanged += (sender, e) => current.MinDate = min.Value ?? DateTime.MinValue;
			max.ValueChanged += (sender, e) => current.MaxDate = max.Value ?? DateTime.MaxValue;
			setButton.Click += (sender, e) =>
			{
				if (current.Mode == CalendarMode.Range)
					current.SelectedRange = (setValue.Value != null && toValue.Value != null) ? new Range<DateTime>(setValue.Value.Value, toValue.Value.Value) : current.SelectedRange;
				else
					current.SelectedDate = setValue.Value ?? current.SelectedDate;
			};
			LogEvents(current);

			return layout;
		}

		void LogEvents(Calendar control)
		{
			control.SelectedDateChanged += (sender, e) => Log.Write(control, "SelectedDateChanged, Value: {0}", control.SelectedDate);
			control.SelectedRangeChanged += (sender, e) => Log.Write(control, "SelectedRangeChanged, Value: {0}", control.SelectedRange);
		}
	}
}

