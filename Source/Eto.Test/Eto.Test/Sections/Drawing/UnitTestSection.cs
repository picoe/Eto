using Eto.Drawing;
using Eto.Forms;
using Eto.Test.UnitTests;

namespace Eto.Test.Sections.Drawing
{

	public class DrawingTestSection : Scrollable
	{
		public DrawingTestSection()
		{
			var layout = new DynamicLayout(this);
			var button = new Button { Text = "Start Tests" };
			var drawable = new Drawable();
			layout.BeginVertical(xscale: true);
			layout.Add(drawable, yscale: true);
			layout.Add(button);
			layout.EndVertical();

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
			Assert.AreEqual((Size)Graphics.ClipBounds.Size, Drawable.ClientSize);
		}
	}
}
