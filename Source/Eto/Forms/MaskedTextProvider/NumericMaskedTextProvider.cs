using System;
using System.Text;
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

		// use dictionary instead of reflection for Xamarin.Mac linking
		Dictionary<Type, Info> numericTypes = new Dictionary<Type, Info>
		{
			{ typeof(decimal), new Info { Parse = s => { decimal d; return decimal.TryParse(s, out d) ? (object)d : null; }, AllowSign = true, AllowDecimal = true } },
			{ typeof(double), new Info { Parse = s => { double d; return double.TryParse(s, out d) ? (object)d : null; }, ToText = v => ((double)v).ToString("F99").TrimEnd('0', '.'), AllowSign = true, AllowDecimal = true } },
			{ typeof(float), new Info { Parse = s => { float d; return float.TryParse(s, out d) ? (object)d : null; }, ToText = v => ((float)v).ToString("F99").TrimEnd('0', '.'), AllowSign = true, AllowDecimal = true } },
			{ typeof(int), new Info { Parse = s => { int d; return int.TryParse(s, out d) ? (object)d : null; }, AllowSign = true } },
			{ typeof(uint), new Info { Parse = s => { uint d; return uint.TryParse(s, out d) ? (object)d : null; } } },
			{ typeof(long), new Info { Parse = s => { long d; return long.TryParse(s, out d) ? (object)d : null; }, AllowSign = true } },
			{ typeof(ulong), new Info { Parse = s => { ulong d; return ulong.TryParse(s, out d) ? (object)d : null; } } },
			{ typeof(short), new Info { Parse = s => { short d; return short.TryParse(s, out d) ? (object)d : null; }, AllowSign = true } },
			{ typeof(ushort), new Info { Parse = s => { ushort d; return ushort.TryParse(s, out d) ? (object)d : null; } } },
			{ typeof(byte), new Info { Parse = s => { byte d; return byte.TryParse(s, out d) ? (object)d : null; } } },
			{ typeof(sbyte), new Info { Parse = s => { sbyte d; return sbyte.TryParse(s, out d) ? (object)d : null; }, AllowSign = true } }
		};

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.NumericMaskedTextProvider{T}"/> class.
		/// </summary>
		public NumericMaskedTextProvider()
		{
			var type = typeof(T);
			var underlyingType = Nullable.GetUnderlyingType(type) ?? type;
			Info info;
			if (numericTypes.TryGetValue(underlyingType, out info))
			{
				AllowSign = info.AllowSign;
				AllowDecimal = info.AllowDecimal;
				parse = text =>
				{
					var val = info.Parse(text);
					return val == null ? default(T) : (T)val;
				};
				if (info.ToText != null)
					toString = val => info.ToText(val);
				else
					toString = val => val.ToString();
				Validate = text => info.Parse(text) != null;
			}
			else
			{
				// use reflection for other types
				AllowSign = Convert.ToBoolean(underlyingType.GetRuntimeField("MinValue").GetValue(null));
				AllowDecimal = underlyingType == typeof(decimal) || underlyingType == typeof(double) || underlyingType == typeof(float);

				var tryParseMethod = underlyingType.GetRuntimeMethod("TryParse", new [] { typeof(string), underlyingType.MakeByRefType() });
				if (tryParseMethod == null || tryParseMethod.ReturnType != typeof(bool))
					throw new ArgumentException(string.Format("Type of T ({0}) must implement a static bool TryParse(string, out T) method", typeof(T)));

				parse = text =>
				{
					var parameters = new object[] { Text, null };
					if ((bool)tryParseMethod.Invoke(null, parameters))
					{
						return (T)parameters[1];
					}
					return default(T);
				};
				toString = val => val.ToString();

				Validate = text =>
				{
					var parameters = new object[] { text, null };
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
			get
			{
				return parse(Text);
			}
			set
			{
				Text = toString(value);
			}
		}
	}

	/// <summary>
	/// Masked text provider for numeric input.
	/// </summary>
	public class NumericMaskedTextProvider : VariableMaskedTextProvider
	{
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
		/// Initializes a new instance of the <see cref="Eto.Forms.NumericMaskedTextProvider"/> class.
		/// </summary>
		public NumericMaskedTextProvider()
		{
			DecimalCharacter = '.';
			SignCharacters = new [] { '+', '-' };
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
			var allow = Allow(character, ref position);
			return allow && base.Replace(character, ref position);
		}

		bool Allow(char character, ref int position)
		{
			bool allow = false;
			if (!allow && AllowDecimal && character == DecimalCharacter)
			{
				var val = Text;
				if (val.IndexOf(DecimalCharacter) == -1)
				{
					allow = true;
					if (position < val.Length && !char.IsDigit(val[position]))
					{
						// insert at correct location and move cursor
						int idx;
						for (idx = 0; idx < val.Length; idx++)
						{
							if (char.IsDigit(val[idx]))
							{
								break;
							}
						}
						position = idx;
						allow = true;
					}
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

			var allow = Allow(character, ref position);

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
