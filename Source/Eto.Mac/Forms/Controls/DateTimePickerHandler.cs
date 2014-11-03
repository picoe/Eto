using System;
using Eto.Forms;
using Eto.Drawing;
#if XAMMAC2
using AppKit;
using Foundation;
using CoreGraphics;
using ObjCRuntime;
using CoreAnimation;
#else
using MonoMac.AppKit;
using MonoMac.Foundation;
using MonoMac.CoreGraphics;
using MonoMac.ObjCRuntime;
using MonoMac.CoreAnimation;
#if Mac64
using CGSize = MonoMac.Foundation.NSSize;
using CGRect = MonoMac.Foundation.NSRect;
using CGPoint = MonoMac.Foundation.NSPoint;
using nfloat = System.Double;
using nint = System.Int64;
using nuint = System.UInt64;
#else
using CGSize = System.Drawing.SizeF;
using CGRect = System.Drawing.RectangleF;
using CGPoint = System.Drawing.PointF;
using nfloat = System.Single;
using nint = System.Int32;
using nuint = System.UInt32;
#endif
#endif

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
				if (Handler.curValue != null)
					base.DrawRect(dirtyRect);
				else
				{
					// paint with no elements visible
					var old = DatePickerElements;
					DatePickerElements = 0;
					base.DrawRect(dirtyRect);
					DatePickerElements = old;
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
			#pragma warning disable 612,618
			this.Mode = DateTimePicker.DefaultMode;
			#pragma warning restore 612,618
			// apple+backspace clears the value
			Control.ValidateProposedDateValue += HandleValidateProposedDateValue;
		}

		protected override void Initialize()
		{
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
			if (color != null)
			{
				Control.Cell.BackgroundColor = color.Value.ToNSUI();
				Control.Cell.DrawsBackground = true;
			}
			else
			{
				Control.Cell.BackgroundColor = NSColor.ControlBackground;
				Control.Cell.DrawsBackground = true;
			}
		}
	}
}

