using System;
using System.ComponentModel;
using System.Globalization;

namespace Eto.Drawing
{
	public class RectangleFConverter : TypeConverter
	{
		public override object ConvertFrom (ITypeDescriptorContext context, CultureInfo culture, object value)
		{
			string text = value as string;
			if (text != null) {
				var parts = text.Split (culture.TextInfo.ListSeparator.ToCharArray ());
				if (parts.Length != 4)
					throw new ArgumentException (string.Format ("Cannot parse value '{0}'. Should be in the form of 'x, y, width, height'", text));
				var converter = new SingleConverter ();
				return new RectangleF (
					(float)converter.ConvertFromString (context, culture, parts [0]),
					(float)converter.ConvertFromString (context, culture, parts [1]),
					(float)converter.ConvertFromString (context, culture, parts [2]),
					(float)converter.ConvertFromString (context, culture, parts [3])
				);
			}
			return base.ConvertFrom (context, culture, value);
		}
	}
}

