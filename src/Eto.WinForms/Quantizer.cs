/* 
  THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF 
  ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO 
  THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A 
  PARTICULAR PURPOSE. 
  
    This is sample code and is freely distributable. 
*/ 

namespace ImageManipulation
{
	/// <summary>
	/// Summary description for Class1.
	/// </summary>
	public unsafe abstract class Quantizer
	{
		/// <summary>
		/// Construct the quantizer
		/// </summary>
		/// <param name="singlePass">If true, the quantization only needs to loop through the source pixels once</param>
		/// <remarks>
		/// If you construct this class with a true value for singlePass, then the code will, when quantizing your image,
		/// only call the 'QuantizeImage' function. If two passes are required, the code will call 'InitialQuantizeImage'
		/// and then 'QuantizeImage'.
		/// </remarks>
		protected Quantizer(bool singlePass)
		{
			_singlePass = singlePass;
		}

		/// <summary>
		/// Quantize an image and return the resulting output bitmap
		/// </summary>
		/// <param name="source">The image to quantize</param>
		/// <returns>A quantized version of the image</returns>
		public sd.Bitmap Quantize(sd.Image source)
		{
			// Get the size of the source image
			int	height = source.Height;
			int width = source.Width;

			// And construct a rectangle from these dimensions
			var bounds = new sd.Rectangle(0, 0, width, height);

			// First off take a 32bpp copy of the image
			var copy = new sd.Bitmap(width, height, sdi.PixelFormat.Format32bppArgb);

			// And construct an 8bpp version
			var output = new sd.Bitmap(width, height, sdi.PixelFormat.Format8bppIndexed);

			// Now lock the bitmap into memory
			using (var g = sd.Graphics.FromImage(copy))
			{
				g.PageUnit = sd.GraphicsUnit.Pixel;

				// Draw the source image onto the copy bitmap,
				// which will effect a widening as appropriate.
				g.DrawImageUnscaled(source, bounds);
			}

			// Define a pointer to the bitmap data
			sdi.BitmapData	sourceData = null;

			try
			{
				// Get the source image bits and lock into memory
				sourceData = copy.LockBits(bounds, sdi.ImageLockMode.ReadOnly, sdi.PixelFormat.Format32bppArgb);

				// Call the FirstPass function if not a single pass algorithm.
				// For something like an octree quantizer, this will run through
				// all image pixels, build a data structure, and create a palette.
				if (!_singlePass)
					FirstPass(sourceData, width, height);

				// Then set the color palette on the output bitmap. I'm passing in the current palette 
				// as there's no way to construct a new, empty palette.
				output.Palette = GetPalette(output.Palette);

				// Then call the second pass which actually does the conversion
				SecondPass(sourceData, output, width, height, bounds);
			}
			finally
			{
				// Ensure that the bits are unlocked
				copy.UnlockBits(sourceData);
			}

			// Last but not least, return the output bitmap
			return output;
		}

		/// <summary>
		/// Execute the first pass through the pixels in the image
		/// </summary>
		/// <param name="sourceData">The source data</param>
		/// <param name="width">The width in pixels of the image</param>
		/// <param name="height">The height in pixels of the image</param>
		protected virtual void FirstPass(sdi.BitmapData sourceData, int width, int height)
		{
			// Define the source data pointers. The source row is a byte to
			// keep addition of the stride value easier (as this is in bytes)
			var	pSourceRow = (byte*)sourceData.Scan0.ToPointer();
			Int32*	pSourcePixel;

			// Loop through each row
			for (int row = 0; row < height; row++)
			{
				// Set the source pixel to the first pixel in this row
				pSourcePixel = (Int32*)pSourceRow;

				// And loop through each column
				for (int col = 0; col < width; col++ , pSourcePixel++)
				// Now I have the pixel, call the FirstPassQuantize function...
					InitialQuantizePixel((Color32*)pSourcePixel);

				// Add the stride to the source row
				pSourceRow += sourceData.Stride;
			}
		}

