using System;
using System.IO;
using Eto.Drawing;
using MonoTouch.UIKit;

namespace Eto.Platform.iOS.Drawing
{
	interface IImageHandler
	{
		void DrawImage (GraphicsHandler graphics, float x, float y);

		void DrawImage (GraphicsHandler graphics, float x, float y, float width, float height);

		void DrawImage (GraphicsHandler graphics, RectangleF source, RectangleF destination);

		UIImage GetUIImage ();
	}

	public static class ImageExtensions
	{
		public static UIImage ToUIImage (this Image image)
		{
			if (image == null)
				return null;
			var handler = image.Handler as IImageHandler;
			if (handler != null)
				return handler.GetUIImage ();
			else
				return null;
		}
	}

	public abstract class ImageHandler<T, W> : WidgetHandler<T, W>, IImage, IImageHandler
		where T: class
		where W: Image
	{

		public abstract Size Size { get; }

		public virtual void DrawImage (GraphicsHandler graphics, float x, float y)
		{
			DrawImage (graphics, x, y, Size.Width, Size.Height);
		}

		public virtual void DrawImage (GraphicsHandler graphics, float x, float y, float width, float height)
		{
			DrawImage (graphics, new RectangleF (Size), new RectangleF (x, y, width, height));
		}

		public abstract void DrawImage (GraphicsHandler graphics, RectangleF source, RectangleF destination);

		public abstract UIImage GetUIImage ();
		public int Width
		{
			get { return 0;/* TODO */ }
		}
		
		public int Height
		{
			get { return 0;/* TODO */ }
		}
	}
}
