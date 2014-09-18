using System;

using MouseEventHandler = System.EventHandler<Eto.Forms.MouseEventArgs>;

namespace Eto.Forms
{
	/// <summary>
	/// Defines an interface for controls or classes that implement mouse events.
	/// </summary>
	public interface IMouseInputSource
    {
		/// <summary>
		/// Occurs when a mouse button is released
		/// </summary>
        event MouseEventHandler MouseUp;

		/// <summary>
		/// Occurs when mouse moves within the bounds of the control, or when the mouse is captured
		/// </summary>
		/// <remarks>
		/// The mouse is captured after a <see cref="MouseDown"/> event within the control, 
		/// and is released when the mouse button is released
		/// </remarks>
        event MouseEventHandler MouseMove;

		/// <summary>
		/// Occurs when the mouse enters the bounds of the control
		/// </summary>
		event MouseEventHandler MouseEnter;

		/// <summary>
		/// Occurs when mouse leaves the bounds of the control
		/// </summary>
        event MouseEventHandler MouseLeave;

		/// <summary>
		/// Occurs when a mouse button has been pressed
		/// </summary>
		/// <remarks>
		/// Controls will typically capture the mouse after a mouse button is pressed and will be released
		/// only after the <see cref="MouseUp"/> event.
		/// </remarks>
        event MouseEventHandler MouseDown;

		/// <summary>
		/// Occurs when a mouse button is double clicked within the bounds of the control
		/// </summary>
		/// <remarks>
		/// If you do not set the <see cref="MouseEventArgs.Handled"/> property to true, and the default behaviour of
		/// the control does not accept double clicks, the <see cref="MouseDown"/> event will be called for each click of
		/// the mouse button. 
		/// 
		/// For example, if the user clicks twice in succession, the following will be called:
		/// 1. MouseDown for the first click
		/// 2. MouseDoubleClick for the second click
		/// 3. If Handled has not been set in #2, MouseDown will be called a 2nd time
		/// </remarks>
        event MouseEventHandler MouseDoubleClick;

		/// <summary>
		/// Occurs when mouse wheel has been changed
		/// </summary>
        event MouseEventHandler MouseWheel;
    }
}
