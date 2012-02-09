using System;
using System.IO;
using Eto.Drawing;
using MonoMac.AppKit;

namespace Eto.Platform.Mac.Drawing
{
	interface IImageHandler
	{
		NSImage GetImage ();
		
		void DrawImage (GraphicsHandler graphics, int x, int y);

		void DrawImage (GraphicsHandler graphics, int x, int y, int width, int height);

		void DrawImage (GraphicsHandler graphics, Rectangle source, Rectangle destination);
	}

	public abstract class ImageHandler<T, W> : WidgetHandler<T, W>, IImage, IImageHandler
		where T: class
		where W: Image
	{

		public abstract Size Size { get; }

		public abstract NSImage GetImage ();

		public virtual void DrawImage (GraphicsHandler graphics, int x, int y)
		{
			DrawImage (graphics, x, y, Size.Width, Size.Height);
		}

		public virtual void DrawImage (GraphicsHandler graphics, int x, int y, int width, int height)
		{
			DrawImage (graphics, new Rectangle (new Point (0, 0), Size), new Rectangle (x, y, width, height));
		}

		public abstract void DrawImage (GraphicsHandler graphics, Rectangle source, Rectangle destination);

	}
}
