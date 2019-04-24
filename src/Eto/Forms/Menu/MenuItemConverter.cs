using System;
using sc = System.ComponentModel;

namespace Eto.Forms
{
	class MenuItemConverter : sc.TypeConverter
	{
		public override bool CanConvertFrom(sc.ITypeDescriptorContext context, Type sourceType)
		{
			return typeof(Command).IsAssignableFrom(sourceType);
		}

		public override bool CanConvertTo(sc.ITypeDescriptorContext context, Type destinationType)
		{
			return false;
		}

		public override object ConvertFrom(sc.ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
		{
			var command = value as Command;
			if (command != null)
				return command.CreateMenuItem();
			return base.ConvertFrom(context, culture, value);
		}
	}
}

