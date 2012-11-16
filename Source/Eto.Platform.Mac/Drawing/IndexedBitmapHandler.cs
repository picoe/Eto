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
		public IndexedBitmapDataHandler (IntPtr data, int scanWidth, object controlObject)
			: base(data, scanWidth, controlObject)
		{
		}

		public override uint TranslateArgbToData (uint argb)
		{
			return argb;
		}

		public override uint TranslateDataToArgb (uint bitmapData)
		{
			return bitmapData;
		}
	}

	public class IndexedBitmapHandler : ImageHandler<object, IndexedBitmap>, IIndexedBitmap
	{
		Size size;
		int bytesPerRow;
		uint[] colors;
		BitmapHandler bmp;
		IntPtr ptr;
		

		public IndexedBitmapHandler ()
		{
		}

		public int RowStride {
			get { return bytesPerRow; }
		}

		public override Size Size {
			get { return size; }
		}

        public override int Width
        {
            get { return 0;/* TODO */ }
        }

        public override int Height
        {
            get { return 0;/* TODO */ }
        }

		public void Create (int width, int height, int bitsPerPixel)
		{
			bytesPerRow = width * bitsPerPixel / 8;
			int colorCount = (int)Math.Pow (2, bitsPerPixel);
			colors = new uint[colorCount];
			for (int i=0; i<colorCount; i++) {
				colors [i] = 0xffffffff;
			}

			size = new Size (width, height);
			ptr = Marshal.AllocHGlobal (height * bytesPerRow);
			//Control = new byte[height * bytesPerRow];
			bmp = new BitmapHandler ();
			bmp.Create (size.Width, size.Height, PixelFormat.Format32bppRgb);
		}

		public void Resize (int width, int height)
		{
			throw new NotSupportedException ("Cannot resize an indexed image");
		}

		public BitmapData Lock ()
		{
			//IntPtr ptr = Marshal.AllocHGlobal (Control.Length);
			//Marshal.Copy (Control, 0, ptr, Control.Length);
			return  new IndexedBitmapDataHandler (ptr, bytesPerRow, null);
		}

		public void Unlock (BitmapData bitmapData)
		{
			//IntPtr ptr = bitmapData.Data;
			//Console.WriteLine("Ugh...");
			//Marshal.Copy (ptr, Control, 0, Control.Length);
			//Marshal.FreeHGlobal (ptr);
		}

		public Palette Palette {
			get {
				var pal = new Palette ();
				pal.AddRange (colors.Select (r => Color.FromArgb (BitmapDataHandler.DataToArgb (r))));
				return pal;
			}
			set {
				if (value.Count != colors.Length)
					throw new ArgumentException ("Input palette must have the same colors as the output");
				for (int i=0; i<value.Count; i++) {
					colors [i] = BitmapDataHandler.ArgbToData (value [i].ToArgb ());
				}
			}
		}
		
		public override NSImage GetImage ()
		{
			var bmp = new BitmapHandler();
			bmp.Create (size.Width, size.Height, PixelFormat.Format32bppRgb);
			CopyTo(bmp, new Rectangle(size));
			return bmp.Control;
		}
		
		void CopyTo (BitmapHandler bmp, Rectangle source)
		{
			var bd = bmp.Lock ();
			if (source.Top < 0 || source.Left < 0 || source.Right > size.Width || source.Bottom > size.Height)
				throw new Exception ("Source rectangle exceeds image size");
			
			// we have to draw to a temporary bitmap pixel by pixel
			unsafe {
				/*fixed (byte* pSrc = Control)*/ {
					var dest = (byte*)bd.Data;
					//var src = pSrc;
					var src = (byte*)ptr;
					var scany = size.Width;
					/*if (false)
					{
						src += Control.Length - scany;
						scany = -scany;
					}*/
					
					dest += source.Top * bd.ScanWidth;
					dest += source.Left * sizeof(uint);

					src += source.Top * scany;
					src += source.Left;

					int bottom = source.Bottom;
					int right = source.Right;
					int left = source.Left;
					for (int y=source.Top; y < bottom; y++) {
						var srcrow = src;
						var destrow = (uint*)dest;
						for (int x=left; x < right; x++) {
							*
							destrow = colors [*srcrow];
							destrow++;
							srcrow++;
						}
						dest += bd.ScanWidth;
						src += scany;
					}
				}
			}
			bmp.Unlock (bd);
			
		}

		public override void DrawImage (GraphicsHandler graphics, Rectangle source, Rectangle destination)
		{
			CopyTo (bmp, source);
			bmp.DrawImage (graphics, source, destination);
		}
		
		protected override void Dispose (bool disposing)
		{
			if (bmp != null) {
				bmp.Dispose ();
				bmp = null;
			}
			if (ptr != IntPtr.Zero) {
				Marshal.FreeHGlobal (ptr);
				ptr = IntPtr.Zero;
			}
			base.Dispose (disposing);
		}
	}
}
