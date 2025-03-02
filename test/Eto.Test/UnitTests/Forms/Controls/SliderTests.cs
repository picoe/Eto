using NUnit.Framework;
namespace Eto.Test.UnitTests.Forms.Controls
{
	[TestFixture]
	public class SliderTests : TestBase
	{
		[Test]
		public void TickFrequencyShouldAllowZero()
		{
			Invoke(() =>
			{
				var slider = new Slider();
				slider.TickFrequency = 0;
				Assert.That(slider.TickFrequency, Is.EqualTo(0));
				slider.Value = 10;
				slider.TickFrequency = 20;
				Assert.That(slider.TickFrequency, Is.EqualTo(20));
				Assert.That(slider.Value, Is.EqualTo(10));
			});
		}
	}
}
