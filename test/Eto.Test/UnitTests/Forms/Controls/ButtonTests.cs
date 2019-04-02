using System;
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
	}
}
