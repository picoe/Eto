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
	public class BrushTests : TestBase
	{
		[Test]
		public async Task SolidBrushShouldWorkInMultipleThreads()
		{
			await BrushTest(new SolidBrush(Colors.Blue));
		}

		[Test]
		public async Task LinearGradientBrushShouldWorkInMultipleThreads()
		{
			await BrushTest(new LinearGradientBrush(Colors.Blue, Colors.Green, new PointF(0, 0), new PointF(30, 30)));
		}

		[Test]
		public async Task RadialGradientBrushShouldWorkInMultipleThreads()
		{
			await BrushTest(new RadialGradientBrush(Colors.Blue, Colors.Green, new PointF(10, 10), new PointF(15, 15), new SizeF(15, 15)));
		}

		[Test]
		public async Task TextureBrushShouldWorkInMultipleThreads()
		{
			await BrushTest(new TextureBrush(TestIcons.Logo));
		}

		async Task BrushTest(Brush brush)
		{
			// just test that it doesn't crash at this point (for WPF), no actual output test yet.
			var bmp = new Bitmap(30, 30, PixelFormat.Format32bppRgba);
			using (var g = new Graphics(bmp))
			{
				g.FillRectangle(brush, 0, 0, 10, 10);
			}

			await Task.Run(() =>
			{
				bmp = new Bitmap(30, 30, PixelFormat.Format32bppRgba);
				using (var g = new Graphics(bmp))
				{
					g.FillRectangle(brush, 0, 0, 10, 10);
				}
			});
		}

		[Test]
		public void LinearGradientBrushShouldFillWithRectangleAndAngle()
		{
			var bmp = new Bitmap(100, 100, PixelFormat.Format32bppRgba);
			using (var g = new Graphics(bmp))
			{
				var brush = new LinearGradientBrush(
					new Rectangle(0, 0, 100, 100),
					Colors.Blue,
					Colors.Green,
					0);
				GraphicsPath path = new GraphicsPath();
				path.AddLines(new PointF(0, 0), new PointF(100, 100), new PointF(0, 100));
				path.CloseFigure();
				g.FillPath(brush, path);
			}

			// start out mostly blue
			var startPixel = bmp.GetPixel(1, 2);
			Assert.LessOrEqual(startPixel.Rb, 10, "#1.1");
			Assert.LessOrEqual(startPixel.Gb, 10, "#1.2");
			Assert.GreaterOrEqual(startPixel.Bb, 10, "#1.3");

			// end mostly green
			var endPixel = bmp.GetPixel(98, 99);
			Assert.LessOrEqual(endPixel.Rb, 10, "#2.1");
			Assert.GreaterOrEqual(endPixel.Gb, 80, "#2.2");
			Assert.LessOrEqual(endPixel.Bb, 10, "#2.3");
		}

	}
}
