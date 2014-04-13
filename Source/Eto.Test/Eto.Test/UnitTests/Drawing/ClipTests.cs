using Eto.Drawing;
using Eto.Forms;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Reflection;
using System.Diagnostics;
using System.IO;
using System.Security.Policy;
using System.Security;
using System.Security.Permissions;
using System.Threading.Tasks;

namespace Eto.Test.UnitTests.Drawing
{
	[TestFixture]
	public class ClipTests
	{
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
