using System;
using System.ComponentModel;
using System.Globalization;

namespace Eto.Forms
{
	class StackLayoutItemConverter : TypeConverter
	{
		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
		{
			return false;
		}

		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
		{
			return sourceType == typeof(string) || typeof(Control).IsAssignableFrom(sourceType);
		}

		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
		{
			throw new NotSupportedException();
		}

		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
		{
			var text = value as string;
			if (text != null)
				return new StackLayoutItem { Control = text };
			return new StackLayoutItem { Control = value as Control };
		}
	}
}
