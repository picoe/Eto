namespace Eto.GirCore.Drawing
{
	public class IconHandler : WidgetHandler<object, Icon>, Icon.IHandler, IGirImage
	{
		readonly Dictionary<Size, GdkPixbuf.Pixbuf> sizes = new();
		List<IconFrame> frames = new();
		Gdk.Texture? texture;

		public IconHandler()
		{
			Control = new object();
		}

		public GdkPixbuf.Pixbuf Pixbuf => GetPixbuf();

		public Gdk.Texture Texture => texture ??= Gdk.Texture.NewForPixbuf(Pixbuf);

		public Size Size => frames.Count > 0 ? frames[0].Bitmap.Size : Size.Empty;

		public IEnumerable<IconFrame> Frames => frames;

		public void Create(string fileName)
		{
			using var fs = File.OpenRead(fileName);
			Create(fs);
		}

		public void Create(Stream stream)
		{
			using var ms = new MemoryStream();
			stream.CopyTo(ms);
			ms.Position = 0;
			CreateFrames(ms);
			ClearCache();
		}

		const int IconDirSize = 6;
		const int IconDirEntrySize = 16;

		void CreateFrames(MemoryStream input)
		{
			var source = input.ToArray();
			var count = BitConverter.ToInt16(source, 4);
			var result = new List<IconFrame>();

			for (var i = 0; i < count; i++)
			{
				using var destStream = new MemoryStream();
				using var writer = new BinaryWriter(destStream);

				var pos = 0;
				writer.Write(source, pos, IconDirSize - 2);
				writer.Write((short)1);

				pos += IconDirSize + (IconDirEntrySize * i);
				writer.Write(source, pos, IconDirEntrySize - 4);
				writer.Write(IconDirSize + IconDirEntrySize);
				pos += 8;

				var imageSize = BitConverter.ToInt32(source, pos);
				pos += 4;
				var imageOffset = BitConverter.ToInt32(source, pos);
				if (imageOffset + imageSize > source.Length)
					throw new InvalidDataException("Icon is not a valid format.");

				writer.Write(source, imageOffset, imageSize);
				writer.Flush();
				destStream.Position = 0;
				result.Add(new IconFrame(1f, new Bitmap(destStream)));
			}

			if (result.Count == 0)
			{
				input.Position = 0;
				result.Add(new IconFrame(1f, new Bitmap(input)));
			}

			frames = result;
		}

		public void Create(IEnumerable<IconFrame> frames)
		{
			this.frames = new List<IconFrame>(frames);
			ClearCache();
		}

		public GdkPixbuf.Pixbuf GetPixbuf(Size? size = null, ImageInterpolation interpolation = ImageInterpolation.Default)
		{
			var frame = size == null ? Widget.GetFrame(1) : Widget.GetFrame(1, size);
			var pixbuf = frame.Bitmap.ToPixbuf();
			if (size == null || pixbuf.GetWidth() == size.Value.Width && pixbuf.GetHeight() == size.Value.Height)
				return pixbuf;

			if (!sizes.TryGetValue(size.Value, out var scaled))
			{
				scaled = pixbuf.ScaleSimple(size.Value.Width, size.Value.Height, interpolation.ToPixbuf());
				sizes[size.Value] = scaled;
			}
			return scaled;
		}

		void ClearCache()
		{
			sizes.Clear();
			texture = null;
		}
	}
}
