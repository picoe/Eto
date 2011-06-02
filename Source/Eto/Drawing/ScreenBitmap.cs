using System;
using System.Reflection;
using System.IO;

namespace Eto.Drawing
{
	public interface IScreenBitmap : IImage
	{
		void Create(Bitmap bitmap);
	}
	
	public class ScreenBitmap : Image
	{
		IScreenBitmap inner;
		public ScreenBitmap(Generator g, Bitmap bitmap) : base(g, typeof(IIndexedBitmap))
		{
			inner = (IScreenBitmap)Handler;
			inner.Create(bitmap);
		}
	}
}
