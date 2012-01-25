using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Eto.Forms;
using mwc = Microsoft.Windows.Controls;

namespace Eto.Platform.Wpf.Forms.Controls
{
	public class DateTimePickerHandler : WpfControl<mwc.DateTimePicker, DateTimePicker>, IDateTimePicker
	{
		DateTimePickerMode mode;

		public DateTimePickerHandler ()
		{
			Control = new mwc.DateTimePicker ();
			Mode = DateTimePickerMode.Date;
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
			get; set; 
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
						Control.Format = mwc.DateTimeFormat.FullDateTime;
						break;
					case DateTimePickerMode.Time:
						Control.Format = mwc.DateTimeFormat.LongTime;
						break;
					default:
						throw new NotSupportedException ();
				}
			}
		}
	}
}
