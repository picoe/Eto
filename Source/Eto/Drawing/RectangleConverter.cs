using System;
using System.ComponentModel;
using System.Globalization;

namespace Eto.Drawing
{
	public class RectangleConverter : TypeConverter
	{
		public override object ConvertFrom (ITypeDescriptorContext context, CultureInfo culture, object value)
		{
			string text = value as string;
			if (text != null) {
				var parts = text.Split (culture.TextInfo.ListSeparator.ToCharArray ());
				if (parts.Length != 4)
					throw new ArgumentException (string.Format ("Cannot parse value '{0}'. Should be in the form of 'x, y, width, height'", text));
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

