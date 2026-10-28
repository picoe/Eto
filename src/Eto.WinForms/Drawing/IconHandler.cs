using System.Windows.Controls;

namespace Eto.WinForms.Drawing
{
	public interface IWindowsIconSource
	{
		sd.Icon GetIcon();
	}

	public class IconHandler : WidgetHandler<sd.Icon, Icon>, Icon.IHandler, IWindowsImage, IWindowsIconSource
	{
		Dictionary<int, sd.Image> _cachedImages;
		List<IconFrame> _frames;
		IconFrame _idealFrame;
		bool _destroyIcon;

		public IconHandler(sd.Icon control)
		{
			this.Control = control;
		}
		public IconHandler(sd.Icon control, bool destroyIcon)
		{
			this.Control = control;
			_destroyIcon = destroyIcon;
		}

		public IconHandler()
		{
		}

		Size? size;
		public Size Size
		{
			get
			{
				return size ?? (size = GetIdealIcon().Size).Value;
			}
		}

		public IEnumerable<IconFrame> Frames
		{
			get
			{
				return _frames ?? SplitIcon(Control);
			}
		}

		public void Create(Stream stream)
		{
			Control = new sd.Icon(stream);
		}

		public void Create(string fileName)
		{
			Control = new sd.Icon(fileName);
		}

		public IconFrame GetIdealIcon()
		{
			if (_idealFrame != null)
				return _idealFrame;
			var orderedFrames = SplitIcon(Control).OrderByDescending(r => r.PixelSize.Width * r.PixelSize.Height);
			var dpi = Win32.SystemDpi;
			_idealFrame = orderedFrames.FirstOrDefault(r => Math.Abs(r.Scale - dpi) < 0.0001f) ?? orderedFrames.First();
			return _idealFrame;
		}

		public sd.Icon GetIconClosestToSize(int width)
		{
			var splitIcons = SplitIcon(Control);
			var curicon = splitIcons[0];
			if (curicon.PixelSize.Width == width)
				return (sd.Icon)curicon.ControlObject;
			foreach (var icon in splitIcons)
			{
				if (icon.PixelSize.Width > width && icon.PixelSize.Width - width < curicon.PixelSize.Width - width)
					curicon = icon;
			}
			return (sd.Icon)GetIdealIcon().ControlObject;
		}

		const int sICONDIR = 6;            // sizeof(ICONDIR) 
		const int sICONDIRENTRY = 16;      // sizeof(ICONDIRENTRY)

		public List<IconFrame> SplitIcon(sd.Icon icon)
		{
			if (_frames != null)
				return _frames;
			if (icon == null)
			{
				throw new ArgumentNullException("icon");
			}

			// Get multiple .ico file image.
			byte[] srcBuf;
			using (var stream = new MemoryStream())
			{
				icon.Save(stream);
				stream.Flush();
				srcBuf = stream.ToArray();
			}

			var splitIcons = new List<sd.Icon>();
			int count = BitConverter.ToInt16(srcBuf, 4); // ICONDIR.idCount

			if (count == 1)
			{
				splitIcons.Add(icon);
			}
			else
			{
				for (int i = 0; i < count; i++)
				{
					using (var destStream = new MemoryStream())
					using (var writer = new BinaryWriter(destStream))
					{
						// Copy ICONDIR and ICONDIRENTRY.
						int pos = 0;
						writer.Write(srcBuf, pos, sICONDIR - 2);
						writer.Write((short)1);    // ICONDIR.idCount == 1;

						pos += sICONDIR;
						pos += sICONDIRENTRY * i;

						writer.Write(srcBuf, pos, sICONDIRENTRY - 4); // write out icon info (minus old offset)
						writer.Write(sICONDIR + sICONDIRENTRY);    // write offset of icon data
						pos += 8;

						// Copy picture and mask data.
						int imgSize = BitConverter.ToInt32(srcBuf, pos);       // ICONDIRENTRY.dwBytesInRes
						pos += 4;
						int imgOffset = BitConverter.ToInt32(srcBuf, pos);    // ICONDIRENTRY.dwImageOffset
						if (imgOffset + imgSize > srcBuf.Length)
							throw new ArgumentException("ugh");
						writer.Write(srcBuf, imgOffset, imgSize);
						writer.Flush();

						// Create new icon.
						destStream.Seek(0, SeekOrigin.Begin);
						splitIcons.Add(new sd.Icon(destStream));
					}
				}
			}

			_frames = splitIcons.Select(r => IconFrame.FromControlObject(1, r)).ToList();
			return _frames;
		}

		protected override void Dispose(bool disposing)
		{
			if (_destroyIcon && Control != null)
			{
				Win32.DestroyIcon(Control.Handle);
			}
			base.Dispose(disposing);
			if (disposing)
			{
				if (_frames != null)
				{
					foreach (var frame in _frames)
					{
						((sd.Icon)frame.ControlObject).Dispose();
					}
					_frames = null;
				}
			}
		}

