using System;
using Eto.Drawing;
using Eto.Forms;
using Eto.Mac.Forms.Controls;
using Eto.Test.UnitTests;
using NUnit.Framework;
#if XAMMAC2
using AppKit;
using CoreGraphics;
#else
using MonoMac.AppKit;
using MonoMac.CoreGraphics;
#if SDCOMPAT
using CGSize = System.Drawing.SizeF;
using CGRect = System.Drawing.RectangleF;
using CGPoint = System.Drawing.PointF;
#endif
#endif

namespace Eto.Test.Mac.UnitTests
{
	[TestFixture]
	public class RadioButtonTests : TestBase
	{
		[Test, ManualTest]
		public void ButtonShouldNotBeClipped()
		{
			ManualForm("All buttons should be fully visible without any clipping", form =>
			{
				form.Styles.Add<RadioButtonHandler>("small", c =>
				{
					c.Control.Cell.ControlSize = NSControlSize.Small;
					c.Control.Font = NSFont.SystemFontOfSize(NSFont.SystemFontSizeForControlSize(NSControlSize.Small));
				});
				form.Styles.Add<RadioButtonHandler>("mini", c =>
				{
					c.Control.Cell.ControlSize = NSControlSize.Mini;
					c.Control.Font = NSFont.SystemFontOfSize(NSFont.SystemFontSizeForControlSize(NSControlSize.Mini));
				});

				return new TableLayout
				{
					Rows =
					{
						new Panel { Height = 10, BackgroundColor = Colors.White },
						new TableLayout(new TableRow(null, new RadioButton { Text = "Normal" }, null)),
						new Panel { Height = 10, BackgroundColor = Colors.White },
						new TableLayout(new TableRow(null, new RadioButton { Text = "Small", Style = "small" }, null)),
						new Panel { Height = 10, BackgroundColor = Colors.White },
						new TableLayout(new TableRow(null, new RadioButton { Text = "Mini", Style = "mini" }, null)),
						new Panel { Height = 10, BackgroundColor = Colors.White },
					}
				};
			});
		}
	}
}