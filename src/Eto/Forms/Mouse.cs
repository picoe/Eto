using Eto.Drawing;
using System;
using System.ComponentModel;

namespace Eto.Forms
{
	/// <summary>
	/// Static methods to get the current mouse state
	/// </summary>
	[Handler(typeof(IHandler))]
	public static class Mouse
	{
		/// <summary>
		/// Gets a value indicating whether the current platform supports mouse functions in this class
		/// </summary>
		/// <value><c>true</c> if is supported; otherwise, <c>false</c>.</value>
		public static bool IsSupported
		{
			get { return Platform.Instance.Supports<IHandler>(); }
		}

		static IHandler Handler
		{
			get { return Platform.Instance.CreateShared<IHandler>(); }
		}

		/// <summary>
		/// Gets or sets the current mouse position in screen coordinates
		/// </summary>
		/// <returns>The mouse position.</returns>
		public static PointF Position
		{
			get { return Handler.Position; }
			set { Handler.Position = value; }
		}

		/// <summary>
		/// Gets the current state of the mouse buttons
		/// </summary>
		/// <returns>The mouse button state.</returns>
		public static MouseButtons Buttons
		{
			get { return Handler.Buttons; }
		}

		/// <summary>
		/// Returns true if any of the specified mouse buttons is pressed.
		/// </summary>
		/// <param name="buttons"></param>
		/// <returns></returns>
		public static bool IsAnyButtonPressed(MouseButtons buttons)
		{
			return (Buttons & buttons) != MouseButtons.None;
		}

		/// <summary>
		/// Handler interface for the <see cref="Mouse"/> class
		/// </summary>
		public interface IHandler
		{
			/// <summary>
			/// Gets the current mouse position in screen coordinates
			/// </summary>
			/// <value>The mouse position.</value>
			PointF Position { get; set; }

			/// <summary>
			/// Gets the current state of the mouse buttons
			/// </summary>
			/// <value>The mouse button state.</value>
			MouseButtons Buttons { get; }
		}
	}
}
