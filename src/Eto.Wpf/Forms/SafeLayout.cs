namespace Eto.Wpf.Forms
{
	/// <summary>
	/// Runs a measure or arrange so that a failure in it cannot take down the application.
	/// </summary>
	/// <remarks>
	/// WPF's layout pass gives you nowhere to handle an exception: it runs from the dispatcher's render
	/// callback, so anything an element throws while measuring or arranging goes straight to the top and
	/// terminates the process. Usually that is a bug that should be fixed, but not all of it is ours to
	/// fix — most notably text formatting, which throws for fonts WPF reported as installed but cannot
	/// actually load (a corrupt font file, or one the process is not allowed to read).
	///
	/// This is opt-in, for the places that are known to lay out content we don't control. Use it from an
	/// element's <see cref="sw.FrameworkElement.MeasureOverride"/> and
	/// <see cref="sw.FrameworkElement.ArrangeOverride"/> so a failure there costs a badly sized element
	/// rather than the whole application:
	///
	/// <code>
	/// protected override sw.Size MeasureOverride(sw.Size constraint) => SafeLayout.Measure(base.MeasureOverride, constraint);
	/// protected override sw.Size ArrangeOverride(sw.Size arrangeSize) => SafeLayout.Arrange(base.ArrangeOverride, arrangeSize);
	/// </code>
	///
	/// Guard both: an element that fails to measure fails to arrange as well, and either one is fatal.
	/// </remarks>
	public static class SafeLayout
	{
		/// <summary>
		/// Gets or sets whether a failing measure/arrange is swallowed. Set to false to let it through,
		/// which is useful when debugging a layout problem of your own.
		/// </summary>
		public static bool Enabled { get; set; } = true;

		/// <summary>
		/// Raised when a measure or arrange failed and was swallowed.
		/// </summary>
		public static event EventHandler<UnhandledExceptionEventArgs> Failed;

		/// <summary>
		/// Measures using <paramref name="measure"/>, returning an empty size if it fails.
		/// </summary>
		public static sw.Size Measure(Func<sw.Size, sw.Size> measure, sw.Size constraint)
		{
			try
			{
				return measure(constraint);
			}
			catch (Exception ex) when (CanSwallow(ex))
			{
				OnFailed(ex);
				return new sw.Size();
			}
		}

		/// <summary>
		/// Arranges using <paramref name="arrange"/>, returning <paramref name="finalSize"/> if it fails.
		/// </summary>
		public static sw.Size Arrange(Func<sw.Size, sw.Size> arrange, sw.Size finalSize)
		{
			try
			{
				return arrange(finalSize);
			}
			catch (Exception ex) when (CanSwallow(ex))
			{
				OnFailed(ex);
				return finalSize;
			}
		}

		static bool CanSwallow(Exception ex) => Enabled && !(ex is OutOfMemoryException);

		static void OnFailed(Exception ex)
		{
			// Trace, not Debug: this is at its most useful diagnosing a release build, where the failing
			// font/content is on the user's machine and not ours. Debug.WriteLine is compiled out there.
			Trace.WriteLine($"Layout failed and was ignored: {ex}");
			Failed?.Invoke(null, new UnhandledExceptionEventArgs(ex, false));
		}
	}
}
