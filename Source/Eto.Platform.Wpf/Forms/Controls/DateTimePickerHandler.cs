using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Eto.Forms;
using mwc = Microsoft.Windows.Controls;
using System.Globalization;

namespace Eto.Platform.Wpf.Forms.Controls
{
	public class DateTimePickerHandler : WpfControl<mwc.DateTimePicker, DateTimePicker>, IDateTimePicker
	{
		DateTimePickerMode mode;
		bool sizeSet;

		public DateTimePickerHandler ()
		{
			Control = new mwc.DateTimePicker {

			};
			Mode = DateTimePicker.DefaultMode;
		}

		public override void OnLoad (EventArgs e)
		{
			base.OnLoad (e);
			Control.ValueChanged += delegate {
				Widget.OnValueChanged (EventArgs.Empty);
			};
		}

		public DateTime? Value
		{
			get { return Control.Value; }
			set { Control.Value = value; }
		}

		public DateTime MinDate
		{
			get;
			set;
		}

		public DateTime MaxDate
		{
			get;
			set;
		}

		public override Eto.Drawing.Size Size
		{
			get { return base.Size; }
			set
			{
				base.Size = value;
				sizeSet = value.Width != -1;
				SetWidth ();
			}
		}
		void SetWidth ()
		{
			if (!sizeSet) {
				Control.Width = mode == DateTimePickerMode.DateTime ? 180 : 120;
			}
		}

		public DateTimePickerMode Mode
		{
			get { return mode; }
			set
			{
				mode = value;
				switch (mode) {
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
					throw new NotSupportedException ();
				}
				SetWidth ();
			}
		}
	}
}
