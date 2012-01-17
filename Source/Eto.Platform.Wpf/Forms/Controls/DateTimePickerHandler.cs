using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Eto.Forms;

namespace Eto.Platform.Wpf.Forms.Controls
{
	public class DateTimePickerHandler : WpfControl<System.Windows.Controls.DatePicker, DateTimePicker>, IDateTimePicker
	{
		public DateTimePickerHandler ()
		{
			Control = new System.Windows.Controls.DatePicker ();
		}


		public DateTime? Value
		{
			get; set; 
		}

		public DateTime MinDate
		{
			get; set; 
		}

		public DateTime MaxDate
		{
			get; set; 
		}

		public DateTimePickerMode Mode
		{
			get; set; 
		}
	}
}
