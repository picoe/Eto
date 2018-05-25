using System;
using Eto.Drawing;
using Eto.Forms;
using NUnit.Framework;

namespace Eto.Test.UnitTests.Forms
{
	[TestFixture]
	public class DialogTests : TestBase
	{
		[Test, ManualTest]
		public void DialogShouldShowContent()
		{
			Invoke(() =>
			{
				var dlg = new Dialog<bool>();

				dlg.Resizable = true;
				dlg.ClientSize = new Size(300, 400);

				dlg.Content = new TableLayout
				{
					Rows = {
						"Some content that should be shown",
						null,
						new Panel {
							Size = new Size(30, 30),
							Content = new Label { Text = "This should be above the buttons", TextColor = Colors.White },
							BackgroundColor = Colors.Blue
						}
					}
				};

				var passButton = new Button { Text = "Pass" };
				passButton.Click += (sender, e) => dlg.Close(true);

				var failButton = new Button { Text = "Fail" };
				failButton.Click += (sender, e) => dlg.Close(false);

				dlg.PositiveButtons.Add(passButton);
				dlg.NegativeButtons.Add(failButton);

				var result = dlg.ShowModal();

				Assert.IsTrue(result);
			}, timeout: -1);
		}
	}
}
