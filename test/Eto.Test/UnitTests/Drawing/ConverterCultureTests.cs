using Eto.Serialization.Json;
using Eto.Serialization.Xaml;
using NUnit.Framework;
using System.Text;

namespace Eto.Test.UnitTests.Drawing
{
	/// <summary>
	/// Converters follow the culture they're given; markup loaders pass the invariant culture so files load the same everywhere.
	/// </summary>
	[TestFixture]
	public class ConverterCultureTests : TestBase
	{
		static readonly CultureInfo German = CultureInfo.GetCultureInfo("de-DE");

		static IEnumerable<TestCaseData> InvariantValues()
		{
			yield return new TestCaseData("1.5", new SizeF(1.5f, 1.5f));
			yield return new TestCaseData("3, 4.25", new SizeF(3f, 4.25f));
			yield return new TestCaseData("0.5, -2.75", new PointF(0.5f, -2.75f));
			yield return new TestCaseData("1, 2, 3.5, 4.25", new RectangleF(1f, 2f, 3.5f, 4.25f));
			yield return new TestCaseData("1.5", new PaddingF(1.5f));
			yield return new TestCaseData("1.5, 2.5", new PaddingF(1.5f, 2.5f));
			yield return new TestCaseData("1.5, 2.5, 3.5, 4.5", new PaddingF(1.5f, 2.5f, 3.5f, 4.5f));
			yield return new TestCaseData("3, 4", new Size(3, 4));
			yield return new TestCaseData("-1, 2", new Point(-1, 2));
			yield return new TestCaseData("1, 2, 3, 4", new Rectangle(1, 2, 3, 4));
			yield return new TestCaseData("1, 2, 3, 4", new Padding(1, 2, 3, 4));
		}

		// ';' separates values when the decimal mark is ',', like WPF
		static IEnumerable<TestCaseData> GermanValues()
		{
			yield return new TestCaseData("1,5", new SizeF(1.5f, 1.5f));
			yield return new TestCaseData("3; 4,25", new SizeF(3f, 4.25f));
			yield return new TestCaseData("0,5; -2,75", new PointF(0.5f, -2.75f));
			yield return new TestCaseData("1; 2; 3,5; 4,25", new RectangleF(1f, 2f, 3.5f, 4.25f));
			yield return new TestCaseData("1,5; 2,5", new PaddingF(1.5f, 2.5f));
			yield return new TestCaseData("3; 4", new Size(3, 4));
			yield return new TestCaseData("1; 2; 3; 4", new Padding(1, 2, 3, 4));
		}

		static sc.TypeConverter Converter(object value) => sc.TypeDescriptor.GetConverter(value.GetType());

		static void WithCulture(CultureInfo culture, Action action)
		{
			var oldCulture = CultureInfo.CurrentCulture;
			try
			{
				CultureInfo.CurrentCulture = culture;
				action();
			}
			finally
			{
				CultureInfo.CurrentCulture = oldCulture;
			}
		}

		[TestCaseSource(nameof(InvariantValues)), SetCulture("de-DE")]
		public void InvariantCultureShouldIgnoreCurrentCulture(string text, object expected)
		{
			var converter = Converter(expected);
			Assert.That(converter.ConvertFromInvariantString(text), Is.EqualTo(expected), "#1");
			var written = converter.ConvertToInvariantString(expected);
			Assert.That(converter.ConvertFromInvariantString(written), Is.EqualTo(expected), "#2 " + written);
		}

		[TestCaseSource(nameof(GermanValues)), SetCulture("de-DE")]
		public void CurrentCultureShouldBeUsedWhenNoneGiven(string text, object expected)
		{
			Assert.That(Converter(expected).ConvertFromString(text), Is.EqualTo(expected));
		}

		[TestCaseSource(nameof(GermanValues))]
		public void GivenCultureShouldBeUsed(string text, object expected)
		{
			Assert.That(Converter(expected).ConvertFrom(null, German, text), Is.EqualTo(expected));
		}

		[TestCaseSource(nameof(InvariantValues)), SetCulture("de-DE")]
		public void CurrentCultureShouldRoundTrip(string text, object expected)
		{
			var converter = Converter(expected);
			var written = converter.ConvertToString(expected);
			Assert.That(converter.ConvertFromString(written), Is.EqualTo(expected), written);
		}

		[Test]
		public void ConvertToShouldUseGivenCulture()
		{
			var converter = Converter(new SizeF());
			Assert.That(converter.ConvertTo(null, German, new SizeF(1.5f, 2f), typeof(string)), Is.EqualTo("1,5;2"), "#1");
			Assert.That(converter.ConvertTo(null, CultureInfo.InvariantCulture, new SizeF(1.5f, 2f), typeof(string)), Is.EqualTo("1.5,2"), "#2");
		}

		[TestCaseSource(nameof(InvariantValues)), SetCulture("de-DE")]
		public void XamlConvertersShouldIgnoreCurrentCulture(string text, object expected)
		{
			var result = typeof(XamlConverters).GetMethod(expected.GetType().Name).Invoke(null, new object[] { text });
			Assert.That(result, Is.EqualTo(expected));
		}

		[Test]
		public void XamlShouldLoadInvariantValues() => Invoke(() => WithCulture(German, () =>
		{
			var xaml = $"<Panel xmlns='{XamlReader.EtoFormsNamespace}' Padding='1, 2, 3, 4' />";
			var panel = XamlReader.Load<Panel>(new StringReader(xaml), null);
			Assert.That(panel.Padding, Is.EqualTo(new Padding(1, 2, 3, 4)));
		}));

		[Test]
		public void JsonShouldLoadInvariantValues() => Invoke(() => WithCulture(German, () =>
		{
			var json = "{ $type: 'Panel', Padding: '1, 2, 3, 4' }";
			var panel = JsonReader.Load<Panel>(new MemoryStream(Encoding.UTF8.GetBytes(json)));
			Assert.That(panel.Padding, Is.EqualTo(new Padding(1, 2, 3, 4)));
		}));

		[Test, SetCulture("de-DE")]
		public void DashStyleShouldIgnoreCurrentCulture()
		{
			var style = DashStyle.Parse("0.5, 1.5, 2.25");
			Assert.That(style.Offset, Is.EqualTo(0.5f), "#1");
			Assert.That(style.Dashes, Is.EqualTo(new[] { 1.5f, 2.25f }), "#2");
		}

		[TestCase("10.5", "")]
		[TestCase("10,5", "de-DE")]
		public void FontSizeShouldUseGivenCulture(string size, string cultureName) => Invoke(() => WithCulture(German, () =>
		{
			var font = (Font)sc.TypeDescriptor.GetConverter(typeof(Font)).ConvertFrom(null, CultureInfo.GetCultureInfo(cultureName), SystemFonts.Default().FamilyName + "+" + size);
			Assert.That(font.Size, Is.EqualTo(10.5f));
		}));
	}
}
