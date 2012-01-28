using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using swm = System.Windows.Media;
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

	public class BitmapHandler : WidgetHandler<swm.Imaging.BitmapSource, Bitmap>, IBitmap
	{
		IntPtr pixels;
		int stride;

		public void Create (string fileName)
		{
			Control = swm.Imaging.BitmapFrame.Create (new Uri (fileName));
		}

		public void Create (System.IO.Stream stream)
		{
			Control = swm.Imaging.BitmapFrame.Create (stream);
		}

		public void SetBitmap (swm.Imaging.BitmapSource bitmap)
		{
			this.Control = bitmap;
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
				case PixelFormat.Format32bppRgba:
					format = swm.PixelFormats.Pbgra32;
					break;
				default:
					throw new NotSupportedException ();
			}
			stride = (width * format.BitsPerPixel + 7) / 8;

			var bufferSize = stride * height;
			pixels = Marshal.AllocHGlobal (bufferSize); 
			Control = swm.Imaging.WriteableBitmap.Create (width, height, 96, 96, format, null, pixels, bufferSize, stride);
		}

		public void Resize (int width, int height)
		{
			
		}

		public BitmapData Lock ()
		{
			if (pixels != IntPtr.Zero) {
				return new BitmapDataHandler (pixels, (int)stride, Control);
			}
			else {
				var buffer = new WPFUtil.BitmapBuffer (Control);
				return new BitmapDataHandler (buffer.BufferPointer, (int)buffer.Stride, Control);
			}
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


		protected override void Dispose (bool disposing)
		{
			base.Dispose (disposing);
			if (pixels != IntPtr.Zero) {
				Marshal.FreeHGlobal (pixels);
				pixels = IntPtr.Zero;
			}
		}
	}
}
