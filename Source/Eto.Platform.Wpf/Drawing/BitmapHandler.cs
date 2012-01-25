using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using swm = System.Windows.Media;
using Eto.Drawing;

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

	public class BitmapHandler : WidgetHandler<swm.Imaging.BitmapSource, Bitmap>, IBitmap
	{
		byte[] pixels;

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
				case PixelFormat.Format16bppRgb555:
					format = swm.PixelFormats.Bgr555;
					break;
				case PixelFormat.Format24bppRgb:
					format = swm.PixelFormats.Bgr24;
					break;
				case PixelFormat.Format32bppRgb:
					format = swm.PixelFormats.Bgr32;
					break;
				default:
					throw new NotSupportedException ();
			}
			int stride = (width * format.BitsPerPixel + 7) / 8;

			pixels = new byte[height * stride];
			Control = swm.Imaging.BitmapSource.Create (width, height, 96, 96, format, null, pixels, stride);
		}

		public void Resize (int width, int height)
		{
			throw new NotImplementedException ();
		}

		public BitmapData Lock ()
		{
			var buffer = new WPFUtil.BitmapBuffer (Control);
			return new BitmapDataHandler (buffer.BufferPointer, (int)buffer.Stride, Control);
		}

		public void Unlock (BitmapData bitmapData)
		{

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
	}
}
