using NUnit.Framework;

namespace Eto.Test.UnitTests.Drawing
{
	[TestFixture]
	public class ClipTests
	{
		[Test]
		public void ClipBoundsShouldMatchClientSize()
		{
			var size = new Size(300, 300);
			TestBase.Paint((drawable, e) =>
			{
				var graphics = e.Graphics;
				Assert.That(drawable.ClientSize, Is.EqualTo(size), "Drawable client size should be 300x300");
				Assert.That(Size.Round(graphics.ClipBounds.Size), Is.EqualTo(Size.Round(drawable.ClientSize)), "Clip bounds should match drawable client size");
			}, size);
		}

		[Test]
		public void ClipRectangleShouldTranslate()
		{
			TestBase.Paint((drawable, e) =>
			{
				var graphics = e.Graphics;
				// Clip to the upper-left quadrant
				var clipTo = drawable.ClientSize / 2;
				graphics.SetClip(new RectangleF(PointF.Empty, clipTo));

				// Translate to the bottom-right quadrant
				graphics.TranslateTransform(new Point(clipTo));

				// Check that the clip region was correctly translated
				var clip = graphics.ClipBounds;
				var expectedClip = new RectangleF(-new Point(clipTo), clipTo);
				Assert.That(Rectangle.Round(clip), Is.EqualTo(Rectangle.Round(expectedClip)), "Clip rectangle wasn't translated properly");
			});
		}
	}
}
