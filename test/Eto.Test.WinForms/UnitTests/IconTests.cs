using Eto.Test.UnitTests;
using Eto.WinForms.Forms;
using NUnit.Framework;

namespace Eto.Test.WinForms.UnitTests
{
	[TestFixture]
	public class IconTests : TestBase
	{
		static Bitmap CreateBitmap(int size)
		{
			var bmp = new Bitmap(size, size, PixelFormat.Format32bppRgba);
			using (var g = new Graphics(bmp))
				g.FillEllipse(Colors.Green, 0, 0, size, size);
			return bmp;
		}

		[Test]
		public void IconFromFramesShouldSetAllSizesOnWindow() => Invoke(() =>
		{
			var sizes = new[] { 16, 32, 64, 256 };
			var icon = new Icon(sizes.Select(r => new IconFrame(1, CreateBitmap(r))));
			var form = new Form { Icon = icon };
			try
			{
				var native = ((FormHandler)form.Handler).Control.Icon;
				Assert.That(native, Is.Not.Null, "#1 window should have a native icon");

				byte[] data;
				using (var stream = new MemoryStream())
				{
					native.Save(stream);
					data = stream.ToArray();
				}
				var count = BitConverter.ToInt16(data, 4);
				var widths = Enumerable.Range(0, count).Select(i => data[6 + 16 * i] == 0 ? 256 : data[6 + 16 * i]);
				Assert.That(widths, Is.EquivalentTo(sizes), "#2 every frame should be in the icon");

				// System.Drawing never matches 256px entries when picking a size, though Windows does
				foreach (var size in sizes.Where(r => r < 256))
				{
					using (var sized = new sd.Icon(native, size, size))
					using (var bitmap = sized.ToBitmap())
					{
						Assert.That(bitmap.Width, Is.EqualTo(size), $"#3.{size} should have a frame for this size");
						Assert.That(bitmap.GetPixel(0, 0).A, Is.EqualTo(0), $"#4.{size} corner should be transparent");
						var center = bitmap.GetPixel(size / 2, size / 2);
						Assert.That(center.A, Is.EqualTo(255), $"#5.{size} center should be opaque");
						Assert.That(center.G, Is.GreaterThan(center.R), $"#6.{size} center should be green");
					}
				}
			}
			finally
			{
				form.Close();
			}
		});

		[Test]
		public void IconFromFramesLargerThanIcoShouldSetWindowIcon() => Invoke(() =>
		{
			var icon = new Icon(1, CreateBitmap(512));
			var form = new Form { Icon = icon };
			try
			{
				var native = ((FormHandler)form.Handler).Control.Icon;
				Assert.That(native, Is.Not.Null, "#1 window should have a native icon");
				Assert.That(native.Width, Is.EqualTo(512), "#2");
			}
			finally
			{
				form.Close();
			}
		});
	}
}
