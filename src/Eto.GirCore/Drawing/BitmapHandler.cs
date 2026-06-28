using Eto.Shared.Drawing;
using System.Runtime.InteropServices;

namespace Eto.GirCore.Drawing
{
	public class BitmapDataHandler : BaseBitmapData
	{
		public BitmapDataHandler(Image image, IntPtr data, int scanWidth, int bitsPerPixel, object controlObject, bool premultipliedAlpha)
			: base(image, data, scanWidth, bitsPerPixel, controlObject, premultipliedAlpha)
		{
		}

		public override int TranslateArgbToData(int argb)
		{
			return unchecked((int)(((uint)argb & 0xFF00FF00) | (((uint)argb & 0xFF) << 16) | (((uint)argb & 0xFF0000) >> 16)));
		}

		public override int TranslateDataToArgb(int bitmapData)
		{
			return unchecked((int)(((uint)bitmapData & 0xFF00FF00) | (((uint)bitmapData & 0xFF) << 16) | (((uint)bitmapData & 0xFF0000) >> 16)));
		}
	}

	public class BitmapHandler : WidgetHandler<GdkPixbuf.Pixbuf, Bitmap>, Bitmap.IHandler, IGirImage
	{
		readonly Dictionary<Size, GdkPixbuf.Pixbuf> sizes = new();
		Gdk.Texture? texture;

		public BitmapHandler()
		{
		}

		public BitmapHandler(GdkPixbuf.Pixbuf pixbuf)
		{
			Control = pixbuf;
		}

		public GdkPixbuf.Pixbuf Pixbuf => Control;

		public Gdk.Texture Texture => texture ??= Gdk.Texture.NewForPixbuf(Control);

		public Size Size => new(Control.GetWidth(), Control.GetHeight());

		public void Create(string fileName)
		{
			Control = GdkPixbuf.Pixbuf.NewFromFile(fileName);
			ClearCache();
		}

		public void Create(Stream stream)
		{
			using var memory = new MemoryStream();
			stream.CopyTo(memory);
			using var bytes = GLib.Bytes.New(memory.ToArray());
			using var input = Gio.MemoryInputStream.NewFromBytes(bytes);
			Control = GdkPixbuf.Pixbuf.NewFromStream(input, null);
			ClearCache();
		}

		public void Create(int width, int height, PixelFormat pixelFormat)
		{
			var hasAlpha = pixelFormat != PixelFormat.Format24bppRgb;
			Control = CreatePixbuf(width, height, hasAlpha);
			if (pixelFormat == PixelFormat.Format32bppRgb)
				FillOpaqueAlpha(Control);
			ClearCache();
		}

		public void Create(int width, int height, Graphics graphics)
		{
			Create(width, height, PixelFormat.Format32bppRgba);
		}

		public void Create(Image image, int width, int height, ImageInterpolation interpolation)
		{
			Control = image.ToPixbuf().ScaleSimple(width, height, interpolation.ToPixbuf());
			ClearCache();
		}

		public BitmapData Lock()
		{
			return new BitmapDataHandler(Widget, Control.Pixels, Control.Rowstride, Control.HasAlpha ? 32 : 24, null, false);
		}

		public void Unlock(BitmapData bitmapData)
		{
			ClearCache();
		}

		string ToPixbufFormat(ImageFormat format) => format switch
		{
			ImageFormat.Jpeg => "jpeg",
			ImageFormat.Bitmap => "bmp",
			ImageFormat.Tiff => "tiff",
			ImageFormat.Gif => "gif",
			_ => "png"
		};

		public void Save(string fileName, ImageFormat format)
		{
			Control.Savev(fileName, ToPixbufFormat(format), Array.Empty<string>(), Array.Empty<string>());
		}

		public void Save(Stream stream, ImageFormat format)
		{
			var fileName = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N") + "." + ToPixbufFormat(format));
			try
			{
				Save(fileName, format);
				using var fileStream = File.OpenRead(fileName);
				fileStream.CopyTo(stream);
			}
			finally
			{
				if (File.Exists(fileName))
					File.Delete(fileName);
			}
		}

		public Bitmap Clone(Rectangle? rectangle = null)
		{
			if (rectangle == null)
				return new Bitmap(new BitmapHandler(Control.Copy()));

			var rect = rectangle.Value;
			var pixbuf = Control.NewSubpixbuf(rect.X, rect.Y, rect.Width, rect.Height).Copy();
			return new Bitmap(new BitmapHandler(pixbuf));
		}

		public Color GetPixel(int x, int y)
		{
			using var data = Lock();
			return data.GetPixel(x, y);
		}

		public void SetPixel(int x, int y, Color color)
		{
			using var data = Lock();
			data.SetPixel(x, y, color);
			Unlock(data);
		}

		public GdkPixbuf.Pixbuf GetPixbuf(Size? size = null, ImageInterpolation interpolation = ImageInterpolation.Default)
		{
			if (size == null || size == Size)
				return Control;

			var target = size.Value;
			if (!sizes.TryGetValue(target, out var pixbuf))
			{
				pixbuf = Control.ScaleSimple(target.Width, target.Height, interpolation.ToPixbuf());
				sizes[target] = pixbuf;
			}
			return pixbuf;
		}

		void ClearCache()
		{
			sizes.Clear();
			texture = null;
		}

		static GdkPixbuf.Pixbuf CreatePixbuf(int width, int height, bool hasAlpha)
		{
			var channels = hasAlpha ? 4 : 3;
			var rowstride = width * channels;
			var data = new byte[rowstride * height];
			using var bytes = GLib.Bytes.New(data);
			return GdkPixbuf.Pixbuf.NewFromBytes(bytes, GdkPixbuf.Colorspace.Rgb, hasAlpha, 8, width, height, rowstride);
		}

		static void FillOpaqueAlpha(GdkPixbuf.Pixbuf pixbuf)
		{
			if (!pixbuf.HasAlpha)
				return;

			var rowstride = pixbuf.Rowstride;
			var width = pixbuf.Width;
			var height = pixbuf.Height;
			for (var y = 0; y < height; y++)
			{
				for (var x = 0; x < width; x++)
					Marshal.WriteByte(pixbuf.Pixels, (y * rowstride) + (x * 4) + 3, 0xFF);
			}
		}
	}
}
