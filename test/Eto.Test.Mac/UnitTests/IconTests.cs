using Eto.Test.UnitTests;
using NUnit.Framework;
using Eto.Mac;

namespace Eto.Test.Mac64.UnitTests
{
	[TestFixture]
	public class IconTests : TestBase
	{
		[Test]
		public void BitmapToIconShouldNotChangeBitmapSize()
		{
			var bmp = TestIcons.LogoBitmap;
			var bitmapNSImage = bmp.ControlObject as NSImage;
			var bitmapRep = bitmapNSImage.Representations()[0] as NSBitmapImageRep;

			var oldSize = bmp.Size;

			var newSize = new Size(32, 32);
			// initial sanity check
			Assert.That(bitmapRep.Size.ToEtoSize(), Is.EqualTo(oldSize), "#1");

			var icon = bmp.WithSize(newSize);

			var iconNSImage = icon.ControlObject as NSImage;
			var iconRep = iconNSImage.Representations()[0] as NSBitmapImageRep;

			Assert.That(oldSize, Is.EqualTo(bmp.Size), "#2.1");
			Assert.That(icon.Size, Is.EqualTo(newSize), "#2.2");
			Assert.That(icon.Frames.First().PixelSize, Is.EqualTo(bmp.Size), "#2.3");

			// rep in icon needs the new size
			Assert.That(iconRep.Size.ToEtoSize(), Is.EqualTo(newSize), "#2.4");

			// rep in bitmap should have the old size still..
			Assert.That(bitmapRep.Size.ToEtoSize(), Is.EqualTo(oldSize), "#3");

			icon.Dispose();
			bmp.Dispose();
		}
	}
}
