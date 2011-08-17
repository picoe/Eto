using System;
using Eto.Forms;
using Gtk;
using System.Globalization;

namespace Eto.Platform.GtkSharp.CustomControls
{
	public class DateComboBox : BaseComboBox
	{
		DateTime selectedDate = DateTime.Now;
		DateTimePickerMode mode = DateTimePickerMode.DateTime;
	
		public event EventHandler DateChanged;
		
		protected void OnDateChanged (EventArgs e)
		{
			if (DateChanged != null)
				DateChanged (this, e);
		}

		public DateTimePickerMode Mode {
			get { return mode; }
			set
			{
				mode = value;
				SetValue();
			}
		}

		public Gdk.Color ErrorColor {
			get;
			set; 
		}

		public Gdk.Color NormalColor {
			get;
			set; 
		}
		
		public string Text {
			get {
				return Entry.Text;
			}
		}
		public DateTime MinDate {
			get; set;
		}

		public DateTime MaxDate {
			get; set;
		}
		
		public DateTime SelectedDate {
			get {
				return selectedDate;
			}
			set {
				selectedDate = value;
				SetValue();
			}
		}
		
		void SetValue()
		{
			switch (Mode) {
			case DateTimePickerMode.DateTime:
				Entry.Text = selectedDate.ToString ();
				break;
			case DateTimePickerMode.Date:
				Entry.Text = selectedDate.ToShortDateString ();
				break;
			case DateTimePickerMode.Time:
				Entry.Text = selectedDate.ToShortTimeString ();
				break;
			}
			
		}
		
		public DateComboBox ()
		{
			
			MinDate = DateTime.MinValue;
			MaxDate = DateTime.MaxValue;
			
			ErrorColor = new Gdk.Color (255, 0, 0);
			NormalColor = Entry.Style.Text (Gtk.StateType.Normal);

			Entry.Changed += delegate {
				Entry.ModifyText (Gtk.StateType.Normal, IsDateValid () ? NormalColor : ErrorColor);
				OnDateChanged (EventArgs.Empty);
			};
			PopupButton.Clicked += delegate {
				var dlg = new DateComboBoxDialog (selectedDate, this.Mode);
				dlg.DateChanged += delegate {
					SelectedDate = dlg.SelectedDate;
				};
				dlg.ShowPopup (this);
			};
		}
		
		public bool IsDateValid ()
		{
			DateTime date;
			if (DateTime.TryParse (Entry.Text, CultureInfo.CurrentCulture, DateTimeStyles.None, out date)) {
				if (date >= MinDate && date <= MaxDate) {
					selectedDate = date;
					return true;
				}
			}
			return false;
		}
	}
}

