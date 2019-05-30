using System;
using System.Threading.Tasks;
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

		[InvokeOnUI, ManualTest]
		[TestCase(true, true, true)]
		[TestCase(true, true, false)]
		[TestCase(true, false, false)]
		[TestCase(false, true, true)]
		[TestCase(false, true, false)]
		[TestCase(false, false, false)]
		public void MultipleDialogsShouldAllowClosingInDifferentOrders(bool inOrder, bool passParent, bool attached)
		{
			var parentForm = passParent ? Application.Instance.MainForm : null;
			int firstClosedCount = 0;
			int secondClosedCount = 0;
			bool firstWasClosedFirst = false;
			var modal1 = new Dialog { Content = "Modal 1", Size = new Size(200, 200) };
			var closeButton = new Button { Text = "Close" };
			var modal2 = new Dialog
			{
				Content = new StackLayout
				{
					Items = {
						"Modal 2\nWait until Modal 1 closes, then try to close this window.\nThis window should also resize and adjust the label correctly",
						closeButton
					}
				},
				Size = new Size(200, 200),
				Resizable = true
			};

			closeButton.Click += (sender, e) => modal2.Close();

			if (attached)
			{
				modal1.DisplayMode = DialogDisplayMode.Attached;
				modal2.DisplayMode = DialogDisplayMode.Attached;
			}

			modal2.Closed += (sender, e) =>
			{
				firstWasClosedFirst = firstClosedCount > 0;
				secondClosedCount++;
			};

			modal1.Closed += (sender, e) => firstClosedCount++;

			Task.Run(() =>
			{
				System.Threading.Thread.Sleep(TimeSpan.FromSeconds(2));
				Application.Instance.AsyncInvoke(modal1.Close);
			});

			Application.Instance.AsyncInvoke(() =>
			{
				if (inOrder)
					modal1.ShowModal(attached ? (Window)modal2 : parentForm);
				else
					modal2.ShowModal(attached ? (Window)modal1 : parentForm);
			});

			if (inOrder)
				modal2.ShowModal(parentForm);
			else
				modal1.ShowModal(parentForm);

			Assert.AreNotEqual(0, firstClosedCount, "Modal 1 was not closed");
			Assert.AreNotEqual(0, secondClosedCount, "Modal 2 was not closed");
			Assert.AreEqual(1, firstClosedCount, "Modal 1 must trigger Closed only once");
			Assert.AreEqual(1, secondClosedCount, "Modal 2 must trigger Closed only once");
			Assert.IsTrue(firstWasClosedFirst, "Modal 1 did not close before Modal 2");
		}

	}
}
