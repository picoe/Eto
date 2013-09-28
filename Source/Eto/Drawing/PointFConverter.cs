using System;
using System.ComponentModel;
using System.Globalization;

namespace Eto.Drawing
{
	/// <summary>
	/// Converter for the <see cref="PointF"/> class
	/// </summary>
	/// <remarks>
	/// Allows conversion from a string to a <see cref="PointF"/> via json/xaml or other sources.
	/// </remarks>
	public class PointFConverter : TypeConverter
	{
		/// <summary>
		/// Determines if this converter can convert from the specified <paramref name="sourceType"/>
		/// </summary>
		/// <param name="context">Conversion context</param>
		/// <param name="sourceType">Type to convert from</param>
		/// <returns>True if this converter can convert from the specified type, false otherwise</returns>
		public override bool CanConvertFrom (ITypeDescriptorContext context, Type sourceType)
		{
			return sourceType == typeof(string) || base.CanConvertFrom (context, sourceType);
		}

		/// <summary>
		/// Converts the specified value to a <see cref="PointF"/>
		/// </summary>
		/// <param name="context">Conversion context</param>
		/// <param name="culture">Culture to perform the conversion</param>
		/// <param name="value">Value to convert</param>
		/// <returns>A new instance of a <see cref="PointF"/> converted from the specified <paramref name="value"/></returns>
		public override object ConvertFrom (ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
		{
			var text = value as string;
			if (text != null) {
				var parts = text.Split (culture.TextInfo.ListSeparator.ToCharArray ());
				if (parts.Length != 2)
					throw new ArgumentException (string.Format (CultureInfo.CurrentCulture, "Cannot parse value '{0}' as point.  Should be in the form of 'x,y'", text));

				var converter = new SingleConverter ();
				return new PointF (
					(float)converter.ConvertFromString (context, culture, parts [0]),
					(float)converter.ConvertFromString (context, culture, parts [1])
				);
			}
			return base.ConvertFrom (context, culture, value);
		}
	}
}

