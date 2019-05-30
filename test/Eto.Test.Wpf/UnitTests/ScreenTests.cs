using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Eto.Drawing;
using NUnit.Framework;
using sd = System.Drawing;

namespace Eto.Test.Wpf.UnitTests
{
	[TestFixture]
	public class ScreenTests
	{
		public class TestScreen
		{
			public sd.Rectangle Bounds { get; set; }

			public sd.SizeF LogicalSize => new sd.SizeF(Bounds.Width / LogicalPixelSize, Bounds.Height / LogicalPixelSize);

			public float LogicalPixelSize { get; set; } = 1f;
		}

		public class TestLogicalScreenHelper : LogicalScreenHelper<TestScreen>
		{
			public List<TestScreen> Screens { get; } = new List<TestScreen>();

			public TestScreen Primary { get; set; }

			public override IEnumerable<TestScreen> AllScreens => Screens;

			public override TestScreen PrimaryScreen => Primary;

			public override sd.Rectangle GetBounds(TestScreen screen) => screen.Bounds;

			public override float GetLogicalPixelSize(TestScreen screen) => screen.LogicalPixelSize;

			public override SizeF GetLogicalSize(TestScreen screen) => screen.LogicalSize.ToEto();
		}

		static IEnumerable<object[]> ThreeMonitorSource()
		{
			for (int x = 1; x < 4; x++)
				for (int y = 1; y < 4; y++)
					for (int z = 1; z < 4; z++)
						yield return new object[] { x, y, z };
		}

		static IEnumerable<object[]> TwoMonitorSource()
		{
			for (int x = 1; x < 4; x++)
				for (int y = 1; y < 4; y++)
					yield return new object[] { x, y };
		}

		[TestCaseSource(nameof(ThreeMonitorSource))]
		public void ScreenLocationsShouldBeCorrect1(float pixelSize1, float pixelSize2, float pixelSize3)
		{
			//                --------------
			//  -------------| 1.           |
			// | 3.          |              |
			// |             |              |
			// |             |--------------
			// |------------- 
			// | 2.          |
			// |             |
			// |             |
			//  -------------

			var helper = new TestLogicalScreenHelper();
			helper.Screens.Add(helper.Primary = new TestScreen { Bounds = new sd.Rectangle(0, 0, 1000, 1000), LogicalPixelSize = pixelSize1 });
			helper.Screens.Add(new TestScreen { Bounds = new sd.Rectangle(-1000, 10, 1000, 1000), LogicalPixelSize = pixelSize3 });
			helper.Screens.Add(new TestScreen { Bounds = new sd.Rectangle(-500, 1010, 500, 500), LogicalPixelSize = pixelSize2 });

			Assert.AreEqual(new PointF(0, 0), helper.GetLogicalLocation(helper.Screens[0]));
			Assert.AreEqual(new PointF(-1000 / pixelSize3, 10 / helper.GetMaxLogicalPixelSize()), helper.GetLogicalLocation(helper.Screens[1]));
			Assert.AreEqual(new PointF(-500 / pixelSize2, 1010 / helper.GetMaxLogicalPixelSize()), helper.GetLogicalLocation(helper.Screens[2]));
		}

		[TestCaseSource(nameof(ThreeMonitorSource))]
		public void ScreenLocationsShouldBeCorrect2(float pixelSize1, float pixelSize2, float pixelSize3)
		{
			//  ------------- --------------
			// | 3.          | 1.           |
			// |             |              |
			// |             |              |
			// |------------- --------------
			// | 2.          |
			// |             |
			// |             |
			//  -------------

			var helper = new TestLogicalScreenHelper();
			helper.Screens.Add(helper.Primary = new TestScreen { Bounds = new sd.Rectangle(0, 0, 1000, 1000), LogicalPixelSize = pixelSize1 });
			helper.Screens.Add(new TestScreen { Bounds = new sd.Rectangle(-500, 1000, 500, 500), LogicalPixelSize = pixelSize2 });
			helper.Screens.Add(new TestScreen { Bounds = new sd.Rectangle(-1000, 0, 1000, 1000), LogicalPixelSize = pixelSize3 });

			Assert.AreEqual(new PointF(0, 0), helper.GetLogicalLocation(helper.Screens[0]));
			Assert.AreEqual(new PointF(-500 / pixelSize2, 1000 / pixelSize1), helper.GetLogicalLocation(helper.Screens[1]));
			Assert.AreEqual(new PointF(-1000 / pixelSize3, 0), helper.GetLogicalLocation(helper.Screens[2]));
		}

