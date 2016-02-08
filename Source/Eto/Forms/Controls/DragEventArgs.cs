using System;
using System.ComponentModel;

namespace Eto.Forms
{
	/// <summary>
	/// Enumeration of drag actions.
	/// </summary>
	public enum DragDropAction
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
		public Control Source
		{
			get;
		}

		/// <summary>
		/// Gets drag data.
		/// </summary>
		public DragDropData Data
		{
			get;
		}

		/// <summary>
		/// Gets allowed drag/drop operation.
		/// </summary>
		public DragDropAction AllowedAction
		{
			get;
		}

		/// <summary>
		/// Gets or sets target drag/drop operation.
		/// </summary>
		public DragDropAction Effect = DragDropAction.None;

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.DragEventArgs"/> class.
		/// </summary>
		/// <param name="source">Drag operation source control.</param>
		/// <param name="data">Drag data.</param>
		/// <param name="allowedAction">Allowed operation.</param>
		public DragEventArgs(Control source, DragDropData data, DragDropAction allowedAction)
		{
			Data = data;
			Source = source;
			AllowedAction = allowedAction;
		}
	}
}
