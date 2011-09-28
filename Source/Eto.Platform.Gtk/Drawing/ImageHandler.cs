using System;
using System.IO;
using Eto.Drawing;

namespace Eto.Platform.GtkSharp.Drawing
{
	public interface IGtkPixbuf
	{
		Gdk.Pixbuf Pixbuf { get; }

		Gdk.Pixbuf GetPixbuf (Size maxSize);
	}
	
	public interface IImageHandler
	{
		void DrawImage (GraphicsHandler graphics, int x, int y);

		void DrawImage (GraphicsHandler graphics, int x, int y, int width, int height);

		void DrawImage (GraphicsHandler graphics, Rectangle source, Rectangle destination);
		
		void SetImage (Gtk.Image imageView);
		
	}
	
	public abstract class ImageHandler<T, W> : WidgetHandler<T, W>, IImage, IImageHandler
		where W: Image
	{

		#region IImage Members

		public abstract Size Size { get; }
		
		public abstract void SetImage (Gtk.Image imageView);

		#endregion


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
