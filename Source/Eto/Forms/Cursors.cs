using System.Collections.Generic;
using System;

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
		static readonly object cursorCache = new object();

		static Cursor GetCursor(CursorType type)
		{
			var cache = Platform.Instance.Cache<CursorType, Cursor> (cursorCache);
			Cursor cursor;
			lock (cache) {
				if (!cache.TryGetValue (type, out cursor)) {
					cursor = new Cursor(type);
					cache [type] = cursor;
				}
			}
			return cursor;
		}

		/// <summary>
		/// Gets a cached cursor with the specified <paramref name="type"/>
		/// </summary>
		/// <param name="type">Type of cursor to get</param>
		/// <returns>A cached instance of the specified cursor</returns>
		public static Cursor Cached(CursorType type)
		{
			return GetCursor(type);
		}
		
		/// <summary>
		/// Clears the cursor cache
		/// </summary>
		/// <remarks>
		/// This is useful if you are using the <see cref="Cached(CursorType)"/> method to cache pens and want to clear it
		/// to conserve memory or resources.
		/// </remarks>
		public static void ClearCache()
		{
			var cache = Platform.Instance.Cache<CursorType, Cursor> (cursorCache);
			lock (cache) {
				cache.Clear ();
			}
		}

		/// <summary>
		/// Default cursor, which is usually an arrow but may be different depending on the control
		/// </summary>
		public static Cursor Default
		{
			get { return GetCursor(CursorType.Default); }
		}

		/// <summary>
		/// Standard arrow cursor
		/// </summary>
		public static Cursor Arrow
		{
			get { return GetCursor(CursorType.Arrow); }
		}

		/// <summary>
		/// Cursor with a cross hair
		/// </summary>
		public static Cursor Crosshair
		{
			get { return GetCursor(CursorType.Crosshair); }
		}

		/// <summary>
		/// Pointer cursor, which is usually a hand
		/// </summary>
		public static Cursor Pointer
		{
			get { return GetCursor(CursorType.Pointer); }
		}

		/// <summary>
		/// All direction move cursor
		/// </summary>
		public static Cursor Move
		{
			get { return GetCursor(CursorType.Move); }
		}

		/// <summary>
		/// I-beam cursor for selecting text or placing the text cursor
		/// </summary>
		public static Cursor IBeam
		{
			get { return GetCursor(CursorType.IBeam); }
		}
		
		/// <summary>
		/// Vertical sizing cursor
		/// </summary>
		public static Cursor VerticalSplit
		{
			get { return GetCursor(CursorType.VerticalSplit); }
		}
		
		/// <summary>
		/// Horizontal sizing cursor
		/// </summary>
		public static Cursor HorizontalSplit
		{
			get { return GetCursor(CursorType.HorizontalSplit); }
		}
	}
}