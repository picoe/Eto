using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using swm = System.Windows.Media;
using sw = System.Windows;
using swmi = System.Windows.Media.Imaging;
using Eto.Drawing;
using System.Runtime.InteropServices;

namespace Eto.Platform.Wpf.Drawing
{
	/// <summary>
	/// Bitmap data handler.
	/// </summary>
	/// <copyright>(c) 2012-2013 by Curtis Wensley</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	public class BitmapDataHandler : BitmapData
	{
		public BitmapDataHandler (Image image, IntPtr data, int scanWidth, int bitsPerPixel, object controlObject)
			: base (image, data, scanWidth, bitsPerPixel, controlObject)
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

	/// <summary>
	/// Bitmap handler.
	/// </summary>
	/// <copyright>(c) 2012-2013 by Curtis Wensley</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	public class BitmapHandler : WidgetHandler<swm.Imaging.BitmapSource, Bitmap>, IBitmap, IWpfImage
	{
		int stride;

		public BitmapHandler ()
		{
		}

		public BitmapHandler (swm.Imaging.BitmapSource source)
		{
			this.Control = source;
		}

		public void Create (string fileName)
		{
			Control = swmi.BitmapFrame.Create (new Uri (fileName));
		}

		public void Create (System.IO.Stream stream)
		{
			Control = swmi.BitmapFrame.Create (stream);
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

		public void Create (int width, int height, Graphics graphics)
		{
			Create (width, height, PixelFormat.Format32bppRgba);
		}

		public void Create (Image image, int width, int height, ImageInterpolation interpolation)
		{
			var source = image.ToWpf ();
			// use drawing group to allow for better quality scaling
			var group = new swm.DrawingGroup ();
			swm.RenderOptions.SetBitmapScalingMode (group, swm.BitmapScalingMode.HighQuality);
			group.Children.Add (new swm.ImageDrawing (source, new sw.Rect (0, 0, width, height)));

			var drawingVisual = new swm.DrawingVisual ();
			using (var drawingContext = drawingVisual.RenderOpen ())
				drawingContext.DrawDrawing (group);

			var resizedImage = new swm.Imaging.RenderTargetBitmap (width, height, source.DpiX, source.DpiY, swm.PixelFormats.Default);
			resizedImage.Render (drawingVisual);
			Control = resizedImage;
		}

		public void SetBitmap (swm.Imaging.BitmapSource bitmap)
		{
			this.Control = bitmap;
		}

		public Color GetPixel (int x, int y)
		{
			var rect = new sw.Int32Rect (x, y, 1, 1);
			
			var stride = (rect.Width * Control.Format.BitsPerPixel + 7) / 8;
			
			var pixels = new byte[stride * rect.Height];
			
			Control.CopyPixels (rect, pixels, stride: stride, offset: 0);
			
			if (Control.Format == swm.PixelFormats.Rgb24)
				return Color.FromArgb (red: pixels [0], green: pixels [1], blue: pixels [2]);
			else if (Control.Format == swm.PixelFormats.Bgr32)
				return Color.FromArgb (blue: pixels [0], green: pixels [1], red: pixels [2]);
			else if (Control.Format == swm.PixelFormats.Pbgra32)
				return Color.FromArgb (blue: pixels [0], green: pixels [1], red: pixels [2], alpha: pixels [3]);
			else
				throw new NotSupportedException ();
		}

		public BitmapData Lock ()
		{
			var wb = Control as swm.Imaging.WriteableBitmap;
			if (wb != null) {
				wb.Lock ();
				return new BitmapDataHandler (Widget, wb.BackBuffer, (int)stride, Control.Format.BitsPerPixel, Control);
			}
			else {
				wb = new swm.Imaging.WriteableBitmap (Control);
				wb.Lock ();
				Control = wb;
				return new BitmapDataHandler (Widget, wb.BackBuffer, (int)stride, Control.Format.BitsPerPixel, wb);
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
			encoder.Frames.Add (swmi.BitmapFrame.Create (Control));
			encoder.Save (stream);
		}

		public Size Size
		{
			get { return new Size (Control.PixelWidth, Control.PixelHeight); }
		}

		public swmi.BitmapSource GetImageClosestToSize (int? width)
		{
			return Control;
		}

		public IBitmap Clone(Rectangle? rectangle = null)
		{
			swmi.BitmapSource clone = null;

			if (rectangle != null)
			{
				int stride = Control.PixelWidth * (Control.Format.BitsPerPixel / 8);
				byte[] data = new byte[stride * Control.PixelHeight];
				Control.CopyPixels(data, stride, 0);
				var target = new swmi.WriteableBitmap(Control.PixelWidth, Control.PixelHeight, Control.DpiX, Control.DpiY, Control.Format, null);
				var r = rectangle.Value;
				target.WritePixels(new sw.Int32Rect(r.X, r.Y, r.Width, r.Height), data, stride, destinationX: 0, destinationY: 0);
				clone = target;
			}
			else
				clone = Control.Clone(); 

			return new BitmapHandler (clone); 
		}
	}
}
