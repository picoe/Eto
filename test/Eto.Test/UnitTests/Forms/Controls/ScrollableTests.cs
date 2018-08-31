using System;
using Eto.Drawing;
using Eto.Forms;
using NUnit.Framework;

namespace Eto.Test.UnitTests.Forms.Controls
{
	[TestFixture]
	public class ScrollableTests : TestBase
	{
		[Test, ManualTest]
		public void TwoScrollablesShouldNotClipControls()
		{
			ManualForm(
				"When resizing on macOS with System Preferences > General > Show Scroll Bars set to 'Always', the scrollbars should not obscure content when resizing the form to a smaller size.\nAlso, the top panel should never get a vertical scroll bar, only horizontal.",
				form =>
			{
				form.ClientSize = new Size(400, -1);
				form.Padding = 20;
				var pixelLayout1 = new PixelLayout { BackgroundColor = Colors.Green };
				pixelLayout1.Add(new Panel { BackgroundColor = Colors.Blue, Size = new Size(200, 30) }, Point.Empty);
				var scrollable1 = new Scrollable { Content = pixelLayout1 };

				var pixelLayout2 = new PixelLayout { BackgroundColor = Colors.Green };
				pixelLayout2.Add(new Panel { BackgroundColor = Colors.Blue, Size = new Size(300, 300) }, Point.Empty);
				var scrollable2 = new Scrollable { Content = pixelLayout2 };
				return new TableLayout
				{
					Rows = {
						scrollable1,
						scrollable2
					}
				};
			});
		}
	}
}
