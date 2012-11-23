using System;

namespace Eto.Drawing
{
	/// <summary>
	/// Bitmap data information when accessing a <see cref="Bitmap"/>'s data directly
	/// </summary>
	/// <remarks>
	/// The bitmap data is accessed through <see cref="Bitmap.Lock"/>, which locks the data
	/// for direct access using the <see cref="BitmapData.Data"/> pointer.
	/// 
	/// Ensure you call <see cref="Bitmap.Unlock"/> with the same instance when you are done
	/// accessing or writing the data.
	/// </remarks>
	public abstract class BitmapData
	{
		IntPtr data;
		int scanWidth;
		object controlObject;

		/// <summary>
		/// Initializes a new instance of the BitmapData class
		/// </summary>
		/// <param name="data">Pointer to the bitmap data</param>
		/// <param name="scanWidth">Width of each scan row, in bytes</param>
		/// <param name="controlObject">Platform specific object for the bitmap data (if any)</param>
		protected BitmapData(IntPtr data, int scanWidth, object controlObject)
		{
			this.data = data;
			this.scanWidth = scanWidth;
			this.controlObject = controlObject;
		}

		/// <summary>
		/// Translates a 32-bit ARGB value to the platform specific pixel format value
		/// </summary>
		/// <remarks>
		/// Use this method to translate an ARGB (Alpha in most significant) to the value
		/// required by the bitmap for the pixel.
		/// 
		/// Each platform can have a different pixel format, and this allows you to abstract 
		/// setting the data directly.
		/// 
		/// The ARGB value can be easily retrieved using <see cref="Color.ToArgb"/>.
		/// 
		/// For non-alpha bitmaps, the alpha component will be ignored
		/// </remarks>
		/// <param name="argb">ARGB pixel value to translate into the platform-specific format</param>
		/// <returns>Platform-specific format of the pixels that can be set directly onto the data</returns>
		public abstract uint TranslateArgbToData(uint argb);

		/// <summary>
		/// Translates the platform specific pixel format to a 32-bit ARGB value
		/// </summary>
		/// <remarks>
		/// Use this method to translate an value from the bitmap data to a 32-bit ARGB (Alpha in most significant byte).
		/// 
		/// Each platform can have a different pixel format, and this allows you to abstract 
		/// getting the data into a 32-bit colour.
		/// 
		/// The ARGB value can be easily handled using <see cref="C:Color(uint)"/>.
		/// 
		/// For non-alpha bitmaps, the alpha component will be ignored
		/// </remarks>
		/// <param name="bitmapData">Platform specific bitmap data for a pixel to translate</param>
		/// <returns>Translated ARGB value from the bitmap data</returns>
		public abstract uint TranslateDataToArgb(uint bitmapData);

		/// <summary>
		/// Gets the pointer to the data of the bitmap
		/// </summary>
		/// <remarks>
		/// This does not include any headers, etc. so it directly points to the beginning of the data.
		/// 
		/// Each row may not be on a pixel boundary, so to increment to the next row, use the <see cref="ScanWidth"/>
		/// to increment the pointer to the next row.
		/// </remarks>
		public IntPtr Data
		{
			get { return data; }
		}
		
		/// <summary>
		/// Gets a value indicating that the data is flipped (upside down)
		/// </summary>
		/// <remarks>
		/// Some platforms may handle memory bitmaps in a flipped fashion, such that the top of the image
		/// is at the bottom of the data.
		/// 
		/// If this is true, then the starting row of the data is the bottom row of the image.
		/// </remarks>
		public virtual bool Flipped
		{
			get { return false; }
		}

		/// <summary>
		/// Gets the width (in bytes) of each scan line (row) of pixel data
		/// </summary>
		/// <remarks>
		/// When advancing to the next row, use this to increment the pointer.  The number of bytes
		/// for each row might not be equivalent to the bytes per pixel multiplied by the width of the image.
		/// </remarks>
		public int ScanWidth
		{
			get { return scanWidth; }
		}

		/// <summary>
		/// Gets the platform-specific control object for the bitmap data
		/// </summary>
		public object ControlObject
		{
			get { return controlObject; }
		}
	}
}

