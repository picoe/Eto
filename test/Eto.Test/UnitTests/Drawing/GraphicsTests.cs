using Eto.Drawing;
using Eto.Forms;
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
				Assert.Greater(c.B, 0);
				Assert.AreEqual(0, c.G);
				Assert.AreEqual(0, c.R);
			}

			void hasRed(Color c)
			{
				Assert.AreEqual(0, c.B);
				Assert.AreEqual(0, c.G);
				Assert.Greater(c.R, 0);
			}

			void hasRedAndBlue(Color c)
			{
				Assert.Greater(c.B, 0);
				Assert.AreEqual(0, c.G);
				Assert.Greater(c.R, 0);
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
				Assert.AreEqual(Colors.Blue, bd.GetPixel(x + 0, y + 0));
				Assert.AreEqual(Colors.Blue, bd.GetPixel(x + 99, y + 0));
				Assert.AreEqual(Colors.Blue, bd.GetPixel(x + 50, y + 50));
				Assert.AreEqual(Colors.Blue, bd.GetPixel(x + 50, y + 49));
				Assert.AreEqual(Colors.Blue, bd.GetPixel(x + 49, y + 49));
				Assert.AreEqual(Colors.Red, bd.GetPixel(x + 49, y + 51));
				Assert.AreEqual(Colors.Red, bd.GetPixel(x + 0, y + 99));
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
