using Eto.Test.UnitTests;
using NUnit.Framework;

namespace Eto.Test.Wpf.UnitTests
{
	[TestFixture]
	public class BitmapTests : TestBase
	{
		[Test, Timeout(1000)]
		public void CreatingManySmallBitmapsShouldBeFast()
		{
			var sw = new Stopwatch();
			sw.Start();
			for (int i = 0; i < 100; i++)
			{
				var bmp = new Bitmap(20, 20, PixelFormat.Format32bppRgba);

				using (var g = new Graphics(bmp))
				{
					g.Clear(Colors.Blue);
				}
			}
			sw.Stop();
			Console.WriteLine($"Total time: {sw.Elapsed}");
		}

	}
}