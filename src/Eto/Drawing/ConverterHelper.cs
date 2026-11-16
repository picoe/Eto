namespace Eto.Drawing;

/// <summary>
/// Splits and joins multi-value text for the drawing type converters using the given culture.
/// </summary>
static class ConverterHelper
{
	// ';' when the decimal mark is ',' so text like "1,5;2" stays unambiguous, same rule as WPF
	public static char GetSeparator(CultureInfo culture)
	{
		var decimalSeparator = culture.NumberFormat.NumberDecimalSeparator;
		return decimalSeparator.Length > 0 && decimalSeparator[0] == ',' ? ';' : ',';
	}

	public static string[] Split(string text, CultureInfo culture)
		=> text.Split(new[] { GetSeparator(culture) }, StringSplitOptions.RemoveEmptyEntries);

	public static string Join(CultureInfo culture, params IFormattable[] values)
	{
		culture ??= CultureInfo.CurrentCulture;
		return string.Join(GetSeparator(culture).ToString(), values.Select(v => v.ToString(null, culture)));
	}
}
