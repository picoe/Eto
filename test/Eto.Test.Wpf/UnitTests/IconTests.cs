using Eto.Test.UnitTests;
using Eto.Wpf.Forms;
using NUnit.Framework;
using swmi = System.Windows.Media.Imaging;

namespace Eto.Test.Wpf.UnitTests;

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
	public void IconFromFramesShouldSetWindowIcon() => Invoke(() =>
	{
		var sizes = new[] { 16, 32, 64, 256 };
		var icon = new Icon(sizes.Select(r => new IconFrame(1, CreateBitmap(r))));
		var form = new Form { Icon = icon };
		try
		{
			var native = ((FormHandler)form.Handler).Control.Icon as swmi.BitmapFrame;
			Assert.That(native, Is.Not.Null, "#1 window should have a native icon");
			Assert.That(native.Decoder?.Frames.Select(r => r.PixelWidth), Is.EquivalentTo(sizes), "#2 every frame should be in the icon");
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
			var native = ((FormHandler)form.Handler).Control.Icon as swmi.BitmapSource;
			Assert.That(native, Is.Not.Null, "#1 window should have a native icon");
			Assert.That(native.PixelWidth, Is.EqualTo(512), "#2");
		}
		finally
		{
			form.Close();
		}
	});
}
