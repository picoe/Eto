using NUnit.Framework;

namespace Eto.Test.UnitTests.Forms.Controls
{
	[TestFixture]
	public class DrawableTests : TestBase
	{
		[Test, ManualTest]
		public void DrawableWithCanFocusShouldGetFirstMouseDownOnInactiveWindow()
		{
			bool wasClicked = false;
			bool gotFocusBeforeClick = false;
			Form(form =>
			{

				var drawable = new Drawable();
				var font = SystemFonts.Default();
				drawable.Paint += (sender, e) =>
				{
					e.Graphics.FillRectangle(Colors.Blue, 0, 0, drawable.Width, drawable.Height);
					e.Graphics.DrawText(font, SystemColors.ControlText, 0, 0, "Clicking once on this control should close the form");
				};
				drawable.Size = new Size(350, 200);
				drawable.CanFocus = true;
				drawable.MouseDown += (sender, e) =>
				{
					wasClicked = true;
					form.Close();
				};
				form.Content = drawable;
				form.GotFocus += (sender, e) =>
				{
					Application.Instance.AsyncInvoke(() =>
					{
						if (!wasClicked)
							gotFocusBeforeClick = true;
					});
				};

				form.ShowActivated = false;
				form.Owner = Application.Instance.MainForm;
			}, -1);

			Assert.IsTrue(wasClicked, "#1 Drawable didn't get clicked");
			Assert.IsFalse(gotFocusBeforeClick, "#2 Form should not have got focus before MouseDown event");
		}

		[Test]
		public void DrawableWithWrappedLabelShouldAutoSizeToConstraints()
		{
			Form form = null;
			Label labelThatShouldWrap = null;
			Shown(f =>
			{
				form = f;
				labelThatShouldWrap = new Label
				{
					Text = "This is some label that should wrap and show you the entire string including the period at the end."
				};

				var drawable = new Drawable
				{
					Content = labelThatShouldWrap
				};

				form.Content = drawable;
				form.Width = 100;
				form.WindowStyle = WindowStyle.Utility;
			},
			() =>
			{
				Assert.GreaterOrEqual(labelThatShouldWrap.Height, 20, "#1 - Label should have wrapped!");

				Assert.AreEqual(100, form.Width, "#2 - Form width should be 100");
			}
			);
		}

	}
}