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
		MethodInfo tryParseMethod;

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.NumericMaskedTextProvider{T}"/> class.
		/// </summary>
		public NumericMaskedTextProvider()
		{
			var type = typeof(T);
			var underlyingType = Nullable.GetUnderlyingType(type) ?? type;
			AllowSign = Convert.ToBoolean(underlyingType.GetRuntimeField("MinValue").GetValue(null));
			AllowDecimal = underlyingType == typeof(decimal) || underlyingType == typeof(double) || underlyingType == typeof(float);

			tryParseMethod = underlyingType.GetRuntimeMethod("TryParse", new [] { typeof(string), underlyingType.MakeByRefType() });
			if (tryParseMethod == null || tryParseMethod.ReturnType != typeof(bool))
				throw new ArgumentException(string.Format("Type of T ({0}) must implement a static bool TryParse(string, out T) method", typeof(T)));

			Validate = text =>
			{
				var parameters = new object[] { text, null };
				return (bool)tryParseMethod.Invoke(null, parameters);
			};
		}

		/// <summary>
		/// Gets or sets the translated value of the mask.
		/// </summary>
		/// <value>The value of the mask.</value>
		public T Value
		{
			get
			{
				var parameters = new object[] { Text, null };
				if ((bool)tryParseMethod.Invoke(null, parameters))
				{
					return (T)parameters[1];
				}
				return default(T);
			}
			set
			{
				Text = Convert.ToString(value);
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
