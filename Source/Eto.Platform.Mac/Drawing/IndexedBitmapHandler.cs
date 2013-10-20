using System;
using System.IO;
using System.Runtime.InteropServices;
using Eto.Drawing;
using MonoMac.CoreGraphics;
using MonoMac.AppKit;
using MonoMac.Foundation;
using System.Linq;

namespace Eto.Platform.Mac.Drawing
{
	public class IndexedBitmapDataHandler : BitmapData
	{
		public IndexedBitmapDataHandler(Image image, IntPtr data, int scanWidth, int bitsPerPixel, object controlObject)
			: base(image, data, scanWidth, bitsPerPixel, controlObject)
		{
		}

		public override uint TranslateArgbToData(uint argb)
		{
			return argb;
		}

		public override uint TranslateDataToArgb(uint bitmapData)
		{
			return bitmapData;
		}
	}

	public class IndexedBitmapHandler : ImageHandler<object, IndexedBitmap>, IIndexedBitmap
	{
		Size size;
		int bytesPerRow;
		int bitsPerPixel;
		uint[] colors;
		BitmapHandler bmp;
		IntPtr ptr;

		public int RowStride
		{
			get { return bytesPerRow; }
		}

		public override Size Size
		{
			get { return size; }
		}

		public void Create(int width, int height, int bitsPerPixel)
		{
			this.bitsPerPixel = bitsPerPixel;
			bytesPerRow = width * bitsPerPixel / 8;
			int colorCount = (int)Math.Pow(2, bitsPerPixel);
			colors = new uint[colorCount];
			for (int i=0; i<colorCount; i++)
			{
				colors[i] = 0xffffffff;
			}

			size = new Size(width, height);
			ptr = Marshal.AllocHGlobal(height * bytesPerRow);
			//Control = new byte[height * bytesPerRow];
			bmp = new BitmapHandler();
			bmp.Create(size.Width, size.Height, PixelFormat.Format32bppRgb);
		}

		public void Resize(int width, int height)
		{
			throw new NotSupportedException("Cannot resize an indexed image");
		}

		public BitmapData Lock()
		{
			//IntPtr ptr = Marshal.AllocHGlobal (Control.Length);
			//Marshal.Copy (Control, 0, ptr, Control.Length);
			return  new IndexedBitmapDataHandler(Widget, ptr, bytesPerRow, bitsPerPixel, null);
		}

		public void Unlock(BitmapData bitmapData)
		{
			//IntPtr ptr = bitmapData.Data;
			//Console.WriteLine("Ugh...");
			//Marshal.Copy (ptr, Control, 0, Control.Length);
			//Marshal.FreeHGlobal (ptr);
		}

		public Palette Palette
		{
			get
			{
				return new Palette(colors.Select(r => Color.FromArgb(BitmapDataHandler.DataToArgb(r))));
			}
			set
			{
				if (value.Count != colors.Length)
					throw new ArgumentException("Input palette must have the same colors as the output");
				for (int i=0; i<value.Count; i++)
				{
					colors[i] = BitmapDataHandler.ArgbToData(value[i].ToArgb());
				}
			}
		}

		public override NSImage GetImage()
		{
			var copy = new BitmapHandler();
			copy.Create(size.Width, size.Height, PixelFormat.Format32bppRgb);
			CopyTo(copy, new Rectangle(size));
			return copy.Control;
		}

		void CopyTo(BitmapHandler bmp, Rectangle source)
		{
			if (source.Top < 0 || source.Left < 0 || source.Right > size.Width || source.Bottom > size.Height)
				throw new Exception("Source rectangle exceeds image size");
			
			// we have to draw to a temporary bitmap pixel by pixel
			var bd = bmp.Lock();
			unsafe
			{
				var dest = (byte*)bd.Data;
				var src = (byte*)ptr;
				var scany = size.Width;
					
				dest += source.Top * bd.ScanWidth;
				dest += source.Left * bd.BytesPerPixel;

				src += source.Top * scany;
				src += source.Left;

				int bottom = source.Bottom;
				int right = source.Right;
				int left = source.Left;
				scany = scany - (right - left);
				for (int y=source.Top; y < bottom; y++)
				{
					var destrow = (uint*)dest;
					for (int x=left; x < right; x++)
					{
						*destrow = colors[*src];
						destrow++;
						src++;
					}
					dest += bd.ScanWidth;
					src += scany;
				}
			}
			bmp.Unlock(bd);
		}

		public override void DrawImage(GraphicsHandler graphics, RectangleF source, RectangleF destination)
		{
			CopyTo(bmp, Rectangle.Truncate(source));
			bmp.DrawImage(graphics, source, destination);
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && bmp != null)
			{
				bmp.Dispose();
				bmp = null;
			}
			if (ptr != IntPtr.Zero)
			{
				Marshal.FreeHGlobal(ptr);
				ptr = IntPtr.Zero;
			}
			base.Dispose(disposing);
		}
	}
}
