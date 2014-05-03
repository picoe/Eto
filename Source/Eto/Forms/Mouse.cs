using Eto.Drawing;
using System;
using System.ComponentModel;

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

		static IMouse Handler
		{
			get { return Platform.Instance.CreateShared<IMouse>(); }
		}

		/// <summary>
		/// Gets the current mouse position in screen coordinates
		/// </summary>
		/// <returns>The mouse position.</returns>
		public static PointF Position
		{
			get { return Handler.Position; }
		}

		/// <summary>
		/// Gets the current state of the mouse buttons
		/// </summary>
		/// <returns>The mouse button state.</returns>
		public static MouseButtons Buttons
		{
			get { return Handler.Buttons; }
		}

		#pragma warning disable 612,618

		/// <summary>
		/// Gets the current mouse position in screen coordinates
		/// </summary>
		/// <returns>The mouse position.</returns>
		/// <param name="generator">Generator to get the mouse position for</param>
		[Obsolete("Use Mouse.Position")]
		public static PointF GetPosition(Generator generator = null)
		{
			return Handler.Position;
		}

		/// <summary>
		/// Gets the current state of the mouse buttons
		/// </summary>
		/// <returns>The mouse button state.</returns>
		/// <param name="generator">Generator to get the buttons from</param>
		[Obsolete("Use Mouse.Buttons")]
		public static MouseButtons GetButtons(Generator generator = null)
		{
			return Handler.Buttons;
		}

		#pragma warning restore 612,618

		/// <summary>
		/// Returns true if any of the specified mouse buttons is pressed.
		/// </summary>
		/// <param name="buttons"></param>
		/// <returns></returns>
		public static bool IsAnyButtonPressed(MouseButtons buttons)
		{
			return (Buttons & buttons) != MouseButtons.None;
		}
	}
}
