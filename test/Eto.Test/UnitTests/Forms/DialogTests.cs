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

		/// <summary>
		/// Test specific for WPF to ensure minimum size is correct, and the size is reported properly after being set.
		/// 
		/// To test this, you need to set Switch.System.Windows.DoNotScaleForDpiChanges=true in app.config for Eto.Test.Wpf.
		/// </summary>
		[Test, ManualTest]
		public void DialogShouldHaveCorrectMinimumSize()
		{
			Invoke(() =>
			{
				var dlg = new Dialog<bool>();
				Size? sizeAfterSetting = null;
				dlg.Resizable = true;
				// check output of size changed 
				dlg.SizeChanged += (sender, e) => Log.Write(this, $"SizeChanged: {dlg.Size}, {dlg.RestoreBounds}");
				dlg.Size = new Size(300, 300);
				dlg.MinimumSize = new Size(300, 300);
				dlg.LoadComplete += (sender, e) =>
				{
					dlg.Size = new Size(400, 400);

					// get size after load complete is.. uh.. fully complete.
					Application.Instance.AsyncInvoke(() => sizeAfterSetting = dlg.Size);
				};
				if (Platform.Instance.IsWpf)
				{
					AppContext.TryGetSwitch("Switch.System.Windows.DoNotScaleForDpiChanges", out bool isEnabled);
					Assert.IsTrue(isEnabled, "Set Switch.System.Windows.DoNotScaleForDpiChanges=true in app.config");
				}
				dlg.Content = new StackLayout
				{
					HorizontalContentAlignment = HorizontalAlignment.Center,
					AlignLabels = false,
					Items = {
						new Label {
							Text = "This dialog should start as 400x400 and not shrink smaller than 300x300.",
							Wrap = WrapMode.Word,
							BackgroundColor = Colors.Blue,
							Size = new Size(300, 300)
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

				Assert.AreEqual(new Size(400, 400), sizeAfterSetting, "Size after setting is incorrect!");
				Assert.IsTrue(result);
			}, timeout: -1);
		}
	}
}
