using System;
using System.ComponentModel;
using System.Globalization;

namespace Eto.Drawing
{
	public class PaddingConverter : TypeConverter
	{
		public override bool CanConvertFrom (ITypeDescriptorContext context, Type sourceType)
		{
			return sourceType == typeof(string) || base.CanConvertFrom (context, sourceType);
		}
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

