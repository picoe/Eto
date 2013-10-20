using System;
using MonoMac.AppKit;
using Eto.Forms;
using MonoMac.Foundation;
using Eto.Drawing;

namespace Eto.Platform.Mac.Forms.Controls
{
	public class DateTimePickerHandler : MacControl<NSDatePicker, DateTimePicker>, IDateTimePicker
	{
		DateTime? curValue;
		DateTimePickerMode mode;

		public class EtoDatePicker : NSDatePicker, IMacControl
		{
			public override void DrawRect(System.Drawing.RectangleF dirtyRect)
			{
				if (Handler.curValue != null)
					base.DrawRect(dirtyRect);
				else
				{
					// paint with no elements visible
					var old = this.DatePickerElements;
					this.DatePickerElements = 0;
					base.DrawRect(dirtyRect);
					this.DatePickerElements = old;
				}
			}

			public WeakReference WeakHandler { get; set; }

			public DateTimePickerHandler Handler
			{ 
				get { return (DateTimePickerHandler)WeakHandler.Target; }
				set { WeakHandler = new WeakReference(value); } 
			}
		}

		public DateTimePickerHandler()
		{
			Control = new EtoDatePicker
			{
				Handler = this,
				TimeZone = NSTimeZone.LocalTimeZone,
				Calendar = NSCalendar.CurrentCalendar,
				DateValue = DateTime.Now.ToNS()
			};
			this.Mode = DateTimePicker.DefaultMode;
			// apple+backspace clears the value
			Control.ValidateProposedDateValue += HandleValidateProposedDateValue;
		}

		protected override void Initialize()
		{
			base.Initialize();
			//Widget.KeyDown += HandleKeyDown;
			// when clicking, set the value if it is null
			Widget.MouseDown += HandleMouseDown;
		}

		static void HandleKeyDown(object sender, KeyEventArgs e)
		{
			var handler = ((Control)sender).Handler as DateTimePickerHandler;
			if (!e.Handled)
			{
				if (e.KeyData == (Key.Application | Key.Backspace))
				{
					handler.curValue = null;
					handler.Widget.OnValueChanged(EventArgs.Empty);
					handler.Control.NeedsDisplay = true;
				}
			}
		}

		static void HandleMouseDown(object sender, MouseEventArgs e)
		{
			var handler = ((Control)sender).Handler as DateTimePickerHandler;
			if (e.Buttons == MouseButtons.Primary)
			{
				if (handler.curValue == null)
				{
					handler.curValue = handler.Control.DateValue.ToEto();
					handler.Widget.OnValueChanged(EventArgs.Empty);
					handler.Control.NeedsDisplay = true;
				}
			}
		}

		static void HandleValidateProposedDateValue(object sender, NSDatePickerValidatorEventArgs e)
		{
			var datePickerCell = (NSDatePickerCell)sender;
			var handler = GetHandler(datePickerCell.ControlView) as DateTimePickerHandler;
			var date = e.ProposedDateValue.ToEto();
			if (date != handler.Control.DateValue.ToEto())
			{
				handler.curValue = date;
				handler.Widget.OnValueChanged(EventArgs.Empty);
			}
		}

		protected override SizeF GetNaturalSize(SizeF availableSize)
		{
			return SizeF.Max(new Size(mode == DateTimePickerMode.DateTime ? 180 : 120, 10), base.GetNaturalSize(availableSize));
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
						Control.DatePickerElements = NSDatePickerElementFlags.YearMonthDateDay;
						break;
					case DateTimePickerMode.Time:
						Control.DatePickerElements = NSDatePickerElementFlags.HourMinuteSecond;
						break;
					case DateTimePickerMode.DateTime:
						Control.DatePickerElements = NSDatePickerElementFlags.YearMonthDateDay | NSDatePickerElementFlags.HourMinuteSecond;
						break;
					default:
						throw new NotSupportedException();
				}
			}
		}

		public DateTime MinDate
		{
			get { return Control.MinDate.ToEto() ?? DateTime.MinValue; }
			set { Control.MinDate = value.ToNS(); }
		}

		public DateTime MaxDate
		{
			get { return Control.MaxDate.ToEto() ?? DateTime.MaxValue; }
			set { Control.MaxDate = value.ToNS(); }
		}

		public DateTime? Value
		{
			get
			{
				return curValue;
			}
			set
			{
				curValue = value;
				if (value != null)
					Control.DateValue = value.ToNS();
				else
					Control.DateValue = DateTime.Now.ToNS();
			}
		}
	}
}