		/// <summary>
		/// Execute a second pass through the bitmap
		/// </summary>
		/// <param name="sourceData">The source bitmap, locked into memory</param>
		/// <param name="output">The output bitmap</param>
		/// <param name="width">The width in pixels of the image</param>
		/// <param name="height">The height in pixels of the image</param>
		/// <param name="bounds">The bounding rectangle</param>
		protected virtual void SecondPass(sdi.BitmapData sourceData, sd.Bitmap output, int width, int height, sd.Rectangle bounds)
		{
			sdi.BitmapData	outputData = null;

			try
			{
				// Lock the output bitmap into memory
				outputData = output.LockBits(bounds, sdi.ImageLockMode.WriteOnly, sdi.PixelFormat.Format8bppIndexed);

				// Define the source data pointers. The source row is a byte to
				// keep addition of the stride value easier (as this is in bytes)
				var	pSourceRow = (byte*)sourceData.Scan0.ToPointer();
				var	pSourcePixel = (Int32*)pSourceRow;
				Int32*	pPreviousPixel = pSourcePixel;

				// Now define the destination data pointers
				var	pDestinationRow = (byte*)outputData.Scan0.ToPointer();
				byte*	pDestinationPixel = pDestinationRow;

				// And convert the first pixel, so that I have values going into the loop
				byte	pixelValue = QuantizePixel((Color32*)pSourcePixel);

				// Assign the value of the first pixel
				*pDestinationPixel = pixelValue;

				// Loop through each row
				for (int row = 0; row < height; row++)
				{
					// Set the source pixel to the first pixel in this row
					pSourcePixel = (Int32*)pSourceRow;

					// And set the destination pixel pointer to the first pixel in the row
					pDestinationPixel = pDestinationRow;

					// Loop through each pixel on this scan line
					for (int col = 0; col < width; col++ , pSourcePixel++ , pDestinationPixel++)
					{
						// Check if this is the same as the last pixel. If so use that value
						// rather than calculating it again. This is an inexpensive optimisation.
						if (*pPreviousPixel != *pSourcePixel)
						{
							// Quantize the pixel
							pixelValue = QuantizePixel((Color32*)pSourcePixel);

							// And setup the previous pointer
							pPreviousPixel = pSourcePixel;
						}

						// And set the pixel in the output
						*pDestinationPixel = pixelValue;
					}

					// Add the stride to the source row
					pSourceRow += sourceData.Stride;

					// And to the destination row
					pDestinationRow += outputData.Stride;
				}
			}
			finally
			{
				// Ensure that I unlock the output bits
				output.UnlockBits(outputData);
			}
		}

		/// <summary>
		/// Override this to process the pixel in the first pass of the algorithm
		/// </summary>
		/// <param name="pixel">The pixel to quantize</param>
		/// <remarks>
		/// This function need only be overridden if your quantize algorithm needs two passes,
		/// such as an Octree quantizer.
		/// </remarks>
		protected virtual void InitialQuantizePixel(Color32* pixel)
		{
		}

		/// <summary>
		/// Override this to process the pixel in the second pass of the algorithm
		/// </summary>
		/// <param name="pixel">The pixel to quantize</param>
		/// <returns>The quantized value</returns>
		protected abstract byte QuantizePixel(Color32* pixel) ;

		/// <summary>
		/// Retrieve the palette for the quantized image
		/// </summary>
		/// <param name="original">Any old palette, this is overrwritten</param>
		/// <returns>The new color palette</returns>
		protected abstract sdi.ColorPalette GetPalette(sdi.ColorPalette original) ;

		/// <summary>
		/// Flag used to indicate whether a single pass or two passes are needed for quantization.
		/// </summary>
		readonly bool	_singlePass;

		/// <summary>
		/// Struct that defines a 32 bpp colour
		/// </summary>
		/// <remarks>
		/// This struct is used to read data from a 32 bits per pixel image
		/// in memory, and is ordered in this manner as this is the way that
		/// the data is layed out in memory
		/// </remarks>
		[StructLayout(LayoutKind.Explicit)]
		public struct Color32
		{
			/// <summary>
			/// Holds the blue component of the colour
			/// </summary>
			[FieldOffset(0)]
			public byte Blue;
			/// <summary>
			/// Holds the green component of the colour
			/// </summary>
			[FieldOffset(1)]
			public byte Green;
			/// <summary>
			/// Holds the red component of the colour
			/// </summary>
			[FieldOffset(2)]
			public byte Red;
			/// <summary>
			/// Holds the alpha component of the colour
			/// </summary>
			[FieldOffset(3)]
			public byte Alpha;
			/// <summary>
			/// Permits the color32 to be treated as an int32
			/// </summary>
			[FieldOffset(0)]
			public int ARGB;

			/// <summary>
			/// Return the color for this Color32 object
			/// </summary>
			public sd.Color Color
			{
				get	{ return sd.Color.FromArgb(Alpha, Red, Green, Blue); }
			}
		}
	}
}
