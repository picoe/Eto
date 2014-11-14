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
			TableLayout toValueSection;

			var layout = new TableLayout(
				new TableRow(new Label { Text = "Min Value" }, min),
				new TableRow(new Label { Text = "Max Value" }, max),
				new TableRow(new Label { Text = "Mode" }, modeSelect), 
				new TableRow(new Label { Text = "Set to value" },
					new TableLayout(new TableRow(setValue,
						toValueSection = new TableLayout(new TableRow(new Label { Text = " to " }, toValue)) { Visible = false, Padding = Padding.Empty },
						setButton
					)) { Padding = Padding.Empty }
				),
				new TableRow(new Label { Text = "Value" }, TableLayout.AutoSized(current), null),
				null
			);

			modeSelect.SelectedValueBinding.Bind(() => current.Mode, v => {
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
			
		void LogEvents (Calendar control)
		{
			control.SelectedDateChanged += (sender, e) => Log.Write(control, "SelectedDateChanged, Value: {0}", control.SelectedDate);
			control.SelectedRangeChanged += (sender, e) => Log.Write(control, "SelectedRangeChanged, Value: {0}", control.SelectedRange);
		}
	}
}

