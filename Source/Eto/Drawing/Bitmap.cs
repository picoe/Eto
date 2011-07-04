using System;
using System.IO;

namespace Eto.Drawing
{
	public enum PixelFormat
	{
		Format32bppRgb,
		Format24bppRgb,
		Format16bppRgb555
	}
	
	public enum ImageFormat
	{
		Jpeg,
		Bitmap,
		Tiff,
		Png,
		Gif
	}

	public interface IBitmap : IImage
	{
		void Create (string fileName);
		
		void Create (Stream stream);
		
		void Create (int width, int height, PixelFormat pixelFormat);
		
		void Resize (int width, int height);
		
		BitmapData Lock ();
		
		void Unlock (BitmapData bitmapData);
		
		void Save (Stream stream, ImageFormat format);
	}
	
	public class Bitmap : Image
	{
		IBitmap inner;
		
		public Bitmap (string fileName) : this(Generator.Current, fileName)
		{
		}

		public Bitmap (Stream stream) : this(Generator.Current, stream)
		{
		}
		
		public Bitmap (int width, int height, PixelFormat pixelFormat) : this(Generator.Current, width, height, pixelFormat)
		{
		}
		
		public Bitmap (Generator g, string fileName) : this(g)
		{
			inner.Create (fileName);
		}
		
		public Bitmap (Generator g, Stream stream) : this(g)
		{
			inner.Create (stream);
		}

		public Bitmap (Generator g, int width, int height, PixelFormat pixelFormat) : this(g)
		{
			inner.Create (width, height, pixelFormat);
		}
		
		private Bitmap (Generator g)
			: base(g, typeof(IBitmap))
		{
			inner = (IBitmap)Handler;
		}

		public Bitmap (Generator g, IBitmap handler)
			: base(g, handler)
		{
			inner = (IBitmap)handler;
		}
		
		public void Resize (int width, int height)
		{
			inner.Resize (width, height);
		}

		public BitmapData Lock ()
		{
			return inner.Lock ();
		}

		public void Unlock (BitmapData bitmapData)
		{
			inner.Unlock (bitmapData);
		}

		public void Save (string fileName, ImageFormat format)
		{
			using (var stream = new FileStream(fileName, FileMode.Create, FileAccess.Write)) {
				Save (stream, format);
			}
		}
		
		public void Save (Stream stream, ImageFormat format)
		{
			inner.Save (stream, format);	
		} 

	}
}
