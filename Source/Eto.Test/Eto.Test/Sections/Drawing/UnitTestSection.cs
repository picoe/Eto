using Eto.Drawing;
using Eto.Forms;
using Eto.Test.UnitTests;

namespace Eto.Test.Sections.Drawing
{
	public class UnitTestSection : Scrollable
	{
		public UnitTestSection()
		{
			var layout = new DynamicLayout(this);
			var button = new Button { Text = "Start Tests" };
			var drawable = new Drawable();
			layout.BeginVertical(xscale: true);
			layout.Add(drawable, yscale: true);
			layout.Add(button);
			layout.EndVertical();
			
			// Run the tests in a Paint callback
			var startTests = false;
			
			button.Click += (s, e) => {
				startTests = true;
				drawable.Invalidate();
			};

			drawable.Paint += (s, e) => {
				if (startTests)
				{
					startTests = false;
					button.Enabled = false;
					// run the tests
					try
					{
						new TestRunner().RunTests<DrawingTests>(() => new DrawingTests { 
							Drawable = drawable,
							Graphics = e.Graphics });
					}
					finally
					{
						button.Enabled = true;
					}
				}
			};
		}
	}

	public class DrawingTests : TestFixture
	{
		public Drawable Drawable { get; set; }
		public Graphics Graphics { get; set; }

		[UnitTest]
		public void ClipTest()
		{
			Assert.AreEqual("Verifying clipbounds size", (Size)Graphics.ClipBounds.Size, Drawable.ClientSize);
			
			// Clip to the upper-left quadrant
			var clipTo = Drawable.ClientSize / 2;
			Graphics.SetClip(new RectangleF(PointF.Empty, clipTo));

			// Translate to the bottom-right quadrant
			Graphics.TranslateTransform(new Point(clipTo));
						
			// Check that the clip region was correctly translated
			var clip = Graphics.ClipBounds;
			var expectedClip = new RectangleF(-new Point(clipTo), clipTo);
			Assert.AreEqual("Verifying clip after translation ", expectedClip, clip);
		}
	}
}
