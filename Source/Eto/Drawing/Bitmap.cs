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
	public enum PixelFormat
	{
		//Format16bppRgb555,

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
	/// The format is typically used only when saving via <see cref="M:Bitmap.Save"/>
	/// </remarks>
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
		/// Handler interface for the <see cref="Bitmap"/> class
		/// </summary>
	public interface IBitmap : IImage
	{
		/// <summary>
		/// Create a bitmap from a file
		/// </summary>
		/// <param name="fileName">File to load as a bitmap</param>
		void Create (string fileName);

		/// <summary>
		/// Create a bitmap from a specified stream
		/// </summary>
		/// <param name="stream">Stream to load from the bitmap</param>
		void Create (Stream stream);

		/// <summary>
		/// Creates a new bitmap in-memory with the specified format
		/// </summary>
		/// <param name="width">Initial width of the bitmap</param>
		/// <param name="height">Initial height of the bitmap</param>
		/// <param name="pixelFormat">Format of each of the pixels in the bitmap</param>
		void Create (int width, int height, PixelFormat pixelFormat);

        void Create(int width, int height, Graphics graphics);

		/// <summary>
		/// Resizes the image to the specified width and height
		/// </summary>
		/// <remarks>
		/// This will scale the existing image to the desired size
		/// </remarks>
		/// <param name="width">New width for the resized image</param>
		/// <param name="height">New height for the resized image</param>
		void Resize (int width, int height);

		/// <summary>
		/// Locks the data of the image to directly access the bytes of the image
		/// </summary>
		/// <remarks>
		/// This locks the data to read and write to directly using unsafe pointers. After reading or updating
		/// the data, you must call <see cref="Unlock"/> to unlock the data.
		/// </remarks>
		/// <returns>A <see cref="BitmapData"/> object with information about the locked data</returns>
		BitmapData Lock ();

		/// <summary>
		/// Unlocks the previously locked data
		/// </summary>
		/// <remarks>
		/// This will unlock the data, and in some platforms write the data back to the image.  You must
		/// call this method before using the bitmap again.
		/// </remarks>
		/// <param name="bitmapData">The data previously locked via the <see cref="Lock"/> method</param>
		void Unlock (BitmapData bitmapData);

		/// <summary>
		/// Saves the bitmap to a stream in the specified format
		/// </summary>
		/// <param name="stream">Stream to save the bitmap to</param>
		/// <param name="format">Format to save as</param>
		void Save (Stream stream, ImageFormat format);

        IBitmap Clone();

        Color GetPixel(int x, int y);

        byte[] ToPNGByteArray();
	}
	
	/// <summary>
	/// Represents an image
	/// </summary>
	/// <remarks>
	/// The Bitmap object
	/// </remarks>
	public class Bitmap : Image
	{
		new IBitmap Handler { get { return (IBitmap)base.Handler; } }

		/// <summary>
		/// Loads a bitmap from the specified resource in the caller's assembly
		/// </summary>
		/// <param name="resourceName">Name of the resource in the caller's assembly to load</param>
		/// <returns>A new instance of a Bitmap loaded from the specified resource</returns>
		public static Bitmap FromResource (string resourceName)
		{
			var asm = Assembly.GetCallingAssembly ();
			return FromResource (asm, resourceName);
		}

		/// <summary>
		/// Loads a bitmap from the resource in the specified assembly
		/// </summary>
		/// <param name="asm">Assembly to load the resource from</param>
		/// <param name="resourceName">Resource to load in the specified assembly</param>
		/// <returns>A new instance of the Bitmap loaded from the resource</returns>
		public static Bitmap FromResource (Assembly asm, string resourceName)
		{
			if (asm == null) asm = Assembly.GetCallingAssembly ();
			using (var stream = asm.GetManifestResourceStream (resourceName)) {
				if (stream == null)
					throw new ResourceNotFoundException (asm, resourceName);
				return new Bitmap (stream);
			}
		}

		/// <summary>
		/// Obsolete. Do not use.
		/// </summary>
		[Obsolete ("use Bitmap.FromResource")]
		public Bitmap (Assembly asm, string resourceName)
			: this (Generator.Current)
		{
			if (asm == null) asm = Assembly.GetCallingAssembly ();
			using (var stream = asm.GetManifestResourceStream (resourceName)) {
				if (stream == null)
					throw new ResourceNotFoundException (asm, resourceName);
				Handler.Create (stream);
			}
		}

		/// <summary>
		/// Initializes a new instance of a Bitmap from a file
		/// </summary>
		/// <param name="fileName">File to load as a bitmap</param>
		public Bitmap (string fileName)
			: this (Generator.Current, fileName)
		{
		}

		/// <summary>
		/// Initializes a new instance of a Bitmap from a stream
		/// </summary>
		/// <param name="stream">Stream to load from the bitmap</param>
		public Bitmap (Stream stream)
			: this (Generator.Current, stream)
		{
		}

		/// <summary>
		/// Initializes a new instance of a Bitmap with the specified size and format
		/// </summary>
		/// <param name="size">Size of the bitmap to create</param>
		/// <param name="pixelFormat">Format of each pixel</param>
		public Bitmap (Size size, PixelFormat pixelFormat) 
            : this(size.Width, size.Height, pixelFormat)
		{
		}

		/// <summary>
		/// Initializes a new instance of a Bitmap with the specified size and format
		/// </summary>
		/// <param name="width">Width of the new bitmap</param>
		/// <param name="height">Height of the new bitmap</param>
		/// <param name="pixelFormat">Format of each pixel</param>
		public Bitmap (int width, int height, PixelFormat pixelFormat)
			: this (Generator.Current, width, height, pixelFormat)
		{
		}

        public Bitmap(int width, int height, Graphics graphics) :
            this(Generator.Current)
        {
            Handler.Create(width, height, graphics);
        }

        /// <summary>
		/// Initializes a new instance of a Bitmap from a <paramref name="bytes"/> array
		/// </summary>
		/// <param name="bytes">Array of bytes containing the image data in one of the supported <see cref="ImageFormat"/> types</param>
		public Bitmap (byte[] bytes)
			: this (Generator.Current, bytes)
		{
		}

        /// <summary>
		/// Initializes a new instance of a Bitmap from a file
		/// </summary>
		/// <param name="generator">Generator to use to create the bitmap</param>
		/// <param name="fileName">File to load as a bitmap</param>
		public Bitmap (Generator generator, string fileName)
			: this (generator)
		{
			Handler.Create (fileName);
		}

		/// <summary>
		/// Initializes a new instance of a Bitmap from a stream
		/// </summary>
		/// <param name="generator">Generator to use to create the bitmap</param>
		/// <param name="stream">Stream to load from the bitmap</param>
		public Bitmap (Generator generator, Stream stream)
			: this (generator)
		{
			Handler.Create (stream);
		}

		/// <summary>
		/// Initializes a new instance of a Bitmap from a <paramref name="bytes"/> array
		/// </summary>
		/// <param name="generator">Generator to use to create the bitmap</param>
		/// <param name="bytes">Array of bytes containing the image data in one of the supported <see cref="ImageFormat"/> types</param>
        public Bitmap(Generator generator, byte[] bytes)
            : this(generator, new MemoryStream(bytes))
        {
        }

		/// <summary>
		/// Initializes a new instance of a Bitmap with the specified size and format
		/// </summary>
		/// <param name="generator">Generator to use to create the bitmap</param>
		/// <param name="width">Width of the new bitmap</param>
		/// <param name="height">Height of the new bitmap</param>
		/// <param name="pixelFormat">Format of each pixel</param>
		public Bitmap (Generator generator, int width, int height, PixelFormat pixelFormat)
			: this(generator)
		{
			Handler.Create (width, height, pixelFormat);
		}
		
		Bitmap (Generator generator)
			: base(generator, typeof(IBitmap))
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
		public Bitmap (Generator generator, IBitmap handler)
			: base(generator, handler)
		{
		}

		/// <summary>
		/// Resizes the image to the specified width and height
		/// </summary>
		/// <remarks>
		/// This will scale the existing image to the desired size
		/// </remarks>
		/// <param name="width">New width for the resized image</param>
		/// <param name="height">New height for the resized image</param>
		public void Resize (int width, int height)
		{
			Handler.Resize (width, height);
		}

		/// <summary>
		/// Locks the data of the image to directly access the bytes of the image
		/// </summary>
		/// <remarks>
		/// This locks the data to read and write to directly using unsafe pointers. After reading or updating
		/// the data, you must call <see cref="Unlock"/> to unlock the data.
		/// </remarks>
		/// <returns>A <see cref="BitmapData"/> object with information about the locked data</returns>
		public BitmapData Lock ()
		{
			return Handler.Lock ();
		}

		/// <summary>
		/// Unlocks the previously locked data
		/// </summary>
		/// <remarks>
		/// This will unlock the data, and in some platforms write the data back to the image.  You must
		/// call this method before using the bitmap again.
		/// </remarks>
		/// <param name="bitmapData">The data previously locked via the <see cref="Lock"/> method</param>
		public void Unlock (BitmapData bitmapData)
		{
			Handler.Unlock (bitmapData);
		}

		/// <summary>
		/// Saves the bitmap to a file in the specified format
		/// </summary>
		/// <param name="fileName">File to save the bitmap to</param>
		/// <param name="format">Format to save as</param>
		public void Save (string fileName, ImageFormat format)
		{
			using (var stream = new FileStream(fileName, FileMode.Create, FileAccess.Write)) {
				Save (stream, format);
			}
		}

		/// <summary>
		/// Saves the bitmap to a stream in the specified format
		/// </summary>
		/// <param name="stream">Stream to save the bitmap to</param>
		/// <param name="format">Format to save as</param>
		public void Save (Stream stream, ImageFormat format)
		{
			Handler.Save (stream, format);	
		}

		/// <summary>
		/// Saves the bitmap to an image of the specified <paramref name="imageFormat"/> into a byte array
		/// </summary>
		/// <remarks>
		/// This is merely a helper to save to a byte array instead of a stream.
		/// </remarks>
		/// <param name="imageFormat"></param>
		/// <returns></returns>
		public byte[] ToByteArray (ImageFormat imageFormat)
		{
			using (var memoryStream = new MemoryStream ()) {
				this.Save (memoryStream, imageFormat);
				return memoryStream.ToArray ();
			}
		}
        public byte[] ToPNGByteArray()
        {
            return Handler.ToPNGByteArray();
        }

        public string ToPNGBase64String()
        {
            string result = null;

            var bytes = this.ToPNGByteArray();

            if (bytes != null)
                result =
                    System.Convert.ToBase64String(
                        bytes);

            return result;
        }

        public Bitmap Clone()
        {
            return
                new Bitmap(
                    this.Generator,
                    this.Handler.Clone());
        }

        public Color GetPixel(int x, int y)
        {
            return Handler.GetPixel(x, y);
        }
    }
}
