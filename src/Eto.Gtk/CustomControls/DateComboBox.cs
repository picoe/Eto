using System;
using Eto.Forms;
using Eto.Drawing;
using System.Globalization;

namespace Eto.GtkSharp.CustomControls
{
	public class DateComboBox : BaseComboBox
	{
		DateTime? selectedDate;
		DateTimePickerMode mode = DateTimePickerMode.DateTime;

		public event EventHandler DateChanged;

		protected void OnDateChanged(EventArgs e)
		{
			if (DateChanged != null)
				DateChanged(this, e);
		}

		public DateTimePickerMode Mode
		{
			get { return mode; }
			set
			{
				mode = value;
				SetValue();
			}
		}

		public Color ErrorColor { get; set; }

		public Color NormalColor { get; set; }

		public string Text { get { return Entry.Text; } }

		public DateTime MinDate { get; set; }

		public DateTime MaxDate { get; set; }

		public DateTime? SelectedDate
		{
			get { return selectedDate; }
			set
			{
				selectedDate = value;
				SetValue();
			}
		}

		public bool AllowInvalidDates { get; set; }

		void SetValue()
		{
			if (selectedDate == null)
			{
				Entry.Text = string.Empty;
			}
			else
			{
				switch (Mode)
				{
					case DateTimePickerMode.DateTime:
						Entry.Text = selectedDate.Value.ToString();
						break;
					case DateTimePickerMode.Date:
						Entry.Text = selectedDate.Value.ToShortDateString();
						break;
					case DateTimePickerMode.Time:
						Entry.Text = selectedDate.Value.ToShortTimeString();
						break;
				}
			}
		}

		public DateComboBox()
		{
			
			MinDate = DateTime.MinValue;
			MaxDate = DateTime.MaxValue;
			
			ErrorColor = Colors.Red;
			NormalColor = Entry.GetTextColor();

			Entry.Changed += HandleChanged;
			PopupButtonClicked += delegate
			{
				var dlg = new DateComboBoxDialog(selectedDate ?? DateTime.Now, this.Mode);
				dlg.DateChanged += delegate
				{
					selectedDate = dlg.SelectedDate;
					ValidateDateRange();
					SetValue();
				};
				dlg.ShowPopup(this);
			};
		}

		void HandleChanged(object sender, EventArgs e)
		{
			var isValid = IsDateValid();
			if (!ValidateDateRange())
			{
				SetValue();
				return;
			}
			Entry.SetTextColor(isValid ? NormalColor : ErrorColor);
			OnDateChanged(EventArgs.Empty);
		}

		bool ValidateDateRange()
		{
			if (!AllowInvalidDates && selectedDate != null)
			{
				if (selectedDate < MinDate)
				{
					SelectedDate = MinDate;
					return false;
				}
				if (selectedDate > MaxDate)
				{
					SelectedDate = MaxDate;
					return false;
				}
			}
			return true;
		}

		public bool IsDateValid()
		{
			if (string.IsNullOrEmpty(Entry.Text))
			{
				selectedDate = null;
				return true;
			}
			DateTime date;
			if (DateTime.TryParse(Entry.Text, CultureInfo.CurrentCulture, DateTimeStyles.None, out date))
			{
				if (date >= MinDate && date <= MaxDate)
				{
					selectedDate = date;
					return true;
				}
			}
			return false;
		}
	}
}