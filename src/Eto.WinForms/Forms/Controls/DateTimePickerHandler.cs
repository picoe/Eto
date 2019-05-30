using System;
using System.Linq;
using Eto.Forms;
using System.Globalization;
using swf = System.Windows.Forms;
using Eto.WinForms.CustomControls;

namespace Eto.WinForms.Forms.Controls
{
	public class DateTimePickerHandler : WindowsControl<ExtendedDateTimePicker, DateTimePicker, DateTimePicker.ICallback>, DateTimePicker.IHandler
	{
		public DateTimePickerHandler()
		{
			Control = new ExtendedDateTimePicker { ExtendedMode = true };
			Control.ShowCheckBox = true;
			Mode = DateTimePickerMode.Date;
			Value = null;
			Control.ValueChanged += delegate
			{
				Callback.OnValueChanged(Widget, EventArgs.Empty);
			};
		}

		public bool ShowBorder
		{
			get { return Control.ShowBorder; }
			set { Control.ShowBorder = value; }
		}

		public DateTimePickerMode Mode
		{
			get
			{
				switch (Control.Format)
				{
					case swf.DateTimePickerFormat.Long:
						return DateTimePickerMode.DateTime;
					case swf.DateTimePickerFormat.Short:
						return DateTimePickerMode.Date;
					case swf.DateTimePickerFormat.Time:
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
						Control.Format = swf.DateTimePickerFormat.Custom;
						var format = CultureInfo.CurrentUICulture.DateTimeFormat;
						Control.CustomFormat = format.ShortDatePattern + " " + format.LongTimePattern;
						break;
					case DateTimePickerMode.Date:
						Control.Format = swf.DateTimePickerFormat.Short;
						break;
					case DateTimePickerMode.Time:
						Control.Format = swf.DateTimePickerFormat.Time;
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
				return Control.MinDate == swf.DateTimePicker.MinimumDateTime ? DateTime.MinValue : Control.MinDate;
			}
			set
			{
				Control.MinDate = value == DateTime.MinValue ? swf.DateTimePicker.MinimumDateTime : value;
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
				Control.MaxDate = value == DateTime.MaxValue ? swf.DateTimePicker.MaximumDateTime : value;
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

		static readonly Win32.WM[] intrinsicEvents = { Win32.WM.LBUTTONDOWN, Win32.WM.LBUTTONUP, Win32.WM.LBUTTONDBLCLK };
		public override bool ShouldBubbleEvent(swf.Message msg)
		{
			return !intrinsicEvents.Contains((Win32.WM)msg.Msg) && base.ShouldBubbleEvent(msg);
		}
	}
}

