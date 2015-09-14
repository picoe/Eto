using System;

namespace Eto.Forms
{
	/// <summary>
	/// Enumeration of the cursor types supported by the <see cref="Cursor"/> object
	/// </summary>
	public enum CursorType
	{
		/// <summary>
		/// Default cursor, which is usually an arrow but may be different depending on the control
		/// </summary>
		Default,

		/// <summary>
		/// Standard arrow cursor
		/// </summary>
		Arrow,

		/// <summary>
		/// Cursor with a cross hair
		/// </summary>
		Crosshair,

		/// <summary>
		/// Pointer cursor, which is usually a hand
		/// </summary>
		Pointer,

		/// <summary>
		/// All direction move cursor
		/// </summary>
		Move,

		/// <summary>
		/// I-beam cursor for selecting text or placing the text cursor
		/// </summary>
		IBeam,

		/// <summary>
		/// Vertical sizing cursor
		/// </summary>
		VerticalSplit,

		/// <summary>
		/// Horizontal sizing cursor
		/// </summary>
		HorizontalSplit
	}

	/// <summary>
	/// Class for a particular Mouse cursor type
	/// </summary>
	/// <remarks>
	/// This can be used to specify a cursor for a particular control
	/// using <see cref="Control.Cursor"/>
	/// </remarks>
	[Handler(typeof(Cursor.IHandler))]
	public class Cursor : Widget
	{
		new IHandler Handler { get { return (IHandler)base.Handler; } }

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.Cursor"/> class with the specified <paramref name="type"/>.
		/// </summary>
		/// <param name="type">Type of cursor.</param>
		public Cursor(CursorType type)
		{
			Handler.Create(type);
			Initialize();
		}

		/// <summary>
		/// Platform interface for the <see cref="Cursor"/> class
		/// </summary>
		[AutoInitialize(false)]
		public new interface IHandler : Widget.IHandler
		{
			/// <summary>
			/// Creates the cursor instance with the specified <paramref name="type"/>.
			/// </summary>
			/// <param name="type">Cursor type.</param>
			void Create(CursorType type);
		}
	}
}

