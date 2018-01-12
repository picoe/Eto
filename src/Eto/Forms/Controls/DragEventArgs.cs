using Eto.Drawing;
using System;
using System.ComponentModel;

namespace Eto.Forms
{
	/// <summary>
	/// Enumeration of drag actions.
	/// </summary>
	[Flags]
	public enum DragEffects
	{
		/// <summary>
		/// No drag operation.
		/// </summary>
		None = 0,
		/// <summary>
		/// Copy data operation.
		/// </summary>
		Copy = 1,
		/// <summary>
		/// Move data operation.
		/// </summary>
		Move = 2,
		/// <summary>
		/// Link data operation.
		/// </summary>
		Link = 4,
		/// <summary>
		/// All data operations.
		/// </summary>
		All = Copy | Move | Link
	}

	/// <summary>
	/// Drag/Drop event arguments.
	/// </summary>
	public class DragEventArgs : EventArgs
	{
		/// <summary>
		/// Gets source control of drag operation.
		/// </summary>
		public Control Source { get; }

		/// <summary>
		/// Gets drag data.
		/// </summary>
		public DataObject Data { get; }

		/// <summary>
		/// Gets allowed drag/drop operation.
		/// </summary>
		public DragEffects AllowedEffects { get; }

		/// <summary>
		/// Gets or sets target drag/drop operation.
		/// </summary>
		public DragEffects Effects { get; set; }

		/// <summary>
		/// Location of the cursor in control coordinates
		/// </summary>
		public PointF Location { get; }

		/// <summary>
		/// Modifier keys pressed
		/// </summary>
		public Keys Modifiers { get; }

		/// <summary>
		/// The mouse buttons pressed during the drag
		/// </summary>
		public MouseButtons Buttons { get; }

		/// <summary>
		/// Gets the instance of the platform-specific object associated with the drag event
		/// </summary>
		/// <remarks>
		/// This can be used by platform implementations to store additional information about the drag operation, 
		/// such as the parent object and child index for <see cref="TreeGridView.GetDragInfo"/>.
		/// </remarks>
		/// <value>The platform-specific control object.</value>
		public object ControlObject { get; }

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.DragEventArgs"/> class.
		/// </summary>
		/// <param name="source">Drag operation source control.</param>
		/// <param name="data">Drag data.</param>
		/// <param name="allowedEffects">Allowed operation.</param>
		/// <param name="location">Location of the mouse cursor for this event</param>
		/// <param name="modifiers">Modifier buttons pressed for this event</param>
		/// <param name="buttons">Mouse buttons pressed for this event</param>
		/// <param name="controlObject">Platform-specific object to store additional arguments/info</param>
		public DragEventArgs(Control source, DataObject data, DragEffects allowedEffects, PointF location, Keys modifiers, MouseButtons buttons, object controlObject = null)
		{
			Data = data;
			Source = source;
			AllowedEffects = allowedEffects;
			Location = location;
			Modifiers = modifiers;
			Buttons = buttons;
			ControlObject = controlObject;
		}
	}
}
