using System;
using System.ComponentModel;
using System.Globalization;
#if PCL
using MissingTypes;
#endif

namespace Eto.Forms
{
	class DynamicItemConverter : TypeConverter
	{
		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
		{
			return false;
		}

		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
		{
			return typeof(Control).IsAssignableFrom(sourceType);
		}

		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
		{
			throw new NotSupportedException();
		}

		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
		{
			return new DynamicControl { Control = value as Control };
		}
	}
}
