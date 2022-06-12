using System;
using System.Threading;
using Eto.Forms;
using Eto.Drawing;

using System.Globalization;
using Eto.GtkSharp.Forms;

namespace Eto.GtkSharp.CustomControls
{
	public class DateComboBox : BaseComboBox
	{
		DateTime? selectedDate;
		private DateComboBoxDialog dlg;
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
			Entry.FocusOutEvent += Entry_FocusOutEvent;

			// called on button press..
			PopupButtonClicked += delegate
			{	
				// if we are still up somehow, take the old dialog down..
				if (dlg != null)
				{
					// this will appear to the user as a click that doesn't 
					// bring up a window..
					dlg.CloseDialog();
					CleanDialog();
				}
				else			
				{
#if GTKCORE
					// move the focus to the Entry control so we can detect it moving away..
					Entry.GrabFocusWithoutSelecting();
#endif
					dlg = new DateComboBoxDialog(selectedDate ?? DateTime.Now, this.Mode);
					dlg.DateChanged += Dlg_DateChanged;
					dlg.DialogClosed += Dlg_DialogClosed;
					dlg.ShowPopup(this);
				}
			};
		}

		private void CleanDialog()
		{
			if (dlg == null)
				return;

			dlg.DateChanged -= Dlg_DateChanged;
			dlg.DialogClosed -= Dlg_DialogClosed;
			dlg = null;
		}

		private void Dlg_DateChanged(object sender, EventArgs e)
		{
			if (dlg == null)
				return;
			selectedDate = dlg.SelectedDate;
			ValidateDateRange();
			SetValue();
		}

		/// <summary>
		/// remove our reference to the dialog ONLY when we know it's 
		/// really gone..
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Dlg_DialogClosed(object sender, EventArgs e)
		{
			CleanDialog();
		}

		/// <summary>
		/// If the drop down dialog is up and lose focus on the parent field, close it
		/// </summary>
		/// <param name="o"></param>
		/// <param name="args"></param>
		private void Entry_FocusOutEvent(object o, Gtk.FocusOutEventArgs args)
		{
			// if the pull-down is are up, close it
			dlg?.CloseDialog();
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