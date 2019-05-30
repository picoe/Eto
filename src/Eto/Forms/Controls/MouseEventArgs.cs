using System;
using Eto.Drawing;

namespace Eto.Forms
{
	/// <summary>
	/// Enumeration of mouse buttons
	/// </summary>
	[Flags]
	public enum MouseButtons
	{
		/// <summary>
		/// No mouse button
		/// </summary>
		None = 0x00,
		/// <summary>
		/// The primary button, usually Left but can be Right depending on the user's preferences for a right handed mouse.
		/// </summary>
		Primary = 0x01,
		/// <summary>
		/// The alternate button, usually Right but can be Left depending on the user's preferences for a right handed mouse.
		/// </summary>
		Alternate = 0x02,
		/// <summary>
		/// The middle mouse button
		/// </summary>
		Middle = 0x04
	}

	/// <summary>
	/// Mouse event arguments.
	/// </summary>
	public class MouseEventArgs : EventArgs
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.MouseEventArgs"/> class.
		/// </summary>
		/// <param name="buttons">Buttons involved in the event.</param>
		/// <param name="modifiers">Key modifiers such as Control, Alt, or Shift.</param>
		/// <param name="location">Location of the mouse cursor for the event.</param>
		/// <param name="delta">Delta of the scroll wheel.</param>
		/// <param name="pressure">Pressure of a stylus or touch, if applicable. 1.0f for full pressure or not supported</param>
		public MouseEventArgs(MouseButtons buttons, Keys modifiers, PointF location, SizeF? delta = null, float pressure = 1.0f)
		{
			this.Modifiers = modifiers;
			this.Buttons = buttons;
			this.Location = location;
			this.Pressure = pressure;
			this.Delta = delta ?? SizeF.Empty;
		}

		/// <summary>
		/// Gets the key modifiers such as <see cref="Keys.Control"/>, <see cref="Keys.Alt"/>, or <see cref="Keys.Shift"/>.
		/// </summary>
		/// <value>The key modifiers.</value>
		public Keys Modifiers { get; private set; }

		/// <summary>
		/// Gets the mouse buttons involved in the event.
		/// </summary>
		/// <value>The mouse buttons.</value>
		public MouseButtons Buttons { get; private set; }

		/// <summary>
		/// Gets the location of the mouse relative to the control that raised the event.
		/// </summary>
		/// <value>The location of the mouse cursor.</value>
		public PointF Location { get; private set; }

		/// <summary>
		/// Gets or sets a value indicating whether the event is handled.
		/// </summary>
		/// <remarks>
		/// Set this to true if you perform logic with the event and wish the default event to be cancelled.
		/// Some platforms may cause audio feedback if the user's action does not perform anything.
		/// </remarks>
		/// <value><c>true</c> if handled; otherwise, <c>false</c>.</value>
		public bool Handled { get; set; }

		/// <summary>
		/// Gets or sets the pressure of the mouse/stylus press, if applicable. 1.0 if full pressure or not supported.
		/// </summary>
		/// <value>The pressure of the mouse/stylus.</value>
		public float Pressure { get; private set; }

		/// <summary>
		/// Gets or sets the delta change of the scroll wheel for the event.
		/// </summary>
		/// <value>The scroll wheel delta.</value>
		public SizeF Delta { get; private set; }
	}
}

