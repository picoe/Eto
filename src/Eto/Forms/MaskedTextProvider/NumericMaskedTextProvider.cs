namespace Eto.Forms;

/// <summary>
/// Masked text provider for numeric input of the specified type.
/// </summary>
public class NumericMaskedTextProvider<T> : NumericMaskedTextProvider, IMaskedTextProvider<T>
{
	Func<string, T> _parse;
	Func<T, string> _toString;
	string _formatString;
	int _decimalPlaces;
	int _maximumDecimalPlaces;

	class Info
	{
		public bool AllowSign;
		public bool AllowDecimal;
		public Func<string, object> Parse;
		public Func<object, string> ToText;
		public int MaxDecimalPlaces;
	}

	// do all conversions with invariant culture
	static CultureInfo Inv => CultureInfo.InvariantCulture;

	// use dictionary instead of reflection for linking
	static readonly Dictionary<Type, Info> numericTypes = new Dictionary<Type, Info>
	{
		{ typeof(decimal), new Info { Parse = s => decimal.TryParse(s, NumberStyles.Any, Inv, out var d) ? (object)d : null, AllowSign = true, AllowDecimal = true, MaxDecimalPlaces = 28 } },
		{ typeof(double), new Info { Parse = s => double.TryParse(s, NumberStyles.Any, Inv, out var d) ? (object)d : null, ToText = DoubleToText, AllowSign = true, AllowDecimal = true, MaxDecimalPlaces = 17 } },
		{ typeof(float), new Info { Parse = s => float.TryParse(s, NumberStyles.Any, Inv, out var d) ? (object)d : null, ToText = FloatToText, AllowSign = true, AllowDecimal = true, MaxDecimalPlaces = 8 } },
		{ typeof(int), new Info { Parse = s => int.TryParse(s, NumberStyles.Any, Inv, out var d) ? (object)d : null, AllowSign = true } },
		{ typeof(uint), new Info { Parse = s => uint.TryParse(s, NumberStyles.Any, Inv, out var d) ? (object)d : null } },
		{ typeof(long), new Info { Parse = s => long.TryParse(s, NumberStyles.Any, Inv, out var d) ? (object)d : null, AllowSign = true } },
		{ typeof(ulong), new Info { Parse = s => ulong.TryParse(s, NumberStyles.Any, Inv, out var d) ? (object)d : null } },
		{ typeof(short), new Info { Parse = s => short.TryParse(s, NumberStyles.Any, Inv, out var d) ? (object)d : null, AllowSign = true } },
		{ typeof(ushort), new Info { Parse = s => ushort.TryParse(s, NumberStyles.Any, Inv, out var d) ? (object)d : null } },
		{ typeof(byte), new Info { Parse = s => byte.TryParse(s, NumberStyles.Any, Inv, out var d) ? (object)d : null } },
		{ typeof(sbyte), new Info { Parse = s => sbyte.TryParse(s, NumberStyles.Any, Inv, out var d) ? (object)d : null, AllowSign = true } }
	};

