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

		[TestCase("#000", 255, 0, 0, 0)]
		[TestCase("#123", 255, 17, 34, 51)]
		[TestCase("#FFF", 255, 255, 255, 255)]
		[TestCase("#000000", 255, 0, 0, 0)]
		[TestCase("#123456", 255, 18, 52, 86)]
		[TestCase("#FFFFFF", 255, 255, 255, 255)]
		[TestCase("0, 0, 0", 255, 0, 0, 0)]
		[TestCase("12, 34, 56", 255, 12, 34, 56)]
		[TestCase("12 34 56", 255, 12, 34, 56)]
		[TestCase("rgb(12, 34, 56)", 255, 12, 34, 56)]
		[TestCase("255, 255, 255", 255, 255, 255, 255)]

		[TestCase("#0000", 0, 0, 0, 0)]
		[TestCase("#1234", 17, 34, 51, 68)]
		[TestCase("#FFFF", 255, 255, 255, 255)]
		[TestCase("#00000000", 0, 0, 0, 0)]
		[TestCase("#12345678", 18, 52, 86, 120)]
		[TestCase("#FFFFFFFF", 255, 255, 255, 255)]
		[TestCase("0, 0, 0, 0", 0, 0, 0, 0)]
		[TestCase("12, 34, 56, 78", 12, 34, 56, 78)]
		[TestCase("12 34 56 78", 12, 34, 56, 78)]
		[TestCase("255, 255, 255, 255", 255, 255, 255, 255)]
		[TestCase("rgba(12,34,56,0.5)", 127, 12, 34, 56)]
		[TestCase("rgba(50%,20%,100%,0.3)", 76, 127, 51, 255)]
		[TestCase("rgba(50%,20.5%,100%,0.3)", 76, 127, 52, 255)]
		[TestCase("0", 0, 0, 0, 0)]
		[TestCase("4294967295", 255, 255, 255, 255)] // #FFFFFFFF
		[TestCase("16777215", 0, 255, 255, 255)] // #FFFFFF
		[TestCase("305419896", 18, 52, 86, 120)] // #12345678 A = 12

		[TestCase("#0000", 255, 0, 0, 0, ColorStyles.ExcludeAlpha)]
		[TestCase("#1234", 255, 34, 51, 68, ColorStyles.ExcludeAlpha)]
		[TestCase("#00000000", 255, 0, 0, 0, ColorStyles.ExcludeAlpha)]
		[TestCase("#12345678", 255, 52, 86, 120, ColorStyles.ExcludeAlpha)]
		[TestCase("0, 0, 0, 0", 255, 0, 0, 0, ColorStyles.ExcludeAlpha)]
		[TestCase("12, 34, 56, 78", 255, 34, 56, 78, ColorStyles.ExcludeAlpha)]
		[TestCase("255, 255, 255, 255", 255, 255, 255, 255, ColorStyles.ExcludeAlpha)]
		[TestCase("rgba(12,34,56,0.5)", 255, 12, 34, 56, ColorStyles.ExcludeAlpha)] // alpha always last
		[TestCase("rgba(50%,20%,100%,0.3)", 255, 127, 51, 255, ColorStyles.ExcludeAlpha)]
		[TestCase("rgba(50%,20.5%,100%,0.3)", 255, 127, 52, 255, ColorStyles.ExcludeAlpha)]
		[TestCase("0", 255, 0, 0, 0, ColorStyles.ExcludeAlpha)]
		[TestCase("4294967295", 255, 255, 255, 255, ColorStyles.ExcludeAlpha)] // #FFFFFFFF
		[TestCase("16777215", 255, 255, 255, 255, ColorStyles.ExcludeAlpha)] // #FFFFFF
		[TestCase("305419896", 255, 52, 86, 120, ColorStyles.ExcludeAlpha)] // #12345678 A = 12

		[TestCase("#0000", 0, 0, 0, 0, ColorStyles.AlphaLast)]
		[TestCase("#1234", 68, 17, 34, 51, ColorStyles.AlphaLast)]
		[TestCase("#00000000", 0, 0, 0, 0, ColorStyles.AlphaLast)]
		[TestCase("#12345678", 120, 18, 52, 86, ColorStyles.AlphaLast)]
		[TestCase("0, 0, 0, 0", 0, 0, 0, 0, ColorStyles.AlphaLast)]
		[TestCase("12, 34, 56, 78", 78, 12, 34, 56, ColorStyles.AlphaLast)]
		[TestCase("255, 255, 255, 255", 255, 255, 255, 255, ColorStyles.AlphaLast)]
		[TestCase("rgba(12,34,56,0.3)", 76, 12, 34, 56, ColorStyles.AlphaLast)]
		[TestCase("rgba(50%,20%,100%,0.3)", 76, 127, 51, 255, ColorStyles.AlphaLast)]
		[TestCase("rgba(50%,20.5%,100%,0.3)", 76, 127, 52, 255, ColorStyles.AlphaLast)]
		[TestCase("0", 0, 0, 0, 0, ColorStyles.AlphaLast)]
		[TestCase("4294967295", 255, 255, 255, 255, ColorStyles.AlphaLast)] // #FFFFFFFF
		[TestCase("16777215", 255, 0, 255, 255, ColorStyles.AlphaLast)] // #FFFFFF
		[TestCase("305419896", 120, 18, 52, 86, ColorStyles.AlphaLast)] // #12345678 A = 78
		public void ColorShouldParse(string text, int a, int r, int g, int b, ColorStyles? style = null)
		{
			Color color;
			var result = style == null ? Color.TryParse(text, out color) : Color.TryParse(text, out color, style.Value);
			Assert.IsTrue(result, "#1 - Color could not be parsed from text");

			Assert.AreEqual(a, color.Ab, "#2.1 - Alpha component is incorrect");
			Assert.AreEqual(r, color.Rb, "#2.2 - Red component is incorrect");
			Assert.AreEqual(g, color.Gb, "#2.3 - Green component is incorrect");
			Assert.AreEqual(b, color.Bb, "#2.4 - Blue component is incorrect");
		}

		[TestCase("#0000", 0, 0, 0, 0, ColorStyles.ShortHex)]
		[TestCase("#1234", 17, 34, 51, 68, ColorStyles.ShortHex)]
		[TestCase("#FFFF", 255, 255, 255, 255, ColorStyles.ShortHex)]
		[TestCase("#12345678", 18, 52, 86, 120, ColorStyles.ShortHex)]
		[TestCase("#00000000", 0, 0, 0, 0)]
		[TestCase("#12345678", 18, 52, 86, 120)]
		[TestCase("#FFFFFFFF", 255, 255, 255, 255)]

		[TestCase("#000", 255, 0, 0, 0, ColorStyles.ExcludeAlpha | ColorStyles.ShortHex)]
		[TestCase("#234", 255, 34, 51, 68, ColorStyles.ExcludeAlpha | ColorStyles.ShortHex)]
		[TestCase("#345678", 255, 52, 86, 120, ColorStyles.ExcludeAlpha | ColorStyles.ShortHex)]
		[TestCase("#000000", 255, 0, 0, 0, ColorStyles.ExcludeAlpha)]
		[TestCase("#345678", 255, 52, 86, 120, ColorStyles.ExcludeAlpha)]

		[TestCase("#0000", 0, 0, 0, 0, ColorStyles.AlphaLast | ColorStyles.ShortHex)]
		[TestCase("#1234", 68, 17, 34, 51, ColorStyles.AlphaLast | ColorStyles.ShortHex)]
		[TestCase("#12345678", 120, 18, 52, 86, ColorStyles.AlphaLast | ColorStyles.ShortHex)]
		[TestCase("#00000000", 0, 0, 0, 0, ColorStyles.AlphaLast)]
		[TestCase("#12345678", 120, 18, 52, 86, ColorStyles.AlphaLast)]
		public void ColorToHexShouldBeCorrect(string text, int a, int r, int g, int b, ColorStyles? style = null)
		{
			var color = Color.FromArgb(r, g, b, a);
			var value = style != null ? color.ToHex(style.Value) : color.ToHex();
			Assert.AreEqual(text, value, "#1 Hex value incorrect");
		}
	}
}