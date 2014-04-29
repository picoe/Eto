using System;
using Eto.Forms;

namespace Eto.GtkSharp.Forms.Controls
{
	public class DateTimePickerHandler : GtkControl<CustomControls.DateComboBox, DateTimePicker>, IDateTimePicker
	{
		public DateTimePickerHandler()
		{
			Control = new CustomControls.DateComboBox();
			this.Mode = DateTimePicker.DefaultMode;
		}

		protected override void Initialize()
		{
			base.Initialize();
			Control.DateChanged += Connector.HandleDateChanged;
		}

		protected new DateTimePickerConnector Connector { get { return (DateTimePickerConnector)base.Connector; } }

		protected override WeakConnector CreateConnector()
		{
			return new DateTimePickerConnector();
		}

		protected class DateTimePickerConnector : GtkControlConnector
		{
			public new DateTimePickerHandler Handler { get { return (DateTimePickerHandler)base.Handler; } }

			public void HandleDateChanged(object sender, EventArgs e)
			{
				Handler.Widget.OnValueChanged(EventArgs.Empty);
			}
		}

		protected override Gtk.Widget FontControl
		{
			get { return Control.Entry; }
		}

		public DateTime? Value
		{
			get { return Control.SelectedDate; }
			set { Control.SelectedDate = value; }
		}

		public DateTime MinDate
		{
			get { return Control.MinDate; }
			set { Control.MinDate = value; }
		}

		public DateTime MaxDate
		{
			get { return Control.MaxDate; }
			set { Control.MaxDate = value; }
		}

		public DateTimePickerMode Mode
		{
			get { return Control.Mode; }
			set { Control.Mode = value; }
		}
	}
}

