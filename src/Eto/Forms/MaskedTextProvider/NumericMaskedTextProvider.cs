using System;
using System.Linq;
using System.Globalization;
using System.ComponentModel;
using System.Collections.Generic;
using System.Reflection;

namespace Eto.Forms
{
	/// <summary>
	/// Masked text provider for numeric input of the specified type.
	/// </summary>
	public class NumericMaskedTextProvider<T> : NumericMaskedTextProvider, IMaskedTextProvider<T>
	{
		Func<string, T> parse;
		Func<T, string> toString;

		class Info
		{
			public bool AllowSign;
			public bool AllowDecimal;
			public Func<string, object> Parse;
			public Func<object, string> ToText;
		}

		// do all conversions with invariant culture
		static CultureInfo Inv => CultureInfo.InvariantCulture;

		// use dictionary instead of reflection for Xamarin.Mac linking
		static readonly Dictionary<Type, Info> numericTypes = new Dictionary<Type, Info>
		{
			{ typeof(decimal), new Info { Parse = s => decimal.TryParse(s, NumberStyles.Any, Inv, out var d) ? (object)d : null, AllowSign = true, AllowDecimal = true } },
			{ typeof(double), new Info { Parse = s => double.TryParse(s, NumberStyles.Any, Inv, out var d) ? (object)d : null, ToText = DoubleToText, AllowSign = true, AllowDecimal = true } },
			{ typeof(float), new Info { Parse = s => float.TryParse(s, NumberStyles.Any, Inv, out var d) ? (object)d : null, ToText = FloatToText, AllowSign = true, AllowDecimal = true } },
			{ typeof(int), new Info { Parse = s => int.TryParse(s, NumberStyles.Any, Inv, out var d) ? (object)d : null, AllowSign = true } },
			{ typeof(uint), new Info { Parse = s => uint.TryParse(s, NumberStyles.Any, Inv, out var d) ? (object)d : null } },
			{ typeof(long), new Info { Parse = s => long.TryParse(s, NumberStyles.Any, Inv, out var d) ? (object)d : null, AllowSign = true } },
			{ typeof(ulong), new Info { Parse = s => ulong.TryParse(s, NumberStyles.Any, Inv, out var d) ? (object)d : null } },
			{ typeof(short), new Info { Parse = s => short.TryParse(s, NumberStyles.Any, Inv, out var d) ? (object)d : null, AllowSign = true } },
			{ typeof(ushort), new Info { Parse = s => ushort.TryParse(s, NumberStyles.Any, Inv, out var d) ? (object)d : null } },
			{ typeof(byte), new Info { Parse = s => byte.TryParse(s, NumberStyles.Any, Inv, out var d) ? (object)d : null } },
			{ typeof(sbyte), new Info { Parse = s => sbyte.TryParse(s, NumberStyles.Any, Inv, out var d) ? (object)d : null, AllowSign = true } }
		};

		static string DoubleToText(object v) => ((double?)v)?.ToString("F99", Inv).TrimEnd('0').TrimEnd('.') ?? string.Empty;
		static string FloatToText(object v) => ((float?)v)?.ToString("F99", Inv).TrimEnd('0').TrimEnd('.') ?? string.Empty;

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
				parse = text =>
				{
					var val = info.Parse(text);
					return val == null ? default : (T)val;
				};

				if (info.ToText != null)
					toString = val => info.ToText(val);
				else
					toString = val => Convert.ToString(val, CultureInfo.InvariantCulture);
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

				parse = text =>
				{
					var parameters = new object[] { Text, null };
					if ((bool)tryParseMethod.Invoke(null, parameters))
					{
						return (T)parameters[1];
					}
					return default;
				};
				toString = val => Convert.ToString(val, CultureInfo.InvariantCulture);

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
			get => parse(Text?.Replace(DecimalCharacter, '.'));
			set => Text = toString(value)?.Replace('.', DecimalCharacter);
		}

		internal override void SetCulture()
		{
			var value = Value;
			base.SetCulture();
			Value = value;
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
}
