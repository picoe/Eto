using NUnit.Framework;
namespace Eto.Test.UnitTests.Drawing
{
	[TestFixture]
	public class IconTests : TestBase
	{
		[TestCase(.25f, .5f)]
		[TestCase(1f, 1f)]
		[TestCase(1.5f, 1.5f)]
		[TestCase(1.75f, 2f)]
		[TestCase(2f, 2f)]
		[TestCase(4f, 4f)]
		[TestCase(5f, 4f)]
		public void IconShouldSupportMultipleResolutions(float scale, float expectedResult)
		{
			var icon = TestIcons.Logo;

			Assert.That(icon, Is.Not.Null, "#1");
			var expectedScales = new [] { 0.5f, 1f, 1.5f, 2f, 4f };

			Assert.That(icon.Frames.Count(), Is.EqualTo(expectedScales.Length), "#2 - Should be a representation for each image with @<scale>");

			Assert.That(icon.Frames.Select(r => r.Scale).OrderBy(r => r), Is.EqualTo(expectedScales), "#3 - scales weren't loaded");

			Assert.That(icon.GetFrame(scale).Scale, Is.EqualTo(expectedResult), "#4");
		}

		[Test]
		public void IconFromIcoShouldSetFrames()
		{
			var icon = TestIcons.TestIcon;

			Assert.That(icon, Is.Not.Null, "#1");

			Assert.That(icon.Frames.Count(), Is.EqualTo(5), "#2");

			var sizes = new []
			{
				new Size(16, 16),
				new Size(32, 32),
				new Size(48, 48),
				new Size(64, 64),
				new Size(128, 128),
			};
			Assert.That(icon.Frames.Select(r => r.PixelSize), Is.EquivalentTo(sizes), "#3");

			Assert.That(icon.Frames.All(r => r.Scale == 1), Is.True, "#4");
		}

		[TestCase(.50f, 64, null)]
		[TestCase(.25f, 32, null)]
		[TestCase(1, 128, null)]
		[TestCase(2, 128, 64)]
		[TestCase(2, 128, null)]
		public void GetFrameWithScaleShouldWorkWithIco(float scale, int expectedSize, int? fittingSize)
		{
			var icon = TestIcons.TestIcon;

			// sanity check
			Assert.That(icon, Is.Not.Null, "#1");
			Assert.That(icon.Frames.Count(), Is.EqualTo(5), "#2");
			Assert.That(icon.Frames.All(r => r.Scale == 1), Is.True, "#5");

			var fs = fittingSize != null ? (Size?)new Size(fittingSize.Value, fittingSize.Value) : null;
			Assert.That(icon.GetFrame(scale, fs).PixelSize, Is.EqualTo(new Size(expectedSize, expectedSize)), "");
		}

		[TestCase("Some.File.That.Does.Not.Exist.png")]
		[TestCase("Some.File.That.Does.Not.Exist.ico")]
		public void InvalidResourceShouldThrowException(string resourceName)
		{
			Assert.Throws<ArgumentException>(() => Icon.FromResource(resourceName));
		}

		[Test]
		public void DrawingManyIconsShouldNotCrash()
		{
			// on WPF, some resources like RenderTargetBitmap use up GDI handles (that are limited)
			// when drawing an icon with a different size.
			// without a GC, it would cause a crash.  
			// When drawing the same size icon, we now cache the result so it doesn't get out of control
			using (var icon = TestIcons.Logo)
			using (var bmp = new Bitmap(100, 100, PixelFormat.Format32bppRgba))
			using (var g = new Graphics(bmp))
				for (int i = 0; i < 10000; i++)
				{
					g.DrawImage(icon, 0, 0, 50, 50);
				}
		}

		[TestCase("Eto.Test.Images.LogoWith288DPI.png", 128, 128)]
		[TestCase("Eto.Test.Images.Logo.png", 128, 128)]
		public void BitmapDpiShouldNotAffectIconSize(string resourceName, int width, int height)
		{
			var icon = Icon.FromResource(resourceName);
			Assert.That(icon.Size.Width, Is.EqualTo(width), "Icon width is incorrect");
			Assert.That(icon.Size.Height, Is.EqualTo(height), "Icon width is incorrect");

			int i = 0;
			foreach (var frame in icon.Frames)
			{
				i++;
				Assert.That(frame.Size.Width, Is.EqualTo(width), $"Frame #{i} with scale {frame.Scale} does not match icon width");
				Assert.That(frame.Size.Height, Is.EqualTo(height), $"Frame #{i} with scale {frame.Scale} does not match icon height");
			}
		}

		[Test]
		public void BitmapToIconShouldNotChangeBitmapSize()
		{
			var bmp = TestIcons.LogoBitmap;
			var oldSize = bmp.Size;

			var icon = bmp.WithSize(32, 32);

			Assert.That(oldSize, Is.EqualTo(bmp.Size), "#1");
			Assert.That(icon.Size, Is.EqualTo(new Size(32, 32)), "#2");
			Assert.That(icon.Frames.First().PixelSize, Is.EqualTo(bmp.Size), "#3");
		}

		[Test]
		public void IconFromBackgroundThreadShouldBeUsable()
		{
			// we are running tests in a background thread already, just generate it there.
			var icon = TestIcons.TestIcon;

			// test showing it on a form
			Shown(f => new ImageView { Image = icon });
		}

		[Test, ManualTest]
		public void UsingDisposedMemoryStreamShouldNotCrashAndShowImage()
		{
			ManualForm("Image should be shown, and the window icon\nshould also be set (for platforms that support it)", form => {
				Icon icon;
				using (var ms = new MemoryStream())
				{
					// use a seperate memory stream that we dispose
					GetType().Assembly.GetManifestResourceStream("Eto.Test.Images.TestIcon.ico").CopyTo(ms);
					ms.Position = 0;
					icon = new Icon(ms);
				}
				var imageView = new ImageView();

				imageView.Image = icon;

				form.Icon = icon;

				return imageView;
			});
		}
	}
}

