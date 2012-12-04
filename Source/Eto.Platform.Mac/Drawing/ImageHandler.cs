using System;
using System.IO;
using Eto.Drawing;
using MonoMac.AppKit;

namespace Eto.Platform.Mac.Drawing
{
	interface IImageSource
	{
		NSImage GetImage ();
	}
	
	interface IImageHandler : IImageSource
	{
		void DrawImage (GraphicsHandler graphics, float x, float y);

		void DrawImage (GraphicsHandler graphics, float x, float y, float width, float height);

		void DrawImage (GraphicsHandler graphics, RectangleF source, RectangleF destination);
	}

	public abstract class ImageHandler<T, W> : WidgetHandler<T, W>, IImage, IImageHandler
		where T: class
		where W: Image
	{

		public abstract Size Size { get; }

        public abstract int Width { get; }

        public abstract int Height { get; }

		public abstract NSImage GetImage ();

		public virtual void DrawImage (GraphicsHandler graphics, float x, float y)
		{
			DrawImage (graphics, x, y, Size.Width, Size.Height);
		}

		public virtual void DrawImage (GraphicsHandler graphics, float x, float y, float width, float height)
		{
			DrawImage (graphics, new RectangleF (PointF.Empty, Size), new RectangleF (x, y, width, height));
		}

		public abstract void DrawImage (GraphicsHandler graphics, RectangleF source, RectangleF destination);

	}
}
