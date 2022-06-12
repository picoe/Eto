using System;
using System.Threading.Tasks;
using Eto.Drawing;
using Eto.Forms;
using NUnit.Framework;

namespace Eto.Test.UnitTests.Forms
{
	[TestFixture]
	public class FontDialogTests : TestBase
	{
		/// <summary>
		/// Fonts the dialog from modal dialog should work.
		/// 
		/// On macOS, the dialog returns immediately after ShowDialog because it is not a modal form.
		/// We need to make sure it still fires the event.
		/// </summary>
		[Test, ManualTest]
		public void FontDialogFromModalDialogShouldWork()
		{
			bool wasChanged = false;
			Font selectedFont;
			FontDialog fd; // don't let this GC until after the test
			ManualDialog("Click on the label and change the font", form =>
			{
				selectedFont = SystemFonts.User();
				var label = new Label { Text = selectedFont.FamilyName, TextColor = SystemColors.Highlight };

				label.MouseDown += (sender, e) =>
				{
					fd = new FontDialog();
					fd.Font = selectedFont;
					fd.FontChanged += (sender2, e2) =>
					{
						selectedFont = fd.Font;
						label.Text = fd.Font.FamilyName;
						wasChanged = true;
					};

					fd.ShowDialog(label);
					Application.Instance.AsyncInvoke(() =>
					{
						GC.Collect();
						GC.WaitForPendingFinalizers();
					});
				};


				return new StackLayout { Items = { label, new TextArea() }, Padding = 10 };
			});

			Assert.IsTrue(wasChanged, "#1 - Font was not changed!");
		}

		[Test, ManualTest]
		public void FontDialogFromFormShouldWork()
		{
			bool wasChanged = false;
			Font selectedFont;
			FontDialog fd = null; // don't let this GC until after the test
			ManualForm("Click on the label and change the font", form =>
			{
				selectedFont = SystemFonts.User();
				var label = new Label { Text = selectedFont.FamilyName, TextColor = SystemColors.Highlight };

				label.MouseDown += (sender, e) =>
				{
					fd = new FontDialog();
					fd.Font = selectedFont;
					fd.FontChanged += (sender2, e2) =>
					{
						selectedFont = fd.Font;
						label.Text = fd.Font.FamilyName;
						wasChanged = true;
					};
					fd.ShowDialog(label);

					Application.Instance.AsyncInvoke(() =>
					{
						GC.Collect();
						GC.WaitForPendingFinalizers();
					});
				};


				return new StackLayout { Items = { label, new TextArea() }, Padding = 10 };
			});

			Assert.IsTrue(wasChanged, "#1 - Font was not changed!");
		}
	}
}
