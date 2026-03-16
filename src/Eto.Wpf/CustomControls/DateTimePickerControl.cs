namespace Eto.Wpf.CustomControls
{
	/// <summary>
	/// Custom WPF DateTimePicker control.
	/// Uses native WPF DatePicker for date selection and a TextBox for time entry.
	/// </summary>
	public class DateTimePickerControl : swc.Control
	{
		swc.DatePicker _datePicker;
		swc.TextBox _timeTextBox;
		swc.Grid _grid;
		bool _suppressValueChanged;

		public static readonly sw.DependencyProperty ValueProperty =
			sw.DependencyProperty.Register(nameof(Value), typeof(DateTime?), typeof(DateTimePickerControl),
				new sw.FrameworkPropertyMetadata(null, OnValueChanged));

		public static readonly sw.DependencyProperty MinimumProperty =
			sw.DependencyProperty.Register(nameof(Minimum), typeof(DateTime?), typeof(DateTimePickerControl),
				new sw.PropertyMetadata(null, OnMinMaxChanged));

		public static readonly sw.DependencyProperty MaximumProperty =
			sw.DependencyProperty.Register(nameof(Maximum), typeof(DateTime?), typeof(DateTimePickerControl),
				new sw.PropertyMetadata(null, OnMinMaxChanged));

		public static readonly sw.DependencyProperty ModeProperty =
			sw.DependencyProperty.Register(nameof(Mode), typeof(DateTimePickerMode), typeof(DateTimePickerControl),
				new sw.PropertyMetadata(DateTimePickerMode.Date, OnModeChanged));

		public event EventHandler<EventArgs> ValueChanged;

		public DateTime? Value
		{
			get => (DateTime?)GetValue(ValueProperty);
			set => SetValue(ValueProperty, value);
		}

		public DateTime? Minimum
		{
			get => (DateTime?)GetValue(MinimumProperty);
			set => SetValue(MinimumProperty, value);
		}

		public DateTime? Maximum
		{
			get => (DateTime?)GetValue(MaximumProperty);
			set => SetValue(MaximumProperty, value);
		}

		public DateTimePickerMode Mode
		{
			get => (DateTimePickerMode)GetValue(ModeProperty);
			set => SetValue(ModeProperty, value);
		}

		public DateTimePickerControl()
		{
			Focusable = false;
			IsTabStop = false;

			_grid = new swc.Grid();

			_datePicker = new swc.DatePicker
			{
				VerticalAlignment = sw.VerticalAlignment.Center,
			};
			_datePicker.SelectedDateChanged += DatePicker_SelectedDateChanged;

			_timeTextBox = new swc.TextBox
			{
				VerticalAlignment = sw.VerticalAlignment.Center,
				VerticalContentAlignment = sw.VerticalAlignment.Center,
				MinWidth = 70,
			};
			_timeTextBox.LostFocus += TimeTextBox_LostFocus;
			_timeTextBox.KeyDown += TimeTextBox_KeyDown;

			UpdateLayout(DateTimePickerMode.Date);
		}

		void UpdateLayout(DateTimePickerMode mode)
		{
			_grid.Children.Clear();
			_grid.ColumnDefinitions.Clear();

			bool showDate = mode.HasFlag(DateTimePickerMode.Date);
			bool showTime = mode.HasFlag(DateTimePickerMode.Time);

			if (showDate && showTime)
			{
				_grid.ColumnDefinitions.Add(new swc.ColumnDefinition { Width = new sw.GridLength(1, sw.GridUnitType.Star) });
				_grid.ColumnDefinitions.Add(new swc.ColumnDefinition { Width = sw.GridLength.Auto });

				swc.Grid.SetColumn(_datePicker, 0);
				swc.Grid.SetColumn(_timeTextBox, 1);
				_timeTextBox.Margin = new sw.Thickness(2, 0, 0, 0);

				_grid.Children.Add(_datePicker);
				_grid.Children.Add(_timeTextBox);
			}
			else if (showDate)
			{
				_grid.Children.Add(_datePicker);
			}
			else if (showTime)
			{
				_grid.Children.Add(_timeTextBox);
			}

			UpdateTimeText();
			SyncDatePickerFromValue();
		}

		protected override int VisualChildrenCount => 1;

		protected override swm.Visual GetVisualChild(int index)
		{
			if (index != 0)
				throw new ArgumentOutOfRangeException(nameof(index));
			return _grid;
		}

		public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();
			AddVisualChild(_grid);
			AddLogicalChild(_grid);
		}

		protected override void OnInitialized(EventArgs e)
		{
			base.OnInitialized(e);
			AddVisualChild(_grid);
			AddLogicalChild(_grid);
		}

		protected override sw.Size MeasureOverride(sw.Size constraint)
		{
			_grid.Measure(constraint);
			return _grid.DesiredSize;
		}

		protected override sw.Size ArrangeOverride(sw.Size arrangeBounds)
		{
			_grid.Arrange(new sw.Rect(arrangeBounds));
			return arrangeBounds;
		}

		void DatePicker_SelectedDateChanged(object sender, swc.SelectionChangedEventArgs e)
		{
			if (_suppressValueChanged)
				return;

			var date = _datePicker.SelectedDate;
			if (date == null)
			{
				Value = null;
				return;
			}

			var current = Value;
			if (current != null && Mode.HasFlag(DateTimePickerMode.Time))
			{
				// Preserve time component
				Value = date.Value.Date + current.Value.TimeOfDay;
			}
			else
			{
				Value = date.Value;
			}
		}

		void TimeTextBox_LostFocus(object sender, sw.RoutedEventArgs e)
		{
			CommitTimeText();
		}

		void TimeTextBox_KeyDown(object sender, swi.KeyEventArgs e)
		{
			if (e.Key == swi.Key.Enter)
			{
				CommitTimeText();
				e.Handled = true;
			}
		}

		void CommitTimeText()
		{
			if (TimeSpan.TryParse(_timeTextBox.Text, CultureInfo.CurrentUICulture, out var time))
			{
				var current = Value ?? DateTime.Today;
				Value = current.Date + time;
			}
			else
			{
				// Revert to current value
				UpdateTimeText();
			}
		}

		void UpdateTimeText()
		{
			var val = Value;
			if (val != null)
			{
				var format = CultureInfo.CurrentUICulture.DateTimeFormat;
				_timeTextBox.Text = val.Value.ToString(format.LongTimePattern, CultureInfo.CurrentUICulture);
			}
			else
			{
				_timeTextBox.Text = string.Empty;
			}
		}

		void SyncDatePickerFromValue()
		{
			_suppressValueChanged = true;
			try
			{
				_datePicker.SelectedDate = Value?.Date;
				if (Minimum != null)
					_datePicker.DisplayDateStart = Minimum.Value.Date;
				else
					_datePicker.DisplayDateStart = null;
				if (Maximum != null)
					_datePicker.DisplayDateEnd = Maximum.Value.Date;
				else
					_datePicker.DisplayDateEnd = null;
			}
			finally
			{
				_suppressValueChanged = false;
			}
		}

		static void OnValueChanged(sw.DependencyObject d, sw.DependencyPropertyChangedEventArgs e)
		{
			var ctrl = (DateTimePickerControl)d;
			var val = (DateTime?)e.NewValue;

			// Clamp to min/max
			if (val != null)
			{
				if (ctrl.Minimum != null && val < ctrl.Minimum)
					val = ctrl.Minimum;
				if (ctrl.Maximum != null && val > ctrl.Maximum)
					val = ctrl.Maximum;
				if (val != (DateTime?)e.NewValue)
				{
					ctrl.Value = val;
					return;
				}
			}

			ctrl.SyncDatePickerFromValue();
			ctrl.UpdateTimeText();
			ctrl.ValueChanged?.Invoke(ctrl, EventArgs.Empty);
		}

		static void OnMinMaxChanged(sw.DependencyObject d, sw.DependencyPropertyChangedEventArgs e)
		{
			var ctrl = (DateTimePickerControl)d;
			ctrl.SyncDatePickerFromValue();

			// Clamp current value
			var val = ctrl.Value;
			if (val != null)
			{
				if (ctrl.Minimum != null && val < ctrl.Minimum)
					ctrl.Value = ctrl.Minimum;
				else if (ctrl.Maximum != null && val > ctrl.Maximum)
					ctrl.Value = ctrl.Maximum;
			}
		}

		static void OnModeChanged(sw.DependencyObject d, sw.DependencyPropertyChangedEventArgs e)
		{
			var ctrl = (DateTimePickerControl)d;
			ctrl.UpdateLayout((DateTimePickerMode)e.NewValue);
		}

		internal swc.Control FocusableControl
		{
			get
			{
				if (Mode.HasFlag(DateTimePickerMode.Date))
					return _datePicker;
				return _timeTextBox;
			}
		}
	}
}
