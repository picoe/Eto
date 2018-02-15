using System;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using Eto.Drawing;
using Eto.Shared.Drawing;

#if XAMMAC2
using AppKit;
using Foundation;
using CoreGraphics;
using ObjCRuntime;
using CoreAnimation;
using CoreImage;
#else
using MonoMac.AppKit;
using MonoMac.Foundation;
using MonoMac.CoreGraphics;
using MonoMac.ObjCRuntime;
using MonoMac.CoreAnimation;
using MonoMac.CoreImage;
#if Mac64
using nfloat = System.Double;
using nint = System.Int64;
using nuint = System.UInt64;
#else
using nfloat = System.Single;
using nint = System.Int32;
using nuint = System.UInt32;
#endif
#if SDCOMPAT
using CGSize = System.Drawing.SizeF;
using CGRect = System.Drawing.RectangleF;
using CGPoint = System.Drawing.PointF;
#endif
#endif

namespace Eto.Mac.Drawing
{
	public class IndexedBitmapDataHandler : BaseBitmapData
	{
		public IndexedBitmapDataHandler(Image image, IntPtr data, int scanWidth, int bitsPerPixel, object controlObject)
			: base(image, data, scanWidth, bitsPerPixel, controlObject)
		{
		}

		public override int TranslateArgbToData(int argb)
		{
			return argb;
		}

		public override int TranslateDataToArgb(int bitmapData)
		{
			return bitmapData;
		}
	}

	public class IndexedBitmapHandler : ImageHandler<object, IndexedBitmap>, IndexedBitmap.IHandler
	{
		Size size;
		int bytesPerRow;
		int bitsPerPixel;
		int[] colors;
		Bitmap bmp;
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
			colors = new int[colorCount];
			for (int i=0; i<colorCount; i++)
			{
				colors[i] = unchecked((int)0xffffffff);
			}

			size = new Size(width, height);
			ptr = Marshal.AllocHGlobal(height * bytesPerRow);
			//Control = new byte[height * bytesPerRow];
			bmp = new Bitmap(size.Width, size.Height, PixelFormat.Format32bppRgb);
		}

		public void Resize(int width, int height)
		{
			throw new NotSupportedException(string.Format(CultureInfo.CurrentCulture, "Cannot resize an indexed image"));
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
					throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, "Input palette must have the same colors as the output"));
				for (int i=0; i<value.Count; i++)
				{
					colors[i] = BitmapDataHandler.ArgbToData(value[i].ToArgb());
				}
			}
		}

		public override NSImage GetImage()
		{
			CopyTo(bmp, new Rectangle(size));
			return bmp.ToNS();
		}

		void CopyTo(Bitmap bmp, Rectangle source)
		{
			if (source.Top < 0 || source.Left < 0 || source.Right > size.Width || source.Bottom > size.Height)
				throw new ArgumentOutOfRangeException(string.Format(CultureInfo.CurrentCulture, "Source rectangle exceeds image size"));
			
			// we have to draw to a temporary bitmap pixel by pixel
			using (var bd = bmp.Lock())
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
					var destrow = (int*)dest;
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
		}

		public override void DrawImage(GraphicsHandler graphics, RectangleF source, RectangleF destination)
		{
			CopyTo(bmp, Rectangle.Truncate(source));
			(bmp.Handler as BitmapHandler)?.DrawImage(graphics, source, destination);
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
