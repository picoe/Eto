using Eto.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using swf = System.Windows.Forms;

namespace Eto.WinForms.Forms.Controls
{
	public class CalendarHandler : WindowsControl<swf.MonthCalendar, Calendar, Calendar.ICallback>, Calendar.IHandler
	{
		public CalendarHandler()
		{
			Control = new swf.MonthCalendar { MaxSelectionCount = 1, SelectionStart = DateTime.Today };
		}

		public override void AttachEvent(string id)
		{
			switch (id)
			{
				case Calendar.SelectedDateChangedEvent:
					DateTime? lastDate = null;
					Control.DateChanged += (sender, e) =>
					{
						var date = SelectedDate;
						if (lastDate != date)
						{
							lastDate = date;
							Callback.OnSelectedDateChanged(Widget, EventArgs.Empty);
						}
					};
					break;
				case Calendar.SelectedRangeChangedEvent:
					Range<DateTime>? lastRange = null;
					Control.DateChanged += (sender, e) =>
					{
						var date = SelectedRange;
						if (lastRange != date)
						{
							lastRange = date;
							Callback.OnSelectedRangeChanged(Widget, EventArgs.Empty);
						}
					};
					break;
				default:
					base.AttachEvent(id);
					break;
			}
		}

		public DateTime MinDate
		{
			get
			{
				return Control.MinDate == swf.DateTimePicker.MinimumDateTime ? DateTime.MinValue : Control.MinDate;
			}
			set
			{
				if (value != MinDate)
				{
					var oldRange = SelectedRange;
					Control.MinDate = value == DateTime.MinValue ? swf.DateTimePicker.MinimumDateTime : value;
					if (oldRange.Start != SelectedRange.Start)
						Callback.OnSelectedDateChanged(Widget, EventArgs.Empty);
					if (oldRange != SelectedRange)
						Callback.OnSelectedRangeChanged(Widget, EventArgs.Empty);
				}
			}
		}

		public DateTime MaxDate
		{
			get
			{
				return Control.MaxDate == swf.DateTimePicker.MaximumDateTime ? DateTime.MaxValue : Control.MaxDate;
			}
			set
			{
				if (value != MaxDate)
				{
					var oldRange = SelectedRange;
					Control.MaxDate = value == DateTime.MaxValue ? swf.DateTimePicker.MaximumDateTime : value;
					if (oldRange.Start != SelectedRange.Start)
						Callback.OnSelectedDateChanged(Widget, EventArgs.Empty);
					if (oldRange != SelectedRange)
						Callback.OnSelectedRangeChanged(Widget, EventArgs.Empty);
				}
			}
		}

		public DateTime SelectedDate
		{
			get { return Control.SelectionStart; }
			set
			{
				if (value != SelectedDate)
				{
					Control.SetDate(value);
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
					var range = SelectedRange;
					mode = value;
					Control.MaxSelectionCount = mode == CalendarMode.Range ? int.MaxValue : 1;
					if (range != SelectedRange)
						Callback.OnSelectedRangeChanged(Widget, EventArgs.Empty);
				}
			}
		}

		public Range<DateTime> SelectedRange
		{
			get { return mode == CalendarMode.Range ? Control.SelectionRange.ToEto() : new Range<DateTime>(Control.SelectionStart); }
			set
			{
				if (value != SelectedRange)
				{
					Control.SelectionRange = value.ToSWF();
				}
			}
		}

		static readonly Win32.WM[] intrinsicEvents = {
														 Win32.WM.LBUTTONDOWN, Win32.WM.LBUTTONUP, Win32.WM.LBUTTONDBLCLK,
														 Win32.WM.RBUTTONDOWN, Win32.WM.RBUTTONUP, Win32.WM.RBUTTONDBLCLK
													 };
		public override bool ShouldBubbleEvent(swf.Message msg)
		{
			return !intrinsicEvents.Contains((Win32.WM)msg.Msg) && base.ShouldBubbleEvent(msg);
		}
	}
}
