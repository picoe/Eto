namespace Eto.GirCore.Drawing
{
	public interface IGirImage
	{
		GdkPixbuf.Pixbuf Pixbuf { get; }

		Gdk.Texture Texture { get; }

		GdkPixbuf.Pixbuf GetPixbuf(Size? size = null, ImageInterpolation interpolation = ImageInterpolation.Default);
	}

	internal static class GirImageHelper
	{
		public static GdkPixbuf.InterpType ToPixbuf(this ImageInterpolation interpolation) => interpolation switch
		{
			ImageInterpolation.None => GdkPixbuf.InterpType.Nearest,
			ImageInterpolation.Low => GdkPixbuf.InterpType.Tiles,
			ImageInterpolation.Medium => GdkPixbuf.InterpType.Bilinear,
			ImageInterpolation.High => GdkPixbuf.InterpType.Hyper,
			_ => GdkPixbuf.InterpType.Bilinear
		};

		public static GdkPixbuf.Pixbuf ToPixbuf(this Image image, Size? size = null, ImageInterpolation interpolation = ImageInterpolation.Default)
		{
			if (image.Handler is IGirImage girImage)
				return girImage.GetPixbuf(size, interpolation);
			throw new NotSupportedException($"Image handler '{image.Handler?.GetType().FullName}' does not expose a GirCore pixbuf.");
		}

		public static Gdk.Texture ToTexture(this Image image, Size? size = null, ImageInterpolation interpolation = ImageInterpolation.Default)
		{
			if (image.Handler is IGirImage girImage)
			{
				if (size == null)
					return girImage.Texture;
				return Gdk.Texture.NewForPixbuf(girImage.GetPixbuf(size, interpolation));
			}
			throw new NotSupportedException($"Image handler '{image.Handler?.GetType().FullName}' does not expose a GirCore texture.");
		}

		public static Gtk.Image CreateImage(Image image, Size? size = null, ImageInterpolation interpolation = ImageInterpolation.Default)
		{
			var control = Gtk.Image.New();
			control.SetFromPaintable(image.ToTexture(size, interpolation));
			return control;
		}
	}
}
