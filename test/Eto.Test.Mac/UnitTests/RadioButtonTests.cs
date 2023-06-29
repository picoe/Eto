using Eto.Mac.Forms.Controls;
using Eto.Test.UnitTests;
using NUnit.Framework;

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