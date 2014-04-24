using Eto.Drawing;
using NUnit.Framework;

namespace Eto.Test.UnitTests.Drawing
{
	[TestFixture]
	public class ClipTests
	{
		// TODO:Fix: This is currently broken, as there is no UI
		// associated with Eto.Test.dll.
		// This could just be a method, not a test, that can then be invoked 
		// from test code in a platform test assembly.
		[Ignore] 
		[Test]
		public void ClipTest()
		{
			FormTester.Paint((drawable, e) =>
			{
				var graphics = e.Graphics;
				Assert.AreEqual(Size.Round(drawable.ClientSize), Size.Round(graphics.ClipBounds.Size), "Clip bounds should match drawable client size");

				// Clip to the upper-left quadrant
				var clipTo = drawable.ClientSize / 2;
				graphics.SetClip(new RectangleF(PointF.Empty, clipTo));

				// Translate to the bottom-right quadrant
				graphics.TranslateTransform(new Point(clipTo));

				// Check that the clip region was correctly translated
				var clip = graphics.ClipBounds;
				var expectedClip = new RectangleF(-new Point(clipTo), clipTo);
				Assert.AreEqual(Rectangle.Round(expectedClip), Rectangle.Round(clip), "Clip rectangle wasn't translated properly");
			});
		}
	}
}
