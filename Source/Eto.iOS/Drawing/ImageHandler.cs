using System;
using System.IO;
using Eto.Drawing;
using UIKit;

namespace Eto.iOS.Drawing
{
	interface IImageHandler
	{
		void DrawImage(GraphicsHandler graphics, float x, float y);

		void DrawImage(GraphicsHandler graphics, float x, float y, float width, float height);

		void DrawImage(GraphicsHandler graphics, RectangleF source, RectangleF destination);

		UIImage GetUIImage(int? maxSize = null);
	}

	public abstract class ImageHandler<T, W> : WidgetHandler<T, W>, Image.IHandler, IImageHandler
		where T: class
		where W: Image
	{

		public abstract Size Size { get; }

		public virtual void DrawImage(GraphicsHandler graphics, float x, float y)
		{
			DrawImage(graphics, x, y, Size.Width, Size.Height);
		}

		public virtual void DrawImage(GraphicsHandler graphics, float x, float y, float width, float height)
		{
			DrawImage(graphics, new RectangleF(Size), new RectangleF(x, y, width, height));
		}

		public abstract void DrawImage(GraphicsHandler graphics, RectangleF source, RectangleF destination);

		public abstract UIImage GetUIImage(int? maxSize = null);
	}
}
