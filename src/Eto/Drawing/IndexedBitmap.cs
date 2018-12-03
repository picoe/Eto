using System;

namespace Eto.Drawing
{
	/// <summary>
	/// Represents a bitmap where each pixel is specified as an index in a <see cref="Palette"/>
	/// </summary>
	/// <copyright>(c) 2012-2014 by Curtis Wensley</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	[Handler(typeof(IndexedBitmap.IHandler))]
	public class IndexedBitmap : Image
	{
		new IHandler Handler { get { return (IHandler)base.Handler; } }

		/// <summary>
		/// Gets the number of bits per pixel for this bitmap
		/// </summary>
		public int BitsPerPixel { get; private set; }

		/// <summary>
		/// Initializes a new instance of the IndexedBitmap class
		/// </summary>
		/// <param name="width">Width of the bitmap in pixels</param>
		/// <param name="height">Height of the bitmap in pixels</param>
		/// <param name="bitsPerPixel">Number of bits per pixel, usually 4 (16 colours), 6 (64 colours), or 8 (256 colours)</param>
		public IndexedBitmap(int width, int height, int bitsPerPixel)
		{
			if (width <= 0)
				throw new ArgumentOutOfRangeException("width", "width must be greater than zero");
			if (height <= 0)
				throw new ArgumentOutOfRangeException("height", "height must be greater than zero");
			if (bitsPerPixel <= 0)
				throw new ArgumentOutOfRangeException("bitsPerPixel", "bitsPerPixel must be greater than zero");
			BitsPerPixel = bitsPerPixel;
			Handler.Create(width, height, bitsPerPixel);
			Initialize();
		}

		/// <summary>
		/// Resizes the bitmap to the specified size
		/// </summary>
		/// <param name="width">New width of the bitmap</param>
		/// <param name="height">New height of the bitmap</param>
		public void Resize(int width, int height)
		{
			if (width <= 0)
				throw new ArgumentOutOfRangeException("width", "width must be greater than zero");
			if (height <= 0)
				throw new ArgumentOutOfRangeException("height", "height must be greater than zero");
			Handler.Resize(width, height);
		}

		/// <summary>
		/// Locks the data of the image to directly access the bytes of the image
		/// </summary>
		/// <remarks>
		/// This locks the data to read and write to directly using unsafe pointers. After reading or updating
		/// the data, you must call <see cref="BitmapData.Dispose()"/> to unlock the data before using the bitmap.
		/// e.g.:
		/// 
		/// <code>
		/// using (var bd = bitmap.Lock ()) {
		/// 	byte* pdata = bd.Data;
		/// 	// access data
		/// }
		/// </code>
		/// </remarks>
		/// <returns>A BitmapData object that carries a pointer and functions for manipulating the data directly</returns>
		public BitmapData Lock()
		{
			return Handler.Lock();
		}

		/// <summary>
		/// Gets or sets the palette of the image
		/// </summary>
		/// <remarks>
		/// Note that the number of colors in the palette must match the number of colors specified by the bits per pixel of this bitmap
		/// </remarks>
		public Palette Palette
		{
			get { return Handler.Palette; }
			set { Handler.Palette = value; }
		}

		/// <summary>
		/// Handler for the <see cref="IndexedBitmap"/> class
		/// </summary>
		/// <copyright>(c) 2012-2014 by Curtis Wensley</copyright>
		/// <license type="BSD-3">See LICENSE for full terms</license>
		[AutoInitialize(false)]
		public new interface IHandler : Image.IHandler, ILockableImage
		{
			/// <summary>
			/// Creates a new indexed bitmap with the specified size and bits per pixel
			/// </summary>
			/// <param name="width">Width in pixels of the bitmap</param>
			/// <param name="height">Height in pixels of the bitmap</param>
			/// <param name="bitsPerPixel">Number of bits per pixel, usually 4 (16 colours), 8 (64 colours), or 8 (256 colours)</param>
			void Create(int width, int height, int bitsPerPixel);

			/// <summary>
			/// Resizes the bitmap to the specified size
			/// </summary>
			/// <param name="width">New width of the bitmap</param>
			/// <param name="height">New height of the bitmap</param>
			void Resize(int width, int height);

			/// <summary>
			/// Gets or sets the palette of the image
			/// </summary>
			Palette Palette { get; set; }
		}
	}
}
