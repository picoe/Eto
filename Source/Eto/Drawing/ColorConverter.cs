using System;
using System.ComponentModel;
using Eto.Drawing;
using System.Globalization;

namespace Eto.Drawing
{
	/// <summary>
	/// Converts instances of other types to and from a <see cref="Color"/>.
	/// </summary>
	/// <remarks>
	/// This only supports converting from a string supported by the <see cref="Color.TryParse"/> method.
	/// 
	/// When converting to a string, it converts to a Hex format via <see cref="Color.ToHex"/>
	/// </remarks>
	public class ColorConverter : TypeConverter
	{
		/// <summary>
		/// Determines if this can convert a <see cref="Color"/> to the <paramref name="destinationType"/>
		/// </summary>
		/// <param name="context">Context of the conversion</param>
		/// <param name="destinationType">Type to convert to</param>
		/// <returns>True if this converter supports the <paramref name="destinationType"/>, false otherwise</returns>
		public override bool CanConvertTo (ITypeDescriptorContext context, Type destinationType)
		{
			if (destinationType == typeof (string)) return true;
			return base.CanConvertTo (context, destinationType);
		}

		/// <summary>
		/// Determines if this can convert a value with the type of <paramref name="sourceType"/> to a <see cref="Color"/>
		/// </summary>
		/// <param name="context">Context of the conversion</param>
		/// <param name="sourceType">Type to convert from</param>
		/// <returns>True if this can convert to the <paramref name="sourceType"/>, false otherwise</returns>
		public override bool CanConvertFrom (ITypeDescriptorContext context, Type sourceType)
		{
			if (sourceType == typeof(string)) return true;
			return base.CanConvertFrom (context, sourceType);
		}

		/// <summary>
		/// Converts the <paramref name="value"/> to an instance of a <see cref="Color"/>
		/// </summary>
		/// <param name="context">Context of the conversion</param>
		/// <param name="culture">Culture to use for the conversion</param>
		/// <param name="value">Value to convert</param>
		/// <returns>A <see cref="Color"/> instance with the converted value</returns>
		public override object ConvertFrom (ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
		{
			var str = value as string;
			if (str != null) {
				Color color;
				if (!Color.TryParse (str, out color, culture))
					throw new ArgumentException (str + " is not a valid color value.");
				return color;
			}
			return base.ConvertFrom (context, culture, value);
		}

		/// <summary>
		/// Converts a <see cref="Color"/> instance to the specified <paramref name="destinationType"/>
		/// </summary>
		/// <param name="context">Context of the conversion</param>
		/// <param name="culture">Culture to use for the conversion</param>
		/// <param name="value"><see cref="Color"/> value to convert</param>
		/// <param name="destinationType">Type to convert the <paramref name="value"/> to</param>
		/// <returns>An object of type <paramref name="destinationType"/> converted from <paramref name="value"/></returns>
		public override object ConvertTo (ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
		{
			if (destinationType == typeof (string)) {
				return ((Color)value).ToHex ();
			}
			return base.ConvertTo (context, culture, value, destinationType);
		}
	}
}

