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

	}
}
