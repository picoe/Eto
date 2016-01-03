using System;
using NUnit.Framework;
using Eto.Drawing;
using System.Linq;

namespace Eto.Test.UnitTests.Drawing
{
	[TestFixture]
	public class IconTests
	{
		[TestCase(.25f, .5f)]
		[TestCase(1f, 1f)]
		[TestCase(1.5f, 1.5f)]
		[TestCase(1.75f, 2f)]
		[TestCase(2f, 2f)]
		[TestCase(4f, 3f)]
		public void IconShouldSupportMultipleResolutions(float scale, float expectedResult)
		{
			var icon = Icon.FromResource("Eto.Test.Images.Logo.png");

			Assert.IsNotNull(icon, "#1");
			var expectedScales = new [] { 0.5f, 1f, 1.5f, 2f, 3f };

			Assert.AreEqual(expectedScales.Length, icon.Frames.Count(), "#2 - Should be a representation for each image with @<scale>");

			CollectionAssert.AreEqual(expectedScales, icon.Frames.Select(r => r.Scale).OrderBy(r => r), "#3 - scales weren't loaded");

			Assert.AreEqual(expectedResult, icon.GetFrame(scale).Scale, "#4");
		}

		[Test]
		public void IconFromIcoShouldSetFrames()
		{
			var icon = Icon.FromResource("Eto.Test.Images.TestIcon.ico");

			Assert.IsNotNull(icon, "#1");

			Assert.AreEqual(5, icon.Frames.Count(), "#2");

			var sizes = new []
			{
				new Size(16, 16),
				new Size(32, 32),
				new Size(48, 48),
				new Size(64, 64),
				new Size(128, 128),
			};
			CollectionAssert.AreEquivalent(sizes, icon.Frames.Select(r => r.PixelSize), "#3");

			Assert.IsTrue(icon.Frames.All(r => r.Scale == 1), "#4");
		}

		[TestCase(.50f, 64, null)]
		[TestCase(.25f, 32, null)]
		[TestCase(1, 128, null)]
		[TestCase(2, 128, 64)]
		[TestCase(2, 128, null)]
		public void GetFrameWithScaleShouldWorkWithIco(float scale, int expectedSize, int? fittingSize)
		{
			var icon = Icon.FromResource("Eto.Test.Images.TestIcon.ico");

			// sanity check
			Assert.IsNotNull(icon, "#1");
			Assert.AreEqual(5, icon.Frames.Count(), "#2");
			Assert.IsTrue(icon.Frames.All(r => r.Scale == 1), "#5");

			var fs = fittingSize != null ? (Size?)new Size(fittingSize.Value, fittingSize.Value) : null;
			Assert.AreEqual(new Size(expectedSize, expectedSize), icon.GetFrame(scale, fs).PixelSize, "");
		}
	}
}

