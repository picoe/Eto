#if DESKTOP
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
		static object cursorCache = new object();

		static Cursor GetCursor (CursorType type, Generator generator = null)
		{
			var cache = generator.Cache<CursorType, Cursor> (cursorCache);
			Cursor cursor;
			lock (cache) {
				if (!cache.TryGetValue (type, out cursor)) {
					cursor = new Cursor (generator, type);
					cache [type] = cursor;
				}
			}
			return cursor;
		}

		/// <summary>
		/// Gets a cached cursor with the specified <paramref name="type"/>
		/// </summary>
		/// <param name="type">Type of cursor to get</param>
		/// <param name="generator">Generator to get the cached pen for</param>
		/// <returns>A cached instance of the specified cursor</returns>
		public static Cursor Cached (CursorType type, Generator generator = null)
		{
			return GetCursor (type, generator);
		}
		
		/// <summary>
		/// Clears the cursor cache
		/// </summary>
		/// <remarks>
		/// This is useful if you are using the <see cref="Cached"/> method to cache pens and want to clear it
		/// to conserve memory or resources.
		/// </remarks>
		/// <param name="generator">Generator to clear the pen cache for</param>
		public static void ClearCache (Generator generator = null)
		{
			var cache = generator.Cache<CursorType, Cursor> (cursorCache);
			lock (cache) {
				cache.Clear ();
			}
		}

		/// <summary>
		/// Default cursor, which is usually an arrow but may be different depending on the control
		/// </summary>
		public static Cursor Default (Generator generator = null)
		{
			return GetCursor (CursorType.Default, generator);
		}

		/// <summary>
		/// Standard arrow cursor
		/// </summary>
		public static Cursor Arrow (Generator generator = null)
		{
			return GetCursor (CursorType.Arrow, generator);
		}

		/// <summary>
		/// Cursor with a cross hair
		/// </summary>
		public static Cursor Crosshair (Generator generator = null)
		{
			return GetCursor (CursorType.Crosshair, generator);
		}

		/// <summary>
		/// Pointer cursor, which is usually a hand
		/// </summary>
		public static Cursor Pointer (Generator generator = null)
		{
			return GetCursor (CursorType.Pointer, generator);
		}

		/// <summary>
		/// All direction move cursor
		/// </summary>
		public static Cursor Move (Generator generator = null)
		{
			return GetCursor (CursorType.Move, generator);
		}

		/// <summary>
		/// I-beam cursor for selecting text or placing the text cursor
		/// </summary>
		public static Cursor IBeam (Generator generator = null)
		{
			return GetCursor (CursorType.IBeam, generator);
		}
		
		/// <summary>
		/// Vertical sizing cursor
		/// </summary>
		public static Cursor VerticalSplit (Generator generator = null)
		{
			return GetCursor (CursorType.VerticalSplit, generator);
		}
		
		/// <summary>
		/// Horizontal sizing cursor
		/// </summary>
		public static Cursor HorizontalSplit (Generator generator = null)
		{
			return GetCursor (CursorType.HorizontalSplit, generator);
		}
	}
}
#endif