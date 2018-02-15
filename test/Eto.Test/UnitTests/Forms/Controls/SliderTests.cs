using System;
using NUnit.Framework;
using Eto.Forms;

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
				Assert.AreEqual(0, slider.TickFrequency);
				slider.Value = 10;
				slider.TickFrequency = 20;
				Assert.AreEqual(20, slider.TickFrequency);
				Assert.AreEqual(10, slider.Value);
			});
		}
	}
}
