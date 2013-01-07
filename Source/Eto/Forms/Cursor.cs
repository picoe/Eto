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
	/// Platform interface for the <see cref="Cursor"/> class
	/// </summary>
	public interface ICursor : IInstanceWidget
	{
		void Create (CursorType cursor);
	}
	
	/// <summary>
	/// Class for a particular Mouse cursor type
	/// </summary>
	/// <remarks>
	/// This can be used to specify a cursor for a particular control
	/// using <see cref="Control.Cursor"/>
	/// </remarks>
	public class Cursor : InstanceWidget
	{
		new ICursor Handler { get { return (ICursor)base.Handler; } }
		
		public Cursor (CursorType cursor)
			: this (Generator.Current, cursor)
		{
		}
		
		public Cursor (Generator generator, CursorType cursor)
			: base (generator, typeof(ICursor), false)
		{
			Handler.Create (cursor);
			Initialize ();
		}
		
		protected Cursor (Generator generator)
			: this (generator, typeof(ICursor))
		{
		}
		
		protected Cursor (Generator generator, Type type, bool initialize = true)
			: base(generator, type, initialize)
		{
		}
	}
}

