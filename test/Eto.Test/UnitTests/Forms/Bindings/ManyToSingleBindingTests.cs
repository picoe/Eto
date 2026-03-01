using NUnit.Framework;

namespace Eto.Test.UnitTests.Forms.Bindings;

[TestFixture]
public class ManyToSingleBindingTests
{
	class ManyItem
	{
		public int Value { get; set; }
	}

	class ManyContainer
	{
		public IEnumerable<ManyItem> Items { get; set; }
	}

	[Test]
	public void ManyToSingleShouldReturnSingleOrMixedValue()
	{
		var binding = Binding.Property((ManyContainer c) => c.Items).ManyToSingle(i => i.Value, mixedValue: -1);

		var sameValues = new ManyContainer
		{
			Items = new[]
			{
				new ManyItem { Value = 2 },
				new ManyItem { Value = 2 }
			}
		};

		var mixedValues = new ManyContainer
		{
			Items = new[]
			{
				new ManyItem { Value = 2 },
				new ManyItem { Value = 3 }
			}
		};

		Assert.That(binding.GetValue(sameValues), Is.EqualTo(2));
		Assert.That(binding.GetValue(mixedValues), Is.EqualTo(-1));
	}

	[Test]
	public void ManyToSingleShouldSetValueForEachItem()
	{
		var binding = Binding.Property((ManyContainer c) => c.Items).ManyToSingle(
			getValue: i => i.Value,
			setValue: (i, v) => i.Value = v
		);

		var item1 = new ManyItem { Value = 1 };
		var item2 = new ManyItem { Value = 2 };
		var data = new ManyContainer { Items = new[] { item1, item2 } };

		binding.SetValue(data, 9);

		Assert.That(item1.Value, Is.EqualTo(9));
		Assert.That(item2.Value, Is.EqualTo(9));
		Assert.That(binding.GetValue(data), Is.EqualTo(9));
	}
}
