using System;
using System.IO;

namespace Eto.Drawing
{
	/// <summary>
	/// Handler for the <see cref="IndexedBitmap"/> class
	/// </summary>
	public interface IIndexedBitmap : IImage
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
		/// Locks the bits of the bitmap for direct manipulation
		/// </summary>
		/// <remarks>
		/// Note you must call <see cref="Unlock"/> after modifying the bitmap data
		/// </remarks>
		/// <returns>A BitmapData object that carries a pointer and functions for manipulating the data directly</returns>
		BitmapData Lock();

		/// <summary>
		/// Unlocks the bits of the bitmap
		/// </summary>
		/// <param name="bitmapData">Instance of the bitmap data retrieved from the <see cref="Lock"/> method</param>
		void Unlock(BitmapData bitmapData);

		/// <summary>
		/// Gets or sets the palette of the image
		/// </summary>
		Palette Palette { get; set; }
	}

	/// <summary>
	/// Represents a bitmap where each pixel is specified as an index in a <see cref="Palette"/>
	/// </summary>
	public class IndexedBitmap : Image
	{
		IIndexedBitmap handler;

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
			handler = (IIndexedBitmap)Handler;
			handler.Create(width, height, bitsPerPixel);
		}

		/// <summary>
		/// Resizes the bitmap to the specified size
		/// </summary>
		/// <param name="width">New width of the bitmap</param>
		/// <param name="height">New height of the bitmap</param>
		public void Resize (int width, int height)
		{
			handler.Resize(width, height);
		}

		/// <summary>
		/// Locks the bits of the bitmap for direct manipulation
		/// </summary>
		/// <remarks>
		/// Note you must call <see cref="Unlock"/> after modifying the bitmap data
		/// </remarks>
		/// <returns>A BitmapData object that carries a pointer and functions for manipulating the data directly</returns>
		public BitmapData Lock ()
		{
			return handler.Lock();
		}

		/// <summary>
		/// Unlocks the bits of the bitmap
		/// </summary>
		/// <param name="bitmapData">Instance of the bitmap data retrieved from the <see cref="Lock"/> method</param>
		public void Unlock (BitmapData bitmapData)
		{
			handler.Unlock(bitmapData);
		}

		/// <summary>
		/// Gets or sets the palette of the image
		/// </summary>
		/// <remarks>
		/// Note that the number of colors in the palette must match the number of colors specified by the bits per pixel of this bitmap
		/// </remarks>
		public Palette Palette 
		{
			get { return handler.Palette; }
			set { handler.Palette = value; }
		}

	}
}
