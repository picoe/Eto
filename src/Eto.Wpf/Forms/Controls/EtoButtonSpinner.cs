using System;

namespace Eto.Wpf.Forms.Controls
{
	public enum SpinDirection
	{
		Increase,
		Decrease
	}

	[Flags]
	public enum ValidSpinDirections
	{
		None = 0,
		Increase = 1,
		Decrease = 2
	}

	public enum ButtonSpinnerLocation
	{
		Left,
		Right
	}

	public enum MouseWheelActiveTrigger
	{
		Focused,
		MouseOver,
		FocusedMouseOver
	}

	public class SpinEventArgs : EventArgs
	{
		public SpinEventArgs(SpinDirection direction, bool usingMouseWheel)
		{
			Direction = direction;
			UsingMouseWheel = usingMouseWheel;
		}

		public SpinDirection Direction { get; }

		public bool UsingMouseWheel { get; }

		public bool Handled { get; set; }
	}

	public class EtoButtonSpinner : swc.ContentControl, IEtoWpfControl
	{
		swcp.RepeatButton increaseButton;
		swcp.RepeatButton decreaseButton;

		public IWpfFrameworkElement Handler { get; set; }

		
		public EtoButtonSpinner()
		{
			IsEnabledChanged += (sender, e) => UpdateSpinButtonStates();
		}


		protected override sw.Size MeasureOverride(sw.Size constraint)
		{
			return Handler?.MeasureOverride(constraint, base.MeasureOverride) ?? base.MeasureOverride(constraint);
		}

		public static readonly sw.DependencyProperty ValidSpinDirectionProperty = sw.DependencyProperty.Register(
			nameof(ValidSpinDirection),
			typeof(ValidSpinDirections),
			typeof(EtoButtonSpinner),
			new sw.FrameworkPropertyMetadata(ValidSpinDirections.Increase | ValidSpinDirections.Decrease, OnSpinPropertyChanged));

		public static readonly sw.DependencyProperty ShowButtonSpinnerProperty = sw.DependencyProperty.Register(
			nameof(ShowButtonSpinner),
			typeof(bool),
			typeof(EtoButtonSpinner),
			new sw.FrameworkPropertyMetadata(true, OnSpinPropertyChanged));

		public static readonly sw.DependencyProperty ButtonSpinnerLocationProperty = sw.DependencyProperty.Register(
			nameof(ButtonSpinnerLocation),
			typeof(ButtonSpinnerLocation),
			typeof(EtoButtonSpinner),
			new sw.FrameworkPropertyMetadata(ButtonSpinnerLocation.Right));

		public static readonly sw.DependencyProperty MouseWheelActiveTriggerProperty = sw.DependencyProperty.Register(
			nameof(MouseWheelActiveTrigger),
			typeof(MouseWheelActiveTrigger),
			typeof(EtoButtonSpinner),
			new sw.FrameworkPropertyMetadata(MouseWheelActiveTrigger.FocusedMouseOver));

		public static readonly sw.DependencyProperty ShowContentAreaProperty = sw.DependencyProperty.Register(
			nameof(ShowContentArea),
			typeof(bool),
			typeof(EtoButtonSpinner),
			new sw.FrameworkPropertyMetadata(true));

		public ValidSpinDirections ValidSpinDirection
		{
			get => (ValidSpinDirections)GetValue(ValidSpinDirectionProperty);
			set => SetValue(ValidSpinDirectionProperty, value);
		}

		public bool ShowButtonSpinner
		{
			get => (bool)GetValue(ShowButtonSpinnerProperty);
			set => SetValue(ShowButtonSpinnerProperty, value);
		}

		public ButtonSpinnerLocation ButtonSpinnerLocation
		{
			get => (ButtonSpinnerLocation)GetValue(ButtonSpinnerLocationProperty);
			set => SetValue(ButtonSpinnerLocationProperty, value);
		}

		public MouseWheelActiveTrigger MouseWheelActiveTrigger
		{
			get => (MouseWheelActiveTrigger)GetValue(MouseWheelActiveTriggerProperty);
			set => SetValue(MouseWheelActiveTriggerProperty, value);
		}