	static string DoubleToText(object v)
	{
		var d = (double?)v;
		if (d == null) return string.Empty;
		// Ensure we don't have imprecise values with too many decimals
		var str = d.Value.ToString("G15", Inv);
		// Also ensure we don't lose precision with scientific notation
		if (str.IndexOfAny(new[] { 'E', 'e' }) >= 0 && double.TryParse(str, NumberStyles.Any, Inv, out var parsed))
			return parsed.ToString("F99", Inv).TrimEnd('0').TrimEnd('.');
		return str;
	}
	static string FloatToText(object v)
	{
		var f = (float?)v;
		if (f == null) return string.Empty;
		// Ensure we don't have imprecise values with too many decimals
		var str = f.Value.ToString("G7", Inv);
		// Also ensure we don't lose precision with scientific notation
		if (str.IndexOfAny(new[] { 'E', 'e' }) >= 0 && float.TryParse(str, NumberStyles.Any, Inv, out var parsed))
			return parsed.ToString("F99", Inv).TrimEnd('0').TrimEnd('.');
		return str;
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="Eto.Forms.NumericMaskedTextProvider{T}"/> class.
	/// </summary>
	public NumericMaskedTextProvider()
	{
		var type = typeof(T);
		var underlyingType = Nullable.GetUnderlyingType(type) ?? type;
		if (numericTypes.TryGetValue(underlyingType, out Info info))
		{
			AllowSign = info.AllowSign;
			AllowDecimal = info.AllowDecimal;
			MaximumDecimalPlaces = info.MaxDecimalPlaces;
			_parse = text =>
			{
				var val = info.Parse(text);
				return val == null ? default : (T)val;
			};

			if (info.ToText != null)
				_toString = val => info.ToText(val);
			else
				_toString = val => Convert.ToString(val, CultureInfo.InvariantCulture);
			Validate = text => info.Parse(text?.Replace(DecimalCharacter, '.')) != null;
		}
		else
		{
			// use reflection for other types
			AllowSign = Convert.ToBoolean(underlyingType.GetRuntimeField("MinValue").GetValue(null));
			AllowDecimal = underlyingType == typeof(decimal) || underlyingType == typeof(double) || underlyingType == typeof(float);

			var tryParseMethod = underlyingType.GetRuntimeMethod("TryParse", new[] { typeof(string), underlyingType.MakeByRefType() });
			if (tryParseMethod == null || tryParseMethod.ReturnType != typeof(bool))
				throw new ArgumentException(string.Format("Type of T ({0}) must implement a static bool TryParse(string, out T) method", typeof(T)));

			_parse = text =>
			{
				var parameters = new object[] { Text, null };
				if ((bool)tryParseMethod.Invoke(null, parameters))
				{
					return (T)parameters[1];
				}
				return default;
			};
			_toString = val => Convert.ToString(val, CultureInfo.InvariantCulture);

			Validate = text =>
			{
				var parameters = new object[] { text?.Replace(DecimalCharacter, '.'), null };
				return (bool)tryParseMethod.Invoke(null, parameters);
			};
		}
	}

	/// <summary>
	/// Gets or sets the translated value of the mask.
	/// </summary>
	/// <value>The value of the mask.</value>
	public T Value
	{
		get => _parse(Text?.Replace(DecimalCharacter, '.'));
		set
		{
			Text = _toString(value)?.Replace('.', DecimalCharacter);
			CommitText();
		}
	}
	
	/// <summary>
	/// Gets or sets the minimum number of decimal places to include in the output.
	/// </summary>
	public int DecimalPlaces
	{
		get => _decimalPlaces;
		set
		{
			value = Math.Max(0, value);
			if (_decimalPlaces == value)
				return;
			_decimalPlaces = value;
			if (_maximumDecimalPlaces < value)
				_maximumDecimalPlaces = value;
			_formatString = null; // reset format string since it will override decimal places
			CommitText();
		}
	}

	/// <summary>
	/// Gets or sets the maximum number of decimal places to include in the output.
	/// </summary>
	public int MaximumDecimalPlaces
	{
		get => _maximumDecimalPlaces;
		set
		{
			value = Math.Max(0, value);
			if (_maximumDecimalPlaces == value)
				return;
			_maximumDecimalPlaces = value;
			if (_decimalPlaces > value)
				_decimalPlaces = value;
			_formatString = null; // reset format string since it will override decimal places
			CommitText();
		}
	}
	
	/// <summary>
	/// Gets or sets the format string to use for formatting the value. 
	/// If specified, this will override the <see cref="DecimalPlaces"/> and <see cref="MaximumDecimalPlaces"/> properties.
	/// </summary>
	public string FormatString
	{
		get => _formatString;
		set
		{
			if (_formatString == value)
				return;
			_formatString = value;
			if (!string.IsNullOrEmpty(value))
			{
				_decimalPlaces = 0;
				_maximumDecimalPlaces = 0;
			}
			CommitText();
		}
	}
	

	internal override void SetCulture()
	{
		var value = Value;
		base.SetCulture();
		Value = value;
	}

	/// <inheritdoc/>
	public override void CommitText()
	{
		if (TryFormatText(base.Text, out var displayText))
			SetBuilderText(displayText);
	}

	void SetBuilderText(string text)
	{
		Builder.Clear();
		if (text != null)
			Builder.Append(text);
	}

	bool TryFormatText(string text, out string displayText)
	{
		displayText = text ?? string.Empty;
		if (string.IsNullOrEmpty(text))
			return true;

		var normalizedInput = text.Replace(DecimalCharacter, '.');
		if (normalizedInput.Length == 1)
		{
			if ((AllowSign && SignCharacters.Contains(normalizedInput[0])) || (AllowDecimal && normalizedInput[0] == '.'))
				return false;
		}
		else if (normalizedInput.Length == 2 && AllowSign && AllowDecimal && SignCharacters.Contains(normalizedInput[0]) && normalizedInput[1] == '.')
		{
			return false;
		}

		// if (Validate != null && !Validate(text))
		// 	return false;

		var value = _parse(normalizedInput);
		if (value is IFormattable formattable)
		{
			var maximumDecimalPlaces = Math.Max(DecimalPlaces, MaximumDecimalPlaces);
			var displayFormat = CreateNumberFormat(DecimalPlaces, maximumDecimalPlaces);
			displayText = formattable.ToString(displayFormat, Inv).Replace('.', DecimalCharacter);
			return true;
		}

		displayText = _toString(value)?.Replace('.', DecimalCharacter) ?? string.Empty;
		return true;
	}

	string CreateNumberFormat(int decimalPlaces, int maximumDecimalPlaces)
	{
		if (!string.IsNullOrEmpty(_formatString))
			return _formatString;
		if (maximumDecimalPlaces == int.MaxValue)
			return "G";
		if (maximumDecimalPlaces <= 0)
			return "0";
		if (decimalPlaces <= 0)
			return $"0.{new string('#', maximumDecimalPlaces)}";
		if (maximumDecimalPlaces <= decimalPlaces)
			return $"0.{new string('0', decimalPlaces)}";
		return $"0.{new string('0', decimalPlaces)}{new string('#', maximumDecimalPlaces - decimalPlaces)}";
	}
}

/// <summary>
/// Masked text provider for numeric input.
/// </summary>
public class NumericMaskedTextProvider : VariableMaskedTextProvider
{
	CultureInfo _culture = CultureInfo.CurrentCulture;

