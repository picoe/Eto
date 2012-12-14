using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using swm = System.Windows.Media;
using sw = System.Windows;
using Eto.Drawing;
using System.Runtime.InteropServices;

namespace Eto.Platform.Wpf.Drawing
{
	public class BitmapDataHandler : BitmapData
	{
		public BitmapDataHandler (IntPtr data, int scanWidth, object controlObject)
			: base (data, scanWidth, controlObject)
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

	public class BitmapHandler : WidgetHandler<swm.Imaging.BitmapSource, Bitmap>, IBitmap, IWpfImage
	{
		int stride;

		public void Create (string fileName)
		{
			Control = swm.Imaging.BitmapFrame.Create (new Uri (fileName));
		}

		public void Create (System.IO.Stream stream)
		{
			Control = swm.Imaging.BitmapFrame.Create (stream);
		}

		public void Create (int width, int height, PixelFormat pixelFormat)
		{
			swm.PixelFormat format;
			switch (pixelFormat) {
				/*case PixelFormat.Format16bppRgb555:
					format = swm.PixelFormats.Bgr555;
					break;*/
				case PixelFormat.Format24bppRgb:
					format = swm.PixelFormats.Bgr24;
					break;
				case PixelFormat.Format32bppRgb:
					format = swm.PixelFormats.Bgr32;
					break;
				case PixelFormat.Format32bppRgba:
					format = swm.PixelFormats.Pbgra32;
					break;
				default:
					throw new NotSupportedException ();
			}
			stride = (width * format.BitsPerPixel + 7) / 8;

			var bufferSize = stride * height;
			var bf = new swm.Imaging.WriteableBitmap(width, height, 96, 96, format, null);
			Control = bf;
			
		}

        public void SetBitmap(swm.Imaging.BitmapSource bitmap)
        {
            this.Control = bitmap;
        }

		public void Resize (int width, int height)
		{
			
		}

		public BitmapData Lock ()
		{
			var wb = Control as swm.Imaging.WriteableBitmap;
			if (wb != null) {
				wb.Lock ();
				return new BitmapDataHandler (wb.BackBuffer, (int)stride, Control);
			}
			else {
				wb = new swm.Imaging.WriteableBitmap (Control);
				wb.Lock ();
				Control = wb;
				return new BitmapDataHandler (wb.BackBuffer, (int)stride, wb);
			}
		}

		public void Unlock (BitmapData bitmapData)
		{
			var wb = Control as swm.Imaging.WriteableBitmap;
			if (wb != null) {
				
				wb.AddDirtyRect (new sw.Int32Rect (0, 0, Size.Width, Size.Height));
				wb.Unlock ();
			}

		}

		public void Save (System.IO.Stream stream, ImageFormat format)
		{
			swm.Imaging.BitmapEncoder encoder;
			switch (format) {
				case ImageFormat.Png:
					encoder = new swm.Imaging.PngBitmapEncoder ();
					break;
				case ImageFormat.Gif:
					encoder = new swm.Imaging.GifBitmapEncoder ();
					break;
				case ImageFormat.Bitmap:
					encoder = new swm.Imaging.BmpBitmapEncoder ();
					break;
				case ImageFormat.Jpeg:
					encoder = new swm.Imaging.JpegBitmapEncoder ();
					break;
				case ImageFormat.Tiff:
					encoder = new swm.Imaging.TiffBitmapEncoder ();
					break;
				default:
					throw new NotSupportedException ();
			}
			encoder.Frames.Add (swm.Imaging.BitmapFrame.Create (Control));
			encoder.Save (stream);
		}

		public Size Size
		{
			get { return new Size (Control.PixelWidth, Control.PixelHeight); }
		}


		public swm.ImageSource GetImageClosestToSize (int? width)
		{
			return Control;
		}
    }
}
