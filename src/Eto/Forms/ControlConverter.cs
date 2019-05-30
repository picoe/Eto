using System;
using sc = System.ComponentModel;

namespace Eto.Forms
{
	class ControlConverter : sc.TypeConverter
	{
		public override bool CanConvertFrom(sc.ITypeDescriptorContext context, Type sourceType)
		{
			return sourceType == typeof(string);
		}

		public override bool CanConvertTo(sc.ITypeDescriptorContext context, Type destinationType)
		{
			return false;
		}

		public override object ConvertFrom(sc.ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
		{
			if (value is string text)
				return new Label { Text = text };
			return null;
		}
	}
}