	/// <summary>
	/// Gets or sets a value indicating that the mask can optionally include a decimal, as specified by the <see cref="DecimalCharacter"/>.
	/// </summary>
	/// <value><c>true</c> to allow the decimal; otherwise, <c>false</c>.</value>
	public bool AllowDecimal { get; set; }

	/// <summary>
	/// Gets or sets a value indicating that the mask can optionally include the sign, as specified by <see cref="SignCharacters"/>.
	/// </summary>
	/// <value><c>true</c> to allow a sign character; otherwise, <c>false</c>.</value>
	public bool AllowSign { get; set; }

	/// <summary>
	/// Gets or sets the sign characters when <see cref="AllowSign"/> is <c>true</c>. Default is '+' and '-'.
	/// </summary>
	/// <value>The sign characters.</value>
	public char[] SignCharacters { get; set; }

	/// <summary>
	/// Gets or sets a delegate used to validate the mask.
	/// </summary>
	/// <value>The validation delegate.</value>
	public Func<string, bool> Validate { get; set; }

	/// <summary>
	/// Gets or sets the decimal character when <see cref="AllowDecimal"/> is <c>true</c>. Default is '.'.
	/// </summary>
	/// <value>The decimal character.</value>
	[DefaultValue('.')]
	public char DecimalCharacter { get; set; }

