using System;
using System.ComponentModel;
using System.Globalization;

namespace Eto.Drawing
{
	/// <summary>
	/// Converter for the <see cref="Padding"/> class
	/// </summary>
	public class PaddingConverter : TypeConverter
	{
		/// <summary>
		/// Determines if the specified <paramref name="sourceType"/> can be converted to a <see cref="Padding"/> object
		/// </summary>
		/// <param name="context">Conversion context</param>
		/// <param name="sourceType">Type to convert from</param>
		/// <returns>True if this converter can convert from the specified type, false otherwise</returns>
		public override bool CanConvertFrom (ITypeDescriptorContext context, Type sourceType)
		{
			return sourceType == typeof(string) || base.CanConvertFrom (context, sourceType);
		}

		/// <summary>
		/// Converts the specified value to a <see cref="Padding"/> object
		/// </summary>
		/// <param name="context">Conversion context</param>
		/// <param name="culture">Culture to perform the conversion for</param>
		/// <param name="value">Value to convert</param>
		/// <returns>A new instance of the <see cref="Padding"/> object with the value represented by <paramref name="value"/></returns>
		public override object ConvertFrom (ITypeDescriptorContext context, CultureInfo culture, object value)
		{
			string text = value as string;
			if (text != null) {
				var parts = text.Split (culture.TextInfo.ListSeparator.ToCharArray ());
				var converter = new Int32Converter ();
				switch (parts.Length) {
				case 1:
					return new Padding (
						(int)converter.ConvertFromString (context, culture, parts [0])
					);
				case 2:
					return new Padding (
						(int)converter.ConvertFromString (context, culture, parts [0]),
						(int)converter.ConvertFromString (context, culture, parts [1])
					);
				case 4:
					return new Padding (
						(int)converter.ConvertFromString (context, culture, parts [0]),
						(int)converter.ConvertFromString (context, culture, parts [1]),
						(int)converter.ConvertFromString (context, culture, parts [2]),
						(int)converter.ConvertFromString (context, culture, parts [3])
					);
				default:
					throw new ArgumentException (string.Format ("Cannot parse value '{0}'. Should be in the form of 'all', 'horizontal,vertical', or 'left, top, right, bottom'", text));
				}
					
			}
			return base.ConvertFrom (context, culture, value);
		}
	}
}

