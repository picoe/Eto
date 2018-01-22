using Eto.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eto.Test.UnitTests.Handlers.Controls
{
	public class TestCalendarHandler : TestControlHandler, Calendar.IHandler
	{
		new Calendar.ICallback Callback { get { return base.Callback as Calendar.ICallback; } }
		new Calendar Widget { get { return base.Widget as Calendar; } }

		DateTime minDate = DateTime.MinValue;
		public DateTime MinDate
		{
			get { return minDate; }
			set
			{
				if (minDate != value)
				{
					minDate = value;
					// ensure we're in range
					var start = range.Start;
					var end = range.End;

					if (start < minDate)
						start = minDate;

					if (end < minDate)
						end = minDate;

					SelectedRange = new Range<DateTime>(start, end);
				}
			}
		}

		DateTime maxDate = DateTime.MaxValue;
		public DateTime MaxDate
		{
			get { return maxDate; }
			set
			{
				if (maxDate != value)
				{
					maxDate = value;
					// ensure we're in range
					var start = range.Start;
					var end = range.End;

					if (start > maxDate)
						start = maxDate;

					if (end > maxDate)
						end = maxDate;

					SelectedRange = new Range<DateTime>(start, end);
				}
			}
		}

		public Range<DateTime> SelectedRange
		{
			get { return mode == CalendarMode.Single ? new Range<DateTime>(range.Start) : range; }
			set
			{
				var old = range;
				if (old != value)
				{
					range = value;

					if (old.Start != value.Start)
						Callback.OnSelectedDateChanged(Widget, EventArgs.Empty);

					Callback.OnSelectedRangeChanged(Widget, EventArgs.Empty);
				}
			}
		}

		Range<DateTime> range = new Range<DateTime>(DateTime.Today);
		public DateTime SelectedDate
		{
			get { return range.Start; }
			set
			{
				if (value != SelectedDate)
				{
					range = new Range<DateTime>(value);

					Callback.OnSelectedDateChanged(Widget, EventArgs.Empty);
					Callback.OnSelectedRangeChanged(Widget, EventArgs.Empty);
				}
			}
		}

		CalendarMode mode;
		public CalendarMode Mode
		{
			get { return mode; }
			set
			{
				if (mode != value)
				{
					var old = SelectedRange;
					mode = value;
					if (old != SelectedRange)
						Callback.OnSelectedRangeChanged(Widget, EventArgs.Empty);
				}
			}
		}
	}
}
