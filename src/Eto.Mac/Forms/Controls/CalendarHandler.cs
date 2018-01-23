using System;
using Eto.Forms;
#if XAMMAC2
using AppKit;
using Foundation;
using CoreGraphics;
using ObjCRuntime;
using CoreAnimation;
#else
using MonoMac.AppKit;
using MonoMac.Foundation;
using MonoMac.CoreGraphics;
using MonoMac.ObjCRuntime;
using MonoMac.CoreAnimation;
#endif

namespace Eto.Mac.Forms.Controls
{
	public class CalendarHandler : MacControl<NSDatePicker, Calendar, Calendar.ICallback>, Calendar.IHandler
	{
		public class EtoDatePicker : NSDatePicker, IMacControl
		{
			public WeakReference WeakHandler { get; set; }

			public CalendarHandler Handler
			{ 
				get { return (CalendarHandler)WeakHandler.Target; }
				set { WeakHandler = new WeakReference(value); } 
			}

			public EtoDatePicker()
			{
				TimeZone = NSTimeZone.LocalTimeZone;
				Calendar = NSCalendar.CurrentCalendar;
				Bezeled = false;
				DateValue = (NSDate)DateTime.Today;
				DatePickerStyle = NSDatePickerStyle.ClockAndCalendar;
				DatePickerElements = NSDatePickerElementFlags.YearMonthDateDay;
			}
		}

		protected override NSDatePicker CreateControl()
		{
			return new EtoDatePicker();
		}

		protected override void Initialize()
		{
			Control.Activated += HandleActivated;
			base.Initialize();
		}

		public override void AttachEvent(string id)
		{
			switch (id)
			{
				case Calendar.SelectedDateChangedEvent:
				case Calendar.SelectedRangeChangedEvent:
					// handled by delegate
					break;
				default:
					base.AttachEvent(id);
					break;
			}
		}

		Range<DateTime> lastRange;

		void HandleActivated(object sender, EventArgs e)
		{
			var range = SelectedRange;
			if (range != lastRange)
			{
				var lastDate = lastRange.Start;
				lastRange = range;
				if (SelectedDate != lastDate)
					Callback.OnSelectedDateChanged(Widget, EventArgs.Empty);

				Callback.OnSelectedRangeChanged(Widget, EventArgs.Empty);
			}
		}

		public DateTime MinDate
		{
			get { return Control.MinDate.ToEto() ?? DateTime.MinValue; }
			set
			{ 
				var range = SelectedRange;
				var date = SelectedDate;
				Control.MinDate = value.ToNS();
				if (date != SelectedDate)
					Callback.OnSelectedDateChanged(Widget, EventArgs.Empty);
				if (range != SelectedRange)
					Callback.OnSelectedRangeChanged(Widget, EventArgs.Empty);
			}
		}

		public DateTime MaxDate
		{
			get { return Control.MaxDate.ToEto() ?? DateTime.MaxValue; }
			set
			{ 
				var range = SelectedRange;
				var date = SelectedDate;
				Control.MaxDate = value.ToNS();
				if (date != SelectedDate)
					Callback.OnSelectedDateChanged(Widget, EventArgs.Empty);
				if (range != SelectedRange)
					Callback.OnSelectedRangeChanged(Widget, EventArgs.Empty);
			}
		}

		public DateTime SelectedDate
		{
			get
			{ 
				return Control.DateValue.ToEto() ?? DateTime.Today;
			}
			set
			{
				if (value != SelectedDate)
				{
					Control.DateValue = value.ToNS();
					Callback.OnSelectedDateChanged(Widget, EventArgs.Empty);
					Callback.OnSelectedRangeChanged(Widget, EventArgs.Empty);
				}
			}
		}

		public Range<DateTime> SelectedRange
		{
			get
			{ 
				var val = Control.DateValue.ToEto();
				if (val != null)
				{
					var date = val.Value;
					if (Mode == CalendarMode.Single)
						return new Range<DateTime>(date);
					return new Range<DateTime>(date, date + TimeSpan.FromSeconds(Control.TimeInterval));
				}
				return new Range<DateTime>(DateTime.Today);
			}
			set
			{
				var old = SelectedRange;
				if (value != old)
				{
					// don't validate otherwise the new value gets overridden when null
					Control.DateValue = value.Start.ToNS();
					Control.TimeInterval = value.Interval().TotalSeconds;
					if (old.Start != value.Start)
						Callback.OnSelectedDateChanged(Widget, EventArgs.Empty);
					Callback.OnSelectedRangeChanged(Widget, EventArgs.Empty);
				}
			}
		}

		public CalendarMode Mode
		{
			get { return Control.DatePickerMode.ToEto(); }
			set
			{
				var triggerChange = Control.TimeInterval > 0;
				Control.DatePickerMode = value.ToNS();
				if (triggerChange || Control.TimeInterval > 0)
					Callback.OnSelectedRangeChanged(Widget, EventArgs.Empty);
			}
		}
	}
}