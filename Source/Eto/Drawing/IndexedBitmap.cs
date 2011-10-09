using System;
using System.IO;

namespace Eto.Drawing
{

	public interface IIndexedBitmap : IImage
	{
		void Create(int width, int height, int bitsPerPixel);
		void Resize(int width, int height);

		BitmapData Lock();
		void Unlock(BitmapData bitmapData);

		Palette Palette { get; set; }
	}

	public class IndexedBitmap : Image
	{
		IIndexedBitmap inner;
		
		public IndexedBitmap(int width, int height, int bitsPerPixel)
			: this(Generator.Current, width, height, bitsPerPixel)
		{
		}
		
		public IndexedBitmap(Generator g, int width, int height, int bitsPerPixel) : base(g, typeof(IIndexedBitmap))
		{
			inner = (IIndexedBitmap)Handler;
			inner.Create(width, height, bitsPerPixel);
		}

		public void Resize(int width, int height)
		{
			inner.Resize(width, height);
		}

		public BitmapData Lock()
		{
			return inner.Lock();
		}

		public void Unlock(BitmapData bitmapData)
		{
			inner.Unlock(bitmapData);
		}

		public Palette Palette 
		{
			get { return inner.Palette; }
			set { inner.Palette = value; }
		}

	}
}
