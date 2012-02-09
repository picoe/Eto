using System;
using Eto.Forms;

namespace Eto.Platform.GtkSharp.Forms.Controls
{
	public class DateTimePickerHandler : GtkControl<CustomControls.DateComboBox, DateTimePicker>, IDateTimePicker
	{
		public DateTimePickerHandler ()
		{
			Control = new CustomControls.DateComboBox ();
			this.Control.DateChanged += delegate {
				Widget.OnValueChanged (EventArgs.Empty);
			};
		}

		public DateTime? Value {
			get {
				return this.Control.SelectedDate;
			}
			set {
				this.Control.SelectedDate = value;
			}
		}

		public DateTime MinDate {
			get {
				return this.Control.MinDate;
			}
			set {
				this.Control.MinDate = value;
			}
		}

		public DateTime MaxDate {
			get {
				return this.Control.MaxDate;
			}
			set {
				this.Control.MaxDate = value;
			}
		}

		public DateTimePickerMode Mode {
			get {
				return this.Control.Mode;
			}
			set {
				this.Control.Mode = value;
			}
		}
	}
}

