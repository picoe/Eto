using System;
using Eto.Forms;

namespace Eto.Platform.GtkSharp.Forms.Controls
{
	public class DateTimePickerHandler : GtkControl<CustomControls.DateComboBox, DateTimePicker>, IDateTimePicker
	{
		public DateTimePickerHandler ()
		{
			Control = new CustomControls.DateComboBox ();
			this.Mode = DateTimePicker.DefaultMode;
		}
		
		public override void OnLoad (EventArgs e)
		{
			base.OnLoad (e);
			Control.DateChanged += delegate {
				Widget.OnValueChanged (EventArgs.Empty);
			};
		}

		protected override Gtk.Widget FontControl
		{
			get { return Control.Entry; }
		}

		public DateTime? Value {
			get {
				return Control.SelectedDate;
			}
			set {
				Control.SelectedDate = value;
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

		public DateTimePickerMode Mode {
			get {
				return Control.Mode;
			}
			set {
				Control.Mode = value;
			}
		}
	}
}

