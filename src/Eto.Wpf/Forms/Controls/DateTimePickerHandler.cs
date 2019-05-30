using System;
using Eto.Forms;
using mwc = Xceed.Wpf.Toolkit;
using sw = System.Windows;
using swc = System.Windows.Controls;
using System.Globalization;
using Eto.Drawing;
using Eto.Wpf.Drawing;

namespace Eto.Wpf.Forms.Controls
{
	public class DateTimePickerHandler : WpfFrameworkElement<swc.Border, DateTimePicker, DateTimePicker.ICallback>, DateTimePicker.IHandler
	{
		mwc.DateTimePicker dtp;
		mwc.DateTimeUpDown dtud;
		DateTime? last;

		DateTimePickerMode mode;

		protected override sw.Size DefaultSize => new sw.Size(mode == DateTimePickerMode.DateTime ? 180 : 120, double.NaN);

		protected override bool PreventUserResize { get { return true; } }

		public DateTimePickerHandler()
		{
			Control = new EtoBorder { Handler = this, Focusable = false };
			Mode = DateTimePickerMode.Date;
		}

		static sw.Thickness DefaultBorderThickness = new mwc.DateTimePicker().BorderThickness;

		public bool ShowBorder
		{
			get { return !dtp.BorderThickness.ToEto().IsZero; }
			set { dtp.BorderThickness = value ? DefaultBorderThickness : new sw.Thickness(0); }
		}

		void CreateDateTimeUpDown()
		{
			if (dtud == null)
			{
				dtud = new mwc.DateTimeUpDown
				{
					Focusable = true,
					IsTabStop = true,
					ClipValueToMinMax = true,
					ShowButtonSpinner = true
				};
				dtud.ValueChanged += UpDown_ValueChanged;
			}
			if (dtp != null)
			{
				CopyValues(dtp, dtud);
				dtp.ValueChanged -= UpDown_ValueChanged;
				dtp = null;
			}
			Control.Child = dtud;
		}

		void CreateDateTimePicker()
		{
			if (dtp == null)
			{
				dtp = new mwc.DateTimePicker
				{
					ShowButtonSpinner = false,
					AutoCloseCalendar = true,
					ClipValueToMinMax = true,
					Focusable = true,
					IsTabStop = true
				};
				dtp.ValueChanged += UpDown_ValueChanged;
			}
			if (dtud != null)
			{
				CopyValues(dtud, dtp);
				dtud.ValueChanged -= UpDown_ValueChanged;
				dtud = null;
			}
			Control.Child = dtp;
		}

		void CopyValues(mwc.Primitives.UpDownBase<DateTime?> source, mwc.Primitives.UpDownBase<DateTime?> dest)
		{
			dest.Minimum = source.Minimum;
			dest.Maximum = source.Maximum;
			dest.Value = source.Value;
			dest.Background = source.Background;
			dest.Foreground = source.Foreground;
			var font = Widget.Properties.Get<Font>(FontKey);
			if (font != null)
				dest.SetEtoFont(font, SetDecorations);
		}

		void UpDown_ValueChanged(object sender, sw.RoutedPropertyChangedEventArgs<object> e)
		{
			var ctl = UpDown;
			var val = Value;
			if (val != null)
			{
				// still need to do this as the popup doesn't limit the value when using the calendar
				if (ctl.Minimum != null && val.Value < ctl.Minimum)
					val = Value = ctl.Minimum.Value;
				else if (ctl.Maximum != null && val.Value > ctl.Maximum)
					val = Value = ctl.Maximum.Value;
			}

			if (last != val && (last == null || val == null || Math.Abs((last.Value - val.Value).TotalSeconds) >= 1))
			{
				Callback.OnValueChanged(Widget, EventArgs.Empty);
				last = val;
			}
		}

		public override bool UseMousePreview { get { return true; } }

		public override bool UseKeyPreview { get { return true; } }

		public DateTime? Value
		{
			get { return UpDown.Value; }
			set { UpDown.Value = value; }
		}

		mwc.Primitives.UpDownBase<DateTime?> UpDown
		{
			get
			{
				return dtp ?? dtud;
			}
		}

		public DateTime MinDate
		{
			get { return UpDown.Minimum ?? DateTime.MinValue; }
			set { UpDown.Minimum = value == DateTime.MinValue ? null : (DateTime?)value; }
		}

		public DateTime MaxDate
		{
			get { return UpDown.Maximum ?? DateTime.MaxValue; }
			set { UpDown.Maximum = value == DateTime.MaxValue ? null : (DateTime?)value; }
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
						CreateDateTimePicker();
						dtp.TimePickerVisibility = sw.Visibility.Collapsed;
						dtp.Format = mwc.DateTimeFormat.ShortDate;
						break;
					case DateTimePickerMode.DateTime:
						CreateDateTimePicker();
						dtp.TimePickerVisibility = sw.Visibility.Visible;
						var format = CultureInfo.CurrentUICulture.DateTimeFormat;
						dtp.Format = mwc.DateTimeFormat.Custom;
						dtp.FormatString = format.ShortDatePattern + " " + format.LongTimePattern;
						break;
					case DateTimePickerMode.Time:
						CreateDateTimeUpDown();
						dtud.Format = mwc.DateTimeFormat.LongTime;
						break;
					default:
						throw new NotSupportedException();
				}
				SetSize();
			}
		}

		public override Color BackgroundColor
		{
			get { return UpDown.Background.ToEtoColor(); }
			set { UpDown.Background = value.ToWpfBrush(Control.Background); }
		}

		protected virtual void SetDecorations(sw.TextDecorationCollection decorations)
		{
		}

		static readonly object FontKey = new object();

		public Font Font
		{
			get { return Widget.Properties.Create<Font>(FontKey, () => new Font(new FontHandler(UpDown))); }
			set
			{
				if (Widget.Properties.Get<Font>(FontKey) != value)
				{
					Widget.Properties[FontKey] = value;
					UpDown.SetEtoFont(value, SetDecorations);
				}
			}
		}

		public Color TextColor
		{
			get { return UpDown.Foreground.ToEtoColor(); }
			set { UpDown.Foreground = value.ToWpfBrush(UpDown.Foreground); }
		}
	}
}