		public bool ShowContentArea
		{
			get => (bool)GetValue(ShowContentAreaProperty);
			set => SetValue(ShowContentAreaProperty, value);
		}

		public event EventHandler<SpinEventArgs> Spin;

		public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();

			if (increaseButton != null)
			{
				increaseButton.Click -= IncreaseButton_Click;
			}
			if (decreaseButton != null)
			{
				decreaseButton.Click -= DecreaseButton_Click;
			}

			increaseButton = GetTemplateChild("PART_IncreaseButton") as swcp.RepeatButton;
			decreaseButton = GetTemplateChild("PART_DecreaseButton") as swcp.RepeatButton;

			if (increaseButton != null)
			{
				increaseButton.Click += IncreaseButton_Click;
			}
			if (decreaseButton != null)
			{
				decreaseButton.Click += DecreaseButton_Click;
			}

			UpdateSpinButtonStates();
		}

		void IncreaseButton_Click(object sender, sw.RoutedEventArgs e)
		{
			RaiseSpin(SpinDirection.Increase, false);
		}

		void DecreaseButton_Click(object sender, sw.RoutedEventArgs e)
		{
			RaiseSpin(SpinDirection.Decrease, false);
		}

		protected override void OnPreviewKeyDown(swi.KeyEventArgs e)
		{
			base.OnPreviewKeyDown(e);
			if (e.Handled)
				return;

			if (e.Key == swi.Key.Up)
				e.Handled = RaiseSpin(SpinDirection.Increase, false);
			else if (e.Key == swi.Key.Down)
				e.Handled = RaiseSpin(SpinDirection.Decrease, false);
		}

		protected override void OnMouseWheel(swi.MouseWheelEventArgs e)
		{
			base.OnMouseWheel(e);
			if (e.Handled || e.Delta == 0 || !CanSpinUsingMouseWheel())
				return;

			e.Handled = RaiseSpin(e.Delta > 0 ? SpinDirection.Increase : SpinDirection.Decrease, true);
		}
		protected virtual void OnSpin(SpinEventArgs e)
		{
			Spin?.Invoke(this, e);
		}

		bool RaiseSpin(SpinDirection direction, bool usingMouseWheel)
		{
			if (!CanSpin(direction))
				return false;

			var e = new SpinEventArgs(direction, usingMouseWheel);
			OnSpin(e);
			return e.Handled;
		}

		bool CanSpin(SpinDirection direction)
		{
			if (!IsEnabled || !ShowButtonSpinner)
				return false;

			if (direction == SpinDirection.Increase)
				return ValidSpinDirection.HasFlag(ValidSpinDirections.Increase);
			return ValidSpinDirection.HasFlag(ValidSpinDirections.Decrease);
		}

		bool CanSpinUsingMouseWheel()
		{
			var textBox = Content as swc.TextBox;
			var hasFocus = textBox?.IsKeyboardFocusWithin == true || IsKeyboardFocusWithin;
			switch (MouseWheelActiveTrigger)
			{
				case MouseWheelActiveTrigger.MouseOver:
					return IsMouseOver;
				case MouseWheelActiveTrigger.Focused:
					return hasFocus;
				default:
					return IsMouseOver && hasFocus;
			}
		}

		void UpdateSpinButtonStates()
		{
			if (increaseButton != null)
				increaseButton.IsEnabled = IsEnabled && ShowButtonSpinner && ValidSpinDirection.HasFlag(ValidSpinDirections.Increase);
			if (decreaseButton != null)
				decreaseButton.IsEnabled = IsEnabled && ShowButtonSpinner && ValidSpinDirection.HasFlag(ValidSpinDirections.Decrease);
		}

		static void OnSpinPropertyChanged(sw.DependencyObject d, sw.DependencyPropertyChangedEventArgs e)
		{
			((EtoButtonSpinner)d).UpdateSpinButtonStates();
		}
	}
}


