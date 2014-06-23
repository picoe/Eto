using Eto.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using swc = System.Windows.Controls;

namespace Eto.Wpf.Forms.Controls
{
	public class CalendarHandler : WpfControl<swc.Calendar, Calendar, Calendar.ICallback>, Calendar.IHandler
	{
		int suppressChanged;
		public CalendarHandler()
		{
			Control = new swc.Calendar
			{
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

		DateRange oldRange;
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
					if (value == CalendarMode.Range)
					{
						if (range != null && oldRange != null && range.Start <= oldRange.End)
							SelectedRange = new DateRange(range.Start, oldRange.End);
						else
							SelectedRange = range;
					}
					else if (range != null)
						SelectedRange = new DateRange(range.Start, range.Start);
					if (value == CalendarMode.Single)
						oldRange = range;
					suppressChanged--;
					if (range != SelectedRange)
						Callback.OnSelectedRangeChanged(Widget, EventArgs.Empty);
				}
			}
		}

		public DateRange SelectedRange
		{
			get
			{
				return Control.SelectedDates.Count == 0
					? null
					: new DateRange(Control.SelectedDates.Min(), Control.SelectedDates.Max());
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
						if (value != null)
						{
							Control.SelectedDates.AddRange(value.Start, value.End);
						}
					}
					else
						Control.SelectedDate = value != null ? (DateTime?)value.Start : null;
					suppressChanged--;
					if (suppressChanged == 0)
					{
						if (old == null || value == null || old.Start != value.Start)
							Callback.OnSelectedDateChanged(Widget, EventArgs.Empty);
						Callback.OnSelectedRangeChanged(Widget, EventArgs.Empty);
					}
				}
			}
		}
	}
}
