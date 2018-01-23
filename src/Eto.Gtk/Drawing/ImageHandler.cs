using Eto.Drawing;

namespace Eto.GtkSharp.Drawing
{
	public interface IGtkPixbuf
	{
		Gdk.Pixbuf Pixbuf { get; }

		Gdk.Pixbuf GetPixbuf(Size maxSize, Gdk.InterpType interpolation = Gdk.InterpType.Bilinear, bool shrink = false);
	}

	public interface IImageHandler
	{
		void DrawImage(GraphicsHandler graphics, float x, float y);

		void DrawImage(GraphicsHandler graphics, float x, float y, float width, float height);

		void DrawImage(GraphicsHandler graphics, RectangleF source, RectangleF destination);

		void SetImage(Gtk.Image imageView, Gtk.IconSize? iconSize);
	}

	public abstract class ImageHandler<TControl, TWidget> : WidgetHandler<TControl, TWidget>, Image.IHandler, IImageHandler
		where TWidget: Image
	{

		public abstract Size Size { get; }

		public abstract void SetImage(Gtk.Image imageView, Gtk.IconSize? iconSize);

		public virtual void DrawImage(GraphicsHandler graphics, float x, float y)
		{
			DrawImage(graphics, x, y, Size.Width, Size.Height);
		}

		public virtual void DrawImage(GraphicsHandler graphics, float x, float y, float width, float height)
		{
			DrawImage(graphics, new RectangleF(new Point(0, 0), Size), new RectangleF(x, y, width, height));
		}

		public abstract void DrawImage(GraphicsHandler graphics, RectangleF source, RectangleF destination);
	}
}
