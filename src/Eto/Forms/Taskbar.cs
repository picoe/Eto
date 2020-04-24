using System;

namespace Eto.Forms
{
	/// <summary>
	/// Methods to interact with application taskbar button.
	/// </summary>
	[Handler(typeof(IHandler))]
	public static class Taskbar
	{
		/// <summary>
		/// Sets the state and progress of the application.
		/// </summary>
		/// <param name="state">Taskbar button state.</param>
		/// <param name="progress">Progress in range from 0.0f to 1.0f.</param>
		public static void SetProgress(TaskbarProgressState state, float progress = 0f)
		{
			if (progress < 0.0f || progress > 1.0f)
				throw new ArgumentOutOfRangeException(nameof(progress), "Progress needs to be in 0.0f to 1.0f range.");

			var handler = Platform.Instance.CreateShared<IHandler>();
			handler?.SetProgress(state, progress);
		}

		/// <summary>
		/// Handler interface for the <see cref="Taskbar"/>.
		/// </summary>
		public interface IHandler
		{
			/// <summary>
			/// Sets the state and progress of the application.
			/// </summary>
			/// <param name="state">Taskbar button state.</param>
			/// <param name="progress">Progress in range from 0.0f to 1.0f.</param>
			void SetProgress(TaskbarProgressState state, float progress);
		}
	}
}
