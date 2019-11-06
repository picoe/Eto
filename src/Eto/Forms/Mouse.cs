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
		public static bool IsSupported => Platform.Instance.Supports<IHandler>();

		static IHandler Handler => Platform.Instance.CreateShared<IHandler>();

		/// <summary>
		/// Gets or sets the current mouse position in screen coordinates
		/// </summary>
		/// <returns>The mouse position.</returns>
		public static PointF Position
		{
			get => Handler.Position;
			set => Handler.Position = value;
		}

		/// <summary>
		/// Gets the current state of the mouse buttons
		/// </summary>
		/// <returns>The mouse button state.</returns>
		public static MouseButtons Buttons => Handler.Buttons;

		/// <summary>
		/// Returns true if any of the specified mouse buttons is pressed.
		/// </summary>
		/// <param name="buttons"></param>
		/// <returns></returns>
		public static bool IsAnyButtonPressed(MouseButtons buttons) => (Buttons & buttons) != MouseButtons.None;

		/// <summary>
		/// Temporarily sets the current mouse pointer to the specified <paramref name="cursor"/>.
		/// </summary>
		/// <remarks>
		/// Some platforms may or may not support this in some cases, and usually the cursor gets reset when it is moved.
		/// Usually you would set the cursor using the <see cref="Control.Cursor"/> property instead.
		/// </remarks>
		/// <param name="cursor">Cursor to set the mouse pointer to temprarily</param>
		public static void SetCursor(Cursor cursor) => Handler.SetCursor(cursor);

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

			/// <summary>
			/// Temporarily sets the current mouse pointer to the specified <paramref name="cursor"/>.
			/// </summary>
			/// <param name="cursor">Cursor to set the mouse pointer to temprarily</param>
			void SetCursor(Cursor cursor);
		}
	}
}
