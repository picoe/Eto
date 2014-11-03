using System;
using Eto.Forms;
using mwc = Xceed.Wpf.Toolkit;
using sw = System.Windows;
using System.Globalization;
using Eto.Drawing;

namespace Eto.Wpf.Forms.Controls
{
	public class DateTimePickerHandler : WpfControl<mwc.DateTimePicker, DateTimePicker, DateTimePicker.ICallback>, DateTimePicker.IHandler
	{
		DateTimePickerMode mode;

		protected override Size DefaultSize { get { return new Size(mode == DateTimePickerMode.DateTime ? 180 : 120, -1); } }

		public override sw.Size GetPreferredSize(sw.Size constraint)
		{
			return base.GetPreferredSize(Conversions.ZeroSize);
		}

		public DateTimePickerHandler()
		{
			Control = new mwc.DateTimePicker
			{
				ShowButtonSpinner = false,
				AutoCloseCalendar = true,
				ClipValueToMinMax = true,
				Focusable = true,
				IsTabStop = true
			};
			#pragma warning disable 612,618
			Mode = DateTimePicker.DefaultMode;
			#pragma warning restore 612,618
			DateTime? last = Value;
			Control.ValueChanged += delegate
			{
				var val = Value;
				if (val != null)
				{
					// still need to do this as the popup doesn't limit the value when using the calendar
					if (Control.Minimum != null && val.Value < Control.Minimum)
						val = Value = Control.Minimum.Value;
					else if (Control.Maximum != null && val.Value > Control.Maximum)
						val = Value = Control.Maximum.Value;
				}

				if (last != val && (last == null || val == null || Math.Abs((last.Value - val.Value).TotalSeconds) >= 1))
				{
					Callback.OnValueChanged(Widget, EventArgs.Empty);
					last = val;
				}
			};
		}

		public override bool UseMousePreview { get { return true; } }

		public override bool UseKeyPreview { get { return true; } }

		public DateTime? Value
		{
			get { return Control.Value; }
			set { Control.Value = value; }
		}

		public DateTime MinDate
		{
			get { return Control.Minimum ?? DateTime.MinValue; }
			set { Control.Minimum = value == DateTime.MinValue ? null : (DateTime?)value; }
		}

		public DateTime MaxDate
		{
			get { return Control.Maximum ?? DateTime.MaxValue; }
			set { Control.Maximum = value == DateTime.MaxValue ? null : (DateTime?)value; }
		}

		public DateTimePickerMode Mode
		{
			get { return mode; }
			set
			{
				mode = value;
				switch (mode)
				{
					case DateTimePickerMode.Date:
						Control.Format = mwc.DateTimeFormat.ShortDate;
						break;
					case DateTimePickerMode.DateTime:
						var format = CultureInfo.CurrentUICulture.DateTimeFormat;
						Control.Format = mwc.DateTimeFormat.Custom;
						Control.FormatString = format.ShortDatePattern + " " + format.LongTimePattern;
						break;
					case DateTimePickerMode.Time:
						Control.Format = mwc.DateTimeFormat.LongTime;
						break;
					default:
						throw new NotSupportedException();
				}
				SetSize();
			}
		}
	}
}
