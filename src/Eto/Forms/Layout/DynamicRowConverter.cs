using System;
using sc = System.ComponentModel;
using System.Globalization;

namespace Eto.Forms
{
	class DynamicRowConverter : sc.TypeConverter
	{
		public override bool CanConvertTo(sc.ITypeDescriptorContext context, Type destinationType)
		{
			return false;
		}

		public override bool CanConvertFrom(sc.ITypeDescriptorContext context, Type sourceType)
		{
			return typeof(Control).IsAssignableFrom(sourceType);
		}

		public override object ConvertTo(sc.ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
		{
			throw new NotSupportedException();
		}

		public override object ConvertFrom(sc.ITypeDescriptorContext context, CultureInfo culture, object value)
		{
			var dynamicRow = new DynamicRow();
			dynamicRow.Add(new DynamicControl { Control = value as Control });
			return dynamicRow;
		}
	}
}
