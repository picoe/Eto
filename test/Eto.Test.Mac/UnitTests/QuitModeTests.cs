using Eto.Test.UnitTests;
using NUnit.Framework;

namespace Eto.Test.Mac.UnitTests
{
	[TestFixture]
	public class QuitModeTests : TestBase
	{
		// the close button goes through windowShouldClose:, unlike Window.Close()
		[Test]
		public void LastWindowClosedShouldQuitWhenUsingCloseButton() => Async(-1, async () =>
		{
			var app = Application.Instance;
			Assume.That(app.Windows.OfType<Form>().Any(r => r.Visible), Is.False, "Another form is open");

			var oldMode = app.QuitMode;
			var terminatingCount = 0;
			EventHandler<CancelEventArgs> terminating = (sender, e) =>
			{
				terminatingCount++;
				e.Cancel = true;
			};
			// no Closing handler, so only the backend's own hookup can see the close button
			var form = new Form { Title = "QuitModeTests", Size = new Size(200, 150) };
			app.Terminating += terminating;
			app.QuitMode = QuitMode.LastWindowClosed;
			try
			{
				form.Show();
				await WaitUntil(() => form.Visible);

				var window = (NSWindow)form.ControlObject;
				window.PerformClose(window);
				await Task.Delay(200);

				Assert.That(terminatingCount, Is.EqualTo(1), "Closing the last form should ask to terminate");
				Assert.That(app.Windows.Contains(form), Is.True, "Cancelling Terminating should cancel the close");
			}
			finally
			{
				app.QuitMode = QuitMode.Explicit;
				await CloseAsync(form);
				app.Terminating -= terminating;
				app.QuitMode = oldMode;
			}
		});
	}
}
