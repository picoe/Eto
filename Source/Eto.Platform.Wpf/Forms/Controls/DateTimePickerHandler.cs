using System;
using Eto.Forms;
using mwc = Xceed.Wpf.Toolkit;
using sw = System.Windows;
using System.Globalization;
using Eto.Drawing;

namespace Eto.Platform.Wpf.Forms.Controls
{
	public class DateTimePickerHandler : WpfControl<mwc.DateTimePicker, DateTimePicker>, IDateTimePicker
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
				ShowButtonSpinner = false
			};
			Mode = DateTimePicker.DefaultMode;
			MinDate = DateTime.MinValue;
			MaxDate = DateTime.MaxValue;
		}

		public override bool UseMousePreview { get { return true; } }

		public override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			DateTime? last = Value;
			Control.ValueChanged += delegate
			{
				var val = Value;
				if (val != null)
				{
					if (val.Value < MinDate)
						val = Value = MinDate;
					else if (val.Value > MaxDate)
						val = Value = MaxDate;
				}

				if (last != val && (last == null || val == null || Math.Abs((last.Value - val.Value).TotalSeconds) >= 1))
				{
					Widget.OnValueChanged(EventArgs.Empty);
					last = val;
				}
			};
		}

		public DateTime? Value
		{
			get { return Control.Value; }
			set { Control.Value = value; }
		}

		public DateTime MinDate { get; set; }

		public DateTime MaxDate { get; set; }

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
