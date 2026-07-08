using System.Numerics;

namespace Eto.Forms;

/// <summary>
/// Masked text box with a variable length numeric mask.
/// </summary>
/// <remarks>
/// This provides a text box that limits the user input to only allow numeric values.
/// </remarks>
/// <typeparam name="T">Numeric type such as int, decimal, double, etc.</typeparam>
public class NumericMaskedTextStepper<T> : MaskedTextStepper<T>
	where T: struct, IComparable<T>
#if NET7_0_OR_GREATER
	, INumber<T>
#endif
{
	T _increment;
	T _minValue;
	T _maxValue;
	bool _wrap;
	T? _value;
	Func<object, int, object> _roundFunc;

	/// <summary>
	/// Gets the numeric provider.
	/// </summary>
	/// <value>The masked text provider.</value>
	public new NumericMaskedTextProvider<T> Provider => (NumericMaskedTextProvider<T>)base.Provider;

	/// <summary>
	/// Gets or sets a value indicating whether the mask can accept a sign.
	/// </summary>
	/// <remarks>
	/// This defaults to whether the type specified by <typeparamref name="T"/> allows negative values.
	/// </remarks>
	/// <value><c>true</c> to allow sign character; otherwise, <c>false</c>.</value>
	public bool AllowSign
	{
		get { return Provider.AllowSign; }
		set { Provider.AllowSign = value; }
	}

	/// <summary>
	/// Gets or sets a value indicating whether the mask can input a decimal.
	/// </summary>
	/// <remarks>
	/// This defaults to whether the type specified by <typeparamref name="T"/> allows decimals, such as when
	/// it is a decimal, double, or float.
	/// </remarks>
	/// <value><c>true</c> to allow decimal; otherwise, <c>false</c>.</value>
	public bool AllowDecimal
	{
		get { return Provider.AllowDecimal; }
		set { Provider.AllowDecimal = value; }
	}
	
	/// <summary>
	/// Gets or sets the culture for the <see cref="NumericMaskedTextProvider.DecimalCharacter"/> and <see cref="NumericMaskedTextProvider.SignCharacters"/> formatting characters.
	/// </summary>
	public CultureInfo Culture
	{
		get => Provider.Culture;
		set
		{
			Provider.Culture = value;
			UpdateText();
		}
	}

	/// <summary>
	/// Gets or sets the number of decimal places to allow.  This will round the value to the specified number of decimal places when set.
	/// </summary>
	public int DecimalPlaces
	{
		get => Provider.DecimalPlaces;
		set
		{
			if (value == Provider.DecimalPlaces)
				return;
			var oldValue = _value;
			Provider.DecimalPlaces = value;
			Provider.AllowDecimal = value > 0 || MaximumDecimalPlaces > 0;
			if (oldValue.HasValue)
				Provider.Value = oldValue.Value;
			UpdateText();
			if (oldValue.HasValue)
				_value = oldValue.Value;
		}
	}
	
	/// <summary>
	/// Gets or sets the maximum number of decimal places allowed.  This will round the value to the specified number of decimal places when set.
	/// </summary>
	public int MaximumDecimalPlaces
	{
		get => Provider.MaximumDecimalPlaces;
		set
		{
			if (value == Provider.MaximumDecimalPlaces)
				return;
			var oldValue = _value;
			Provider.MaximumDecimalPlaces = value;
			Provider.AllowDecimal = DecimalPlaces > 0 || value > 0;
			if (oldValue.HasValue)
				Provider.Value = oldValue.Value;
			UpdateText();
			if (oldValue.HasValue)
				_value = oldValue.Value;
		}
	}
	
	/// <summary>
	/// Gets or sets the format string for the numeric value.
	/// </summary>
	public string FormatString
	{
		get => Provider.FormatString;
		set
		{
			if (value == Provider.FormatString)
				return;
			var oldValue = _value;
			Provider.FormatString = value;
			if (oldValue.HasValue)
				Provider.Value = oldValue.Value;
			UpdateText();
			if (oldValue.HasValue)
				_value = oldValue.Value;
		}
	}
	
	/// <inheritdoc/>
	public T MinValue
	{
		get => _minValue;
		set
		{
			_minValue = value;
			AllowSign = value.CompareTo(default(T)) < 0 || _maxValue.CompareTo(default(T)) < 0;
			if (base.Value.CompareTo(value) < 0)
				base.Value = value;
			UpdateValidDirection();
		}
	}

	/// <summary>
	/// Gets or sets the maximum value allowed.  This will set the value to the maximum if the current value is greater than the new maximum.
	/// </summary>
	public T MaxValue
	{
		get => _maxValue;
		set
		{
			_maxValue = value;
			AllowSign = _minValue.CompareTo(default(T)) < 0 || value.CompareTo(default(T)) < 0;
			if (base.Value.CompareTo(value) > 0)
				base.Value = value;
			UpdateValidDirection();
		}
	}
	
	/// <summary>
	/// Gets or sets the increment value for stepping.  This will be added or subtracted from the current value when the stepper buttons are clicked.
	/// </summary>
	public T Increment
	{
		get => _increment;
		set
		{
			_increment = value;
			UpdateValidDirection();
		}
	}
	
	/// <summary>
	/// Gets or sets whether the value should wrap when incrementing or decrementing past the minimum or maximum values.
	/// </summary>
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
	public override T Value
	{
		get
		{
			var val = _value ?? base.Value;
			
			if (string.IsNullOrEmpty(FormatString) && MaximumDecimalPlaces > 0 && _roundFunc != null)
				val = (T)_roundFunc(val, MaximumDecimalPlaces);
			
			if (val.CompareTo(_minValue) < 0) return _minValue;
			if (val.CompareTo(_maxValue) > 0) return _maxValue;
			return val;
		}
		set
		{
			if (EqualityComparer<T>.Default.Equals(value, Value))
				return;
			var clamped = value;
			if (clamped.CompareTo(_minValue) < 0) clamped = _minValue;
			if (clamped.CompareTo(_maxValue) > 0) clamped = _maxValue;
			base.Value = clamped;
			_value = value;
		}
	}


	/// <summary>
	/// Initializes a new instance of the <see cref="Forms.NumericMaskedTextStepper{T}"/> class.
	/// </summary>
	public NumericMaskedTextStepper()
		: base(new NumericMaskedTextProvider<T>())
	{
		HandleEvent(StepEvent);
		HandleEvent(KeyDownEvent);
		HandleEvent(LostFocusEvent);
		
		var type = typeof(T);
		if (TypeDefaults.TryGetValue(type, out var defaults))
		{
			_minValue = (T)defaults.min;
			_maxValue = (T)defaults.max;
			_increment = (T)defaults.increment;
			_roundFunc = (Func<object, int, object>)defaults.roundFunc;
		}
		else
		{
			throw new NotSupportedException($"The type {type} is not supported by {nameof(NumericMaskedTextStepper<T>)}");
		}
		Provider.Value = default;
	}

	Dictionary<Type, (object min, object max, object increment, int maxdecimals, Func<object, int, object> roundFunc)> TypeDefaults = new ()
	{
		{ typeof(byte), (byte.MinValue, byte.MaxValue, (byte)1, 0, null) },
		{ typeof(sbyte), (sbyte.MinValue, sbyte.MaxValue, (sbyte)1, 0, null) },
		{ typeof(short), (short.MinValue, short.MaxValue, (short)1, 0, null) },
		{ typeof(ushort), (ushort.MinValue, ushort.MaxValue, (ushort)1, 0, null) },
		{ typeof(int), (int.MinValue, int.MaxValue, 1, 0, null) },
		{ typeof(uint), (uint.MinValue, uint.MaxValue, 1u, 0, null) },
		{ typeof(long), (long.MinValue, long.MaxValue, 1L, 0, null) },
		{ typeof(ulong), (ulong.MinValue, ulong.MaxValue, 1UL, 0, null) },
		{ typeof(float), (float.MinValue, float.MaxValue, 1f, 7, (v, i) => Math.Round((float)v, Math.Min(i, 7))) },
		{ typeof(double), (double.MinValue, double.MaxValue, 1d, 15, (v, i) => Math.Round((double)v, Math.Min(i, 15))) },
		{ typeof(decimal), (decimal.MinValue, decimal.MaxValue, 1m, 28, (v, i) => Math.Round((decimal)v, Math.Min(i, 28))) }
	};

	void UpdateValidDirection()
	{
		if (ReadOnly)
		{
			ValidDirection = StepperValidDirections.None;
			return;
		}
		var dir = StepperValidDirections.None;
		var val = Value;
		if (_wrap || val.CompareTo(_maxValue) < 0)
			dir |= StepperValidDirections.Up;
		if (_wrap || val.CompareTo(_minValue) > 0)
			dir |= StepperValidDirections.Down;
		ValidDirection = dir;
	}

	/// <inheritdoc/>
	protected override void OnTextChanged(EventArgs e)
	{
		_value = null;
		base.OnTextChanged(e);
	}

	/// <inheritdoc/>
	protected override void OnValueChanged(EventArgs e)
	{
		base.OnValueChanged(e);
		UpdateValidDirection();
	}

	/// <inheritdoc/>
	protected override void OnKeyDown(KeyEventArgs e)
	{
		base.OnKeyDown(e);
		if (e.Handled || ReadOnly || !Enabled)
			return;
		switch (e.KeyData)
		{
			case Keys.Decimal:
				if (!AllowDecimal)
					return;
				var pos = CaretIndex;
				Provider.Insert(Provider.DecimalCharacter, ref pos);
				UpdateText();
				CaretIndex = pos;
				e.Handled = true;
				break;
			case Keys.Enter:
				UpdateValue();
				e.Handled = true;
				break;
		}
	}

	/// <inheritdoc/>
	protected override void OnLostFocus(EventArgs e)
	{
		base.OnLostFocus(e);
		UpdateValue();
	}
	
	void UpdateValue()
	{
		if (Provider != null)
		{
			var val = Value;
			Provider.Value = val.CompareTo(_minValue) < 0 ? _minValue : val.CompareTo(_maxValue) > 0 ? _maxValue : val;
			if (HasFocus)
				Provider.CommitText();
		}
		UpdateText();
	}

	/// <inheritdoc/>
	protected override void OnStep(StepperEventArgs e)
	{
		base.OnStep(e);
		if (ReadOnly || !Enabled)
			return;
		var val = _value ?? Value;
		if (e.Direction == StepperDirection.Up)
			val = Add(val, _increment);
		else
			val = Subtract(val, _increment);

		if (_wrap)
		{
			if (val.CompareTo(_minValue) < 0)
				val = _maxValue;
			else if (val.CompareTo(_maxValue) > 0)
				val = _minValue;
		}

		Value = val.CompareTo(_minValue) < 0 ? _minValue : val.CompareTo(_maxValue) > 0 ? _maxValue : val;
	}

	/// <inheritdoc/>
	protected override void OnReadOnlyChanged(EventArgs e)
	{
		base.OnReadOnlyChanged(e);
		UpdateValidDirection();
	}

#if !NET7_0_OR_GREATER
	static Delegate GetAddDelegate(Type type)
	{
		// 1. Create expression parameters targeting those types
		ParameterExpression paramA = Expression.Parameter(type, "a");
		ParameterExpression paramB = Expression.Parameter(type, "b");

		// 2. Generate the mathematical Add expression
		BinaryExpression addExpression = Expression.Add(paramA, paramB);

		// 3. Compile the expression into a reusable delegate execution block
		var lambda = Expression.Lambda(addExpression, paramA, paramB);
		var compiledDelegate = lambda.Compile();
		return compiledDelegate;
	}
	
	static Delegate GetSubtractDelegate(Type type)
	{
		// 1. Create expression parameters targeting those types
		ParameterExpression paramA = Expression.Parameter(type, "a");
		ParameterExpression paramB = Expression.Parameter(type, "b");
	
		// 2. Generate the mathematical Subtract expression
		BinaryExpression subtractExpression = Expression.Subtract(paramA, paramB);

		// 3. Compile the expression into a reusable delegate execution block
		var lambda = Expression.Lambda(subtractExpression, paramA, paramB);
		var compiledDelegate = lambda.Compile();
		return compiledDelegate;
	} 

	static Delegate _addDelegate = GetAddDelegate(typeof(T));
	static Delegate _subtractDelegate = GetSubtractDelegate(typeof(T));
#endif

	private T Add(T val, T increment)
	{
#if NET7_0_OR_GREATER
		return val + increment;
#else
		return _addDelegate != null ? (T)_addDelegate.DynamicInvoke(val, increment) : throw new InvalidOperationException($"The type {typeof(T)} does not support addition operator.");
#endif
		
	}

	private T Subtract(T val, T increment)
	{
#if NET7_0_OR_GREATER
		return val - increment;
#else
		return _subtractDelegate != null ? (T)_subtractDelegate.DynamicInvoke(val, increment) : throw new InvalidOperationException($"The type {typeof(T)} does not support subtraction operator.");
#endif
	}
}
