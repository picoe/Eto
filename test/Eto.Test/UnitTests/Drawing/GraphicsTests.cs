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
	public class GraphicsTests
	{
		[Test]
		public void DefaultValuesShouldBeCorrect()
		{
			TestBase.Paint((drawable, e) =>
			{
				var graphics = e.Graphics;

				Assert.AreEqual(PixelOffsetMode.None, graphics.PixelOffsetMode, "PixelOffsetMode should default to None");
				Assert.AreEqual(true, graphics.AntiAlias, "AntiAlias should be true");
				Assert.AreEqual(ImageInterpolation.Default, graphics.ImageInterpolation, "ImageInterpolation should be default");
			});
		}

		[Test]
		public void AntiAliasShouldNotInterfereWithTransform()
		{
			var bmp = new Bitmap(40, 10, PixelFormat.Format32bppRgba);
			using (var g = new Graphics(bmp))
			{
				g.AntiAlias = true;
				g.FillRectangle(Colors.Red, 0, 0, 10, 10);

				g.AntiAlias = false;
				g.TranslateTransform(10, 0);
				g.FillRectangle(Colors.Green, 0, 0, 10, 10);

				g.AntiAlias = true;
				g.TranslateTransform(10, 0);
				g.FillRectangle(Colors.Blue, 0, 0, 10, 10);

				g.AntiAlias = false;
				g.TranslateTransform(10, 0);
				g.FillRectangle(Colors.Yellow, 0, 0, 10, 10);
			}
			using (var bd = bmp.Lock())
			{
				Assert.AreEqual(Colors.Red, bd.GetPixel(0, 0), "#1");
				Assert.AreEqual(Colors.Green, bd.GetPixel(10, 0), "#2");
				Assert.AreEqual(Colors.Blue, bd.GetPixel(20, 0), "#3");
				Assert.AreEqual(Colors.Yellow, bd.GetPixel(30, 0), "#4");
			}
		}
	}
}
