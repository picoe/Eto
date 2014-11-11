using System;
using System.IO;
using System.Reflection;

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
		/// 24-bits (4 bytes) per pixel, ordered by Red in the highest order, followed by Green, and Blue.
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
	public class Bitmap : Image
	{
		new IHandler Handler { get { return (IHandler)base.Handler; } }

		/// <summary>
		/// Loads a bitmap from the resource in the specified or caller's assembly
		/// </summary>
		/// <param name="resourceName">Name of the resource in the caller's assembly to load</param>
		/// <param name="assembly">Assembly to load the resource from, or null to use the caller's assembly</param>
		/// <returns>A new instance of a Bitmap loaded from the specified resource</returns>
		public static Bitmap FromResource(string resourceName, Assembly assembly = null)
		{

			if (assembly == null)
			{
#if PCL
				if (TypeHelper.GetCallingAssembly == null)
					throw new ArgumentNullException("assembly", "This platform doesn't support Assembly.GetCallingAssembly(), so you must pass the assembly directly");
				assembly = (Assembly)TypeHelper.GetCallingAssembly.Invoke(null, new object[0]);
#else
				assembly = Assembly.GetCallingAssembly();
#endif
			}
			using (var stream = assembly.GetManifestResourceStream(resourceName))
			{
				if (stream == null)
					throw new ResourceNotFoundException(assembly, resourceName);
				return new Bitmap(stream);
			}
		}

		/// <summary>
		/// Loads a bitmap from a resource in the same assembly as the specified <paramref name="type"/>
		/// </summary>
		/// <returns>The bitmap instance.</returns>
		/// <param name="resourceName">Full name of the resource in the type's assembly.</param>
		/// <param name="type">Type of the assembly to get the resource.</param>
		public static Bitmap FromResource(string resourceName, Type type)
		{
			if (type == null)
				throw new ArgumentNullException("type");
			#if PCL
			return FromResource(resourceName, type.GetTypeInfo().Assembly);
			#else
			return FromResource(resourceName, type.Assembly);
			#endif
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
			Handler.Create(image, width ?? image.Size.Width, height ?? image.Size.Height, interpolation);
			Initialize();
		}

		/// <summary>
		/// Initializes a new instance of a Bitmap from a <paramref name="bytes"/> array
		/// </summary>
		/// <param name="bytes">Array of bytes containing the image data in one of the supported <see cref="ImageFormat"/> types</param>
		public Bitmap(byte[] bytes)
		{
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
			return Handler.Lock();
		}

		#if !PCL
		/// <summary>
		/// Saves the bitmap to a file in the specified format
		/// </summary>
		/// <param name="fileName">File to save the bitmap to</param>
		/// <param name="format">Format to save as</param>
		public void Save(string fileName, ImageFormat format)
		{
			using (var stream = new FileStream(fileName, FileMode.Create, FileAccess.Write))
			{
				Save(stream, format);
			}
		}
		#endif

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

		#pragma warning disable 612,618

		/// <summary>
		/// Loads a bitmap from the resource in the specified or caller's assembly
		/// </summary>
		/// <param name="resourceName">Name of the resource in the caller's assembly to load</param>
		/// <param name="assembly">Assembly to load the resource from, or null to use the caller's assembly</param>
		/// <param name="generator">Generator for this widget</param>
		/// <returns>A new instance of a Bitmap loaded from the specified resource</returns>
		[Obsolete("Use variation without generator instead")]
		#if PCL
		public static Bitmap FromResource(string resourceName, Assembly assembly, Generator generator)
		#else
		public static Bitmap FromResource(string resourceName, Assembly assembly, Generator generator)
		#endif
		{

			if (assembly == null)
			{
				#if PCL
				if (TypeHelper.GetCallingAssembly == null)
					throw new ArgumentNullException("assembly", "This platform doesn't support Assembly.GetCallingAssembly(), so you must pass the assembly directly");
				assembly = (Assembly)TypeHelper.GetCallingAssembly.Invoke(null, new object[0]);
				#else
				assembly = Assembly.GetCallingAssembly();
				#endif
			}
			using (var stream = assembly.GetManifestResourceStream(resourceName))
			{
				if (stream == null)
					throw new ResourceNotFoundException(assembly, resourceName);
				return new Bitmap(stream, generator);
			}
		}

		/// <summary>
		/// Gets a bitmap from the specified resource.
		/// </summary>
		/// <returns>The resource.</returns>
		/// <param name="resourceName">Resource name.</param>
		/// <param name="type">Type.</param>
		/// <param name="generator">Generator.</param>
		[Obsolete("Use variation without generator instead")]
		public static Bitmap FromResource(string resourceName, Type type, Generator generator)
		{
			#if PCL
			return FromResource(resourceName, type.GetTypeInfo().Assembly, generator);
			#else
			return FromResource(resourceName, type.Assembly, generator);
			#endif
		}

		/// <summary>
		/// Initializes a new instance of a Bitmap from a file
		/// </summary>
		/// <param name="fileName">File to load as a bitmap</param>
		/// <param name="generator">Generator to create the bitmap</param>
		[Obsolete("Use variation without generator instead")]
		public Bitmap(string fileName, Generator generator)
			: this(generator)
		{
			Handler.Create(fileName);
		}

		/// <summary>
		/// Initializes a new instance of a Bitmap from a stream
		/// </summary>
		/// <param name="stream">Stream to load from the bitmap</param>
		/// <param name="generator">Generator to create the bitmap</param>
		[Obsolete("Use variation without generator instead")]
		public Bitmap(Stream stream, Generator generator)
			: this(generator)
		{
			Handler.Create(stream);
		}

		/// <summary>
		/// Initializes a new instance of a Bitmap with the specified size and format
		/// </summary>
		/// <param name="size">Size of the bitmap to create</param>
		/// <param name="pixelFormat">Format of each pixel</param>
		/// <param name="generator">Generator to create the bitmap</param>
		[Obsolete("Use variation without generator instead")]
		public Bitmap(Size size, PixelFormat pixelFormat, Generator generator)
			: this(size.Width, size.Height, pixelFormat, generator)
		{
		}

		/// <summary>
		/// Initializes a new instance of a Bitmap with the specified size and format
		/// </summary>
		/// <param name="width">Width of the new bitmap</param>
		/// <param name="height">Height of the new bitmap</param>
		/// <param name="pixelFormat">Format of each pixel</param>
		/// <param name="generator">Generator to create the bitmap</param>
		[Obsolete("Use variation without generator instead")]
		public Bitmap(int width, int height, PixelFormat pixelFormat, Generator generator)
			: this(generator)
		{
			Handler.Create(width, height, pixelFormat);
		}

		/// <summary>
		/// Creates a new bitmap optimized for drawing on the specified <paramref name="graphics"/>
		/// </summary>
		/// <param name="width">Width of the bitmap</param>
		/// <param name="height">Height of the bitmap</param>
		/// <param name="graphics">Graphics context the bitmap is intended to be drawn on</param>
		/// <param name="generator">Generator to create the bitmap</param>
		[Obsolete("Use variation without generator instead")]
		public Bitmap(int width, int height, Graphics graphics, Generator generator)
			: this(generator)
		{
			Handler.Create(width, height, graphics);
		}

		/// <summary>
		/// Create a new scaled bitmap with the specified <paramref name="width"/> and <paramref name="height"/>
		/// </summary>
		/// <param name="image">Image to scale</param>
		/// <param name="width">Width to scale the source image to</param>
		/// <param name="height">Height to scale the source image to</param>
		/// <param name="interpolation">Interpolation quality</param>
		/// <param name="generator">Generator to create the bitmap</param>
		[Obsolete("Use variation without generator instead")]
		public Bitmap(Image image, int? width, int? height, ImageInterpolation interpolation, Generator generator)
			: this(generator)
		{
			Handler.Create(image, width ?? image.Size.Width, height ?? image.Size.Height, interpolation);
		}

		/// <summary>
		/// Initializes a new instance of a Bitmap from a <paramref name="bytes"/> array
		/// </summary>
		/// <param name="bytes">Array of bytes containing the image data in one of the supported <see cref="ImageFormat"/> types</param>
		/// <param name="generator">Generator to create the bitmap</param>
		[Obsolete("Use variation without generator instead")]
		public Bitmap(byte[] bytes, Generator generator)
			: this(generator)
		{
			Handler.Create(new MemoryStream(bytes, false));
		}

		[Obsolete("Use variation without generator instead")]
		Bitmap(Generator generator)
			: base(generator, typeof(IHandler))
		{
		}

		/// <summary>
		/// Initializes a new instance of a Bitmap with the specified handler
		/// </summary>
		/// <remarks>
		/// This is intended to be used by platform specific code to return bitmap instances with a particular handler
		/// </remarks>
		/// <param name="generator">Generator the handler is created from</param>
		/// <param name="handler">Platform handler to use for this instance</param>
		[Obsolete("Use variation without generator instead")]
		public Bitmap(Generator generator, IHandler handler)
			: base(generator, handler)
		{
		}

		#pragma warning restore 612,618

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
