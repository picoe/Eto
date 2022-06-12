using System;
using Eto.Forms;

namespace Eto.GtkSharp.Forms.Controls
{
	public class CalendarHandler : GtkControl<Gtk.Calendar, Calendar, Calendar.ICallback>, Calendar.IHandler
	{
		Gtk.EventBox align;
		CalendarMode mode;
		Gtk.Calendar endCalendar;
		Gtk.Box box;
		DateTime minDate = DateTime.MinValue;
		DateTime maxDate = DateTime.MaxValue;
		int suppressRangeChanged;

		public override Gtk.Widget ContainerControl
		{
			get { return align; }
		}

		public CalendarHandler()
		{
			Control = new Gtk.Calendar { Date = DateTime.Today };

			align = new Gtk.EventBox();
			align.Child = Control;
		}

		protected override void Initialize()
		{
			base.Initialize();
			Control.DaySelected += Connector.HandleDaySelected;
		}

		void SetContent()
		{
			if (mode == CalendarMode.Single)
			{
				if (box != null)
					box.Remove(Control);
				align.Remove(box);
				align.Child = Control;
			}
			else
			{
				align.Remove(Control);
				suppressRangeChanged++;
				if (endCalendar == null)
				{
					endCalendar = new Gtk.Calendar();
					endCalendar.Show();
					if (Control.Day == 0)
						endCalendar.Day = 0;
					else
						endCalendar.Date = Control.Date;
					endCalendar.DaySelected += Connector.HandleEndDaySelected;
				}
				if (Control.Day == 0)
					endCalendar.Day = 0;
				suppressRangeChanged--;

				if (box == null)
					box = new Gtk.HBox();
				else
					box.Remove(endCalendar);
				box.PackStart(Control, true, true, 0);
				box.PackStart(endCalendar, true, true, 0);
				box.Show();
				align.Child = box;
			}
		}

		void HandleEndDaySelected(object sender, EventArgs e)
		{
			var date = endCalendar.Date;
			if (endCalendar.Day != 0)
			{
				if (date < MinDate)
				{
					endCalendar.Date = MinDate;
					return;
				}
				if (date > MaxDate)
				{
					endCalendar.Date = MaxDate;
					return;
				}
			}
			if (date < Control.Date || (Control.Day == 0 && endCalendar.Day != 0))
			{
				Control.Date = date;
				return;
			}

			if (suppressRangeChanged == 0)
				Callback.OnSelectedRangeChanged(Widget, EventArgs.Empty);
		}

		void HandleDaySelected(object sender, EventArgs e)
		{
			var date = Control.Date;
			if (Control.Day != 0)
			{
				if (date < MinDate)
				{
					Control.Date = MinDate;
					return;
				}
				if (date > MaxDate)
				{
					Control.Date = MaxDate;
					return;
				}
			}

			Callback.OnSelectedDateChanged(Widget, EventArgs.Empty);
			if (endCalendar != null && (date > endCalendar.Date || endCalendar.Day == 0))
			{
				if (Control.Day == 0)
					endCalendar.Day = 0;
				else
					endCalendar.Date = date;
				return;
			}
			if (suppressRangeChanged == 0)
				Callback.OnSelectedRangeChanged(Widget, EventArgs.Empty);
		}

		public override void AttachEvent(string id)
		{
			switch (id)
			{
				case Calendar.SelectedDateChangedEvent:
					break;
				case Calendar.SelectedRangeChangedEvent:
					break;
				default:
					base.AttachEvent(id);
					break;
			}
		}

		public DateTime MinDate
		{
			get { return minDate; }
			set
			{
				if (minDate != value)
				{
					minDate = value;
					var range = SelectedRange;
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

		public DateTime MaxDate
		{
			get { return maxDate; }
			set
			{
				if (maxDate != value)
				{
					maxDate = value;
					var range = SelectedRange;
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
			get
			{
				if (mode == CalendarMode.Range && endCalendar != null)
				{
					return new Range<DateTime>(Control.Date, endCalendar.Date);
				}
				return new Range<DateTime>(Control.Date, Control.Date);
			}
			set
			{
				if (value != SelectedRange)
				{
					if (mode == CalendarMode.Range)
					{
						suppressRangeChanged++;
						if (value.Start != Control.Date)
							Control.Date = value.Start;
						endCalendar.Date = value.End;
						suppressRangeChanged--;
						Callback.OnSelectedRangeChanged(Widget, EventArgs.Empty);
					}
					else
					{
						Control.Date = value.Start;
					}
				}
			}
		}

		public DateTime SelectedDate
		{
			get { return Control.Date; }
			set
			{
				if (value != SelectedDate)
				{
					Control.Date = value;
					if (endCalendar != null)
						endCalendar.Date = value;
				}
			}
		}

		public CalendarMode Mode
		{
			get { return mode; }
			set
			{
				if (mode != value)
				{
					var range = SelectedRange;
					mode = value;
					SetContent();
					if (range != SelectedRange)
						Callback.OnSelectedRangeChanged(Widget, EventArgs.Empty);
				}
			}
		}

		protected new CalendarConnector Connector => (CalendarConnector)base.Connector;

		protected override WeakConnector CreateConnector() => new CalendarConnector();

		/// <summary>
		/// Connector for events to keep a weak reference to allow gtk controls to be garbage collected when no longer referenced
		/// </summary>
		protected class CalendarConnector : GtkControlConnector
		{
			new CalendarHandler Handler => (CalendarHandler)base.Handler;

			public virtual void HandleDaySelected(object sender, EventArgs e) => Handler?.HandleDaySelected(sender, e);

			public virtual void HandleEndDaySelected(object sender, EventArgs e) => Handler?.HandleEndDaySelected(sender, e);
		}
	}
}