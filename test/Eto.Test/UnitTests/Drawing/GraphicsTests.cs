using NUnit.Framework;
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

				Assert.That(graphics.PixelOffsetMode, Is.EqualTo(PixelOffsetMode.None), "PixelOffsetMode should default to None");
				Assert.That(graphics.AntiAlias, Is.EqualTo(true), "AntiAlias should be true");
				Assert.That(graphics.ImageInterpolation, Is.EqualTo(ImageInterpolation.Default), "ImageInterpolation should be default");
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
				Assert.That(bd.GetPixel(0, 0), Is.EqualTo(Colors.Red), "#1");
				Assert.That(bd.GetPixel(10, 0), Is.EqualTo(Colors.Green), "#2");
				Assert.That(bd.GetPixel(20, 0), Is.EqualTo(Colors.Blue), "#3");
				Assert.That(bd.GetPixel(30, 0), Is.EqualTo(Colors.Yellow), "#4");
			}
		}

		/// <summary>
		/// Ensure that ImageInterpolation can be set to different values in one context
		/// </summary>
		[Test]
		public void ImageInterpolationShouldBeIndependent()
		{
			// let's create a teeny tiny bitmap
			var bmp = new Bitmap(2, 2, PixelFormat.Format32bppRgb);
			using (var bd = bmp.Lock())
			{
				// b b
				// r b
				bd.SetPixel(0, 0, Colors.Blue);
				bd.SetPixel(1, 0, Colors.Blue);
				bd.SetPixel(1, 1, Colors.Blue);
				bd.SetPixel(0, 1, Colors.Red);
			}

			// now, let's draw it and ensure it isn't interpolated
			var bmp2 = new Bitmap(200, 200, PixelFormat.Format32bppRgb);
			using (var g = new Graphics(bmp2))
			{
				g.PixelOffsetMode = PixelOffsetMode.None;
				g.ImageInterpolation = ImageInterpolation.Default;
				g.DrawImage(bmp, 0, 0, 100, 100);
				g.ImageInterpolation = ImageInterpolation.None;
				g.DrawImage(bmp, 100, 0, 100, 100);
				g.ImageInterpolation = ImageInterpolation.Default;
				g.DrawImage(bmp, 0, 100, 100, 100);
				g.ImageInterpolation = ImageInterpolation.None;
				g.DrawImage(bmp, 100, 100, 100, 100);
			}

			/* Show output for debugging: *
			Application.Instance.Invoke(() =>
			{
				var dlg = new Dialog { Content = new ImageView { Image = bmp2 } };
				dlg.ShowModal();
			});
			/**/

			void hasBlue(Color c)
			{
				Assert.That(c.B, Is.GreaterThan(0));
				Assert.That(c.G, Is.EqualTo(0));
				Assert.That(c.R, Is.EqualTo(0));
			}

			void hasRed(Color c)
			{
				Assert.That(c.B, Is.EqualTo(0));
				Assert.That(c.G, Is.EqualTo(0));
				Assert.That(c.R, Is.GreaterThan(0));
			}

			void hasRedAndBlue(Color c)
			{
				Assert.That(c.B, Is.GreaterThan(0));
				Assert.That(c.G, Is.EqualTo(0));
				Assert.That(c.R, Is.GreaterThan(0));
			}

			void checkInterpolated(BitmapData bd, int x, int y)
			{
				// upper left should be blue ish
				hasBlue(bd.GetPixel(x + 0, y + 0));
				hasBlue(bd.GetPixel(x + 99, y + 0));

				// check the middle (ish) of the lower left corner which should both red and blue components
				hasRedAndBlue(bd.GetPixel(x + 25, y + 50));

				hasRed(bd.GetPixel(x + 0, y + 99));
			}

			void checkNonInterpolated(BitmapData bd, int x, int y)
			{
				Assert.That(bd.GetPixel(x + 0, y + 0), Is.EqualTo(Colors.Blue));
				Assert.That(bd.GetPixel(x + 99, y + 0), Is.EqualTo(Colors.Blue));
				Assert.That(bd.GetPixel(x + 50, y + 50), Is.EqualTo(Colors.Blue));
				Assert.That(bd.GetPixel(x + 50, y + 49), Is.EqualTo(Colors.Blue));
				Assert.That(bd.GetPixel(x + 49, y + 49), Is.EqualTo(Colors.Blue));
				Assert.That(bd.GetPixel(x + 49, y + 51), Is.EqualTo(Colors.Red));
				Assert.That(bd.GetPixel(x + 0, y + 99), Is.EqualTo(Colors.Red));
			}

			using (var bd = bmp2.Lock())
			{
				// default, should be interpolated 50,50 is somewhere inbetween red and blue
				checkInterpolated(bd, 0, 0);

				// no interpolation in upper right
				checkNonInterpolated(bd, 100, 0);

				// default again, should be interpolated 50,150 is somewhere inbetween red and blue
				checkInterpolated(bd, 0, 100);

				// no interpolation again in lower right
				checkNonInterpolated(bd, 100, 100);
			}
		}
	}
}
