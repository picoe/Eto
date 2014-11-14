using System;
using System.ComponentModel;
using System.Globalization;

namespace Eto.Drawing
{
	/// <summary>
	/// Converter for the <see cref="Rectangle"/> class
	/// </summary>
	/// <remarks>
	/// Allows for conversion from a string to a <see cref="Rectangle"/>.
	/// </remarks>
	/// <copyright>(c) 2014 by Curtis Wensley</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	public class RectangleConverter : TypeConverter
	{
		/// <summary>
		/// Determines if this converter can convert from the specified <paramref name="sourceType"/>
		/// </summary>
		/// <param name="context">Conversion context</param>
		/// <param name="sourceType">Type to convert from</param>
		/// <returns>True if this converter can convert from the specified type, false otherwise</returns>
		public override bool CanConvertFrom (ITypeDescriptorContext context, Type sourceType)
		{
			return sourceType == typeof (string) || base.CanConvertFrom (context, sourceType);
		}

		/// <summary>
		/// Converts the specified value to a <see cref="Rectangle"/>
		/// </summary>
		/// <param name="context">Conversion context</param>
		/// <param name="culture">Culture to perform the conversion</param>
		/// <param name="value">Value to convert</param>
		/// <returns>A new instance of a <see cref="Rectangle"/> converted from the specified <paramref name="value"/></returns>
		public override object ConvertFrom (ITypeDescriptorContext context, CultureInfo culture, object value)
		{
			string text = value as string;
			if (text != null) {
				var parts = text.Split (culture.TextInfo.ListSeparator.ToCharArray ());
				if (parts.Length != 4)
					throw new ArgumentException (string.Format (CultureInfo.CurrentCulture, "Cannot parse value '{0}'. Should be in the form of 'x, y, width, height'", text));
				var converter = new Int32Converter ();
				return new Rectangle (
					(int)converter.ConvertFromString (context, culture, parts [0]),
					(int)converter.ConvertFromString (context, culture, parts [1]),
					(int)converter.ConvertFromString (context, culture, parts [2]),
					(int)converter.ConvertFromString (context, culture, parts [3])
				);
			}
			return base.ConvertFrom (context, culture, value);
		}
	}
}

