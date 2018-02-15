using System;
using System.Runtime.InteropServices;
using System.Linq;
using Eto.Drawing;
using UIKit;
using Eto.Shared.Drawing;

namespace Eto.iOS.Drawing
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

	public class IndexedBitmapHandler : ImageHandler<byte[], IndexedBitmap>, IndexedBitmap.IHandler
	{
		Size size;
		int rowStride;
		int[] colors;
		BitmapHandler bmp;
		bool updated;

		public int RowStride
		{
			get { return rowStride; }
		}

		public override Size Size
		{
			get { return size; }
		}

		public void Create(int width, int height, int bitsPerPixel)
		{
			rowStride = width * bitsPerPixel / 8;
			int colorCount = (int)Math.Pow(2, bitsPerPixel);
			colors = new int[colorCount];
			for (int i=0; i<colorCount; i++)
			{
				colors[i] = unchecked((int)0xffffffff);
			}

			size = new Size(width, height);
			Control = new byte[height * rowStride];
			bmp = new BitmapHandler();
			bmp.Create(size.Width, size.Height, PixelFormat.Format32bppRgb);
		}

		public void Resize(int width, int height)
		{
			throw new NotSupportedException("Cannot resize an indexed image");
		}

		public BitmapData Lock()
		{
			var pinnedArray = GCHandle.Alloc(Control, GCHandleType.Pinned);
			IntPtr ptr = pinnedArray.AddrOfPinnedObject();
			/**
			IntPtr ptr = Marshal.AllocHGlobal(Control.Length);
			Marshal.Copy(Control, 0, ptr, Control.Length);
			/**/
			return new IndexedBitmapDataHandler(Widget, ptr, rowStride, Widget.BitsPerPixel, pinnedArray);
		}

		public void Unlock(BitmapData bitmapData)
		{
			var pinnedArray = (GCHandle)bitmapData.ControlObject;
			pinnedArray.Free();
			/**
			IntPtr ptr = bitmapData.Data;
			Marshal.Copy(ptr, Control, 0, Control.Length);
			Marshal.FreeHGlobal(ptr);
			/**/
			updated = false;
		}

		public Palette Palette
		{
			get
			{
				var pal = new Palette();
				pal.AddRange(colors.Select(Color.FromArgb));
				return pal;
			}
			set
			{
				if (value.Count != colors.Length)
					throw new ArgumentException("Input palette must have the same colors as the output");
				for (int i=0; i<value.Count; i++)
				{
					colors[i] = value[i].ToArgb();
				}
			}
		}

		void UpdateBitmap()
		{
			if (!updated)
			{
				UpdateBitmap(new Rectangle(Size));
				updated = true;
			}
		}

		void UpdateBitmap(Rectangle source, bool flipped = false)
		{
			var bd = bmp.Lock();
			// we have to draw to a temporary bitmap pixel by pixel
			unsafe
			{
				fixed (byte* pSrc = Control)
				{
					var dest = (byte*)bd.Data;
					var src = pSrc;
					var scany = rowStride;
					if (flipped != bd.Flipped)
					{
						src += Control.Length - scany;
						scany = -scany;
					}
					
					dest += source.Top * bd.ScanWidth;
					dest += source.Left * bd.BytesPerPixel;
					
					src += source.Top * scany;
					src += source.Left;
					
					for (int y = source.Top; y <= source.Bottom; y++)
					{
						var srcrow = src;
						var destrow = (int*)dest;
						for (int x = source.Left; x <= source.Right; x++)
						{
							var palindex = *srcrow;
							var color = colors[palindex];
							*destrow = bd.TranslateArgbToData(color);
							destrow++;
							srcrow++;
						}
						dest += bd.ScanWidth;
						src += scany;
					}
				}
			}
			bmp.Unlock(bd);
		}

		public override void DrawImage(GraphicsHandler graphics, RectangleF source, RectangleF destination)
		{
			UpdateBitmap();
			bmp.DrawImage(graphics, source, destination);
		}

		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
			if (disposing && bmp != null)
			{
				bmp.Dispose();
				bmp = null;
			}
		}

		public override UIImage GetUIImage(int? maxSize = null)
		{
			UpdateBitmap();
			return bmp.GetUIImage(maxSize);
		}
	}
}
