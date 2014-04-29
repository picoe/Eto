using Eto.Drawing;

namespace Eto.Forms
{
	/// <summary>
	/// Handler interface for the <see cref="Mouse"/> class
	/// </summary>
	public interface IMouse : IWidget
	{
		/// <summary>
		/// Gets the current mouse position in screen coordinates
		/// </summary>
		/// <value>The mouse position.</value>
		PointF Position { get; }

		/// <summary>
		/// Gets the current state of the mouse buttons
		/// </summary>
		/// <value>The mouse button state.</value>
		MouseButtons Buttons { get; }
	}

	/// <summary>
	/// Static methods to get the current mouse state
	/// </summary>
	public static class Mouse
	{
		public static bool IsSupported
		{
			get { return Platform.Instance.Supports<IMouse>(); }
		}

		static IMouse Handler(Generator generator)
		{
			return generator.CreateShared<IMouse>();
		}

		/// <summary>
		/// Gets the current mouse position in screen coordinates
		/// </summary>
		/// <returns>The mouse position.</returns>
		/// <param name="generator">Generator to get the mouse position for</param>
		public static PointF GetPosition(Generator generator = null)
		{
			return Handler(generator).Position;
		}

		/// <summary>
		/// Gets the current state of the mouse buttons
		/// </summary>
		/// <returns>The mouse button state.</returns>
		/// <param name="generator">Generator to get the buttons from</param>
		public static MouseButtons GetButtons(Generator generator = null)
		{
			return Handler(generator).Buttons;
		}

		/// <summary>
		/// Returns true if any of the specified mouse buttons is pressed.
		/// </summary>
		/// <param name="buttons"></param>
		/// <returns></returns>
		public static bool IsAnyButtonPressed(MouseButtons buttons)
		{
			return (GetButtons() & buttons) != MouseButtons.None;
		}
	}
}
