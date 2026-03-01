using System.Globalization;
using NUnit.Framework;

namespace Eto.Test.UnitTests.Forms.Bindings;

class TrackingConverter : IValueConverter
{
	public object LastConvertValue { get; private set; }
	public Type LastConvertTargetType { get; private set; }
	public object LastConvertParameter { get; private set; }
	public CultureInfo LastConvertCulture { get; private set; }
	public object LastConvertBackValue { get; private set; }
	public Type LastConvertBackTargetType { get; private set; }
	public object LastConvertBackParameter { get; private set; }
	public CultureInfo LastConvertBackCulture { get; private set; }

	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		LastConvertValue = value;
		LastConvertTargetType = targetType;
		LastConvertParameter = parameter;
		LastConvertCulture = culture;

		var delta = System.Convert.ToInt32(parameter, CultureInfo.InvariantCulture);
		var converted = System.Convert.ToInt32(value, CultureInfo.InvariantCulture) + delta;

		if (targetType == typeof(string))
			return converted.ToString(culture);
		if (targetType == typeof(double))
			return (double)converted;

		return converted;
	}

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		LastConvertBackValue = value;
		LastConvertBackTargetType = targetType;
		LastConvertBackParameter = parameter;
		LastConvertBackCulture = culture;

		var delta = System.Convert.ToInt32(parameter, CultureInfo.InvariantCulture);
		var converted = System.Convert.ToInt32(value, CultureInfo.InvariantCulture) - delta;

		if (targetType == typeof(string))
			return converted.ToString(culture);
		if (targetType == typeof(double))
			return (double)converted;

		return converted;
	}
}

[TestFixture]
public class DirectBindingConversionTests
{
	[Test]
	public void ConvertWithConverterShouldRoundTrip()
	{
		var item = new BindObject { IntProperty = 7 };
		var converter = new TrackingConverter();
		var culture = CultureInfo.GetCultureInfo("fr-FR");
		var binding = Binding.Property(item, r => r.IntProperty).Convert<string>(converter, conveterParameter: 3, culture: culture);

		Assert.That(binding.DataValue, Is.EqualTo("10"));
		Assert.That(converter.LastConvertTargetType, Is.EqualTo(typeof(string)));
		Assert.That(converter.LastConvertParameter, Is.EqualTo(3));
		Assert.That(converter.LastConvertCulture, Is.EqualTo(culture));

		binding.DataValue = "11";
		Assert.That(item.IntProperty, Is.EqualTo(8));
		Assert.That(converter.LastConvertBackTargetType, Is.EqualTo(typeof(int)));
		Assert.That(converter.LastConvertBackParameter, Is.EqualTo(3));
		Assert.That(converter.LastConvertBackCulture, Is.EqualTo(culture));
	}

	[Test]
	public void ConvertWithNullConverterShouldThrow()
	{
		var item = new BindObject { IntProperty = 7 };
		var binding = Binding.Property(item, r => r.IntProperty);

		Assert.Throws<ArgumentNullException>(() => binding.Convert<string>((IValueConverter)null));
	}
}
