using System;
using sc = System.ComponentModel;

namespace Eto.Forms
{
	class ControlConverter : sc.TypeConverter
	{
		public override bool CanConvertFrom(sc.ITypeDescriptorContext context, Type sourceType)
		{
			return sourceType == typeof(string) || typeof(Control).IsAssignableFrom(sourceType) || base.CanConvertFrom(context, sourceType);
		}

		public override bool CanConvertTo(sc.ITypeDescriptorContext context, Type destinationType)
		{
			return false;
		}

		public override object ConvertFrom(sc.ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
		{
			var text = value as string;
			if (!string.IsNullOrEmpty(text))
				return new Label { Text = text };
			var control = value as Control;
			if (control != null)
				return control;
			if (value == null)
				return null;
			return base.ConvertFrom(context, culture, value);
		}
	}
}