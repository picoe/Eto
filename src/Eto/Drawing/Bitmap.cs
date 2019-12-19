using System;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using sc = System.ComponentModel;

namespace Eto.Drawing
{
	/// <summary>
	/// Format of bytes used in a <see cref="Bitmap"/>
	/// </summary>
	/// <remarks>
	/// The format is important when modifying the bytes directly via <see cref="Bitmap.Lock"/>.
	/// </remarks>
	/// <copyright>(c) 2012-2014 by Curtis Wensley</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	public enum PixelFormat
	{
		/// <summary>
		/// 32-bits (4 bytes) per pixel, ordered by an Empty byte in the highest order, followed by Red, Green, and Blue.
		/// </summary>
		Format32bppRgb,
		
		/// <summary>
		/// 24-bits (3 bytes) per pixel, ordered by Red in the highest order, followed by Green, and Blue.
		/// </summary>
		Format24bppRgb,

		/// <summary>
		/// 32-bits (4 bytes) per pixel, ordered by an Alpha byte in the highest order, followed by Red, Green, and Blue.
		/// </summary>
		Format32bppRgba
	}

	/// <summary>
	/// Format of the image to use when saving, loading, etc.
	/// </summary>
	/// <remarks>
	/// The format is typically used only when saving via <see cref="M:Eto.Drawing.Bitmap.Save"/>
	/// </remarks>
	/// <copyright>(c) 2012-2014 by Curtis Wensley</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	public enum ImageFormat
	{
		/// <summary>
		/// Jpeg format
		/// </summary>
		Jpeg,

		/// <summary>
		/// Windows BMP format
		/// </summary>
		Bitmap,

		/// <summary>
		/// Tiff format
		/// </summary>
		Tiff,

		/// <summary>
		/// Portable Network Graphics format
		/// </summary>
		Png,

		/// <summary>
		/// Graphics Interchange Format
		/// </summary>
		Gif
	}

	/// <summary>
	/// Represents an image
	/// </summary>
	/// <copyright>(c) 2012-2014 by Curtis Wensley</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	[Handler(typeof(Bitmap.IHandler))]
	[sc.TypeConverter(typeof(BitmapConverter))]
	public class Bitmap : Image
	{
		new IHandler Handler { get { return (IHandler)base.Handler; } }

