namespace Eto.Drawing;

/// <summary>
/// Converter for the <see cref="PaddingF"/> class
/// </summary>
class PaddingFConverterInternal : sc.TypeConverter
{
	/// <summary>
	/// Determines if the specified <paramref name="sourceType"/> can be converted to a <see cref="PaddingF"/> object
	/// </summary>
	/// <param name="context">Conversion context</param>
	/// <param name="sourceType">Type to convert from</param>
	/// <returns>True if this converter can convert from the specified type, false otherwise</returns>
	public override bool CanConvertFrom(sc.ITypeDescriptorContext context, Type sourceType)
	{
		return sourceType == typeof(string);
	}

	// writes 'left,top,right,bottom' so ConvertFrom can read it back
	public override object ConvertTo(sc.ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
	{
		if (destinationType == typeof(string) && value is PaddingF padding)
			return ConverterHelper.Join(culture, padding.Left, padding.Top, padding.Right, padding.Bottom);
		return base.ConvertTo(context, culture, value, destinationType);
	}

	/// <summary>
	/// Converts the specified value to a <see cref="PaddingF"/> object
	/// </summary>
	/// <param name="context">Conversion context</param>
	/// <param name="culture">Culture to perform the conversion for</param>
	/// <param name="value">Value to convert</param>
	/// <returns>A new instance of the <see cref="PaddingF"/> object with the value represented by <paramref name="value"/></returns>
	public override object ConvertFrom(sc.ITypeDescriptorContext context, CultureInfo culture, object value)
	{
		string text = value as string;
		if (text != null)
		{
			culture ??= CultureInfo.CurrentCulture;
			string[] parts = ConverterHelper.Split(text, culture);

			try
			{
				switch (parts.Length)
				{
					case 1:
						return new PaddingF(
							float.Parse(parts[0], culture)
						);
					case 2:
						return new PaddingF(
							float.Parse(parts[0], culture),
							float.Parse(parts[1], culture)
						);
					case 4:
						return new PaddingF(
							float.Parse(parts[0], culture),
							float.Parse(parts[1], culture),
							float.Parse(parts[2], culture),
							float.Parse(parts[3], culture)
						);
					default:
						throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, "Cannot parse value '{0}' as PaddingF. Should be in the form of 'all', 'horizontal, vertical', or 'left, top, right, bottom'", text));
				}
			}
			catch
			{
				throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, "Cannot parse value '{0}' as PaddingF. Should be in the form of 'all', 'horizontal, vertical', or 'left, top, right, bottom'", text));
			}
		}
		return base.ConvertFrom(context, culture, value);
	}
}