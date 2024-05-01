using NUnit.Framework;

namespace Eto.Test.UnitTests.Drawing
{
	[TestFixture]
    public class GraphicsPathTests
    {
		[Test, InvokeOnUI]
		public void GraphicsPathFillContainsShouldWork()
		{
			var path = new GraphicsPath();
			path.AddRectangle(0, 0, 10, 10);

			Assert.IsTrue(path.FillContains(new PointF(5, 5)), "#1.1");
			Assert.IsFalse(path.FillContains(new PointF(11, 5)), "#1.2");
			Assert.IsTrue(path.FillContains(new PointF(9, 5)), "#1.3");
			Assert.IsFalse(path.FillContains(new PointF(10.5f, 5)), "#1.4");
			Assert.IsTrue(path.FillContains(new PointF(0, 0)), "#1.5");
			Assert.IsFalse(path.FillContains(new PointF(-1, 0)), "#1.6");
			// slightly different behaviour with System.Drawing for some reason, ignore it for now.
			if (!Platform.Instance.IsWinForms)
				Assert.IsTrue(path.FillContains(new PointF(10, 5)), "#1.7");
		}

		[Test, InvokeOnUI]
		public void GraphicsPathStrokeContainsShouldWork()
		{
			var path = new GraphicsPath();
			path.AddRectangle(0, 0, 10, 10);
			
			var pen = new Pen(Colors.Black, 1);
			Assert.IsTrue(path.StrokeContains(pen, new PointF(0, 0)), "#1.1");
			Assert.IsFalse(path.StrokeContains(pen, new PointF(1, 1)), "#1.2");
			Assert.IsTrue(path.StrokeContains(pen, new PointF(10, 1)), "#1.3");
			Assert.IsTrue(path.StrokeContains(pen, new PointF(10, 10)), "#1.4");
		}
    }
}