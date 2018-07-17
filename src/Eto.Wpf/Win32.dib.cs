using System;
using System.IO;
using Eto.Drawing;

namespace Eto
{
	partial class Win32
	{
		public static Bitmap FromDIB(MemoryStream ms)
		{
			var header = new byte[40];
			ms.Read(header, 0, header.Length);
			var size = BitConverter.ToInt32(header, 0);
			var width = BitConverter.ToInt32(header, 4);
			var height = BitConverter.ToInt32(header, 8);
			var bpp = BitConverter.ToInt16(header, 14);
			var compression = BitConverter.ToInt32(header, 16);
			if (size > header.Length)
				ms.Seek(size - header.Length, SeekOrigin.Current);

			if (bpp != 32)
				return null;
			if (compression == 3) // BI_BITFIELDS
			{
				// three dwords, each specifies the bits each RGB components takes
				// we require each takes one byte
				var segments = new byte[sizeof(int) * 3];
				ms.Read(segments, 0, segments.Length);
				var rcomp = BitConverter.ToInt32(segments, 0);
				var gcomp = BitConverter.ToInt32(segments, 4);
				var bcomp = BitConverter.ToInt32(segments, 8);
				if (rcomp != 0xFF0000 || gcomp != 0xFF00 || bcomp != 0xFF)
					return null;
			}
			else if (compression != 0) // BI_RGB
				return null;

			var bmp = new Bitmap(width, height, PixelFormat.Format32bppRgba);
			using (var bd = bmp.Lock())
			{
				for (int y = height - 1; y >= 0; y--)
					for (int x = 0; x < width; x++)
					{
						var b = ms.ReadByte();
						var g = ms.ReadByte();
						var r = ms.ReadByte();
						var a = ms.ReadByte();
						var af = a / 255f;
						if (af > 0)
							bd.SetPixel(x, y, new Color(r / af, g / af, b / af, af));
					}
			}
			return bmp;
		}

		static void Write(Stream stream, byte[] val) => stream.Write(val, 0, val.Length);

		public static MemoryStream ToDIB(this Bitmap bitmap, int dpi = 96)
		{
			if (bitmap == null)
				return null;
			using (var bd = bitmap.Lock())
			{
				if (bd.BytesPerPixel == 4 || bd.BytesPerPixel == 3) // only 32bpp or 24bpp supported
				{
					var size = bitmap.Size;
					var ms = new MemoryStream(size.Width * size.Height * bd.BytesPerPixel + 40);
					// write BITMAPINFOHEADER
					const float InchesPerMeter = 39.37f;
					var pelsPerMeter = Math.Round(dpi * InchesPerMeter); // convert dpi to ppm
					Write(ms, BitConverter.GetBytes((uint)40));  // biSize
					Write(ms, BitConverter.GetBytes((uint)size.Width)); // biWidth
					Write(ms, BitConverter.GetBytes((uint)size.Height));// biHeight
					Write(ms, BitConverter.GetBytes((ushort)1));  // biPlanes
					Write(ms, BitConverter.GetBytes((ushort)bd.BitsPerPixel)); // biBitCount
					Write(ms, BitConverter.GetBytes((uint)0));    //  biCompression (BI_RGB, uncompressed)
					Write(ms, BitConverter.GetBytes((uint)0));    //  biSizeImage
					Write(ms, BitConverter.GetBytes((uint)pelsPerMeter)); //  biXPelsPerMeter
					Write(ms, BitConverter.GetBytes((uint)pelsPerMeter)); //  biYPelsPerMeter
					Write(ms, BitConverter.GetBytes((uint)0));    //  biClrUsed
					Write(ms, BitConverter.GetBytes((uint)0));    //  biClrImportant

					var hasAlpha = bd.BytesPerPixel == 4;
					// write RGB data, dibs are flipped vertically
					for (int y = size.Height - 1; y >= 0; y--)
					{
						for (int x = 0; x < size.Width; x++)
						{
							var p = bd.GetPixel(x, y);
							// need to write RGB premultiplied by alpha (and round up)
							ms.WriteByte((byte)(p.B * p.A * 255f + .5f));
							ms.WriteByte((byte)(p.G * p.A * 255f + .5f));
							ms.WriteByte((byte)(p.R * p.A * 255f + .5f));
							if (hasAlpha)
								ms.WriteByte((byte)p.Ab);
						}
					}

					ms.Position = 0;
					return ms;
				}
			}
			return null;
		}
	}
}
