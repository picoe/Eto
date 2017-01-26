using System;
using Eto.Forms;
using w = WinRTDatePicker;
using sw = Windows.UI.Xaml;
using wf = Windows.Foundation;
using System.Globalization;
using Eto.Drawing;

namespace Eto.WinRT.Forms.Controls
{
	/// <summary>
	/// Date-time picker handler.
	/// </summary>
	/// <copyright>(c) 2014 by Vivek Jhaveri</copyright>
	/// <copyright>(c) 2012-2014 by Curtis Wensley</copyright>	
	/// <license type="BSD-3">See LICENSE for full terms</license>
	public class DateTimePickerHandler : WpfControl<w.DatePicker, DateTimePicker, DateTimePicker.ICallback>, DateTimePicker.IHandler
	{
		DateTimePickerMode mode;

		protected override wf.Size DefaultSize => new wf.Size(mode == DateTimePickerMode.DateTime ? 180 : 120, double.NaN);

		public override wf.Size GetPreferredSize(wf.Size constraint)
		{
			return base.GetPreferredSize(Conversions.ZeroSize);
		}

		public DateTimePickerHandler()
		{
			Control = new w.DatePicker
			{
#if TODO_XAML
				ShowButtonSpinner = false
#endif
			};
			Mode = DateTimePickerMode.Date;
			MinDate = DateTime.MinValue;
			MaxDate = DateTime.MaxValue;
		}

		public override bool UseMousePreview { get { return true; } }

		public override bool UseKeyPreview { get { return true; } }

		protected override void Initialize()
		{
			base.Initialize();
			DateTime? last = Value;
			Control.SelectedDateChanged += delegate
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
					Callback.OnValueChanged(Widget, EventArgs.Empty);
					last = val;
				}
			};
		}
		
		public DateTime? Value
		{
			get { return Control.SelectedDate; }
			set
			{
				if (value != null)
					Control.SelectedDate = value.Value;
			}
		}

		public DateTime MinDate { get; set; }

		public DateTime MaxDate { get; set; }

		public DateTimePickerMode Mode
		{
			get { return mode; }
			set
			{
				mode = value;
#if TODO_XAML
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
#endif
			}
		}

		public Color TextColor
		{
			get { return Control.Foreground.ToEtoColor(); }
			set { Control.Foreground = value.ToWpfBrush(Control.Foreground); }
		}
	}
}