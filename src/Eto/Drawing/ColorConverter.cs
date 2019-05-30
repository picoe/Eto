using System;
using sc = System.ComponentModel;
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
	/// <copyright>(c) 2014 by Curtis Wensley</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	class ColorConverterInternal : sc.TypeConverter
	{
		/// <summary>
		/// Determines if this can convert a <see cref="Color"/> to the <paramref name="destinationType"/>
		/// </summary>
		/// <param name="context">Context of the conversion</param>
		/// <param name="destinationType">Type to convert to</param>
		/// <returns>True if this converter supports the <paramref name="destinationType"/>, false otherwise</returns>
		public override bool CanConvertTo(sc.ITypeDescriptorContext context, Type destinationType)
		{
			return destinationType == typeof(string);
		}

		/// <summary>
		/// Determines if this can convert a value with the type of <paramref name="sourceType"/> to a <see cref="Color"/>
		/// </summary>
		/// <param name="context">Context of the conversion</param>
		/// <param name="sourceType">Type to convert from</param>
		/// <returns>True if this can convert to the <paramref name="sourceType"/>, false otherwise</returns>
		public override bool CanConvertFrom(sc.ITypeDescriptorContext context, Type sourceType)
		{
			return sourceType == typeof(string);
		}

		/// <summary>
		/// Converts the <paramref name="value"/> to an instance of a <see cref="Color"/>
		/// </summary>
		/// <param name="context">Context of the conversion</param>
		/// <param name="culture">Culture to use for the conversion</param>
		/// <param name="value">Value to convert</param>
		/// <returns>A <see cref="Color"/> instance with the converted value</returns>
		public override object ConvertFrom(sc.ITypeDescriptorContext context, CultureInfo culture, object value)
		{
			var str = value as string;
			if (str != null)
			{
				Color color;
				if (!Color.TryParse(str, out color))
					throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, "{0} is not a valid color value.", str));
				return color;
			}
			return base.ConvertFrom(context, culture, value);
		}

		/// <summary>
		/// Converts a <see cref="Color"/> instance to the specified <paramref name="destinationType"/>
		/// </summary>
		/// <param name="context">Context of the conversion</param>
		/// <param name="culture">Culture to use for the conversion</param>
		/// <param name="value"><see cref="Color"/> value to convert</param>
		/// <param name="destinationType">Type to convert the <paramref name="value"/> to</param>
		/// <returns>An object of type <paramref name="destinationType"/> converted from <paramref name="value"/></returns>
		public override object ConvertTo(sc.ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
		{
			if (destinationType == typeof(string))
			{
				return ((Color)value).ToHex();
			}
			return base.ConvertTo(context, culture, value, destinationType);
		}
	}

	/// <summary>
	/// Converts instances of other types to and from a <see cref="Color"/>.
	/// </summary>
	/// <remarks>
	/// This only supports converting from a string supported by the <see cref="Color.TryParse"/> method.
	/// 
	/// When converting to a string, it converts to a Hex format via <see cref="Color.ToHex"/>
	/// </remarks>
	/// <copyright>(c) 2014 by Curtis Wensley</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	[Obsolete("Since 2.5. Use TypeDescriptor.GetConverter instead")]
	public class ColorConverter : TypeConverter
	{
		/// <summary>
		/// Determines if this can convert a <see cref="Color"/> to the <paramref name="destinationType"/>
		/// </summary>
		/// <param name="context">Context of the conversion</param>
		/// <param name="destinationType">Type to convert to</param>
		/// <returns>True if this converter supports the <paramref name="destinationType"/>, false otherwise</returns>
		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
		{
			return destinationType == typeof(string);
		}

		/// <summary>
		/// Determines if this can convert a value with the type of <paramref name="sourceType"/> to a <see cref="Color"/>
		/// </summary>
		/// <param name="context">Context of the conversion</param>
		/// <param name="sourceType">Type to convert from</param>
		/// <returns>True if this can convert to the <paramref name="sourceType"/>, false otherwise</returns>
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
		{
			return sourceType == typeof(string);
		}

		/// <summary>
		/// Converts the <paramref name="value"/> to an instance of a <see cref="Color"/>
		/// </summary>
		/// <param name="context">Context of the conversion</param>
		/// <param name="culture">Culture to use for the conversion</param>
		/// <param name="value">Value to convert</param>
		/// <returns>A <see cref="Color"/> instance with the converted value</returns>
		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
		{
			var str = value as string;
			if (str != null)
			{
				Color color;
				if (!Color.TryParse(str, out color))
					throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, "{0} is not a valid color value.", str));
				return color;
			}
			return base.ConvertFrom(context, culture, value);
		}

		/// <summary>
		/// Converts a <see cref="Color"/> instance to the specified <paramref name="destinationType"/>
		/// </summary>
		/// <param name="context">Context of the conversion</param>
		/// <param name="culture">Culture to use for the conversion</param>
		/// <param name="value"><see cref="Color"/> value to convert</param>
		/// <param name="destinationType">Type to convert the <paramref name="value"/> to</param>
		/// <returns>An object of type <paramref name="destinationType"/> converted from <paramref name="value"/></returns>
		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
		{
			if (destinationType == typeof(string))
			{
				return ((Color)value).ToHex();
			}
			return base.ConvertTo(context, culture, value, destinationType);
		}
	}
}

