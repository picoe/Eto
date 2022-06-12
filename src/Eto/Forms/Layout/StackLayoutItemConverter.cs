using System;
using sc = System.ComponentModel;
using System.Globalization;

namespace Eto.Forms
{
	class StackLayoutItemConverter : sc.TypeConverter
	{
		public override bool CanConvertTo(sc.ITypeDescriptorContext context, Type destinationType)
		{
			return false;
		}

		public override bool CanConvertFrom(sc.ITypeDescriptorContext context, Type sourceType)
		{
			return sourceType == typeof(string) || typeof(Control).IsAssignableFrom(sourceType);
		}

		public override object ConvertTo(sc.ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
		{
			throw new NotSupportedException();
		}

		public override object ConvertFrom(sc.ITypeDescriptorContext context, CultureInfo culture, object value)
		{
			var text = value as string;
			if (text != null)
				return new StackLayoutItem { Control = text };
			return new StackLayoutItem { Control = value as Control };
		}
	}
}
