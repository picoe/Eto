using System;
using Eto.Forms;
using Eto.Drawing;
using Eto.Platform.GtkSharp.Drawing;

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
			this.Control.DateChanged += delegate {
				Widget.OnValueChanged (EventArgs.Empty);
			};
		}

		protected override Gtk.Widget FontControl
		{
			get { return Control.Entry; }
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

