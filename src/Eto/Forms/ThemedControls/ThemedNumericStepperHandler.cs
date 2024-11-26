namespace Eto.Forms.ThemedControls;

/// <summary>
/// Themed implementation of the <see cref="NumericStepper"/> control using a <see cref="NumericMaskedTextStepper{T}"/>.
/// </summary>
public class ThemedNumericStepperHandler : ThemedControlHandler<NumericMaskedTextStepper<double>, NumericStepper, NumericStepper.ICallback>, NumericStepper.IHandler
{
	double _increment = 1;
	double _minValue = double.MinValue;
	double _maxValue = double.MaxValue;
	int _decimalPlaces;
	int _maximumDecimalPlaces;
	string _formatString;
	bool _wrap;
	double _lastValue;

	/// <summary>
	/// Initializes a new instance of the <see cref="ThemedNumericStepperHandler"/> class.
	/// </summary>
	public ThemedNumericStepperHandler()
	{
		Control = new NumericMaskedTextStepper<double>();
		Control.AllowSign = true;
		Control.AllowDecimal = true;
		UpdateFormatString();
	}

	/// <inheritdoc/>
	protected override void Initialize()
	{
		base.Initialize();
		Control.Step += Control_Step;
		Control.KeyDown += Control_KeyDown;
		Control.ValueChanged += Control_ValueChanged;
	}

	void Control_ValueChanged(object sender, EventArgs e)
	{
		var val = Value;
		if (_lastValue != val)
		{
			_lastValue = val;
			Callback.OnValueChanged(Widget, EventArgs.Empty);
		}
		UpdateValidDirection();
	}

	void Control_Step(object sender, StepperEventArgs e)
	{
		var val = Value;
		if (e.Direction == StepperDirection.Up)
			val += _increment;
		else
			val -= _increment;

		if (_wrap)
		{
			if (val < _minValue)
				val = _maxValue;
			else if (val > _maxValue)
				val = _minValue;
		}

		Value = Math.Max(_minValue, Math.Min(_maxValue, val));
	}

	void Control_KeyDown(object sender, KeyEventArgs e)
	{
		if (e.KeyData == Keys.Up)
		{
			Control_Step(sender, new StepperEventArgs(StepperDirection.Up));
			e.Handled = true;
		}
		else if (e.KeyData == Keys.Down)
		{
			Control_Step(sender, new StepperEventArgs(StepperDirection.Down));
			e.Handled = true;
		}
	}

	/// <inheritdoc/>
	public override void AttachEvent(string id)
	{
		switch (id)
		{
			default:
				base.AttachEvent(id);
				break;
		}
	}

	/// <inheritdoc/>
	public bool ReadOnly
	{
		get => Control.ReadOnly;
		set => Control.ReadOnly = value;
	}

	/// <inheritdoc/>
	public double Value
	{
		get
		{
			var val = Control.Value;
			if (!HasFormatString)
				val = Math.Round(val, _maximumDecimalPlaces);
			return Math.Max(_minValue, Math.Min(_maxValue, val));
		}
		set
		{
			Control.Value = Math.Max(_minValue, Math.Min(_maxValue, value));
		}
	}

	/// <inheritdoc/>
	public double MinValue
	{
		get => _minValue;
		set
		{
			_minValue = value;
			Control.AllowSign = value < 0 || _maxValue < 0;
			if (Value < value)
				Value = value;
			UpdateValidDirection();
		}
	}

	/// <inheritdoc/>
	public double MaxValue
	{
		get => _maxValue;
		set
		{
			_maxValue = value;
			Control.AllowSign = _minValue < 0 || value < 0;
			if (Value > value)
				Value = value;
			UpdateValidDirection();
		}
	}

	void UpdateValidDirection()
	{
		var dir = StepperValidDirections.None;
		var val = Value;
		if (_wrap || val < _maxValue)
			dir |= StepperValidDirections.Up;
		if (_wrap || val > _minValue)
			dir |= StepperValidDirections.Down;
		Control.ValidDirection = dir;
	}

	/// <inheritdoc/>
	public int DecimalPlaces
	{
		get => _decimalPlaces;
		set
		{
			_decimalPlaces = value;
			if (_maximumDecimalPlaces < value)
				_maximumDecimalPlaces = value;
			UpdateFormatString();
		}
	}

	/// <inheritdoc/>
	public double Increment
	{
		get => _increment;
		set => _increment = value;
	}

	/// <inheritdoc/>
	public Color TextColor
	{
		get => Control.TextColor;
		set => Control.TextColor = value;
	}

	/// <inheritdoc/>
	public int MaximumDecimalPlaces
	{
		get => _maximumDecimalPlaces;
		set
		{
			_maximumDecimalPlaces = value;
			if (_decimalPlaces > value)
				_decimalPlaces = value;
			UpdateFormatString();
		}
	}

	bool HasFormatString => !string.IsNullOrEmpty(_formatString);

	/// <inheritdoc/>
	public string FormatString
	{
		get => _formatString;
		set
		{
			_formatString = value;
			UpdateFormatString();
		}
	}

	/// <inheritdoc/>
	public CultureInfo CultureInfo
	{
		get => Control.Culture;
		set => Control.Culture = value;
	}

	/// <inheritdoc/>
	public bool Wrap
	{
		get => _wrap;
		set
		{
			_wrap = value;
			UpdateValidDirection();
		}
	}

	/// <inheritdoc/>
	public Font Font
	{
		get => Control.Font;
		set => Control.Font = value;
	}

	void UpdateFormatString()
	{
		Control.AllowDecimal = HasFormatString || _maximumDecimalPlaces > 0 || _decimalPlaces > 0;
	}

	/// <inheritdoc/>
	protected override Control KeyboardControl => Control;

	/// <inheritdoc/>
	public override void Focus() => Control.Focus();

	/// <inheritdoc/>
	public override bool HasFocus => base.HasFocus || Control.HasFocus;
}
