using Eto.Wpf.Drawing;
using Eto.Wpf.CustomControls;
using Eto.Forms.ThemedControls;

namespace Eto.Wpf.Forms.Controls
{
	/// <summary>
	/// A <see cref="swc.ComboBox"/> that hosts an arbitrary editable control in its selection area (<see cref="EditContent"/>)
	/// and shows arbitrary content in its drop down popup (<see cref="DropDownContent"/>).
	/// </summary>
	/// <remarks>
	/// The visual template lives in the theme resource dictionaries (themes/generic/CustomComboBox.xaml) so it follows the
	/// active WPF theme (e.g. Fluent). This control only exposes the content hooks the template binds to.
	/// </remarks>
	public class EtoCustomComboBox : swc.ComboBox, IEtoWpfControl
	{
		public IWpfFrameworkElement Handler { get; set; }

		public static readonly sw.DependencyProperty EditContentProperty =
			sw.DependencyProperty.Register(nameof(EditContent), typeof(object), typeof(EtoCustomComboBox), new sw.PropertyMetadata(null));

		public static readonly sw.DependencyProperty DropDownContentProperty =
			sw.DependencyProperty.Register(nameof(DropDownContent), typeof(object), typeof(EtoCustomComboBox), new sw.PropertyMetadata(null));

		/// <summary>
		/// Gets or sets the control shown in the editable selection area of the combo box.
		/// </summary>
		public object EditContent
		{
			get => GetValue(EditContentProperty);
			set => SetValue(EditContentProperty, value);
		}

		/// <summary>
		/// Gets or sets the content shown in the drop down popup of the combo box.
		/// </summary>
		public object DropDownContent
		{
			get => GetValue(DropDownContentProperty);
			set => SetValue(DropDownContentProperty, value);
		}

		protected override sw.Size MeasureOverride(sw.Size constraint)
		{
			return Handler?.MeasureOverride(constraint, base.MeasureOverride) ?? base.MeasureOverride(constraint);
		}
	}

	/// <summary>
	/// WPF <see cref="DateTimePicker"/> handler that combines an Eto <see cref="ThemedDateTimeMaskedTextStepper"/> with a
	/// WPF <see cref="swc.ComboBox"/> showing a <see cref="swc.Calendar"/> in its drop down for
	/// <see cref="DateTimePickerMode.Date"/> and <see cref="DateTimePickerMode.DateTime"/> modes, and an Eto
	/// <see cref="ThemedDateTimeMaskedTextStepper"/> for <see cref="DateTimePickerMode.Time"/> mode.
	/// </summary>
	public class DateTimePickerHandler : WpfFrameworkElement<EtoBorder, DateTimePicker, DateTimePicker.ICallback>, DateTimePicker.IHandler
	{
		DateTimePickerMode _mode = DateTimePickerMode.Date;
		DateTime _minDate = DateTime.MinValue;
		DateTime _maxDate = DateTime.MaxValue;
		DateTime? _last;
		bool _showBorder = true;

		EtoCustomComboBox _combo;
		// hosts the stepper inside the combo box so it can be detached from it synchronously when
		// switching modes. Clearing the combo's EditContent alone doesn't disconnect the stepper from
		// the visual tree until the content presenter is re-measured.
		swc.Decorator _comboEditHost;
		swc.Calendar _calendar;
		DateTimeMaskedTextStepper _stepper;
		int _suppress;

		protected override sw.Size DefaultSize => new sw.Size(_mode == DateTimePickerMode.DateTime ? 180 : _mode == DateTimePickerMode.Time ? 100 : 120, double.NaN);

		protected override bool PreventUserResize => true;

		public override bool UseMousePreview => true;

		public override bool UseKeyPreview => true;

		public DateTimePickerHandler()
		{
			Control = new EtoBorder { Handler = this, Focusable = false };
			BuildPresentation();
		}

		void BuildPresentation()
		{
			EnsureStepper();
			_stepper.Mode = _mode;
			var stepperControl = _stepper.ToNative(true);
			if (_mode == DateTimePickerMode.Time)
			{
				// the stepper is shown directly, so detach it from the combo first
				if (_comboEditHost != null)
					_comboEditHost.Child = null;
				_stepper.ShowStepper = true;
				Control.Child = stepperControl;
			}
			else
			{
				_stepper.ShowStepper = false;
				_stepper.ShowBorder = false;
				// the stepper goes inside the combo, so detach it from this control first
				if (ReferenceEquals(Control.Child, stepperControl))
					Control.Child = null;
				EnsureCombo();
				Control.Child = _combo;
			}

			ApplyMinMax();
			ApplyShowBorder();
			SetSize();
		}

		void EnsureCombo()
		{
			EnsureStepper();

			if (_combo != null)
			{
				_comboEditHost.Child = _stepper.ToNative(true);
				return;
			}

			_calendar = new swc.Calendar { SelectionMode = swc.CalendarSelectionMode.SingleDate };
			_calendar.SelectedDatesChanged += Calendar_SelectedDatesChanged;

			_comboEditHost = new swc.Decorator { Child = _stepper.ToNative(true) };

			// the combo box visuals (background, border, chevron) come from the theme style in
			// themes/generic/CustomComboBox.xaml so it follows the active WPF theme.
			_combo = new EtoCustomComboBox
			{
				Handler = this,
				Focusable = false,
				EditContent = _comboEditHost,
				DropDownContent = _calendar
			};
			_combo.DropDownOpened += (sender, e) => SyncCalendarFromValue();
		}

