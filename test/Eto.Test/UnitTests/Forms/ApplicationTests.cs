using NUnit.Framework;

namespace Eto.Test.UnitTests.Forms;

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
			enableSpinnerCheck.CheckedChanged += (sender, e) =>
			{
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

		Assert.That(stopClicked, Is.True, "#1 - Must press the stop button to close the form");
	}

	[Test]
	public void EnsureUIThreadShouldThrow()
	{
		Form form = null;
		TextBox textBox = null;
		var oldMode = Application.Instance.UIThreadCheckMode;
		Application.Instance.UIThreadCheckMode = UIThreadCheckMode.Error;
		Invoke(() =>
		{
			textBox = new TextBox();
			form = new Form();
		});

		Assert.Throws<UIThreadAccessException>(() => textBox.Text = "hello", "#1");
		Assert.Throws<UIThreadAccessException>(() => form.Bounds = new Rectangle(0, 0, 100, 100), "#2");

		Application.Instance.UIThreadCheckMode = oldMode;
	}
	
	public static Task<T> StartSTATask<T>(Func<T> func)
	{
		var tcs = new TaskCompletionSource<T>();
		var thread = new Thread(() =>
		{
			try
			{
				tcs.SetResult(func());
			}
			catch (Exception e)
			{
				tcs.SetException(e);
			}
		});
#pragma warning disable CA1416
		if (EtoEnvironment.Platform.IsWindows)
			thread.SetApartmentState(ApartmentState.STA);
#pragma warning restore CA1416
		thread.Start();
		return tcs.Task;
	}	

	[Test]
	public void ShowingUIInSeparateThreadShouldWork() => Async(async () =>
	{
		if (!Platform.Instance.SupportedFeatures.HasFlag(PlatformFeatures.MultiThreadedUI))
		{
			Assert.Inconclusive("Platform does not support multi-threaded UI");
			return;
		}
		int mainThreadId = Thread.CurrentThread.ManagedThreadId;
		int? threadId = null;
		bool? isSameThreadForInvoke = null;
		await StartSTATask(() =>
		{
			threadId = Thread.CurrentThread.ManagedThreadId;
			var app = new Application(Eto.Platform.Copy());
			app.Attach();

			var dialog = new Dialog { Title = "From Separate Thread", Size = new(300, 300) };

			app.InvokeAsync(async () =>
			{
				isSameThreadForInvoke = threadId == Thread.CurrentThread.ManagedThreadId;
				await Task.Delay(2000);
				dialog.Close();
			});

			dialog.ShowModal();
			return true;
		});

		Assert.That(isSameThreadForInvoke, Is.Not.Null);
		Assert.That(isSameThreadForInvoke, Is.EqualTo(true));
		Assert.That(threadId, Is.Not.EqualTo(mainThreadId));
	});
}
