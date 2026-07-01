namespace Eto.Test.Sections.Dialogs
{
	/// <summary>
	/// Reusable "[x] Async   Cancel after [n] s" options row shared by the dialog test sections.
	///
	/// Add it to a section's layout and route the section's existing "Show" button through <see cref="Run"/>.
	/// When <see cref="Async"/> is unchecked the dialog is shown synchronously exactly as before; when checked it
	/// is shown via the async API and, if the timeout is greater than zero, cancelled after that many seconds.
	/// </summary>
	public class AsyncDialogOptions : Panel
	{
		/// <summary>Show the dialog using the asynchronous API.</summary>
		public bool Async { get; set; }

		/// <summary>Seconds before the async dialog is automatically cancelled (0 = no cancellation).</summary>
		public double CancelAfterSeconds { get; set; } = 3;

		/// <summary>Whether the next async show will use a cancellation token.</summary>
		public bool UsesCancellation => Async && CancelAfterSeconds > 0;

		public AsyncDialogOptions()
		{
			var asyncCheck = new CheckBox { Text = "Async", ToolTip = "Show the dialog with the asynchronous API" };
			var stepper = new NumericStepper
			{
				MinValue = 0,
				MaxValue = 600,
				DecimalPlaces = 0,
				Value = CancelAfterSeconds,
				Enabled = Async,
				ToolTip = "Cancel the async dialog after this many seconds (0 = no cancellation)"
			};

			asyncCheck.CheckedChanged += (sender, e) =>
			{
				Async = asyncCheck.Checked ?? false;
				stepper.Enabled = Async;
			};
			stepper.ValueChanged += (sender, e) => CancelAfterSeconds = stepper.Value;

			Content = TableLayout.Horizontal(
				5,
				asyncCheck,
				new Label { Text = "Cancel after (s):", VerticalAlignment = VerticalAlignment.Center },
				stepper);
		}

		/// <summary>
		/// Shows a dialog honoring the current options. The section supplies how to show the dialog synchronously
		/// and asynchronously (so dialog-specific parameters/parents are preserved), plus how to log the result.
		/// </summary>
		/// <param name="logSource">Object used as the source for <see cref="Log"/> output.</param>
		/// <param name="showSync">Shows the dialog synchronously and returns its result.</param>
		/// <param name="showAsync">Shows the dialog asynchronously with the supplied cancellation token.</param>
		/// <param name="onResult">Invoked with the result on completion (not called when cancelled or unsupported).</param>
		public async void Run(object logSource, Func<DialogResult> showSync, Func<CancellationToken, Task<DialogResult>> showAsync, Action<DialogResult> onResult)
		{
			if (!Async)
			{
				onResult(showSync());
				return;
			}

			using var cts = UsesCancellation ? new CancellationTokenSource(TimeSpan.FromSeconds(CancelAfterSeconds)) : null;
			if (cts != null)
				Log.Write(logSource, "Showing async, will cancel in {0}s", CancelAfterSeconds);

			try
			{
				onResult(await showAsync(cts?.Token ?? CancellationToken.None));
			}
			catch (OperationCanceledException)
			{
				Log.Write(logSource, "Async dialog was cancelled");
			}
			catch (NotSupportedException ex)
			{
				Log.Write(logSource, "Async cancellation not supported: {0}", ex.Message);
			}
		}

		/// <summary>
		/// Shows a modal <see cref="Dialog"/> honoring the current options, using <see cref="Dialog.ShowModalAsync(CancellationToken)"/>
		/// when async. The dialog is auto-closed (cancelled) after the timeout when one is set.
		/// </summary>
		/// <param name="logSource">Object used as the source for <see cref="Log"/> output.</param>
		/// <param name="showSync">Shows the dialog modally and blocks.</param>
		/// <param name="showAsync">Shows the dialog modally with the supplied cancellation token.</param>
		public async void RunModal(object logSource, Action showSync, Func<CancellationToken, Task> showAsync)
		{
			if (!Async)
			{
				Log.Write(logSource, "Showing dialog (blocking)...");
				showSync();
				Log.Write(logSource, "Dialog closed");
				return;
			}

			using var cts = UsesCancellation ? new CancellationTokenSource(TimeSpan.FromSeconds(CancelAfterSeconds)) : null;
			Log.Write(logSource, cts != null ? $"Showing dialog async, will cancel in {CancelAfterSeconds}s..." : "Showing dialog async...");

			try
			{
				await showAsync(cts?.Token ?? CancellationToken.None);
				Log.Write(logSource, "Dialog closed");
			}
			catch (OperationCanceledException)
			{
				Log.Write(logSource, "Dialog was cancelled");
			}
		}
	}
}
