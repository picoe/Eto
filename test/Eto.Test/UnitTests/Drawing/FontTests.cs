using Eto.Drawing;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eto.Test.UnitTests.Drawing
{
	[TestFixture]
	public class FontTests
	{
		[Test]
		public async Task FontShouldWorkInMultipleThreads()
		{
			var bmp = new Bitmap(100, 20, PixelFormat.Format32bppRgba);
			var font = Fonts.Sans(10, FontStyle.Italic, FontDecoration.Underline);
			using (var g = new Graphics(bmp))
			{
				g.DrawText(font, Colors.Blue, 0, 0, "Some Text");
			}

			await Task.Run(() =>
			{
				bmp = new Bitmap(100, 20, PixelFormat.Format32bppRgba);
				using (var g = new Graphics(bmp))
				{
					g.DrawText(font, Colors.Blue, 0, 0, "Some Text");
				}
			});
		}
	}
}
