using System;
using System.Diagnostics;
using System.Globalization;
using Eto.Drawing;

namespace Eto.Shared.Drawing
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
	/// <copyright>(c) 2012-2015 by Curtis Wensley</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	public abstract class BaseBitmapData : BitmapData
	{
		/// <summary>
		/// Initializes a new instance of the BitmapData class
		/// </summary>
		/// <param name="image">Image this data is for</param>
		/// <param name="data">Pointer to the bitmap data</param>
		/// <param name="scanWidth">Width of each scan row, in bytes</param>
		/// <param name="bitsPerPixel">Bits per pixel</param>
		/// <param name="controlObject">Platform specific object for the bitmap data (if any)</param>
		protected BaseBitmapData(Image image, IntPtr data, int scanWidth, int bitsPerPixel, object controlObject)
			: base(image, data, scanWidth, bitsPerPixel, controlObject)
		{
		}

		/// <summary>
		/// Gets the color of the pixel at the specified coordinates.
		/// </summary>
		/// <returns>The color of the pixel.</returns>
		/// <param name="x">The x coordinate to get the color from.</param>
		/// <param name="y">The y coordinate to get the color from.</param>
		public unsafe override Color GetPixel(int x, int y)
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
		/// Sets the pixel color at the specified coordinates.
		/// </summary>
		/// <param name="x">The x coordinate of the pixel to set.</param>
		/// <param name="y">The y coordinate of the pixel to set.</param>
		/// <param name="color">Color to set the pixel to.</param>
		public unsafe override void SetPixel(int x, int y, Color color)
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
	}
}

