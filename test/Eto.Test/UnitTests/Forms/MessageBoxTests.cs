using NUnit.Framework;

namespace Eto.Test.UnitTests.Forms
{
	[TestFixture]
	public class MessageBoxTests : TestBase
	{
		[Test, ManualTest, InvokeOnUI]
		public void OpenFromSecondaryDialogShouldNotChangeItsOrder()
		{
			var btn1 = new Button { Text = "Click Me" };
			btn1.Click += (s, e) =>
			{
				var btn2 = new Button { Text = "Show MessageBox" };
				btn2.Click += (s1, e1) =>
				{
					MessageBox.Show(btn2.ParentWindow, "This is a message.");
				};
				var dlg2 = new Dialog
				{
					Title = "Test MessageBox",
					ClientSize = new Size(200, 200),
					Content = new TableLayout
					{
						Rows = {
							null,
							"This dialog should remain on top\nafter the message box closes",
							TableLayout.AutoSized(btn2, centered: true),
							null
						}
					}
				};
				dlg2.ShowModal(btn1.ParentWindow);
			};
			var dlg1 = new Dialog
			{
				ClientSize = new Size(200, 200),
				Content = TableLayout.AutoSized(btn1, centered: true)
			};
			dlg1.ShowModal(Application.Instance.MainForm);
		}
	}
}
