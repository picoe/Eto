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
	public class BitmapTests : TestBase
	{
		public BitmapTests()
		{
			// initialize test generator if running through IDE or nunit-gui
			TestBase.Initialize();
		}

		[Test]
		public void TestClone32Bit()
		{
			var image = TestIcons.Textures;
			var clone = image.Clone();
			ValidateImages(image, clone);
		}

		[Test]
		public void TestClone8BitIndexed()
		{
			var image = TestIcons.TexturesIndexed;
			var clone = image.Clone();
			ValidateImages(image, clone);
		}

		[TestCase(0, 0, 128, 128)]
		[TestCase(32, 32, 64, 64)]
		[TestCase(32, 32, 96, 64)]
		[TestCase(32, 32, 64, 96)]
		[TestCase(0, 0, 96, 64)]
		[TestCase(0, 0, 64, 96)]
		public void TestClone32BitRectangle(int x, int y, int width, int height)
		{
			var image = TestIcons.Textures;
			var rect = new Rectangle(x, y, width, height);
			var clone = image.Clone(rect);
			ValidateImages(image, clone, rect);
		}

		[TestCase(0, 0, 128, 128)]
		[TestCase(32, 32, 64, 64)]
		[TestCase(32, 32, 96, 64)]
		[TestCase(32, 32, 64, 96)]
		[TestCase(0, 0, 96, 64)]
		[TestCase(0, 0, 64, 96)]
		public void TestClone8BitIndexedRectangle(int x, int y, int width, int height)
		{
			var image = TestIcons.TexturesIndexed;
			var rect = new Rectangle(x, y, width, height);
			var clone = image.Clone(rect);
			ValidateImages(image, clone, rect);
		}

		[TestCase(1.0f, 0.0f, 0.0f)]
		[TestCase(0.0f, 1.0f, 0.0f)]
		[TestCase(0.0f, 0.0f, 1.0f)]
		[TestCase(0.0f, 1.0f, 1.0f)]
		[TestCase(1.0f, 0.0f, 1.0f)]
		[TestCase(1.0f, 1.0f, 0.0f)]
		public void TestGetPixelWithLock24bit(float red, float green, float blue)
		{
			var colorSet = new Color(red, green, blue);

			var image = new Bitmap(new Size(1, 1), PixelFormat.Format24bppRgb);
			image.SetPixel(0, 0, colorSet);

			using (var data = image.Lock())
			{
				var colorGet = data.GetPixel(0, 0);

				if (colorSet != colorGet)
				{
					Assert.Fail("Pixels are not the same (SetPixel: {0}, GetPixel: {1})", colorSet, colorGet);
				}
			}
		}

		[TestCase(1.0f, 0.0f, 0.0f)]
		[TestCase(0.0f, 1.0f, 0.0f)]
		[TestCase(0.0f, 0.0f, 1.0f)]
		[TestCase(0.0f, 1.0f, 1.0f)]
		[TestCase(1.0f, 0.0f, 1.0f)]
		[TestCase(1.0f, 1.0f, 0.0f)]
		public void TestGetPixelWithoutLock24bit(float red, float green, float blue)
		{
			var colorSet = new Color(red, green, blue);

			var image = new Bitmap(new Size(1, 1), PixelFormat.Format24bppRgb);
			image.SetPixel(0, 0, colorSet);

			var colorGet = image.GetPixel(0, 0);

			if (colorSet != colorGet)
			{
				Assert.Fail("Pixels are not the same (SetPixel: {0}, GetPixel: {1})", colorSet, colorGet);
			}
		}

		[TestCase(1.0f, 0.0f, 0.0f, 1.0f)]
		[TestCase(0.0f, 1.0f, 0.0f, 0.5f)]
		[TestCase(0.0f, 0.0f, 1.0f, 0.0f)]
		[TestCase(0.0f, 1.0f, 1.0f, 1.0f)]
		[TestCase(1.0f, 0.0f, 1.0f, 0.5f)]
		[TestCase(1.0f, 1.0f, 0.0f, 0.0f)]
		public void TestGetPixelWithLock32bit(float red, float green, float blue, float alpha)
		{
			var colorSet = new Color(red, green, blue, alpha);

			var image = new Bitmap(new Size(1, 1), PixelFormat.Format32bppRgba);
			image.SetPixel(0, 0, colorSet);

			using (var data = image.Lock())
			{
				var colorGet = data.GetPixel(0, 0);

				if (colorSet != colorGet)
				{
					Assert.Fail("Pixels are not the same (SetPixel: {0}, GetPixel: {1})", colorSet, colorGet);
				}
			}
		}

		[TestCase(1.0f, 0.0f, 0.0f, 1.0f)]
		[TestCase(0.0f, 1.0f, 0.0f, 0.5f)]
		[TestCase(0.0f, 0.0f, 1.0f, 0.0f)]
		[TestCase(0.0f, 1.0f, 1.0f, 1.0f)]
		[TestCase(1.0f, 0.0f, 1.0f, 0.5f)]
		[TestCase(1.0f, 1.0f, 0.0f, 0.0f)]
		public void TestGetPixelWithoutLock32bit(float red, float green, float blue, float alpha)
		{
			var colorSet = new Color(red, green, blue, alpha);

			var image = new Bitmap(new Size(1, 1), PixelFormat.Format32bppRgba);
			image.SetPixel(0, 0, colorSet);

			var colorGet = image.GetPixel(0, 0);

			if (colorSet != colorGet)
			{
				Assert.Fail("Pixels are not the same (SetPixel: {0}, GetPixel: {1})", colorSet, colorGet);
			}
		}

		static void ValidateImages(Bitmap image, Bitmap clone, Rectangle? rect = null)
		{
			var testRect = rect ?? new Rectangle(image.Size);
			using (var imageData = image.Lock())
			using (var cloneData = clone.Lock())
			{
				if (imageData.BytesPerPixel == 1)
				{
					unsafe
					{
						// test pixels directly
						byte* imageptr = (byte*)imageData.Data;
						imageptr += testRect.Top * imageData.ScanWidth + testRect.Left;
						byte* cloneptr = (byte*)cloneData.Data;
						for (int y = 0; y < testRect.Height; y++)
						{
							byte* imagerow = imageptr;
							byte* clonerow = cloneptr;
							for (int x = 0; x < testRect.Width; x++)
							{
								var imagePixel = *(imagerow++);
								var clonePixel = *(clonerow++);
								if (imagePixel != clonePixel)
								{
									Assert.Fail("Image pixels are not the same at position {0},{1} (source: {2}, clone: {3})", x, y, imagePixel, clonePixel);
								}
							}
							imageptr += imageData.ScanWidth;
							cloneptr += cloneData.ScanWidth;
						}
					}
				}
				else
					for (int x = 0; x < testRect.Width; x++)
						for (int y = 0; y < testRect.Height; y++)
						{
							var imagePixel = imageData.GetPixel(x + testRect.Left, y + testRect.Top);
							var clonePixel = cloneData.GetPixel(x, y);
							if (imagePixel != clonePixel)
								Assert.Fail("Image pixels are not the same at position {0},{1} (source: {2}, clone: {3})", x, y, imagePixel, clonePixel);
						}
			}
		}

		[Test]
		public void BitmapFromBackgroundThreadShouldBeUsable()
		{
			// we are running tests in a background thread already, just generate it there.
			var bmp = new Bitmap(100, 100, PixelFormat.Format32bppRgba);
			using (var g = new Graphics(bmp))
			{
				g.DrawLine(Colors.Blue, 0, 0, 100, 100);
			}

			// test showing it on a form
			Shown(f => new ImageView { Image = bmp });
		}

		[Test]
		public void BitmapShouldBeEditableFromUIThread()
		{
			// we are running tests in a background thread already, just generate it there.
			var bmp = new Bitmap(100, 100, PixelFormat.Format32bppRgba);
			// test showing it on a form
			Shown(
				f => {
					using (var g = new Graphics(bmp))
					{
						g.DrawLine(Colors.Blue, 0, 0, 100, 100);
					}

					return new ImageView { Image = bmp };
				});
		}

		[Test]
		public async Task BitmapShouldAllowMultipleThreads()
		{
			var bmp = new Bitmap(30, 30, PixelFormat.Format32bppRgba);
			await Task.Run(() =>
			{
				using (var g = new Graphics(bmp))
				{
					g.FillRectangle(Colors.Blue, 0, 0, 10, 10);
				}
			});
			await Task.Run(() =>
			{
				using (var g = new Graphics(bmp))
				{
					g.FillRectangle(Colors.Green, 10, 0, 10, 10);
				}
			});

			await Task.Run(() =>
			{
				using (var bd = bmp.Lock())
				{
					for (int y = 0; y < 10; y++)
						for (int x = 20; x < 30; x++)
							bd.SetPixel(x, y, Colors.Red);
				}
			});

			// test output in test thread
			Assert.AreEqual(Colors.Blue, bmp.GetPixel(0, 0), "#1");
			Assert.AreEqual(Colors.Green, bmp.GetPixel(10, 0), "#2");
			Assert.AreEqual(Colors.Red, bmp.GetPixel(20, 0), "#3");

			using (var bd = bmp.Lock())
			{
				Assert.AreEqual(Colors.Blue, bd.GetPixel(0, 0), "#4");
				Assert.AreEqual(Colors.Green, bd.GetPixel(10, 0), "#5");
				Assert.AreEqual(Colors.Red, bd.GetPixel(20, 0), "#6");
			}
			Shown(f => new ImageView { Image = bmp }, 
				iv => {

				// also test in UI thread
				Assert.AreEqual(Colors.Blue, bmp.GetPixel(0, 0), "#7");
				Assert.AreEqual(Colors.Green, bmp.GetPixel(10, 0), "#8");
				Assert.AreEqual(Colors.Red, bmp.GetPixel(20, 0), "#9");

				using (var bd = bmp.Lock())
				{
					Assert.AreEqual(Colors.Blue, bd.GetPixel(0, 0), "#10");
					Assert.AreEqual(Colors.Green, bd.GetPixel(10, 0), "#11");
					Assert.AreEqual(Colors.Red, bd.GetPixel(20, 0), "#12");
				}
			});
		}

		[Test]
		public void LockShouldThrowIfCalledWhileLocked()
		{
			Invoke(() =>
			{
				var bmp = new Bitmap(30, 30, PixelFormat.Format32bppRgba);

				using (var bd = bmp.Lock())
				{
					Assert.Throws<InvalidOperationException>(() => bmp.Lock());
				}
			});
		}

		[Test]
		public void LockShouldNowThrowIfCalledSequentially()
		{
			Invoke(() =>
			{
				var bmp = new Bitmap(30, 30, PixelFormat.Format32bppRgba);

				using (var bd = bmp.Lock())
				{
					for (int y = 0; y < 10; y++)
						for (int x = 0; x < 10; x++)
							bd.SetPixel(x, y, Colors.Red);
				}
			
				using (var bd = bmp.Lock())
				{
					for (int y = 0; y < 10; y++)
						for (int x = 10; x < 20; x++)
							bd.SetPixel(x, y, Colors.Blue);
				}

				// sanity check
				Assert.AreEqual(Colors.Red, bmp.GetPixel(0, 0));
				Assert.AreEqual(Colors.Blue, bmp.GetPixel(10, 0));
			});
		}

		[TestCase(PixelFormat.Format24bppRgb, 265, 16)]
		[TestCase(PixelFormat.Format32bppRgb, 265, 16)]
		[TestCase(PixelFormat.Format32bppRgba, 265, 16)]
		[TestCase(PixelFormat.Format24bppRgb, 256, 16)]
		[TestCase(PixelFormat.Format32bppRgb, 256, 16)]
		[TestCase(PixelFormat.Format32bppRgba, 256, 16)]
		public void LockShouldSetPixelsCorrectly(PixelFormat format, int width, int height)
		{
			Invoke(() =>
			{
				var bitmap = new Bitmap(width, height, format);
				// use Lock() to set pixels
				using (var bd = bitmap.Lock())
				{
					for (int i = 0; i < width; ++i)
					{
						for (int j = 0; j < height; ++j)
						{
							var c = j < height / 2 ? Colors.Red : Colors.Green;
							bd.SetPixel(i, j, c);
						}
					}
				}

				// use GetPixel() to get the pixel to verify (which is typically not implemented using lock())
				for (int i = 0; i < width; ++i)
				{
					for (int j = 0; j < height; ++j)
					{
						var c = j < height / 2 ? Colors.Red : Colors.Green;
						Assert.AreEqual(c, bitmap.GetPixel(i, j), $"Pixel at {i},{j} is incorrect");
					}
				}
			});
		}

		[TestCase(PixelFormat.Format24bppRgb, 265, 16)]
		[TestCase(PixelFormat.Format32bppRgb, 265, 16)]
		[TestCase(PixelFormat.Format32bppRgba, 265, 16)]
		[TestCase(PixelFormat.Format24bppRgb, 256, 16)]
		[TestCase(PixelFormat.Format32bppRgb, 256, 16)]
		[TestCase(PixelFormat.Format32bppRgba, 256, 16)]
		public void LockShouldGetPixelsCorrectly(PixelFormat format, int width, int height)
		{
			Invoke(() =>
			{
				var bitmap = new Bitmap(width, height, format);
				for (int i = 0; i < width; ++i)
				{
					for (int j = 0; j < height; ++j)
					{
						var c = j < height / 2 ? Colors.Red : Colors.Green;
						bitmap.SetPixel(i, j, c);
					}
				}

				using (var bd = bitmap.Lock())
				{
					for (int i = 0; i < width; ++i)
					{
						for (int j = 0; j < height; ++j)
						{
							var c = j < height / 2 ? Colors.Red : Colors.Green;
							Assert.AreEqual(c, bd.GetPixel(i, j), $"Pixel at {i},{j} is incorrect");
						}
					}
				}
			});
		}

		[Test]
		public void SizeShouldBeInPixels()
		{
			Invoke(() =>
			{
				Assert.AreEqual(new Size(128, 128), TestIcons.Logo288Bitmap.Size);

				Assert.AreEqual(new Size(128, 128), TestIcons.LogoBitmap.Size);
			});
		}
	}
}
