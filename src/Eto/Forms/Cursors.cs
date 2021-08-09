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
		public static Cursor Default => GetCursor(CursorType.Default);

		/// <summary>
		/// Standard arrow cursor
		/// </summary>
		public static Cursor Arrow => GetCursor(CursorType.Arrow);

		/// <summary>
		/// Cursor with a cross hair
		/// </summary>
		public static Cursor Crosshair => GetCursor(CursorType.Crosshair);

		/// <summary>
		/// Pointer cursor, which is usually a hand
		/// </summary>
		public static Cursor Pointer => GetCursor(CursorType.Pointer);

		/// <summary>
		/// All direction move cursor
		/// </summary>
		public static Cursor Move => GetCursor(CursorType.Move);

		/// <summary>
		/// I-beam cursor for selecting text or placing the text cursor
		/// </summary>
		public static Cursor IBeam => GetCursor(CursorType.IBeam);
		
		/// <summary>
		/// Vertical sizing cursor
		/// </summary>
		public static Cursor VerticalSplit => GetCursor(CursorType.VerticalSplit);
		
		/// <summary>
		/// Horizontal sizing cursor
		/// </summary>
		public static Cursor HorizontalSplit => GetCursor(CursorType.HorizontalSplit);

		/// <summary>
		/// All direction sizing cursor
		/// </summary>
		public static Cursor SizeAll => GetCursor(CursorType.SizeAll);

		/// <summary>
		/// Left side sizing cursor, which on some platforms is the same as <see cref="SizeRight"/>
		/// </summary>
		public static Cursor SizeLeft => GetCursor(CursorType.SizeLeft);

		/// <summary>
		/// Top side sizing cursor, which on some platforms is the same as <see cref="SizeBottom"/>
		/// </summary>
		public static Cursor SizeTop => GetCursor(CursorType.SizeTop);

		/// <summary>
		/// Right side sizing cursor, which on some platforms is the same as <see cref="SizeLeft"/>
		/// </summary>
		public static Cursor SizeRight => GetCursor(CursorType.SizeRight);

		/// <summary>
		/// Bottom side sizing cursor, which on some platforms is the same as <see cref="SizeTop"/>
		/// </summary>
		public static Cursor SizeBottom => GetCursor(CursorType.SizeBottom);

		/// <summary>
		/// Top-left corner sizing cursor, which on some platforms is the same as <see cref="SizeBottomRight"/>
		/// </summary>
		public static Cursor SizeTopLeft => GetCursor(CursorType.SizeTopLeft);

		/// <summary>
		/// Top-right corner sizing cursor, which on some platforms is the same as <see cref="SizeBottomLeft"/>
		/// </summary>
		public static Cursor SizeTopRight => GetCursor(CursorType.SizeTopRight);

		/// <summary>
		/// Bottom-left corner sizing cursor, which on some platforms is the same as <see cref="SizeTopRight"/>
		/// </summary>
		public static Cursor SizeBottomLeft => GetCursor(CursorType.SizeBottomLeft);

		/// <summary>
		/// Bottom-right corner sizing cursor, which on some platforms is the same as <see cref="SizeTopLeft"/>
		/// </summary>
		public static Cursor SizeBottomRight => GetCursor(CursorType.SizeBottomRight);
	}
}