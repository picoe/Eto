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

		[Test]
		public void RunIterationShouldAllowBlocking()
		{
			Form form = null;
			bool running = true;
			bool stopClicked = false;
			Application.Instance.Invoke(() =>
			{
				form = new Form();
				form.Closed += (sender, e) => running = false;

				var stopButton = new Button { Text = "Stop" };
				stopButton.Click += (sender, e) =>
				{
					running = false;
					stopClicked = true;
				};

				var layout = new DynamicLayout();

				layout.Padding = 10;
				layout.DefaultSpacing = new Size(4, 4);
				layout.Add(new Label { Text = "The controls in this form should\nbe functional while test is running", TextAlignment = TextAlignment.Center });
				layout.Add(new DropDown { DataStore = new[] { "Item 1", "Item 2", "Item 3" } });
				layout.Add(new TextBox());
				layout.Add(new DateTimePicker());
				layout.AddCentered(new Spinner { Enabled = true });
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
				} while (running);
				form.Close();
			});
			
			Assert.IsTrue(stopClicked, "#1 - Must press the stop button to close the form");
		}
	}
}
