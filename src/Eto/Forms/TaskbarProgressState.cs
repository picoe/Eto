
namespace Eto.Forms
{
	/// <summary>
	/// Taskbar state for <see cref="Taskbar"/>.
	/// </summary>
	public enum TaskbarProgressState
	{
		/// <summary>
		/// The default state with no progress or indication.
		/// </summary>
		None,

		/// <summary>
		/// Standard state with visible progress.
		/// </summary>
        Progress,

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