	/// <summary>
	/// Gets or sets the alternate decimal character that can be accepted.
	/// </summary>
	/// <remarks>
	/// This is useful when the DecimalCharacter is localized but you still want to allow alternate characters
	/// </remarks>
	public char[] AltDecimalCharacters { get; set; }

	/// <summary>
	/// Initializes a new instance of the <see cref="Eto.Forms.NumericMaskedTextProvider"/> class.
	/// </summary>
	public NumericMaskedTextProvider()
	{
		SetCultureInternal();
	}

	/// <summary>
	/// Gets or sets the culture of the <see cref="DecimalCharacter"/> and <see cref="SignCharacters"/> formatting characters.
	/// </summary>
	public CultureInfo Culture
	{
		get => _culture;
		set
		{
			_culture = value ?? throw new ArgumentNullException(nameof(value));
			SetCulture();
		}
	}

	internal virtual void SetCulture() => SetCultureInternal();

	void SetCultureInternal()
	{
		var format = _culture.NumberFormat;
		// note: we do not support formats with multiple-characters
		DecimalCharacter = format.NumberDecimalSeparator[0];
		SignCharacters = new[] { format.PositiveSign[0], format.NegativeSign[0] };
		if (DecimalCharacter != '.')
			AltDecimalCharacters = new[] { '.' };
		else
			AltDecimalCharacters = null;
	}

	/// <summary>
	/// Gets a value indicating whether the mask has all required text to pass its validation.
	/// </summary>
	/// <value><c>true</c> if mask is completed; otherwise, <c>false</c>.</value>
	public override bool MaskCompleted
	{
		get { return base.MaskCompleted && Text.ToCharArray().Any(char.IsDigit); }
	}

	/// <summary>
	/// Called to replace a character at the specified position in the masked text.
	/// </summary>
	/// <param name="character">Character to insert.</param>
	/// <param name="position">Position to insert at.</param>
	/// <returns><c>true</c> when the replacement was successful, or <c>false</c> if it failed.</returns>
	public override bool Replace(char character, ref int position)
	{
		var allow = Allow(ref character, ref position);
		return allow && base.Replace(character, ref position);
	}

	bool Allow(ref char character, ref int position)
	{
		bool allow = false;
		if (!allow && AllowDecimal && (character == DecimalCharacter || AltDecimalCharacters?.Contains(character) == true))
		{
			character = DecimalCharacter;
			var decimalIndex = Text.IndexOf(DecimalCharacter);

			if (decimalIndex >= 0)
			{
				Builder.Remove(decimalIndex, 1);
				if (position > decimalIndex)
					position--;
			}

			allow = true;
			if (position < Builder.Length && !char.IsDigit(Builder[position]))
			{
				// insert at correct location and move cursor
				int idx;
				for (idx = 0; idx < Builder.Length; idx++)
				{
					if (char.IsDigit(Builder[idx]))
					{
						break;
					}
				}
				position = idx;
				allow = true;
			}
				
		}
		if (!allow && AllowSign && SignCharacters.Contains(character))
		{
			var val = Text;
			if (val.IndexOfAny(SignCharacters) == 0)
			{
				Builder.Remove(0, 1);
				if (position == 0)
					position++;
			}
			else
				position++;
			Builder.Insert(0, character);
			return false;
		}
		allow |= char.IsDigit(character);
		return allow;
	}

	/// <summary>
	/// Called to insert a character at the specified position in the masked text.
	/// </summary>
	/// <param name="character">Character to insert.</param>
	/// <param name="position">Position to insert at.</param>
	/// <returns><c>true</c> when the insertion was successful, or <c>false</c> if it failed.</returns>
	public override bool Insert(char character, ref int position)
	{
		int pos = position;

		var allow = Allow(ref character, ref position);

		var ret = allow && base.Insert(character, ref position);

		if (ret && Validate != null && MaskCompleted && !Validate(Text))
		{
			Builder.Remove(pos, 1);
			position = pos;
			ret = false;
		}
		return ret;
	}
}
