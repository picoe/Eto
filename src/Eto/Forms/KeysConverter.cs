namespace Eto.Forms;

class KeysConverter : sc.TypeConverter
{
	public override bool CanConvertFrom(sc.ITypeDescriptorContext context, Type sourceType)
	{
		return sourceType == typeof(string);
	}

	public override bool CanConvertTo(sc.ITypeDescriptorContext context, Type destinationType)
	{
		return destinationType == typeof(string);
	}

	public override object ConvertFrom(sc.ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
	{
		if (value is string text)
		{
			return KeysExtensions.FromShortcutString(text);
		}
		return base.ConvertFrom(context, culture, value);
	}

	public override object ConvertTo(sc.ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
	{
		if (destinationType == typeof(string) && value is Keys keys)
		{
			return keys.ToShortcutString();
		}
		return base.ConvertTo(context, culture, value, destinationType);
	}
}