		/// <summary>
		/// Loads a bitmap from the resource in the specified or caller's assembly
		/// </summary>
		/// <param name="resourceName">Name of the resource in the caller's assembly to load. E.g. "MyProject.SomeFolder.YourFile.extension"</param>
		/// <param name="assembly">Assembly to load the resource from, or null to use the caller's assembly</param>
		/// <returns>A new instance of a Bitmap loaded from the specified resource</returns>
		public static Bitmap FromResource(string resourceName, Assembly assembly = null)
		{

			if (assembly == null)
			{
#if NETSTANDARD
				if (TypeHelper.GetCallingAssembly == null)
					throw new ArgumentNullException("assembly", string.Format(CultureInfo.CurrentCulture, "This platform doesn't support Assembly.GetCallingAssembly(), so you must pass the assembly directly"));
				assembly = (Assembly)TypeHelper.GetCallingAssembly.Invoke(null, null);
#else
				assembly = Assembly.GetCallingAssembly();
#endif
			}

			using (var stream = assembly.GetManifestResourceStream(resourceName))
			{
				if (stream == null)
					throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, "Resource '{0}' not found in assembly '{1}'", resourceName, assembly.FullName));
				return new Bitmap(stream);
			}
		}

		/// <summary>
		/// Loads a bitmap from a resource in the same assembly as the specified <paramref name="type"/>
		/// </summary>
		/// <returns>The bitmap instance.</returns>
		/// <param name="resourceName">Full name of the resource in the type's assembly. E.g. "MyProject.SomeFolder.YourFile.extension"</param>
		/// <param name="type">Type of the assembly to get the resource.</param>
		public static Bitmap FromResource(string resourceName, Type type)
		{
			if (type == null)
				throw new ArgumentNullException("type");
			return FromResource(resourceName, type.GetAssembly());
		}

		/// <summary>
		/// Initializes a new instance of a Bitmap from a file
		/// </summary>
		/// <param name="fileName">File to load as a bitmap</param>
		public Bitmap(string fileName)
		{
			Handler.Create(fileName);
			Initialize();
		}

		/// <summary>
		/// Initializes a new instance of a Bitmap from a stream
		/// </summary>
		/// <param name="stream">Stream to load from the bitmap</param>
		public Bitmap(Stream stream)
		{
			Handler.Create(stream);
			Initialize();
		}

		/// <summary>
		/// Initializes a new instance of a Bitmap with the specified size and format
		/// </summary>
		/// <param name="size">Size of the bitmap to create</param>
		/// <param name="pixelFormat">Format of each pixel</param>
		public Bitmap(Size size, PixelFormat pixelFormat)
			: this(size.Width, size.Height, pixelFormat)
		{
		}

		/// <summary>
		/// Initializes a new instance of a Bitmap with the specified size and format
		/// </summary>
		/// <param name="width">Width of the new bitmap</param>
		/// <param name="height">Height of the new bitmap</param>
		/// <param name="pixelFormat">Format of each pixel</param>
		public Bitmap(int width, int height, PixelFormat pixelFormat)
		{
			if (width <= 0)
				throw new ArgumentOutOfRangeException("width", "width must be greater than zero");
			if (height <= 0)
				throw new ArgumentOutOfRangeException("height", "height must be greater than zero");
			Handler.Create(width, height, pixelFormat);
			Initialize();
		}

		/// <summary>
		/// Creates a new bitmap optimized for drawing on the specified <paramref name="graphics"/>
		/// </summary>
		/// <param name="width">Width of the bitmap</param>
		/// <param name="height">Height of the bitmap</param>
		/// <param name="graphics">Graphics context the bitmap is intended to be drawn on</param>
		public Bitmap(int width, int height, Graphics graphics)
		{
			if (width <= 0)
				throw new ArgumentOutOfRangeException("width", "width must be greater than zero");
			if (height <= 0)
				throw new ArgumentOutOfRangeException("height", "height must be greater than zero");
			if (graphics == null)
				throw new ArgumentNullException("graphics");
			Handler.Create(width, height, graphics);
			Initialize();
		}

		/// <summary>
		/// Create a new scaled bitmap with the specified <paramref name="width"/> and <paramref name="height"/>
		/// </summary>
		/// <param name="image">Image to scale</param>
		/// <param name="width">Width to scale the source image to</param>
		/// <param name="height">Height to scale the source image to</param>
		/// <param name="interpolation">Interpolation quality</param>
		public Bitmap(Image image, int? width = null, int? height = null, ImageInterpolation interpolation = ImageInterpolation.Default)
		{
			if (image == null)
				throw new ArgumentNullException("image");
			if (width != null && width <= 0)
				throw new ArgumentOutOfRangeException("width", "width must be greater than zero");
			if (height != null && height <= 0)
				throw new ArgumentOutOfRangeException("height", "height must be greater than zero");
			Handler.Create(image, width ?? image.Size.Width, height ?? image.Size.Height, interpolation);
			Initialize();
		}

		/// <summary>
		/// Creates a new bitmap with the specified dimensions, format and pixel data
		/// </summary>
		/// <param name="width">Width of the bitmap</param>
		/// <param name="height">Height of the bitmap</param>
		/// <param name="pixelFormat">Format of each pixel</param>
		/// <param name="data">Color data enumeration, starting from top-left of the bitmap and width*height length</param>
		public Bitmap(int width, int height, PixelFormat pixelFormat, IEnumerable<Color> data)
			: this(width, height, pixelFormat)
		{
			using (var bd = Lock())
				bd.SetPixels(data);
		}

		/// <summary>
		/// Creates a new bitmap with the specified dimensions, format, and integer pixel data
		/// </summary>
		/// <param name="width">Width of the bitmap</param>
		/// <param name="height">Height of the bitmap</param>
		/// <param name="pixelFormat">Format for each pixel</param>
		/// <param name="data">Colordata enumeration, in ARGB format, starting from top-left of the bitmap and width*height length</param>
		public Bitmap(int width, int height, PixelFormat pixelFormat, IEnumerable<int> data)
			: this(width, height, pixelFormat, data.Select(Color.FromArgb))
		{
		}

		/// <summary>
		/// Initializes a new instance of a Bitmap from a <paramref name="bytes"/> array
		/// </summary>
		/// <param name="bytes">Array of bytes containing the image data in one of the supported <see cref="ImageFormat"/> types</param>
		public Bitmap(byte[] bytes)
		{
			if (bytes == null)
				throw new ArgumentNullException("bytes");
			Handler.Create(new MemoryStream(bytes, false));
		}

		/// <summary>
		/// Initializes a new instance of a Bitmap with the specified handler
		/// </summary>
		/// <remarks>
		/// This is intended to be used by platform specific code to return bitmap instances with a particular handler
		/// </remarks>
		/// <param name="handler">Platform handler to use for this instance</param>
		public Bitmap(IHandler handler)
			: base(handler)
		{
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
			if (BitmapData.IsImageLocked(this))
				throw new InvalidOperationException("Image is already locked. Ensure you dispose the BitmapData object explicitly or with a using() block.");

			return Handler.Lock();
		}

		/// <summary>
		/// Saves the bitmap to a file in the specified format
		/// </summary>
		/// <param name="fileName">File to save the bitmap to</param>
		/// <param name="format">Format to save as</param>
		public void Save(string fileName, ImageFormat format)
		{
			Handler.Save(fileName, format);
		}

		/// <summary>
		/// Saves the bitmap to a stream in the specified format
		/// </summary>
		/// <param name="stream">Stream to save the bitmap to</param>
		/// <param name="format">Format to save as</param>
		public void Save(Stream stream, ImageFormat format)
		{
			Handler.Save(stream, format);	
		}

		/// <summary>
		/// Saves the bitmap to an image of the specified <paramref name="imageFormat"/> into a byte array
		/// </summary>
		/// <remarks>
		/// This is merely a helper to save to a byte array instead of a stream.
		/// </remarks>
		/// <param name="imageFormat"></param>
		/// <returns></returns>
		public byte[] ToByteArray(ImageFormat imageFormat)
		{
			using (var memoryStream = new MemoryStream())
			{
				Save(memoryStream, imageFormat);
				return memoryStream.ToArray();
			}
		}

		/// <summary>
		/// Creates a clone of the bitmap
		/// </summary>
		public Bitmap Clone(Rectangle? rectangle = null)
		{
			return Handler.Clone(rectangle);
		}

		/// <summary>
		/// Gets the color of the pixel at the specified <paramref name="position"/>
		/// </summary>
		/// <remarks>
		/// Note that this method can be extremely slow to go through each pixel of a bitmap.
		/// If you need better performance, use <see cref="Lock"/> to get access to the bitmap's pixel buffer directly, 
		/// then optionally use <see cref="BitmapData.GetPixel(Point)"/> to get each pixel value.
		/// </remarks>
		/// <returns>The color of the pixel.</returns>
		/// <param name="position">Position to get the color of the pixel.</param>
		public Color GetPixel(Point position)
		{
			return GetPixel(position.X, position.Y);
		}

		/// <summary>
		/// Gets the color of the pixel at the specified coordinates.
		/// </summary>
		/// <remarks>
		/// Note that this method can be extremely slow to go through each pixel of a bitmap.
		/// If you need better performance, use <see cref="Lock"/> to get access to the bitmap's pixel buffer directly, 
		/// then optionally use <see cref="BitmapData.GetPixel(int,int)"/> to get each pixel value.
		/// </remarks>
		/// <returns>The color of the pixel at the specified coordinates</returns>
		/// <param name="x">The x coordinate</param>
		/// <param name="y">The y coordinate</param>
		public Color GetPixel(int x, int y)
		{
			return Handler.GetPixel(x, y);
		}

		/// <summary>
		/// Sets the pixel color at the specified <paramref name="position"/>.
		/// </summary>
		/// <remarks>
		/// Note that this method can be extremely slow to set each pixel of a bitmap.
		/// If you need better performance, use <see cref="Lock"/> to get access to the bitmap's pixel buffer directly, 
		/// then optionally use <see cref="BitmapData.SetPixel(Point,Color)"/> to set each pixel value.
		/// </remarks>
		/// <param name="position">Position to set the pixel color.</param>
		/// <param name="color">Color to set.</param>
		public void SetPixel(Point position, Color color)
		{
			SetPixel(position.X, position.Y, color);
		}

		/// <summary>
		/// Sets the color of the pixel at the specified coordinates.
		/// </summary>
		/// <remarks>
		/// Note that this method can be extremely slow to set each pixel of a bitmap.
		/// If you need better performance, use <see cref="Lock"/> to get access to the bitmap's pixel buffer directly, 
		/// then optionally use <see cref="BitmapData.SetPixel(int,int,Color)"/> to set each pixel value.
		/// </remarks>
		/// <param name="x">The x coordinate of the pixel to set.</param>
		/// <param name="y">The y coordinate of the pixel to set.</param>
		/// <param name="color">Color to set the pixel to.</param>
		public void SetPixel(int x, int y, Color color)
		{
			using (var bd = Lock())
			{
				bd.SetPixel(x, y, color);
			}
		}

		/// <summary>
		/// Gets an Icon representation of this Bitmap scaled to draw within the specified fitting size.
		/// </summary>
		/// <remarks>
		/// This is useful when you want to draw the image at a different size than the default size without resizing the image.
		/// Note that the <paramref name="width"/> and <paramref name="height"/> specifies the maxiumum drawing size of the Icon, but will not
		/// change the aspect of each frame's bitmap.  For example, if an existing frame is 128x128, and you specify 16x32,
		/// then the resulting frame will draw at 16x16.
		/// </remarks>
		/// <returns>A new icon that will draw within the fitting size.</returns>
		/// <param name="width">Maxiumum drawing width for the new icon.</param>
		/// <param name="height">Maxiumum drawing height for the new icon.</param>
		public Icon WithSize(int width, int height)
		{
			return new Icon(1, this).WithSize(width, height);
		}

		/// <summary>
		/// Gets an Icon representation of this Bitmap scaled to draw within the specified fitting size.
		/// </summary>
		/// <remarks>
		/// This is useful when you want to draw the image at a different size than the default size without resizing the image.
		/// Note that the <paramref name="fittingSize"/> specifies the maxiumum drawing size of the Icon, but will not
		/// change the aspect of each frame's bitmap.  For example, if an existing frame is 128x128, and you specify 16x32,
		/// then the resulting frame will draw at 16x16.
		/// </remarks>
		/// <returns>A new icon that will draw within the fitting size.</returns>
		/// <param name="fittingSize">The maximum size to draw the Icon.</param>
		public Icon WithSize(Size fittingSize)
		{
			return new Icon(1, this).WithSize(fittingSize);
		}

		#region Handler

		/// <summary>
		/// Handler interface for the <see cref="Bitmap"/> class
		/// </summary>
		/// <copyright>(c) 2012-2014 by Curtis Wensley</copyright>
		/// <license type="BSD-3">See LICENSE for full terms</license>
		[AutoInitialize(false)]
		public new interface IHandler : Image.IHandler, ILockableImage
		{
			/// <summary>
			/// Create a bitmap from a file
			/// </summary>
			/// <param name="fileName">File to load as a bitmap</param>
			void Create(string fileName);

			/// <summary>
			/// Create a bitmap from a specified stream
			/// </summary>
			/// <param name="stream">Stream to load from the bitmap</param>
			void Create(Stream stream);

			/// <summary>
			/// Creates a new bitmap in-memory with the specified format
			/// </summary>
			/// <param name="width">Initial width of the bitmap</param>
			/// <param name="height">Initial height of the bitmap</param>
			/// <param name="pixelFormat">Format of each of the pixels in the bitmap</param>
			void Create(int width, int height, PixelFormat pixelFormat);

			/// <summary>
			/// Creates a new bitmap optimized for drawing on the specified <paramref name="graphics"/>
			/// </summary>
			/// <param name="width">Width of the bitmap</param>
			/// <param name="height">Height of the bitmap</param>
			/// <param name="graphics">Graphics context the bitmap is intended to be drawn on</param>
			void Create(int width, int height, Graphics graphics);

			/// <summary>
			/// Create a new scaled bitmap with the specified <paramref name="width"/> and <paramref name="height"/>
			/// </summary>
			/// <param name="image">Image to scale</param>
			/// <param name="width">Width to scale the source image to</param>
			/// <param name="height">Height to scale the source image to</param>
			/// <param name="interpolation">Interpolation quality</param>
			void Create(Image image, int width, int height, ImageInterpolation interpolation);

			/// <summary>
			/// Saves the bitmap to a stream in the specified format
			/// </summary>
			/// <param name="stream">Stream to save the bitmap to</param>
			/// <param name="format">Format to save as</param>
			void Save(Stream stream, ImageFormat format);

			/// <summary>
			/// Saves the bitmap to a file in the specified format
			/// </summary>
			/// <param name="fileName">File to save the bitmap to</param>
			/// <param name="format">Format to save as</param>
			void Save(string fileName, ImageFormat format);

			/// <summary>
			/// Creates a clone of the bitmap
			/// </summary>
			/// <param name="rectangle">If specified, the region of the bitmap to clone</param>
			/// <returns></returns>
			Bitmap Clone(Rectangle? rectangle = null);

			/// <summary>
			/// Gets the color of the pixel at the specified coordinates
			/// </summary>
			/// <returns>The color of the pixel at the specified coordinates</returns>
			/// <param name="x">The x coordinate</param>
			/// <param name="y">The y coordinate</param>
			Color GetPixel(int x, int y);
		}

		#endregion
	}
}
