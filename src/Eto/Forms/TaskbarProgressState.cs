
namespace Eto.Forms
{
	/// <summary>
	/// Taskbar state for <see cref="Taskbar"/>.
	/// </summary>
	public enum TaskbarProgressState
	{
		/// <summary>
		/// The default state with no progressbar.
		/// </summary>
		NoProgressbar,

		/// <summary>
		/// Standard state with visible progressbar.
		/// </summary>
		ShowProgressbar,

		/// <summary>
		/// Indeterminate state where the progressbar value will be ignored.
		/// </summary>
		Indeterminate,

		/// <summary>
		/// Error state where the taskbar will try to signal to the end user that an error occured.
		/// </summary>
		Error,

		/// <summary>
		/// The paused state.
		/// </summary>
		Paused
	}
}
