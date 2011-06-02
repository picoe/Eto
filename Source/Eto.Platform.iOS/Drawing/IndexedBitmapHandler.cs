using System;
using System.IO;
using System.Runtime.InteropServices;
using Eto.Drawing;

namespace Eto.Platform.iOS.Drawing
{

	public class IndexedBitmapDataHandler : BitmapData
	{
		public IndexedBitmapDataHandler(IntPtr data, int scanWidth, object controlObject)
			: base(data, scanWidth, controlObject)
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



	public class IndexedBitmapHandler : ImageHandler<byte[], IndexedBitmap>, IIndexedBitmap
	{
		Size size;
		int rowStride;
		uint[] colors;
		BitmapHandler bmp;

		public IndexedBitmapHandler()
		{
		}

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
			colors = new uint[colorCount];
			for (int i=0; i<colorCount; i++)
			{
				colors[i] = 0xffffffff;
			}

			size = new Size(width, height);
			Control = new byte[height*rowStride];
			bmp = new BitmapHandler();
			bmp.Create(size.Width, size.Height, PixelFormat.Format32bppRgb);
		}


		public void Resize(int width, int height)
		{
			throw new NotImplementedException("Cannot resize an indexed image");
		}

		public BitmapData Lock()
		{
			IntPtr ptr = Marshal.AllocHGlobal(Control.Length);
			Marshal.Copy(Control, 0, ptr, Control.Length);
			return  new IndexedBitmapDataHandler(ptr, rowStride, null);
		}

		public void Unlock(BitmapData bitmapData)
		{
			IntPtr ptr = bitmapData.Data;
			Marshal.Copy(ptr, Control, 0, Control.Length);
			Marshal.FreeHGlobal(ptr);
		}

		public Palette Palette
		{
			get
			{
				Palette pal = new Palette(colors.Length);
				for (int i=0; i<pal.Size; i++)
				{
					pal[i] = Color.FromArgb((uint)colors[i]);
				}
				return pal;
			}
			set
			{
				if (value.Size != colors.Length) throw new ArgumentException("Input palette must have the same colors as the output");
				for (int i=0; i<value.Size; i++)
				{
					colors[i] = value[i].ToArgb();
				}
			}
		}


		public override void DrawImage(GraphicsHandler graphics, Rectangle source, Rectangle destination)
		{
			var bd = bmp.Lock();
			
			// we have to draw to a temporary bitmap pixel by pixel
			unsafe {
				fixed (byte* pSrc = Control)
				{
					var dest = (byte*)bd.Data;
					var src = pSrc;
					var scany = rowStride;
					if (true)
					{
						src += Control.Length - scany;
						scany = -scany;
					}
					
					dest += source.Top * bd.ScanWidth;
					dest += source.Left * sizeof(uint);

					src += source.Top * scany;
					src += source.Left;
					
					for (int y=source.Top; y <= source.Bottom; y++)
					{
						var srcrow = src;
						var destrow = (uint*)dest;
						for (int x=source.Left; x <= source.Right; x++)
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

			bmp.DrawImage(graphics, source, destination);
		}
		
		protected override void Dispose (bool disposing)
		{
			base.Dispose (disposing);
			if (bmp != null)
			{
				bmp.Dispose();
				bmp = null;
			}
		}
	}
}
