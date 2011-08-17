using System;
using MonoMac.AppKit;
using Eto.Forms;
using MonoMac.Foundation;

namespace Eto.Platform.Mac.Forms.Controls
{
	public class DateTimePickerHandler : MacControl<NSDatePicker, DateTimePicker>, IDateTimePicker
	{
		DateTime? proposedDate;
		public DateTimePickerHandler ()
		{
			Control = new NSDatePicker ();
			/*AddObserver(new NSString("DateValue"), delegate { 
				Widget.OnValueChanged (EventArgs.Empty);
			});*/
			Control.ValidateProposedDateValue += delegate(object sender, NSDatePickerValidatorEventArgs e) {
				proposedDate = e.ProposedDateValue;
				Widget.OnValueChanged (EventArgs.Empty);
				proposedDate = null;
			};
			Mode = DateTimePicker.DefaultMode;
		}
		
		public DateTimePickerMode Mode {
			get {
				var flags = Control.DatePickerElements;
				if ((flags & NSDatePickerElementFlags.YearMonthDate) != 0)
				{
					if ((flags & NSDatePickerElementFlags.HourMinute) != 0) 
						return DateTimePickerMode.DateTime;
					else
						return DateTimePickerMode.Date;
				}
				else if ((flags & NSDatePickerElementFlags.HourMinute) != 0)
				{
					return DateTimePickerMode.Time;
				}
				else throw new NotImplementedException();
			}
			set {
				switch (value)
				{
				case DateTimePickerMode.Date:
					Control.DatePickerElements = NSDatePickerElementFlags.YearMonthDateDay;
					break;
				case DateTimePickerMode.Time:
					Control.DatePickerElements = NSDatePickerElementFlags.HourMinuteSecond;
					break;
				case DateTimePickerMode.DateTime:
					Control.DatePickerElements = NSDatePickerElementFlags.YearMonthDateDay | NSDatePickerElementFlags.HourMinuteSecond;
					break;
				default:
					throw new NotImplementedException();
				}
			}
		}
		
		public DateTime MinDate {
			get {
				return Control.MinDate;
			}
			set {
				Control.MinDate = value;
			}
		}
		
		public DateTime MaxDate {
			get {
				return Control.MaxDate;
			}
			set {
				Control.MaxDate = value;
			}
		}

		public DateTime? Value {
			get {
				return proposedDate ?? Control.DateValue;
			}
			set {
				if (value != null) {
					Control.DateValue = value.Value;
				}
			}
		}

	}
}

