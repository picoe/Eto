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
	public class GraphicsTests
	{
		[Test]
		public void DefaultValuesShouldBeCorrect()
		{
			TestBase.Paint((drawable, e) =>
			{
				var graphics = e.Graphics;

				Assert.AreEqual(PixelOffsetMode.None, graphics.PixelOffsetMode, "PixelOffsetMode should default to None");
				Assert.AreEqual(true, graphics.AntiAlias, "AntiAlias should be true");
				Assert.AreEqual(ImageInterpolation.Default, graphics.ImageInterpolation, "ImageInterpolation should be default");
			});
		}
	}
}
