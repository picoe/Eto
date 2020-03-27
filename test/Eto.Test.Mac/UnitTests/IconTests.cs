using System;
using System.Linq;
using Eto.Drawing;
using Eto.Test.UnitTests;
using NUnit.Framework;
using Eto.Mac;
#if XAMMAC2
using AppKit;
using CoreGraphics;
#else
using MonoMac.AppKit;
using MonoMac.CoreGraphics;
#endif

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
			Assert.AreEqual(oldSize, bitmapRep.Size.ToEtoSize(), "#1");

			var icon = bmp.WithSize(newSize);

			var iconNSImage = icon.ControlObject as NSImage;
			var iconRep = iconNSImage.Representations()[0] as NSBitmapImageRep;

			Assert.AreEqual(bmp.Size, oldSize, "#2.1");
			Assert.AreEqual(newSize, icon.Size, "#2.2");
			Assert.AreEqual(bmp.Size, icon.Frames.First().PixelSize, "#2.3");

			// rep in icon needs the new size
			Assert.AreEqual(newSize, iconRep.Size.ToEtoSize(), "#2.4");

			// rep in bitmap should have the old size still..
			Assert.AreEqual(oldSize, bitmapRep.Size.ToEtoSize(), "#3");

			icon.Dispose();
			bmp.Dispose();
		}
	}
}
