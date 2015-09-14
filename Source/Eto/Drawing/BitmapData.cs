using System;
using System.Diagnostics;
using System.Globalization;

namespace Eto.Drawing
{
	/// <summary>
	/// Bitmap data information when accessing a <see cref="Bitmap"/>'s data directly
	/// </summary>
	/// <remarks>
	/// The bitmap data is accessed through <see cref="Bitmap.Lock"/>, which locks the data
	/// for direct access using the <see cref="BitmapData.Data"/> pointer.
	/// 
	/// Ensure you call dispose the instance when you are done accessing or writing the data,
	/// otherwise the bitmap may be left in an unusable state.
	/// </remarks>
	/// <copyright>(c) 2012-2014 by Curtis Wensley</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	public abstract class BitmapData : IDisposable
	{
		readonly IntPtr data;
		readonly int scanWidth;
		readonly object controlObject;
		readonly Image image;
		readonly int bitsPerPixel;
		readonly int bytesPerPixel;

		/// <summary>
		/// Initializes a new instance of the BitmapData class
		/// </summary>
		/// <param name="image">Image this data is for</param>
		/// <param name="data">Pointer to the bitmap data</param>
		/// <param name="scanWidth">Width of each scan row, in bytes</param>
		/// <param name="bitsPerPixel">Bits per pixel</param>
		/// <param name="controlObject">Platform specific object for the bitmap data (if any)</param>
		protected BitmapData(Image image, IntPtr data, int scanWidth, int bitsPerPixel, object controlObject)
		{
			this.image = image;
			this.data = data;
			this.scanWidth = scanWidth;
			this.bitsPerPixel = bitsPerPixel;
			this.controlObject = controlObject;
			this.bytesPerPixel = (bitsPerPixel + 7) / 8;
		}

		/// <summary>
		/// Gets the image this data is for
		/// </summary>
		/// <value>The bitmap.</value>
		public Image Image
		{
			get { return image; }
		}

		/// <summary>
		/// Gets the bits per pixel
		/// </summary>
		/// <value>The bits per pixel</value>
		public int BitsPerPixel
		{
			get { return bitsPerPixel; }
		}

		/// <summary>
		/// Gets the bytes per pixel
		/// </summary>
		/// <value>The bytes per pixel</value>
		public int BytesPerPixel
		{
			get { return bytesPerPixel; }
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
		public abstract int TranslateArgbToData(int argb);

		/// <summary>
		/// Translates the platform specific pixel format to a 32-bit ARGB value
		/// </summary>
		/// <remarks>
		/// Use this method to translate an value from the bitmap data to a 32-bit ARGB (Alpha in most significant byte).
		/// 
		/// Each platform can have a different pixel format, and this allows you to abstract 
		/// getting the data into a 32-bit colour.
		/// 
		/// The ARGB value can be easily handled using <see cref="C:Eto.Drawing.Color(uint)"/>.
		/// 
		/// For non-alpha bitmaps, the alpha component will be ignored
		/// </remarks>
		/// <param name="bitmapData">Platform specific bitmap data for a pixel to translate</param>
		/// <returns>Translated ARGB value from the bitmap data</returns>
		public abstract int TranslateDataToArgb(int bitmapData);

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
		/// Gets the color of the pixel at the specified <paramref name="position"/>
		/// </summary>
		/// <returns>The color of the pixel.</returns>
		/// <param name="position">Position to get the color of the pixel.</param>
		public Color GetPixel(Point position)
		{
			return GetPixel(position.X, position.Y);
		}

		/// <summary>
		/// Gets the color of the pixel at the specified coordinates.
		/// </summary>
		/// <returns>The color of the pixel.</returns>
		/// <param name="x">The x coordinate to get the color from.</param>
		/// <param name="y">The y coordinate to get the color from.</param>
		public unsafe virtual Color GetPixel(int x, int y)
		{
			var pos = (byte*)Data;
			pos += x * BytesPerPixel + y * ScanWidth;

			if (BytesPerPixel == 4)
			{
				var col = TranslateDataToArgb(*((int*)pos));
				return Color.FromArgb(col);
			}
			if (BytesPerPixel == 3)
			{
				var col = TranslateDataToArgb(*((int*)pos));
				return Color.FromRgb(col);
			}
			var bmp = Image as IndexedBitmap;
			if (bmp != null)
			{
				if (BytesPerPixel == 1)
				{
					var col = *pos;
					return bmp.Palette[col];
				}
			}
			throw new NotSupportedException(string.Format(CultureInfo.CurrentCulture, "This PixelFormat is not supported by GetPixel. Must be 24 or 32 bits per pixel, or 8 bit indexed"));
		}

		/// <summary>
		/// Sets the pixel color at the specified <paramref name="position"/>.
		/// </summary>
		/// <param name="position">Position to set the pixel color.</param>
		/// <param name="color">Color to set.</param>
		public void SetPixel(Point position, Color color)
		{
			SetPixel(position.X, position.Y, color);
		}

		/// <summary>
		/// Sets the pixel color at the specified coordinates.
		/// </summary>
		/// <param name="x">The x coordinate of the pixel to set.</param>
		/// <param name="y">The y coordinate of the pixel to set.</param>
		/// <param name="color">Color to set the pixel to.</param>
		public unsafe virtual void SetPixel(int x, int y, Color color)
		{
			var pos = (byte*)Data;
			pos += x * BytesPerPixel + y * ScanWidth;

			var col = TranslateArgbToData(color.ToArgb());
			if (BytesPerPixel == 4)
			{
				*((int*)pos) = col;
			}
			else if (BytesPerPixel == 3)
			{
				*(pos++) = (byte)(col & 0xFF);
				*(pos++) = (byte)((col >> 8) & 0xFF);
				*(pos++) = (byte)((col >> 16) & 0xFF);
			}
			else
				throw new NotSupportedException(string.Format(CultureInfo.CurrentCulture, "This PixelFormat is not supported by SetPixel. Must be 3 or 4 bytes per pixel"));
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

		/// <summary>
		/// Releases all resource used by the <see cref="Eto.Drawing.BitmapData"/> object.
		/// </summary>
		/// <remarks>Call <see cref="Dispose()"/> when you are finished using the <see cref="Eto.Drawing.BitmapData"/>. The
		/// <see cref="Dispose()"/> method leaves the <see cref="Eto.Drawing.BitmapData"/> in an unusable state. After calling
		/// <see cref="Dispose()"/>, you must release all references to the <see cref="Eto.Drawing.BitmapData"/> so the garbage
		/// collector can reclaim the memory that the <see cref="Eto.Drawing.BitmapData"/> was occupying.</remarks>
		public void Dispose ()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		/// <summary>
		/// Disposes the brush
		/// </summary>
		/// <param name="disposing">If set to <c>true</c> dispose was called explicitly, otherwise specify false if calling from a finalizer</param>
		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				var handler = (ILockableImage)image.Handler;
				handler.Unlock(this);
			}
			else
			{
				Debug.WriteLine("Caller is missing a call to BitmapData.Dispose()");
			}
		}
	}
}

