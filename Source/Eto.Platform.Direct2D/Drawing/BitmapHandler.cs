using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Eto.Drawing;
using s = SharpDX;
using sd = SharpDX.Direct2D1;
using sw = SharpDX.DirectWrite;
using System.IO;

namespace Eto.Platform.Direct2D.Drawing
{
    public class BitmapHandler : WidgetHandler<sd.Bitmap, Bitmap>, IBitmap
    {
        public void Create(string filename)
        {
            using (var decoder = new s.WIC.BitmapDecoder(
                SDFactory.WicImagingFactory,
                filename,
                s.WIC.DecodeOptions.CacheOnDemand))
                Initialize(decoder);
        }

        void Initialize(s.WIC.BitmapDecoder decoder)
        {
            using (var frame = decoder.GetFrame(0))
            {
				using (var f = new s.WIC.FormatConverter(SDFactory.WicImagingFactory))
                {
                    f.Initialize(
                        frame,
						s.WIC.PixelFormat.Format32bppPBGRA,
                        s.WIC.BitmapDitherType.None,
                        null,
                        0f,
                        s.WIC.BitmapPaletteType.Custom);

					double dpX = 96.0f;
					double dpY = 96.0f;
					f.GetResolution(out dpX, out dpY);

					var props = new sd.BitmapProperties(
						this.PixelFormat,
						(float)dpX, (float)dpY);

					// Get bitmap
					Control = sd.Bitmap.FromWicBitmap(
						GraphicsHandler.CurrentRenderTarget, // BUGBUG: fix
						f, 
						props);
                }
            }
        }

		sd.PixelFormat PixelFormat { get { return new sd.PixelFormat(SharpDX.DXGI.Format.B8G8R8A8_UNorm, sd.AlphaMode.Premultiplied); } }

        public void Create(System.IO.Stream stream)
        {
            using (var decoder = new s.WIC.BitmapDecoder(
				SDFactory.WicImagingFactory,
                stream,
                s.WIC.DecodeOptions.CacheOnDemand))
                Initialize(decoder);
        }

        public void Create(int width, int height, PixelFormat pixelFormat)
        {
			Control = new sd.Bitmap(
				GraphicsHandler.CurrentRenderTarget, 
				new s.Size2(width, height), 
				new sd.BitmapProperties(this.PixelFormat));
        }

        public void Create(int width, int height, Graphics graphics)
        {
            throw new NotImplementedException();
        }

        public void Resize(int width, int height)
        {
            throw new NotImplementedException();
        }

        public BitmapData Lock()
        {
            throw new NotImplementedException();
        }

        public void Unlock(BitmapData bitmapData)
        {
            throw new NotImplementedException();
        }

        public void Save(System.IO.Stream stream, ImageFormat format)
        {
            throw new NotImplementedException();
        }

		public Bitmap Clone(Rectangle? rectangle = null)
        {
			var size = rectangle != null ? rectangle.Value.Size : Size;
			var bmp = new sd.Bitmap(GraphicsHandler.CurrentRenderTarget, new s.Size2(size.Width, size.Height), new sd.BitmapProperties(PixelFormat));
			if (rectangle != null)
			{
				bmp.CopyFromBitmap(Control, new s.Point(), rectangle.Value.ToDx());
			}
			else
				bmp.CopyFromBitmap(Control);

			return new Bitmap(Generator, new BitmapHandler { Control = bmp });
        }

        public Color GetPixel(int x, int y)
        {
            throw new NotImplementedException();
        }

        public byte[] ToPNGByteArray()
        {
            throw new NotImplementedException();
        }

        public Size Size
        {
            get { return Control.Size.ToEto(); }
        }

        public int Width
        {
            get { return (int) Control.Size.Width; }
        }

        public int Height
        {
            get { return (int)Control.Size.Height; }
        }

		public void Create(Image image, int width, int height, ImageInterpolation interpolation)
		{
			throw new NotImplementedException();
		}
	}
}
