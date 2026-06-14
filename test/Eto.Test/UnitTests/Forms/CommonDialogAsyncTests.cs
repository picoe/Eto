using NUnit.Framework;

namespace Eto.Test.UnitTests.Forms
{
	/// <summary>
	/// A <see cref="CommonDialog"/> whose handler deliberately does not implement
	/// <see cref="CommonDialog.ICancellableHandler"/>, used to exercise the "cancellation not supported" path of
	/// <see cref="CommonDialog.ShowDialogAsync(Window, CancellationToken)"/> independently of platform capabilities.
	/// </summary>
	[Handler(typeof(IHandler))]
	public class UncancellableDialog : CommonDialog
	{
		public new interface IHandler : CommonDialog.IHandler { }
	}

	public class UncancellableDialogHandler : WidgetHandler<UncancellableDialog>, UncancellableDialog.IHandler
	{
		// Intentionally not ICancellableHandler. Returns immediately without showing UI so it is safe in unit tests.
		public DialogResult ShowDialog(Window parent) => DialogResult.Cancel;
	}

	/// <summary>
	/// Tests for the asynchronous <see cref="CommonDialog.ShowDialogAsync(Window, CancellationToken)"/> contract.
	///
	/// The deterministic tests (token already cancelled, unsupported + cancellable token) never display a dialog,
	/// so they are safe to run unattended. The cancel-while-shown tests do display a native dialog that is
	/// automatically cancelled, so they self-dismiss without user interaction.
	/// </summary>
	[TestFixture]
	public class CommonDialogAsyncTests : TestBase
	{
		static CommonDialogAsyncTests()
		{
			Platform.Instance.Add<UncancellableDialog.IHandler>(() => new UncancellableDialogHandler());
		}

		static CommonDialog CreateCancellable() => new OpenFileDialog();

		// Desktop common dialogs now support cancellation, so the "not supported" path is exercised with a dialog
		// whose handler explicitly opts out of ICancellableHandler.
		static CommonDialog CreateNonCancellable() => new UncancellableDialog();

		[Test, InvokeOnUI]
		public void AlreadyCancelledTokenShouldNotShowDialog()
		{
			// Regardless of whether the dialog supports cancellation, an already-cancelled token must short-circuit
			// before the dialog is ever displayed.
			foreach (var dialog in new[] { CreateCancellable(), CreateNonCancellable() })
			{
				var cts = new CancellationTokenSource();
				cts.Cancel();
				var task = dialog.ShowDialogAsync(null, cts.Token);
				Assert.That(task.IsCanceled, Is.True, $"{dialog.GetType().Name} should return a cancelled task without showing");
			}
		}

		[Test, InvokeOnUI]
		public void CancellableTokenOnUnsupportedDialogShouldThrow()
		{
			// A token that can be cancelled requires cancellation support; if the handler can't honor it we must
			// fail fast rather than silently ignore the token.
			var dialog = CreateNonCancellable();
			using var cts = new CancellationTokenSource();
			Assert.Throws<NotSupportedException>(() => dialog.ShowDialogAsync(null, cts.Token));
		}

		[Test, InvokeOnUI]
		public void NonCancellableTokenOnUnsupportedDialogShouldNotThrow()
		{
			// A missing/non-cancellable token must never throw NotSupportedException, even when the handler has no
			// cancellation support - the dialog is simply shown asynchronously. The show is scheduled on the UI
			// queue; disposing immediately prevents it from ever being presented in this synchronous test.
			var dialog = CreateNonCancellable();
			Task<DialogResult> task = null;
			Assert.DoesNotThrow(() => task = dialog.ShowDialogAsync(null, CancellationToken.None));
			dialog.Dispose();
			// observe the task so a faulted show (against the disposed handler) doesn't surface as unobserved
			task?.ContinueWith(t => { _ = t.Exception; }, TaskScheduler.Default);
		}

		[Test]
		public void FileDialogShouldCancelWhileShownWhenSupported()
		{
			Async(10000, async () =>
			{
				var dialog = CreateCancellable();
				using var cts = new CancellationTokenSource();

				Task<DialogResult> task;
				try
				{
					task = dialog.ShowDialogAsync(null, cts.Token);
				}
				catch (NotSupportedException)
				{
					// Platform cannot cancel this dialog type; throwing satisfies the contract.
					return;
				}

				// give the native dialog a moment to appear, then cancel it
				await Task.Delay(600);
				cts.Cancel();

				OperationCanceledException caught = null;
				try
				{
					await task;
				}
				catch (OperationCanceledException ex)
				{
					caught = ex;
				}

				Assert.That(caught, Is.Not.Null, "A supported dialog should be cancelled by its token while shown");
			});
		}
	}
}
