using System;
using Eto.Drawing;
using Eto.Forms;
using NUnit.Framework;

namespace Eto.Test.UnitTests.Forms
{
	[TestFixture]
	public class ApplicationTests : TestBase
	{
		[Test, InvokeOnUI]
		public void ReinitializingWithNewPlatformShouldThrowException()
		{
			Assert.Throws<InvalidOperationException>(() =>
			{
				_ = new Application(Platform.Instance.GetType().AssemblyQualifiedName);
			});
		}

		[Test, InvokeOnUI]
		public void ReinitializingWithCurrentPlatformShouldThrowException()
		{
			Assert.Throws<InvalidOperationException>(() =>
			{
				_ = new Application(Platform.Instance);
			});
		}
		
		[TestCase(-1), ManualTest]
		[TestCase(10), ManualTest]
		[TestCase(1000), ManualTest]
		public void RunIterationShouldAllowBlocking(int delay)
		{
			int count = 0;
			Label countLabel = null;
			Form form = null;
			bool running = true;
			bool stopClicked = false;
			Application.Instance.Invoke(() =>
			{
				form = new Form();
				form.Closed += (sender, e) => running = false;
				form.Title = "RunIterationShouldAllowBlocking (" + nameof(delay) + ": " + delay + ")"; 
				var stopButton = new Button { Text = "Stop" };
				stopButton.Click += (sender, e) =>
				{
					running = false;
					stopClicked = true;
				};
				
				countLabel = new Label();
				
				var spinner = new Spinner { Enabled = false };
				
				var enableSpinnerCheck = new CheckBox { Text = "Enable spinner" };
				enableSpinnerCheck.CheckedChanged += (sender, e) => {
					spinner.Enabled = enableSpinnerCheck.Checked == true;
				};

				var layout = new DynamicLayout();

				layout.Padding = 10;
				layout.DefaultSpacing = new Size(4, 4);
				layout.Add(new Label { Text = "The controls in this form should\nbe functional while test is running,\nand count should increase without moving the mouse.\nControls should be non-interactable during the delay.", TextAlignment = TextAlignment.Center });
				layout.Add(new DropDown { DataStore = new[] { "Item 1", "Item 2", "Item 3" } });
				layout.Add(new TextBox());
				layout.Add(new DateTimePicker());
				layout.AddCentered(enableSpinnerCheck);
				layout.AddCentered(spinner);
				layout.AddCentered(countLabel);
				layout.AddCentered(stopButton);

				form.Content = layout;
			});

			Application.Instance.Invoke(() =>
			{
				form.Owner = Application.Instance.MainForm;
				form.Show();
				do
				{
					Application.Instance.RunIteration();
					if (delay > 0)
						System.Threading.Thread.Sleep(delay);
					countLabel.Text = $"Iteration Count: {count++}";
				} while (running);
				form.Close();
			});
			
			Assert.IsTrue(stopClicked, "#1 - Must press the stop button to close the form");
		}
		
		[Test]
		public void EnsureUIThreadShouldThrow()
		{
			Form form = null;
			TextBox textBox = null;
			var oldMode = Application.Instance.UIThreadCheckMode;
			Application.Instance.UIThreadCheckMode = UIThreadCheckMode.Error;
			Invoke(() => {
				textBox = new TextBox();
				form = new Form();
			});
			
			Assert.Throws<UIThreadAccessException>(() => textBox.Text = "hello", "#1");
			Assert.Throws<UIThreadAccessException>(() => form.Bounds = new Rectangle(0, 0, 100, 100), "#2");
			
			Application.Instance.UIThreadCheckMode = oldMode;
		}
	}
}
