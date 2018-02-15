using System;
using System.Reflection;
using SD = System.Drawing;
using Eto.Forms;
using UIKit;
using Eto.Drawing;
using Foundation;
using System.Collections.Generic;
using System.Linq;

namespace Eto.iOS.Forms.Controls
{

	public class DateTimePickerHandler : BasePickerHandler<DateTimePicker, DateTimePicker.ICallback, UIDatePicker>, DateTimePicker.IHandler
	{
		public DateTimePickerHandler()
		{
			MinDate = DateTime.MinValue;
			MaxDate = DateTime.MaxValue;
			Mode = DateTimePickerMode.Date;
		}

		public override UIDatePicker CreatePicker()
		{
			return new UIDatePicker();
		}

		protected override void UpdateValue(UIDatePicker picker)
		{
			Value = ((DateTime)picker.Date).ToLocalTime();
		}

		UIDatePickerMode GetMode()
		{
			switch (Mode)
			{
				case DateTimePickerMode.Date:
					return UIDatePickerMode.Date;
				case DateTimePickerMode.Time:
					return UIDatePickerMode.Time;
				case DateTimePickerMode.DateTime:
					return UIDatePickerMode.DateAndTime;
				default:
					throw new NotSupportedException();
			}
		}

		string GetFormat()
		{
			switch (Mode)
			{
				case DateTimePickerMode.Date:
					return "d";
				case DateTimePickerMode.Time:
					return "t";
				case DateTimePickerMode.DateTime:
					return "g";
				default:
					throw new NotSupportedException();
			}
		}

		protected override void UpdatePicker(UIDatePicker picker)
		{
			picker.Mode = GetMode();
			picker.MinimumDate = MinDate == DateTime.MinValue ? (NSDate)null : (NSDate)MinDate.ToUniversalTime();
			picker.MaximumDate = MaxDate == DateTime.MaxValue ? (NSDate)null : (NSDate)MaxDate.ToUniversalTime();
			picker.Date = (NSDate)(Value ?? DateTime.Now).ToUniversalTime();
		}

		protected override string GetTextValue()
		{
			return Value != null ? Value.Value.ToString(GetFormat()) : null;
		}

		protected override IEnumerable<UIBarButtonItem> CreateCustomButtons()
		{
			yield return new UIBarButtonItem("Clear", UIBarButtonItemStyle.Plain, (s, ee) =>
			{
				Value = null;
				Control.ResignFirstResponder();
			});
		}

		DateTime? currentValue;

		public DateTime? Value
		{
			get { return currentValue; }
			set
			{
				if (value != currentValue)
				{
					currentValue = value;
					if (currentValue < MinDate)
						currentValue = MinDate;
					if (currentValue > MaxDate)
						currentValue = MaxDate;
					UpdateText();
					Callback.OnValueChanged(Widget, EventArgs.Empty);
				}
			}
		}

		public DateTime MinDate { get; set; }

		public DateTime MaxDate { get; set; }

		public DateTimePickerMode Mode { get; set; }

		public Color TextColor
		{
			get { return Control.TextColor.ToEto(); }
			set { Control.TextColor = value.ToNSUI(); }
		}
	}
}
