using System;
using Eto.Forms;

namespace Eto.GtkSharp.CustomControls
{
	public class DateComboBoxDialog : Gtk.Window
	{
		readonly DateTimePickerMode mode;
		Gtk.Calendar calendar;
		AnalogClock clock;
		Gtk.SpinButton hourSpin;
		Gtk.SpinButton minutesSpin;
		Gtk.SpinButton secondsSpin;
		
		public event EventHandler<EventArgs> DateChanged;
		
		protected virtual void OnDateChanged (EventArgs e)
		{
			if (DateChanged != null)
				DateChanged (this, e);
		}

		bool HasTime {
			get { return (mode & DateTimePickerMode.Time) != 0; }
		}

		bool HasDate {
			get { return (mode & DateTimePickerMode.Date) != 0; }
		}

		public DateTime SelectedDate {
			get
			{
				if (HasTime)
				{
					DateTime d = HasDate ? calendar.Date : DateTime.Today;
					return new DateTime(d.Year, d.Month, d.Day, (int)hourSpin.Value, (int)minutesSpin.Value, (int)secondsSpin.Value);
				}
				if (HasDate)
				{
					DateTime d = calendar.Date;
					return new DateTime(d.Year, d.Month, d.Day);
				}
				throw new InvalidOperationException();
			}
		}
		
		public DateComboBoxDialog (DateTime dateTime, DateTimePickerMode mode)
			: base(Gtk.WindowType.Popup)
		{
			this.mode = mode;
			this.CreateControls ();

			if (HasDate) {
				calendar.Date = dateTime;
			}
			if (HasTime) {
				hourSpin.Value = dateTime.Hour;
				minutesSpin.Value = dateTime.Minute;
				secondsSpin.Value = dateTime.Second;
				UpdateClock ();
			}

			this.ButtonPressEvent += delegate(object o, Gtk.ButtonPressEventArgs args) {
				if (args.Event.Type == Gdk.EventType.ButtonPress) {
					// single click only!
					CloseDialog ();
				}
			};
			
		}
		
		public void ShowPopup (Gtk.Widget parent)
		{
			int x, y;
			parent.ParentWindow.GetOrigin (out x, out y);
			Move(x + parent.Allocation.Left, y + parent.Allocation.Top + parent.Allocation.Height);

			ShowAll();
			this.Grab ();
		}

#if GTK2
		protected override bool OnExposeEvent (Gdk.EventExpose evnt)
		{
			base.OnExposeEvent (evnt);
			
			int winWidth, winHeight;
			GetSize (out winWidth, out winHeight);
			GdkWindow.DrawRectangle (Style.ForegroundGC (Gtk.StateType.Insensitive), false, 0, 0, winWidth - 1, winHeight - 1);
			
			return false;
		}
#else
		protected override bool OnDrawn (Cairo.Context cr)
		{
			base.OnDrawn (cr);
			this.StyleContext.RenderFrame(cr, 0, 0, this.AllocatedWidth - 1, this.AllocatedHeight - 1);
			return true;
		}
#endif

		void CloseDialog ()
		{
			this.RemoveGrab ();
#if GTKCORE
			Close();
#else
			Destroy();
#endif
		}

		void UpdateClock ()
		{
			if (!HasTime) return;
			clock.Time = SelectedDate;
		}

		Gtk.Widget CalendarControls ()
		{
			var vbox = new Gtk.VBox { 
				Spacing = 5
			};

			calendar = new Gtk.Calendar
			{
				CanFocus = true,
				DisplayOptions = Gtk.CalendarDisplayOptions.ShowHeading | Gtk.CalendarDisplayOptions.ShowDayNames
			};
			
			calendar.DaySelected += delegate {
				OnDateChanged (EventArgs.Empty);
			};
			
			calendar.DaySelectedDoubleClick += delegate {
				OnDateChanged (EventArgs.Empty);
				CloseDialog ();
			};

			vbox.PackStart(calendar, false, false, 0);
			
			var hbox = new Gtk.HBox (true, 6);

			var todayButton = new Gtk.Button {
				CanFocus = true,
				Label = HasTime ? "Now" : "Today"
			};
			todayButton.Clicked += delegate {
				if (HasDate) {
					calendar.Date = DateTime.Now;
				}
				if (HasTime) {
					hourSpin.Value = DateTime.Now.Hour;
					minutesSpin.Value = DateTime.Now.Minute;
					secondsSpin.Value = DateTime.Now.Second;
					UpdateClock ();
				}
				OnDateChanged (EventArgs.Empty);
				CloseDialog ();
			};
			
			hbox.PackStart (todayButton, false, false, 0);

			vbox.PackStart (hbox, false, false, 0);

			return vbox;
		}
		
		Gtk.SpinButton CreateSpinner (int max, int increment, Gtk.SpinButton parent)
		{
			var spin = new Gtk.SpinButton (-1, max, 1){
				CanFocus = true,
				Numeric = true,
				ClimbRate = 1,
				WidthChars = 2,
				Value = 0
			};
			spin.Adjustment.PageIncrement = increment;
			spin.ValueChanged += delegate {
				if (Math.Abs(spin.Value - max) < 0.1f)
				{
					spin.Value = 0;
					if (parent != null)
						parent.Value = parent.Value + 1;
				}
				if (Math.Abs(spin.Value - -1) < 0.1f)
				{
					spin.Value = max - 1;
					if (parent != null)
						parent.Value = parent.Value - 1;
				}
				UpdateClock ();
				OnDateChanged (EventArgs.Empty);
			};
			return spin;
		}
		
		Gtk.Widget ClockControls ()
		{

#if GTK2
			var vbox = new Gtk.VBox ();
			var spinners = new Gtk.HBox ();
#else
			var vbox = new Gtk.HBox ();
			var spinners = new Gtk.VBox ();
#endif
			vbox.Spacing = 6;
			spinners.Spacing = 6;

			clock = new AnalogClock();
			clock.SetSizeRequest (130, 130);
			vbox.PackStart(clock, true, true, 0);


			spinners.PackStart (new Gtk.Label ("Hour"), false, false, 0);
			
			hourSpin = CreateSpinner (24, 1, null);
			spinners.PackStart (hourSpin, false, false, 0);

			spinners.PackStart (new Gtk.Label ("Min"), false, false, 0);

			minutesSpin = CreateSpinner (60, 10, hourSpin);
			spinners.PackStart (minutesSpin, false, false, 0);
			
			spinners.PackStart (new Gtk.Label ("Sec"), false, false, 0);

			secondsSpin = CreateSpinner (60, 10, minutesSpin);
			spinners.PackStart (secondsSpin, false, false, 0);
			
			vbox.PackEnd (spinners, false, false, 0);

			return vbox;
		}
        
		void CreateControls ()
		{
			TypeHint = Gdk.WindowTypeHint.Menu;
			WindowPosition = Gtk.WindowPosition.CenterOnParent;
			BorderWidth = 1;
			Resizable = false;
#if GTK2
			AllowGrow = false;
#else
			Resizable = false;
#endif
			Decorated = false;
			DestroyWithParent = true;
			SkipPagerHint = true;
			SkipTaskbarHint = true;

			var hbox = new Gtk.HBox {
				Spacing = 5,
				BorderWidth = 3
			};
			
			if (HasDate)
				hbox.PackStart (CalendarControls (), true, true, 0);
			
			if (HasTime)
				hbox.PackStart (ClockControls (), true, true, 0);

			Add(hbox);
		}
	}
}

