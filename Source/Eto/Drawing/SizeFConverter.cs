using System;
using System.ComponentModel;

namespace Eto.Drawing
{
	public class SizeFConverter : TypeConverter
	{
		public override bool CanConvertFrom (ITypeDescriptorContext context, Type sourceType)
		{
			return sourceType == typeof(string) || base.CanConvertFrom (context, sourceType);
		}
		
		public override object ConvertFrom (ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
		{
			var text = value as string;
			if (text != null) {
				var parts = text.Split (culture.TextInfo.ListSeparator.ToCharArray ());
				if (parts.Length != 2)
					throw new ArgumentException (string.Format ("Cannot parse value '{0}' as size. Should be in the form of 'width,height'", text));

				var converter = new SingleConverter ();
				return new SizeF (
					(float)converter.ConvertFromString (context, culture, parts [0]),
					(float)converter.ConvertFromString (context, culture, parts [1])
				);
			}
			return base.ConvertFrom (context, culture, value);
		}
	}
}

