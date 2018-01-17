using System;
using System.ComponentModel;

namespace Eto.Forms
{
	class ToolItemConverter : TypeConverter
	{
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
		{
			return typeof(Command).IsAssignableFrom(sourceType) || base.CanConvertFrom(context, sourceType);
		}

		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
		{
			return false;
		}

		public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
		{
			var command = value as Command;
			if (command != null)
				return command.CreateToolItem();
			return base.ConvertFrom(context, culture, value);
		}
	}
}

