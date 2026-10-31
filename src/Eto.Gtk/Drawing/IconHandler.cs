namespace Eto.GtkSharp.Drawing
{

	public class IconHandler : ImageHandler<Gtk.IconSet, Icon>, Icon.IHandler, IGtkPixbuf
	{
		readonly Dictionary<Size, Gdk.Pixbuf> sizes = new Dictionary<Size, Gdk.Pixbuf>();

		public Gdk.Pixbuf Pixbuf { get; set; }

		#region IIcon Members

		public void Create(Stream stream)
		{
			using var ms = new MemoryStream();
			stream.CopyTo(ms);
			var bytes = ms.ToArray();
			// Window and tray icons use only the primary image, so the frames are split on first request.
			// The directory is still checked here so a malformed file fails in the constructor.
			ValidateDirectory(bytes);
			ms.Position = 0;
			try
			{
				Pixbuf = new Gdk.Pixbuf(ms);
				frames = new Lazy<List<IconFrame>>(() => ToFrames(SplitIcon(bytes)));
			}
			catch (GLib.GException)
			{
				// GDK cannot choose a primary image when every frame is PNG-compressed.
				var pixbufs = SplitIcon(bytes);
				var largest = pixbufs[0];
				foreach (var pb in pixbufs)
				{
					if ((long)pb.Width * pb.Height > (long)largest.Width * largest.Height)
						largest = pb;
				}
				Pixbuf = largest.Copy();
				var frameList = ToFrames(pixbufs);
				frames = new Lazy<List<IconFrame>>(() => frameList);
			}
			Control = new Gtk.IconSet(Pixbuf);
		}

		public void Create(string fileName)
		{
			using var fs = File.OpenRead(fileName);
			Create(fs);
		}

		const int sICONDIR = 6;            // sizeof(ICONDIR) 
		const int sICONDIRENTRY = 16;      // sizeof(ICONDIRENTRY)
		static readonly byte[] pngSignature = { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A };

		public Gdk.Pixbuf[] SplitIcon(MemoryStream input)
		{
			return SplitIcon(input.ToArray());
		}

		Gdk.Pixbuf[] SplitIcon(byte[] srcBuf)
		{
			int count = ValidateDirectory(srcBuf);
			var splitIcons = new Gdk.Pixbuf[count];
			try
			{
				for (int i = 0; i < count; i++)
				{
					int entryOffset = sICONDIR + sICONDIRENTRY * i;
					int imgSize = BitConverter.ToInt32(srcBuf, entryOffset + 8); // ICONDIRENTRY.dwBytesInRes
					int imgOffset = BitConverter.ToInt32(srcBuf, entryOffset + 12); // ICONDIRENTRY.dwImageOffset

					// PNG frames are complete images; DIB frames need a single-frame ICO header.
					var isPng = HasPngSignature(srcBuf, imgOffset, imgSize);
					using var imageStream = isPng
						? new MemoryStream(srcBuf, imgOffset, imgSize, false)
						: new MemoryStream();
					if (!isPng)
					{
						using var writer = new BinaryWriter(imageStream, Encoding.UTF8, leaveOpen: true);
						writer.Write(srcBuf, 0, sICONDIR - 2);
						writer.Write((short)1);
						writer.Write(srcBuf, entryOffset, sICONDIRENTRY - 4);
						writer.Write(sICONDIR + sICONDIRENTRY);
						writer.Write(srcBuf, imgOffset, imgSize);
					}
					imageStream.Position = 0;
					var pixbuf = new Gdk.Pixbuf(imageStream);
					splitIcons[i] = pixbuf;
					var width = srcBuf[entryOffset] == 0 ? 256 : srcBuf[entryOffset];
					var height = srcBuf[entryOffset + 1] == 0 ? 256 : srcBuf[entryOffset + 1];
					if (pixbuf.Width != width || pixbuf.Height != height)
						throw new InvalidDataException("Icon not a valid format");
				}
				return splitIcons;
			}
			catch
			{
				foreach (var pixbuf in splitIcons)
					pixbuf?.Dispose();
				throw;
			}
		}

		static bool HasPngSignature(byte[] buffer, int offset, int length)
		{
			if (length < pngSignature.Length)
				return false;
			for (int i = 0; i < pngSignature.Length; i++)
				if (buffer[offset + i] != pngSignature[i])
					return false;
			return true;
		}

		// Checks the ICONDIR and every ICONDIRENTRY without decoding an image; returns the entry count.
		static int ValidateDirectory(byte[] srcBuf)
		{
			if (srcBuf.Length < sICONDIR)
				throw new InvalidDataException("Icon not a valid format");

			var type = BitConverter.ToUInt16(srcBuf, 2);
			int count = BitConverter.ToUInt16(srcBuf, 4); // ICONDIR.idCount
			int directoryEnd = sICONDIR + sICONDIRENTRY * count;
			if (BitConverter.ToUInt16(srcBuf, 0) != 0 || (type != 1 && type != 2)
				|| count == 0 || directoryEnd > srcBuf.Length)
				throw new InvalidDataException("Icon not a valid format");

			for (int i = 0; i < count; i++)
			{
				int entryOffset = sICONDIR + sICONDIRENTRY * i;
				int imgSize = BitConverter.ToInt32(srcBuf, entryOffset + 8); // ICONDIRENTRY.dwBytesInRes
				int imgOffset = BitConverter.ToInt32(srcBuf, entryOffset + 12); // ICONDIRENTRY.dwImageOffset
				if (imgOffset < directoryEnd || imgSize <= 0 || imgOffset > srcBuf.Length - imgSize)
					throw new InvalidDataException("Icon not a valid format");
			}
			return count;
		}

		static List<IconFrame> ToFrames(Gdk.Pixbuf[] pixbufs)
		{
			var frames = new List<IconFrame>(pixbufs.Length);
			foreach (var pb in pixbufs)
				frames.Add(IconFrame.FromControlObject(1, new Bitmap(new BitmapHandler(pb))));
			return frames;
		}

		#endregion

		public override Size Size
		{
			get
			{
				return new Size(Pixbuf.Width, Pixbuf.Height);
			}
		}

		public override void SetImage(Gtk.Image imageView, Gtk.IconSize? iconSize)
		{
			if (iconSize != null)
				imageView.SetFromIconSet(Control, iconSize.Value);
			else
				imageView.Pixbuf = Pixbuf;
		}

		public override void DrawImage(GraphicsHandler graphics, RectangleF source, RectangleF destination)
		{
			var context = graphics.Control;
			context.Save();
			context.Rectangle(destination.ToCairo());
			double scalex = 1;
			double scaley = 1;
			var pb = Widget.GetFrame(1, Size.Ceiling(destination.Size));
			if (Math.Abs(source.Width - destination.Width) > 0.5f || Math.Abs(source.Height - destination.Height) > 0.5f)
			{
				scalex = (double)destination.Width / (double)source.Width;
				scaley = (double)destination.Height / (double)source.Height;
				scalex *= (double)Size.Width / (double)pb.PixelSize.Width;
				scaley *= (double)Size.Height / (double)pb.PixelSize.Height;
				context.Scale(scalex, scaley);
			}
			Gdk.CairoHelper.SetSourcePixbuf(context, pb.Bitmap.ToGdk(), (destination.Left / scalex) - source.Left, (destination.Top / scaley) - source.Top);
			var pattern = (Cairo.SurfacePattern)context.GetSource();
			pattern.Filter = graphics.ImageInterpolation.ToCairo();
			context.Fill();
			context.Restore();
			pattern.Dispose();
		}

		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
			if (disposing)
			{
				frames = null;
				foreach (var scaled in sizes.Values)
					scaled.Dispose();
				sizes.Clear();
				if (Pixbuf != null)
				{
					Pixbuf.Dispose();
					Pixbuf = null;
				}
			}
		}

		public Gdk.Pixbuf GetPixbuf(Size maxSize, Gdk.InterpType interpolation = Gdk.InterpType.Bilinear, bool shrink = false)
		{
			Gdk.Pixbuf pixbuf = Pixbuf;
			if (pixbuf.Width > maxSize.Width && pixbuf.Height > maxSize.Height
				|| (shrink && (pixbuf.Width < maxSize.Width || pixbuf.Height < maxSize.Height)))
			{
				if (!sizes.TryGetValue(maxSize, out pixbuf))
				{
					pixbuf = Pixbuf.ScaleSimple(maxSize.Width, maxSize.Height, interpolation);
					sizes[maxSize] = pixbuf;
				}
			}
			
			return pixbuf;
		}

		Lazy<List<IconFrame>> frames;
		public void Create(IEnumerable<IconFrame> frames)
		{
			var frameList = new List<IconFrame>(frames);
			this.frames = new Lazy<List<IconFrame>>(() => frameList);
			Control = new Gtk.IconSet();
			foreach (var frame in frameList)
			{
				Control.AddSource(new Gtk.IconSource { Pixbuf = frame.Bitmap.ToGdk() });
			}
			Pixbuf = frameList.FirstOrDefault(r => r.Scale == 1)?.Bitmap.ToGdk();
			if (Pixbuf == null)
			{
				var frame = frameList.OrderByDescending(r => r.Scale).FirstOrDefault();
				Pixbuf = frame?.Bitmap.ToGdk().ScaleSimple(frame.Size.Width, frame.Size.Height, Gdk.InterpType.Bilinear);
			}
		}

		public IEnumerable<IconFrame> Frames
		{
			get
			{
				return frames?.Value;
			}
		}
	}
}
