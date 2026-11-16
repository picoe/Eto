using NUnit.Framework;

namespace Eto.Test.UnitTests.Drawing
{
	[TestFixture]
	public class SizeTests
	{
		[TestCase(0)]
		[TestCase(5)]
		[TestCase(-3)]
		public void SizeWithSingleValueShouldSetWidthAndHeight(int value)
		{
			var size = new Size(value);
			Assert.That(size.Width, Is.EqualTo(value), "#1");
			Assert.That(size.Height, Is.EqualTo(value), "#2");
			Assert.That(size, Is.EqualTo(new Size(value, value)), "#3");
		}

		[TestCase(0f)]
		[TestCase(5f)]
		[TestCase(1.5f)]
		[TestCase(-2.25f)]
		public void SizeFWithSingleValueShouldSetWidthAndHeight(float value)
		{
			var size = new SizeF(value);
			Assert.That(size.Width, Is.EqualTo(value), "#1");
			Assert.That(size.Height, Is.EqualTo(value), "#2");
			Assert.That(size, Is.EqualTo(new SizeF(value, value)), "#3");
		}

		static T ConvertFromString<T>(string text) => (T)sc.TypeDescriptor.GetConverter(typeof(T)).ConvertFromInvariantString(text);

		[TestCase("5", 5, 5)]
		[TestCase(" 7 ", 7, 7)]
		[TestCase("3, 4", 3, 4)]
		[TestCase("3,4", 3, 4)]
		public void SizeShouldConvertFromString(string text, int width, int height)
		{
			Assert.That(ConvertFromString<Size>(text), Is.EqualTo(new Size(width, height)));
		}

		[TestCase("")]
		[TestCase("abc")]
		[TestCase("1.5")]
		[TestCase("1, 2, 3")]
		[TestCase("1, abc")]
		public void SizeShouldNotConvertFromInvalidString(string text)
		{
			Assert.Throws<ArgumentException>(() => ConvertFromString<Size>(text));
		}

		[TestCase("5", 5f, 5f)]
		[TestCase("1.5", 1.5f, 1.5f)]
		[TestCase("3, 4.25", 3f, 4.25f)]
		public void SizeFShouldConvertFromString(string text, float width, float height)
		{
			Assert.That(ConvertFromString<SizeF>(text), Is.EqualTo(new SizeF(width, height)));
		}

		[TestCase("")]
		[TestCase("abc")]
		[TestCase("1, 2, 3")]
		[TestCase("1, abc")]
		public void SizeFShouldNotConvertFromInvalidString(string text)
		{
			Assert.Throws<ArgumentException>(() => ConvertFromString<SizeF>(text));
		}
	}
}
