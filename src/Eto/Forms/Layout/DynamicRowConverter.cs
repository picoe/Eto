using System;
using System.ComponentModel;
using System.Globalization;

namespace Eto.Forms
{
	class DynamicRowConverter : TypeConverter
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
			var dynamicRow = new DynamicRow();
			dynamicRow.Add(new DynamicControl { Control = value as Control });
			return dynamicRow;
		}
	}
}
