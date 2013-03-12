using System;
using System.IO;

namespace Eto.Drawing
{
	/// <summary>
	/// Handler for the <see cref="IndexedBitmap"/> class
	/// </summary>
	/// <copyright>(c) 2012-2013 by Curtis Wensley</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	public interface IIndexedBitmap : IImage, ILockableImage
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

	/// <summary>
	/// Represents a bitmap where each pixel is specified as an index in a <see cref="Palette"/>
	/// </summary>
	/// <copyright>(c) 2012-2013 by Curtis Wensley</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	public class IndexedBitmap : Image
	{
		new IIndexedBitmap Handler { get { return (IIndexedBitmap)base.Handler; } }

		/// <summary>
		/// Gets the number of bits per pixel for this bitmap
		/// </summary>
		public int BitsPerPixel { get; private set; }
		
		/// <summary>
		/// Initializes a new instance of the IndexedBitmap class
		/// </summary>
		/// <param name="width">Width of the bitmap in pixels</param>
		/// <param name="height">Height of the bitmap in pixels</param>
		/// <param name="bitsPerPixel">Number of bits per pixel, usually 4 (16 colours), 8 (64 colours), or 8 (256 colours)</param>
		public IndexedBitmap (int width, int height, int bitsPerPixel)
			: this(Generator.Current, width, height, bitsPerPixel)
		{
		}

		/// <summary>
		/// Initializes a new instance of the IndexedBitmap class
		/// </summary>
		/// <param name="generator">Generator to use for the handler</param>
		/// <param name="width">Width of the bitmap in pixels</param>
		/// <param name="height">Height of the bitmap in pixels</param>
		/// <param name="bitsPerPixel">Number of bits per pixel, usually 4 (16 colours), 8 (64 colours), or 8 (256 colours)</param>
		public IndexedBitmap (Generator generator, int width, int height, int bitsPerPixel)
			: base(generator, typeof(IIndexedBitmap))
		{
			this.BitsPerPixel = bitsPerPixel;
			Handler.Create(width, height, bitsPerPixel);
		}

		/// <summary>
		/// Resizes the bitmap to the specified size
		/// </summary>
		/// <param name="width">New width of the bitmap</param>
		/// <param name="height">New height of the bitmap</param>
		public void Resize (int width, int height)
		{
			Handler.Resize(width, height);
		}

		/// <summary>
		/// Locks the data of the image to directly access the bytes of the image
		/// </summary>
		/// <remarks>
		/// This locks the data to read and write to directly using unsafe pointers. After reading or updating
		/// the data, you must call <see cref="BitmapData.Dispose"/> to unlock the data before using the bitmap.
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
		public BitmapData Lock ()
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

		#region Obsolete

		/// <summary>
		/// Unlocks the bits of the bitmap
		/// </summary>
		/// <param name="bitmapData">Instance of the bitmap data retrieved from the <see cref="Lock"/> method</param>
		[Obsolete ("Use BitmapData.Dispose instead")]
		public void Unlock (BitmapData bitmapData)
		{
			Handler.Unlock(bitmapData);
		}

		#endregion
	}
}
