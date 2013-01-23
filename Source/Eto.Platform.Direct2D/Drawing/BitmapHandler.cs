using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Eto.Drawing;
using s = SharpDX;
using sd = SharpDX.Direct2D1;
using sw = SharpDX.DirectWrite;

namespace Eto.Platform.Direct2D.Drawing
{
    public class BitmapHandler : WidgetHandler<sd.Bitmap, Bitmap>, IBitmap
    {
        static s.WIC.ImagingFactory Factory = new s.WIC.ImagingFactory();

        public void Create(string filename)
        {
            using (var decoder = new s.WIC.BitmapDecoder(
                Factory,
                filename,
                s.WIC.DecodeOptions.CacheOnDemand))
                Initialize(decoder);
        }

        private void Initialize(s.WIC.BitmapDecoder decoder)
        {
            using (var frame = decoder.GetFrame(0))
            {
                using (var f = new s.WIC.FormatConverter(Factory))
                {
                    f.Initialize(
                        frame,
                        s.WIC.PixelFormat.Format32bppBGRA,
                        s.WIC.BitmapDitherType.None,
                        null,
                        0f,
                        s.WIC.BitmapPaletteType.MedianCut);

                    sd.RenderTarget renderTarget = null; // BUGBUG: fix

                    Control =
                        sd.Bitmap.FromWicBitmap(
                            renderTarget: null, // TODO
                            wicBitmapSource: frame);
                }
            }
        }

        public void Create(System.IO.Stream stream)
        {
            using (var decoder = new s.WIC.BitmapDecoder(
                Factory,
                stream,
                s.WIC.DecodeOptions.CacheOnDemand))
                Initialize(decoder);
        }

        public void Create(int width, int height, PixelFormat pixelFormat)
        {
            throw new NotImplementedException();
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

		public IBitmap Clone(Rectangle? rectangle = null)
        {
            throw new NotImplementedException();
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
