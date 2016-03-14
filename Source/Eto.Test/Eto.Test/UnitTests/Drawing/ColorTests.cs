using System;
using Eto.Drawing;
using NUnit.Framework;

namespace Eto.Test.UnitTests.Drawing
{
	[TestFixture, Category(TestBase.TestPlatformCategory)]
	public class ColorTests
	{
		[TestCase(unchecked((int)0xFFAABBCC))]
		[TestCase(unchecked((int)0xFF000000))]
		[TestCase(unchecked((int)0xFFFFFFFF))]
		[TestCase(unchecked((int)0x22AABBCC))]
		[TestCase(unchecked((int)0x22000000))]
		[TestCase(unchecked((int)0x22FFFFFF))]
		[TestCase((int)0x00000000)]
		[TestCase((int)0x00AABBCC)]
		[TestCase((int)0x00FFFFFF)]
		public void ToArgbShouldRoundtrip(int argb)
		{
			var color = Color.FromArgb(argb);
			Assert.AreEqual(argb, color.ToArgb(), "Color {0} does not roundtrip", argb);
		}

		[TestCase(unchecked((int)0xFFAABBCC))]
		[TestCase(unchecked((int)0xFF000000))]
		[TestCase(unchecked((int)0xFFFFFFFF))]
		[TestCase(unchecked((int)0x22AABBCC))]
		[TestCase(unchecked((int)0x22000000))]
		[TestCase(unchecked((int)0x22FFFFFF))]
		[TestCase((int)0x00000000)]
		[TestCase((int)0x00AABBCC)]
		[TestCase((int)0x00FFFFFF)]
		public void ToRgbShouldRoundtrip(int rgb)
		{
			var color = Color.FromRgb(rgb);
			Assert.AreEqual(color.Ab, 255, "Alpha should be 255 when using Color.FromRgb");
			Assert.AreEqual(rgb & 0xFFFFFF, color.ToArgb() & 0xFFFFFF, "Color {0} does not roundtrip", rgb);
		}
	}
}

