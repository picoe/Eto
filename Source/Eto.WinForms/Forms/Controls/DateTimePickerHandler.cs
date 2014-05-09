using System;
using Eto.Forms;
using System.Globalization;

namespace Eto.WinForms.Forms.Controls
{
	public class DateTimePickerHandler : WindowsControl<System.Windows.Forms.DateTimePicker, DateTimePicker, DateTimePicker.ICallback>, DateTimePicker.IHandler
	{
		public DateTimePickerHandler()
		{
			Control = new System.Windows.Forms.DateTimePicker();
			Control.ShowCheckBox = true;
			Mode = DateTimePicker.DefaultMode;
			Value = null;
			Control.ValueChanged += delegate
			{
				Callback.OnValueChanged(Widget, EventArgs.Empty);
			};
		}

		public DateTimePickerMode Mode
		{
			get
			{
				switch (Control.Format)
				{
					case System.Windows.Forms.DateTimePickerFormat.Long:
						return DateTimePickerMode.DateTime;
					case System.Windows.Forms.DateTimePickerFormat.Short:
						return DateTimePickerMode.Date;
					case System.Windows.Forms.DateTimePickerFormat.Time:
						return DateTimePickerMode.Time;
					default:
						throw new NotImplementedException();
				}
			}
			set
			{
				switch (value)
				{
					case DateTimePickerMode.DateTime:
						Control.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
						var format = CultureInfo.CurrentUICulture.DateTimeFormat;
						Control.CustomFormat = format.ShortDatePattern + " " + format.LongTimePattern;
						break;
					case DateTimePickerMode.Date:
						Control.Format = System.Windows.Forms.DateTimePickerFormat.Short;
						break;
					case DateTimePickerMode.Time:
						Control.Format = System.Windows.Forms.DateTimePickerFormat.Time;
						break;
					default:
						throw new NotImplementedException();
				}
			}
		}

		public DateTime MinDate
		{
			get
			{
				return Control.MinDate;
			}
			set
			{
				Control.MinDate = value;
			}
		}

		public DateTime MaxDate
		{
			get
			{
				return Control.MaxDate;
			}
			set
			{
				Control.MaxDate = value;
			}
		}

		public DateTime? Value
		{
			get
			{
				return !Control.Checked ? null : (DateTime?)Control.Value;
			}
			set
			{
				if (value != null)
				{
					var date = value.Value;
					if (date < MinDate) date = MinDate;
					if (date > MaxDate) date = MaxDate;
					Control.Value = date;
					Control.Checked = true;
				}
				else
					Control.Checked = false;
			}
		}
	}
}

