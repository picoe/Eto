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
		void Create(CursorType cursor);
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
		ICursor handler;
		
		public Cursor (CursorType cursor)
			: this (Generator.Current, cursor)
		{
		}
		
		public Cursor (Generator generator, CursorType cursor)
			: this (generator)
		{
			handler.Create (cursor);
		}
		
		protected Cursor (Generator generator)
			: this (generator, typeof(ICursor))
		{
		}
		
		protected Cursor (Generator g, Type type, bool initialize = true)
			: base(g, type, initialize)
		{
			handler = (ICursor)Handler;
		}
		
	}

    public class Cursors
    {
        #region Default
        private static Cursor default_;
        public static Cursor Default
        {
            get
            {
                if (default_ == null)
                    default_ = 
                        new Cursor(
                            CursorType.Default);

                return default_;
            }
        }
        #endregion

        #region Arrow
        private static Cursor arrow;
        public static Cursor Arrow
        {
            get
            {
                if (arrow == null)
                    arrow =
                        new Cursor(
                            CursorType.Arrow);

                return arrow;
            }
        }
        #endregion

        #region Crosshair
        private static Cursor crosshair;
        public static Cursor Crosshair
        {
            get
            {
                if (crosshair == null)
                    crosshair =
                        new Cursor(
                            CursorType.Crosshair);

                return crosshair;
            }
        }
        #endregion

        #region Pointer
        private static Cursor pointer;
        public static Cursor Pointer
        {
            get
            {
                if (pointer == null)
                    pointer =
                        new Cursor(
                            CursorType.Pointer);

                return pointer;
            }
        }
        #endregion

        #region Move
        private static Cursor move;
        public static Cursor Move
        {
            get
            {
                if (move == null)
                    move =
                        new Cursor(
                            CursorType.Move);

                return move;
            }
        }
        #endregion

        #region IBeam
        private static Cursor iBeam;
        public static Cursor IBeam
        {
            get
            {
                if (iBeam == null)
                    iBeam =
                        new Cursor(
                            CursorType.IBeam);

                return iBeam;
            }
        }
        #endregion

        #region VerticalSplit
        private static Cursor verticalSplit;
        public static Cursor VerticalSplit
        {
            get
            {
                if (verticalSplit == null)
                    verticalSplit =
                        new Cursor(
                            CursorType.VerticalSplit);

                return verticalSplit;
            }
        }
        #endregion

        #region HorizontalSplit
        private static Cursor horizontalSplit;
        public static Cursor HorizontalSplit
        {
            get
            {
                if (horizontalSplit == null)
                    horizontalSplit =
                        new Cursor(
                            CursorType.HorizontalSplit);

                return horizontalSplit;
            }
        }
        #endregion

    }
}

