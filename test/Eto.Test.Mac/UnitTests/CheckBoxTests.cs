using System;
using Eto.Drawing;
using Eto.Forms;
using Eto.Mac.Forms.Controls;
using Eto.Test.UnitTests;
using NUnit.Framework;

#if MONOMAC
using MonoMac.AppKit;
using MonoMac.CoreGraphics;
#else
using AppKit;
using CoreGraphics;
#endif

namespace Eto.Test.Mac.UnitTests
{
	[TestFixture]
	public class CheckBoxTests : TestBase
	{
		[Test, ManualTest]
		public void ButtonShouldNotBeClipped()
		{
			ManualForm("All buttons should be fully visible without any clipping", form =>
			{
				form.Styles.Add<CheckBoxHandler>("small", c =>
				{
					c.Control.Cell.ControlSize = NSControlSize.Small;
					c.Control.Font = NSFont.SystemFontOfSize(NSFont.SystemFontSizeForControlSize(NSControlSize.Small));
				});
				form.Styles.Add<CheckBoxHandler>("mini", c =>
				{
					c.Control.Cell.ControlSize = NSControlSize.Mini;
					c.Control.Font = NSFont.SystemFontOfSize(NSFont.SystemFontSizeForControlSize(NSControlSize.Mini));
				});

				return new TableLayout
				{
					Rows =
					{
						new Panel { Height = 10, BackgroundColor = Colors.White },
						new TableLayout(new TableRow(null, new CheckBox { Text = "Normal" }, null)),
						new Panel { Height = 10, BackgroundColor = Colors.White },
						new TableLayout(new TableRow(null, new CheckBox { Text = "Small", Style = "small" }, null)),
						new Panel { Height = 10, BackgroundColor = Colors.White },
						new TableLayout(new TableRow(null, new CheckBox { Text = "Mini", Style = "mini" }, null)),
						new Panel { Height = 10, BackgroundColor = Colors.White },
					}
				};
			});
		}
	}
}