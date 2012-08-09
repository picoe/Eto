using System;
using System.ComponentModel;
using Eto.Drawing;
using System.Globalization;

namespace Eto.Drawing
{
	public class ColorConverter : TypeConverter
	{
		public override bool CanConvertFrom (ITypeDescriptorContext context, Type sourceType)
		{
			if (sourceType == typeof(string)) return true;
			return base.CanConvertFrom (context, sourceType);
		}

		public override object ConvertFrom (ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
		{
			if (value is string) {
				return ConvertFromString(context, (string)value, culture);
			}
			return base.ConvertFrom (context, culture, value);
		}
		
		static Color ConvertFromString (ITypeDescriptorContext context, string s, CultureInfo culture)
		{
			Color color;
			if (!Color.TryParse (s, out color, culture))
				throw new ArgumentException (s + " is not a valid color value.");
			return color;
		}
	}
}

