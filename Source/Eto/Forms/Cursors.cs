using System;
using System.Collections.Generic;

namespace Eto.Forms
{
	/// <summary>
	/// Provides direct access to a cached set of cursors for use within your application
	/// </summary>
	/// <remarks>
	/// This provides a preferred method of getting cursors as opposed to creating new instances directly via
	/// <see cref="Cursor"/>, as it will cache the cursors and only create one shared instance.
	/// </remarks>
	public static class Cursors
	{
		static ObjectCache<CursorType, Cursor> cursorCache = new ObjectCache<CursorType, Cursor> ();

		/// <summary>
		/// Gets a cached cursor with the specified <paramref name="type"/>
		/// </summary>
		/// <remarks>
		/// This is a preferred method of getting a cursor (or one of the static properties of this class)
		/// </remarks>
		/// <param name="generator">Generator to get the cursor for</param>
		/// <param name="type">Type of cursor to get</param>
		public static Cursor GetCursor (Generator generator, CursorType type)
		{
			return cursorCache.Get (generator, type, () => new Cursor (generator, type));
		}

		/// <summary>
		/// Default cursor, which is usually an arrow but may be different depending on the control
		/// </summary>
		public static Cursor Default
		{
			get { return GetCursor (null, CursorType.Default); }
		}

		/// <summary>
		/// Standard arrow cursor
		/// </summary>
		public static Cursor Arrow
		{
			get { return GetCursor (null, CursorType.Arrow); }
		}

		/// <summary>
		/// Cursor with a cross hair
		/// </summary>
		public static Cursor Crosshair
		{
			get { return GetCursor (null, CursorType.Crosshair); }
		}

		/// <summary>
		/// Pointer cursor, which is usually a hand
		/// </summary>
		public static Cursor Pointer
		{
			get { return GetCursor (null, CursorType.Pointer); }
		}

		/// <summary>
		/// All direction move cursor
		/// </summary>
		public static Cursor Move
		{
			get { return GetCursor (null, CursorType.Move); }
		}

		/// <summary>
		/// I-beam cursor for selecting text or placing the text cursor
		/// </summary>
		public static Cursor IBeam
		{
			get { return GetCursor (null, CursorType.IBeam); }
		}
		
		/// <summary>
		/// Vertical sizing cursor
		/// </summary>
		public static Cursor VerticalSplit
		{
			get { return GetCursor (null, CursorType.VerticalSplit); }
		}
		
		/// <summary>
		/// Horizontal sizing cursor
		/// </summary>
		public static Cursor HorizontalSplit
		{
			get { return GetCursor (null, CursorType.HorizontalSplit); }
		}
	}
}