		[TestCaseSource(nameof(ThreeMonitorSource))]
		public void ScreenLocationsShouldBeCorrect3(float pixelSize1, float pixelSize2, float pixelSize3)
		{
			//  ------------- --------------
			// | 2.          | 3.           |
			// |             |              |
			// |             |              |
			// |------------- --------------
			// | 1.          |
			// |             |
			// |             |
			//  -------------

			var helper = new TestLogicalScreenHelper();
			helper.Screens.Add(helper.Primary = new TestScreen { Bounds = new sd.Rectangle(0, 0, 1000, 1000), LogicalPixelSize = pixelSize1 });
			helper.Screens.Add(new TestScreen { Bounds = new sd.Rectangle(0, -500, 500, 500), LogicalPixelSize = pixelSize2 });
			helper.Screens.Add(new TestScreen { Bounds = new sd.Rectangle(1000, -1000, 1000, 1000), LogicalPixelSize = pixelSize3 });

			Assert.AreEqual(new PointF(0, 0), helper.GetLogicalLocation(helper.Screens[0]));
			Assert.AreEqual(new PointF(0, -500 / pixelSize2), helper.GetLogicalLocation(helper.Screens[1]));
			Assert.AreEqual(new PointF(1000 / pixelSize1, -1000 / pixelSize3), helper.GetLogicalLocation(helper.Screens[2]));
		}

		[TestCaseSource(nameof(ThreeMonitorSource))]
		public void ScreenLocationsShouldBeCorrect4(float pixelSize1, float pixelSize2, float pixelSize3)
		{
			//  -------------
			// | 1.          |
			// |             |
			// |             |
			//  ------------- --------------
			// | 2.          | 3.           |
			// |             |              |
			// |             |              |
			//  ------------- --------------

			var helper = new TestLogicalScreenHelper();
			helper.Screens.Add(helper.Primary = new TestScreen { Bounds = new sd.Rectangle(0, 0, 1000, 1000), LogicalPixelSize = pixelSize1 });
			helper.Screens.Add(new TestScreen { Bounds = new sd.Rectangle(0, 1000, 500, 500), LogicalPixelSize = pixelSize2 });
			helper.Screens.Add(new TestScreen { Bounds = new sd.Rectangle(1000, 1000, 1000, 1000), LogicalPixelSize = pixelSize3 });

			Assert.AreEqual(new PointF(0, 0), helper.GetLogicalLocation(helper.Screens[0]));
			Assert.AreEqual(new PointF(0, 1000 / pixelSize1), helper.GetLogicalLocation(helper.Screens[1]));
			Assert.AreEqual(new PointF(1000 / pixelSize1, 1000 / pixelSize1), helper.GetLogicalLocation(helper.Screens[2]));
		}


		[TestCaseSource(nameof(TwoMonitorSource))]
		public void ScreenLocationsShouldBeCorrect5(float pixelSize1, float pixelSize2)
		{
			//  ------------- --------------
			// | 1.          | 2.           |
			// |             |              |
			// |             |              |
			//  ------------- --------------

			var helper = new TestLogicalScreenHelper();
			helper.Screens.Add(helper.Primary = new TestScreen { Bounds = new sd.Rectangle(0, 0, 1000, 1000), LogicalPixelSize = pixelSize1 });
			helper.Screens.Add(new TestScreen { Bounds = new sd.Rectangle(1000, 0, 1000, 1000), LogicalPixelSize = pixelSize2 });

			Assert.AreEqual(new PointF(0, 0), helper.GetLogicalLocation(helper.Screens[0]));
			Assert.AreEqual(new PointF(1000 / pixelSize1, 0), helper.GetLogicalLocation(helper.Screens[1]));
		}

		[TestCaseSource(nameof(TwoMonitorSource))]
		public void ScreenLocationsShouldBeCorrect6(float pixelSize1, float pixelSize2)
		{
			//  ------------- --------------
			// | 2.          | 1.           |
			// |             |              |
			// |             |              |
			//  ------------- --------------

			var helper = new TestLogicalScreenHelper();
			helper.Screens.Add(helper.Primary = new TestScreen { Bounds = new sd.Rectangle(0, 0, 1000, 1000), LogicalPixelSize = pixelSize1 });
			helper.Screens.Add(new TestScreen { Bounds = new sd.Rectangle(-1000, 0, 1000, 1000), LogicalPixelSize = pixelSize2 });

			Assert.AreEqual(new PointF(0, 0), helper.GetLogicalLocation(helper.Screens[0]));
			Assert.AreEqual(new PointF(-1000 / pixelSize2, 0), helper.GetLogicalLocation(helper.Screens[1]));
		}
	}
}
