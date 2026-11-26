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

		public override Size? GetDefaultSize(Size availableSize) => new Size(Math.Max(200, Control.PreferredWidth), 23);

		public bool ShowBorder
		{
			get { return Control.ShowBorder; }
			set { Control.ShowBorder = value; }
		}

		DateTimePickerMode mode;
		CultureInfo culture;

		public DateTimePickerMode Mode
		{
			get => mode;
			set
			{
				mode = value;
				SetFormat();
			}
		}

		public CultureInfo Culture
		{
			get => culture ?? CultureInfo.CurrentCulture;
			set
			{
				culture = value;
				SetFormat();
			}
		}

		void SetFormat()
		{
			// the control draws its own text with the culture; only its drop-down calendar follows the system
			Control.Culture = culture;
			switch (mode)
			{
				case DateTimePickerMode.DateTime:
					Control.Format = swf.DateTimePickerFormat.Custom;
					var format = Culture.DateTimeFormat;
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
			// the text can be a different width (the constructor sets the mode before there's a widget)
			if (Widget != null)
			{
				ClearCachedDefaultSize();
				SetMinimumSize(true);
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