		public sd.Image GetImageWithSize(int? size)
		{
			if (size != null)
			{
				if (_cachedImages == null)
					_cachedImages = new Dictionary<int, sd.Image>();

				if (_cachedImages.TryGetValue(size.Value, out var bmp))
					return bmp;

				var icon = GetIconClosestToSize(size.Value);
				bmp = icon.ToBitmap();
				_cachedImages[size.Value] = bmp;
				return bmp;
			}
			return GetIdealIcon().Bitmap.ToSD();
		}
		public sd.Image GetImageWithSize(Size? size)
		{
			if (size != null)
			{
				var sz = size.Value;
				var imageSize = Size;
				var minScale = Math.Min((float)sz.Width / imageSize.Width, (float)sz.Height / imageSize.Height);
				size = Size.Ceiling((SizeF)imageSize * minScale);
			}
			var frame = Widget.GetFrame(1, size);
			return frame.Bitmap.ToSD();
		}


		public void DrawImage(GraphicsHandler graphics, RectangleF source, RectangleF destination)
		{
			var image = Widget.GetFrame(1f, Size.Ceiling(destination.Size));
			graphics.Control.DrawImage(image.Bitmap.ToSD(), destination.ToSD(), source.ToSD(), sd.GraphicsUnit.Pixel);
		}

		public void DrawImage(GraphicsHandler graphics, float x, float y)
		{
			var size = Size;
			var image = Widget.GetFrame(1, size);
			graphics.Control.DrawImage(image.Bitmap.ToSD(), x, y, size.Width, size.Height);
		}

		public void DrawImage(GraphicsHandler graphics, float x, float y, float width, float height)
		{
			var image = Widget.GetFrame(1f, Size.Ceiling(new SizeF(width, height)));
			graphics.Control.DrawImage(image.Bitmap.ToSD(), x, y, width, height);
		}

		public sd.Icon GetIcon()
		{
			return Control;
		}

		public void Create(IEnumerable<IconFrame> frames)
		{
			this._frames = frames.ToList();
			var frame = GetIdealIcon();
			size = frame.Size;
			// a multi-size .ico lets the window pick the best frame for each icon size instead of scaling one
			Control = CreateMultiSizeIcon(_frames) ?? (sd.Icon)frame.ControlObject;
		}

		static sd.Icon CreateMultiSizeIcon(IEnumerable<IconFrame> frames)
		{
			var icons = frames
				.Select(r => (sd.Icon)r.ControlObject)
				.Where(r => r.Width <= 256 && r.Height <= 256)
				.GroupBy(r => r.Size)
				.Select(r => r.First())
				.OrderBy(r => r.Width * r.Height)
				.ToList();
			if (icons.Count == 0)
				return null;

			// uncompressed DIBs rather than PNG, as .NET Framework can't read PNG frames back
			var images = icons.Select(EncodeDib).ToList();
			var stream = new MemoryStream();
			var writer = new BinaryWriter(stream);
			writer.Write((short)0); // reserved
			writer.Write((short)1); // type: icon
			writer.Write((short)icons.Count);
			var offset = sICONDIR + sICONDIRENTRY * icons.Count;
			for (int i = 0; i < icons.Count; i++)
			{
				// 0 means 256
				writer.Write((byte)(icons[i].Width & 0xFF));
				writer.Write((byte)(icons[i].Height & 0xFF));
				writer.Write((byte)0); // palette size
				writer.Write((byte)0); // reserved
				writer.Write((short)1); // planes
				writer.Write((short)32); // bits per pixel
				writer.Write(images[i].Length);
				writer.Write(offset);
				offset += images[i].Length;
			}
			foreach (var image in images)
				writer.Write(image);
			writer.Flush();
			stream.Position = 0;
			return new sd.Icon(stream);
		}

		static byte[] EncodeDib(sd.Icon icon)
		{
			using (var bitmap = icon.ToBitmap())
			using (var stream = new MemoryStream())
			using (var writer = new BinaryWriter(stream))
			{
				var width = bitmap.Width;
				var height = bitmap.Height;
				var maskStride = (width + 31) / 32 * 4;

				// BITMAPINFOHEADER, with the height doubled to include the AND mask
				writer.Write(40);
				writer.Write(width);
				writer.Write(height * 2);
				writer.Write((short)1); // planes
				writer.Write((short)32); // bits per pixel
				writer.Write(0); // compression: none
				writer.Write(width * height * 4 + maskStride * height);
				writer.Write(0);
				writer.Write(0);
				writer.Write(0);
				writer.Write(0);

				var data = bitmap.LockBits(new sd.Rectangle(0, 0, width, height), sd.Imaging.ImageLockMode.ReadOnly, sd.Imaging.PixelFormat.Format32bppArgb);
				try
				{
					var row = new byte[width * 4];
					// DIB rows are stored bottom-up
					for (int y = height - 1; y >= 0; y--)
					{
						Marshal.Copy(data.Scan0 + y * data.Stride, row, 0, row.Length);
						writer.Write(row);
					}
				}
				finally
				{
					bitmap.UnlockBits(data);
				}

				// empty AND mask, transparency comes from the alpha channel
				writer.Write(new byte[maskStride * height]);
				writer.Flush();
				return stream.ToArray();
			}
		}
	}
}
