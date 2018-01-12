using Eto.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using swc = System.Windows.Controls;
using System.Windows;

namespace Eto.Wpf.Forms.Controls
{
	public class EtoCalendar : swc.Calendar, IEtoWpfControl
	{
		public IWpfFrameworkElement Handler { get; set; }

		protected override Size MeasureOverride(Size constraint)
		{
			return Handler?.MeasureOverride(constraint, base.MeasureOverride) ?? base.MeasureOverride(constraint);
		}
	}

	public class CalendarHandler : WpfControl<swc.Calendar, Calendar, Calendar.ICallback>, Calendar.IHandler
	{
		int suppressChanged;
		public CalendarHandler()
		{
			Control = new EtoCalendar
			{
				Handler = this,
				SelectedDate = DateTime.Today
			};
		}

		public override void AttachEvent(string id)
		{
			switch (id)
			{
				case Calendar.SelectedDateChangedEvent:
					{
						var last = SelectedDate;
						Control.SelectedDatesChanged += (sender, e) =>
						{
							var date = SelectedDate;
							if (suppressChanged == 0 && date != last)
							{
								last = date;
								oldRange = null;
								Callback.OnSelectedDateChanged(Widget, EventArgs.Empty);
							}
						};
					}
					break;
				case Calendar.SelectedRangeChangedEvent:
					{
						var last = SelectedRange;
						Control.SelectedDatesChanged += (sender, e) =>
						{
							var range = SelectedRange;
							if (suppressChanged == 0 && last != range)
							{
								last = range;
								oldRange = null;
								Callback.OnSelectedRangeChanged(Widget, EventArgs.Empty);
							}
						};
					}
					break;
				default:
					base.AttachEvent(id);
					break;
			}
		}

		public DateTime MinDate
		{
			get { return Control.DisplayDateStart ?? DateTime.MinValue; }
			set
			{
				var currentMonth = Control.DisplayDate;
				Control.DisplayDateStart = value == DateTime.MinValue ? (DateTime?)null : value;
				Control.DisplayDate = currentMonth;
				if (SelectedDate != null && SelectedDate < value)
					SelectedDate = value;
			}
		}

		public DateTime MaxDate
		{
			get { return Control.DisplayDateEnd ?? DateTime.MaxValue; }
			set
			{
				Control.DisplayDateEnd = value == DateTime.MaxValue ? (DateTime?)null : value;
				if (SelectedDate != null && SelectedDate > value)
					SelectedDate = value;
			}
		}

		public DateTime SelectedDate
		{
			get { return Control.SelectedDate ?? DateTime.Today; }
			set
			{
				var val = value;
				if (val > MaxDate) val = MaxDate;
				if (val < MinDate) val = MinDate;
				if (val != SelectedDate)
				{
					Control.SelectedDate = val;
				}
				oldRange = null;
			}
		}

		Range<DateTime>? oldRange;
		public CalendarMode Mode
		{
			get { return Control.SelectionMode.ToEto(); }
			set
			{
				if (Mode != value)
				{
					suppressChanged++;
					var range = SelectedRange;
					Control.SelectionMode = value.ToWpf();
					Control.SelectedDates.Clear();
					if (value == CalendarMode.Range)
					{
						if (oldRange != null)
							Control.SelectedDates.AddRange(oldRange.Value.Start, oldRange.Value.End);
						else
							Control.SelectedDates.AddRange(range.Start, range.End);
					}
					else
					{
						oldRange = range;
						Control.SelectedDates.Add(range.Start);
					}
					suppressChanged--;
					if (range != SelectedRange)
						Callback.OnSelectedRangeChanged(Widget, EventArgs.Empty);
				}
			}
		}

		public Range<DateTime> SelectedRange
		{
			get
			{
				return Control.SelectedDates.Count == 0
					? new Range<DateTime>(DateTime.Today)
					: new Range<DateTime>(Control.SelectedDates.Min(), Control.SelectedDates.Max());
			}
			set
			{
				var old = SelectedRange;
				if (value != old)
				{
					oldRange = null;
					suppressChanged++;
					if (Mode == CalendarMode.Range)
					{
						Control.SelectedDates.Clear();
						Control.SelectedDates.AddRange(value.Start, value.End);
					}
					else
						Control.SelectedDate = value.Start;
					suppressChanged--;
					if (suppressChanged == 0)
					{
						if (old.Start != value.Start)
							Callback.OnSelectedDateChanged(Widget, EventArgs.Empty);
						Callback.OnSelectedRangeChanged(Widget, EventArgs.Empty);
					}
				}
			}
		}
	}
}
