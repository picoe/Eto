namespace Eto.Mac.Forms.Controls
{
	public class DateTimePickerHandler : MacControl<NSDatePicker, DateTimePicker, DateTimePicker.ICallback>, DateTimePicker.IHandler
	{
		DateTime? curValue;
		DateTimePickerMode mode;

		public class EtoDatePicker : NSDatePicker, IMacControl
		{
			public override void DrawRect(CGRect dirtyRect)
			{
				var h = Handler;
				if (h == null)
				{
					base.DrawRect(dirtyRect);
					return;
				}

				if (h.curValue != null)
				{
					base.DrawRect(dirtyRect);
				}
				else
				{
					// paint with no elements visible
					// use transparent color so sizing is still correct.
					var old = TextColor;
					TextColor = NSColor.Clear;
					base.DrawRect(dirtyRect);
					TextColor = old;
				}
			}

			public WeakReference WeakHandler { get; set; }

			public DateTimePickerHandler Handler
			{
				get { return (DateTimePickerHandler)WeakHandler.Target; }
				set { WeakHandler = new WeakReference(value); }
			}

			public EtoDatePicker()
			{
				TimeZone = NSTimeZone.LocalTimeZone;
				Calendar = NSCalendar.CurrentCalendar;
				DateValue = DateTime.Now.ToNS();
			}
		}

		protected override bool DefaultUseAlignmentFrame => true;

		protected override NSDatePicker CreateControl() => new EtoDatePicker();


		static IntPtr selSetPresentsCalendarOverlay_Handle = Selector.GetHandle("setPresentsCalendarOverlay:");
		static bool SupportsCalendarOverlay => ObjCExtensions.InstancesRespondToSelector<NSDatePicker>("presentsCalendarOverlay");


		protected override void Initialize()
		{
			this.Mode = DateTimePickerMode.Date;
			// apple+backspace clears the value
			Control.ValidateProposedDateValue += HandleValidateProposedDateValue;

			if (SupportsCalendarOverlay)
			{
				// 10.15+ supports having a calendar drop down! finally..
				// no need for spinner as one is presented with the calendar
				Control.DatePickerStyle = NSDatePickerStyle.TextField;
				Messaging.void_objc_msgSend_bool(Control.Handle, selSetPresentsCalendarOverlay_Handle, true);
			}

			base.Initialize();
			Widget.KeyDown += HandleKeyDown;
			// when clicking, set the value if it is null
			Widget.MouseDown += HandleMouseDown;
		}

		static void HandleKeyDown(object sender, KeyEventArgs e)
		{
			var handler = (DateTimePickerHandler)((Control)sender).Handler;
			if (!e.Handled)
			{
				if (e.KeyData == (Keys.Application | Keys.Backspace))
				{
					handler.curValue = null;
					handler.Callback.OnValueChanged(handler.Widget, EventArgs.Empty);
					handler.Control.NeedsDisplay = true;
					e.Handled = true;
				}
				if (e.KeyData == Keys.Enter && handler.curValue == null)
				{
					// pressing enter will set the current value if null, and bring up calendar.
					handler.curValue = handler.Control.DateValue.ToEto();
					handler.Callback.OnValueChanged(handler.Widget, EventArgs.Empty);
					handler.Control.NeedsDisplay = true;
				}
			}
		}

		static void HandleMouseDown(object sender, MouseEventArgs e)
		{
			var handler = (DateTimePickerHandler)((Control)sender).Handler;
			if (e.Buttons == MouseButtons.Primary)
			{
				if (handler.curValue == null)
				{
					handler.curValue = handler.Control.DateValue.ToEto();
					handler.Callback.OnValueChanged(handler.Widget, EventArgs.Empty);
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
				handler.Callback.OnValueChanged(handler.Widget, EventArgs.Empty);
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
				if (value != curValue)
				{
					curValue = value;
					// don't validate otherwise the new value gets overridden when null
					Control.ValidateProposedDateValue -= HandleValidateProposedDateValue;
					Control.DateValue = (value ?? DateTime.Now).ToNS();
					Control.ValidateProposedDateValue += HandleValidateProposedDateValue;
					Callback.OnValueChanged(Widget, EventArgs.Empty);
				}
			}
		}

		public Color TextColor
		{
			get { return Control.TextColor.ToEto(); }
			set { Control.TextColor = value.ToNSUI(); }
		}

		protected override void SetBackgroundColor(Color? color)
		{
			// base.SetBackgroundColor(color);
			if (color != null)
			{
				Control.BackgroundColor = color.Value.ToNSUI();
				Control.DrawsBackground = color.Value.A > 0;
				Control.WantsLayer = color.Value.A < 1;
			}
			else
			{
				Control.BackgroundColor = NSColor.ControlBackground;
				Control.DrawsBackground = true;
			}
		}

		public bool ShowBorder
		{
			get { return Control.Bordered; }
			set
			{
				Control.Bordered = value;

				Control.DatePickerStyle = value && !SupportsCalendarOverlay ? NSDatePickerStyle.TextFieldAndStepper : NSDatePickerStyle.TextField;
			}
		}
	}
}

