using Eto.Serialization.Xaml;
using NUnit.Framework;

namespace Eto.Test.UnitTests.Forms.Xaml
{
	[TestFixture]
	public class XamlConvertersTests
	{
		static IEnumerable<TestCaseData> Values()
		{
			yield return new TestCaseData(typeof(Color), "Red");
			yield return new TestCaseData(typeof(Color), "#80FF0000");
			yield return new TestCaseData(typeof(Color), "10,20,30");
			yield return new TestCaseData(typeof(Size), "5, 6");
			yield return new TestCaseData(typeof(Size), "7");
			yield return new TestCaseData(typeof(SizeF), "1.5,2");
			yield return new TestCaseData(typeof(SizeF), "2.5");
			yield return new TestCaseData(typeof(Point), "-1,2");
			yield return new TestCaseData(typeof(PointF), "0.5,0.25");
			yield return new TestCaseData(typeof(Rectangle), "1,2,3,4");
			yield return new TestCaseData(typeof(RectangleF), "1,2,3.5,4");
			yield return new TestCaseData(typeof(Padding), "10");
			yield return new TestCaseData(typeof(Padding), "1,2");
			yield return new TestCaseData(typeof(Padding), "1,2,3,4");
			yield return new TestCaseData(typeof(PaddingF), "1.5,2");
			yield return new TestCaseData(typeof(Keys), "CommonModifier+Q");
			yield return new TestCaseData(typeof(Keys), "Control+Shift+O");
		}

		static object Convert(Type type, string text)
			=> typeof(XamlConverters).GetMethod(type.Name).Invoke(null, new object[] { text });

		[TestCaseSource(nameof(Values))]
		public void ShouldMatchTypeConverter(Type type, string text)
		{
			var expected = sc.TypeDescriptor.GetConverter(type).ConvertFrom(text);
			Assert.That(Convert(type, text), Is.EqualTo(expected));
		}

		[TestCase(typeof(Size), "1,2,3")]
		[TestCase(typeof(Padding), "1,2,3")]
		[TestCase(typeof(Rectangle), "a,b,c,d")]
		[TestCase(typeof(Color), "notacolor")]
		public void InvalidValuesShouldThrowLikeTypeConverter(Type type, string text)
		{
			Assert.Throws<ArgumentException>(() => sc.TypeDescriptor.GetConverter(type).ConvertFrom(text));
			var ex = Assert.Throws<TargetInvocationException>(() => Convert(type, text));
			Assert.That(ex.InnerException, Is.InstanceOf<ArgumentException>());
		}
	}
}
