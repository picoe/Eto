using System;
using Eto.Drawing;
using NUnit.Framework;

namespace Eto.Test.UnitTests.Drawing
{
	[TestFixture]
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

		[TestCase((uint)0x00000000)]
		[TestCase((uint)0xFF000000)]
		[TestCase((uint)0xFFFF0000)]
		[TestCase((uint)0xFF00FF00)]
		[TestCase((uint)0xFF0000FF)]
		[TestCase((uint)0xFFFFFFFF)]
		[TestCase((uint)0x33330000)]
		[TestCase((uint)0x33003300)]
		[TestCase((uint)0x33000033)]
		[TestCase((uint)0x33333333)]
		public void ColorToHsbShouldNotHaveNan(uint rgb)
		{
			var color = Color.FromArgb(unchecked((int)rgb));
			var hsb = color.ToHSB();
			Assert.AreNotEqual(double.NaN, hsb.A, "#1. A is NaN");
			Assert.AreNotEqual(double.NaN, hsb.H, "#2. H is NaN");
			Assert.AreNotEqual(double.NaN, hsb.S, "#3. S is NaN");
			Assert.AreNotEqual(double.NaN, hsb.B, "#4. B is NaN");
		}

		[TestCase((uint)0x00000000)]
		[TestCase((uint)0xFF000000)]
		[TestCase((uint)0xFFFF0000)]
		[TestCase((uint)0xFF00FF00)]
		[TestCase((uint)0xFF0000FF)]
		[TestCase((uint)0xFFFFFFFF)]
		[TestCase((uint)0x33330000)]
		[TestCase((uint)0x33003300)]
		[TestCase((uint)0x33000033)]
		[TestCase((uint)0x33333333)]
		public void ColorToHslShouldNotHaveNan(uint rgb)
		{
			var color = Color.FromArgb(unchecked((int)rgb));
			var hsb = color.ToHSL();
			Assert.AreNotEqual(double.NaN, hsb.A, "#1. A is NaN");
			Assert.AreNotEqual(double.NaN, hsb.H, "#2. H is NaN");
			Assert.AreNotEqual(double.NaN, hsb.S, "#3. S is NaN");
			Assert.AreNotEqual(double.NaN, hsb.L, "#4. L is NaN");
		}

		[TestCase((uint)0x00000000)]
		[TestCase((uint)0xFF000000)]
		[TestCase((uint)0xFFFF0000)]
		[TestCase((uint)0xFF00FF00)]
		[TestCase((uint)0xFF0000FF)]
		[TestCase((uint)0xFFFFFFFF)]
		[TestCase((uint)0x33330000)]
		[TestCase((uint)0x33003300)]
		[TestCase((uint)0x33000033)]
		[TestCase((uint)0x33333333)]
		public void ColorToCmykShouldNotHaveNan(uint rgb)
		{
			var color = Color.FromArgb(unchecked((int)rgb));
			var hsb = color.ToCMYK();
			Assert.AreNotEqual(double.NaN, hsb.A, "#1. A is NaN");
			Assert.AreNotEqual(double.NaN, hsb.C, "#2. C is NaN");
			Assert.AreNotEqual(double.NaN, hsb.M, "#3. M is NaN");
			Assert.AreNotEqual(double.NaN, hsb.Y, "#4. Y is NaN");
			Assert.AreNotEqual(double.NaN, hsb.K, "#4. K is NaN");
		}
	}
}