		void EnsureStepper()
		{
			if (_stepper != null)
				return;

			_stepper = new DateTimeMaskedTextStepper { ShowBorder = _showBorder };
			_stepper.ValueChanged += Stepper_ValueChanged;

			var stepper = _stepper.ToNative(true);
			var stepperElement = _stepper.GetWpfFrameworkElement();
			stepperElement.SetScale(true, true);
			stepper.HorizontalAlignment = sw.HorizontalAlignment.Stretch;
			stepper.VerticalAlignment = sw.VerticalAlignment.Stretch;
			stepper.Width = double.NaN;
			stepper.Height = double.NaN;

		}

		void Box_ValueChanged(object sender, EventArgs e)
		{
			SyncCalendarFromValue();
			RaiseValueChanged();
		}

		void Stepper_ValueChanged(object sender, EventArgs e) => RaiseValueChanged();

		void Calendar_SelectedDatesChanged(object sender, swc.SelectionChangedEventArgs e)
		{
			if (_suppress > 0 || _stepper == null)
				return;

			var date = _calendar.SelectedDate;
			if (date == null)
				return;

			// preserve the time component when editing in date+time mode
			var current = _stepper.Value;
			var timeOfDay = current?.TimeOfDay ?? TimeSpan.Zero;
			_stepper.Value = _mode.HasFlag(DateTimePickerMode.Time) ? date.Value.Date + timeOfDay : date.Value.Date;
			_combo.IsDropDownOpen = false;
		}

		void SyncCalendarFromValue()
		{
			if (_calendar == null)
				return;
			_suppress++;
			try
			{
				var val = _stepper?.Value;
				if (val != null)
				{
					_calendar.SelectedDate = val.Value.Date;
					_calendar.DisplayDate = val.Value.Date;
				}
				else
				{
					_calendar.SelectedDate = null;
				}
			}
			finally
			{
				_suppress--;
			}
		}

		void RaiseValueChanged()
		{
			var val = Value;
			if (_last != val && (_last == null || val == null || Math.Abs((_last.Value - val.Value).TotalSeconds) >= 1))
			{
				_last = val;
				Callback.OnValueChanged(Widget, EventArgs.Empty);
			}
		}

		void ApplyMinMax()
		{
			if (_stepper != null)
			{
				_stepper.MinDate = _minDate;
				_stepper.MaxDate = _maxDate;
			}
			if (_calendar != null)
			{
				_calendar.DisplayDateStart = _minDate <= DateTime.MinValue ? (DateTime?)null : _minDate;
				_calendar.DisplayDateEnd = _maxDate >= DateTime.MaxValue ? (DateTime?)null : _maxDate;
			}
		}

		void ApplyShowBorder()
		{
			// the inner masked box is always borderless; the combo provides the border for date modes,
			// while the stepper draws its own border for time mode.
			if (_mode != DateTimePickerMode.Time && _combo != null)
				_combo.BorderThickness = _showBorder ? new sw.Thickness(1) : new sw.Thickness(0);
			if (_mode == DateTimePickerMode.Time && _stepper != null)
				_stepper.ShowBorder = _showBorder;
		}

		public DateTime? Value
		{
			get => _stepper?.Value;
			set
			{
				if (_stepper != null)
					_stepper.Value = value;
				_last = value;
			}
		}

		public DateTime MinDate
		{
			get => _minDate;
			set
			{
				_minDate = value;
				if (_maxDate < _minDate)
					_maxDate = _minDate;
				ApplyMinMax();
			}
		}

		public DateTime MaxDate
		{
			get => _maxDate;
			set
			{
				_maxDate = value;
				if (_minDate > _maxDate)
					_minDate = _maxDate;
				ApplyMinMax();
			}
		}

		public DateTimePickerMode Mode
		{
			get => _mode;
			set
			{
				if (_mode == value)
					return;

				var current = Value;
				_mode = value;
				BuildPresentation();
				Value = current;
			}
		}

		public bool ShowBorder
		{
			get => _showBorder;
			set
			{
				_showBorder = value;
				ApplyShowBorder();
			}
		}

		public override Color BackgroundColor
		{
			get => (_combo?.Background ?? Control.Background).ToEtoColor();
			set
			{
				var brush = value.ToWpfBrush();
				if (_combo != null)
					_combo.Background = brush;
			}
		}

		public Color TextColor
		{
			get => _stepper?.TextColor ?? Colors.Black;
			set
			{
				if (_stepper != null)
					_stepper.TextColor = value;
			}
		}

		static readonly object FontKey = new object();

		public Font Font
		{
			get => Widget.Properties.Get<Font>(FontKey) ?? _stepper?.Font;
			set
			{
				Widget.Properties[FontKey] = value;
				if (_stepper != null)
					_stepper.Font = value;
			}
		}

		public override void Focus() => _stepper?.Focus();

		public override bool HasFocus => base.HasFocus || _stepper?.HasFocus == true;
	}
}
