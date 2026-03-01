using NUnit.Framework;

namespace Eto.Test.UnitTests.Forms.Bindings;

[TestFixture]
public class IndirectBindingConversionTests
{
	[Test]
	public void ConvertWithGenericConverterShouldUseInvariantCultureByDefault()
	{
		var item = new BindObject { IntProperty = 7 };
		var converter = new TrackingConverter();
		var binding = Binding.Property((BindObject r) => r.IntProperty).Convert<string>(converter, conveterParameter: 4);

		Assert.That(binding.GetValue(item), Is.EqualTo("11"));
		Assert.That(converter.LastConvertTargetType, Is.EqualTo(typeof(string)));
		Assert.That(converter.LastConvertCulture, Is.EqualTo(CultureInfo.InvariantCulture));

		binding.SetValue(item, "15");
		Assert.That(item.IntProperty, Is.EqualTo(11));
		Assert.That(converter.LastConvertBackTargetType, Is.EqualTo(typeof(int)));
		Assert.That(converter.LastConvertBackCulture, Is.EqualTo(CultureInfo.InvariantCulture));
	}

	[Test]
	public void ConvertWithExplicitPropertyTypeShouldUseConverterType()
	{
		var item = new BindObject { IntProperty = 7 };
		var converter = new TrackingConverter();
		var binding = Binding.Property((BindObject r) => r.IntProperty).Convert(converter, typeof(double), conveterParameter: 2);

		Assert.That(binding.GetValue(item), Is.EqualTo(9d));
		Assert.That(converter.LastConvertTargetType, Is.EqualTo(typeof(double)));

		binding.SetValue(item, 13d);
		Assert.That(item.IntProperty, Is.EqualTo(11));
		Assert.That(converter.LastConvertBackTargetType, Is.EqualTo(typeof(int)));
	}
}
