using Eto.Drawing;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eto.Test.UnitTests.Drawing
{
	[TestFixture]
	public class FontTests : TestBase
	{
		[Test]
		public async Task FontShouldWorkInMultipleThreads()
		{
			var bmp = new Bitmap(100, 20, PixelFormat.Format32bppRgba);
			var font = Fonts.Sans(10, FontStyle.Italic, FontDecoration.Underline);
			using (var g = new Graphics(bmp))
			{
				g.DrawText(font, Colors.Blue, 0, 0, "Some Text");
			}

			await Task.Run(() =>
			{
				bmp = new Bitmap(100, 20, PixelFormat.Format32bppRgba);
				using (var g = new Graphics(bmp))
				{
					g.DrawText(font, Colors.Blue, 0, 0, "Some Text");
				}
			});
		}

		[Test]
		public void FontMeasureStringShouldWorkForMultiLineStrings() => Invoke(() =>
		{
			var font = Fonts.Sans(10);
			var singleLineSize = font.MeasureString("A single-line string!");
			var multiLineSize = font.MeasureString("A\nmulti\nline\nstring!");
			Console.WriteLine($"Single-line: {singleLineSize}, Multi-line: {multiLineSize}");
			Assert.Greater(multiLineSize.Height, singleLineSize.Height, "#1 The multi-line string does not have a greater height than the single line");
			Assert.Less(multiLineSize.Width, singleLineSize.Width, "#2 The multi-line string should not be as wide as the single line string");
		});
	}
}
