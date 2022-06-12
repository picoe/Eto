using System;
using Eto.Drawing;
using Eto.Forms;
using NUnit.Framework;

namespace Eto.Test.UnitTests.Forms.Controls
{
	[TestFixture]
	public class ButtonTests : TestBase
	{
		[Test, ManualTest]
		public void ButtonShouldAlignWithTextBox()
		{
			ManualForm("Buttons should align to the left and right of the text box,\nwithout being clipped.", form =>
			{
				return new TableLayout
				{
					Rows =
					{
						new TableRow(new TableCell(new TextBox { Text = "TextBox"}, true), new TableCell(new Button { Text = "Button 1"}, true)),
						new TableRow(new Button { Text = "Button 2"}),
						null
					}
				};
			});
		}

		[Test, ManualTest]
		public void ButtonShouldNotFlicker()
		{
			// on MacOS the style of the button changes based on the size.
			// This caused the button to flicker constantly by changing the style over and over.
			ManualForm("Button should not flicker and appear as a single style", form =>
			{
				var layout = new TableLayout()
				{
					Padding = 10,
					Spacing = new Size(5, 5),
					Rows =
					{
						new TableRow(new NumericStepper(), new Button{ Text = "Test"})
					}
				};
				return layout;
			});
		}
	}
}
