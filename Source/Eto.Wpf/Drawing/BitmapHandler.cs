using System;
using swm = System.Windows.Media;
using sw = System.Windows;
using swmi = System.Windows.Media.Imaging;
using Eto.Drawing;
using Eto.Wpf.Forms;
using System.IO;
using Eto.Shared.Drawing;
using System.Linq;
using System.Collections.Generic;
using Eto.Forms;

namespace Eto.Wpf.Drawing
{
	/// <summary>
	/// Bitmap data handler.
	/// </summary>
	/// <copyright>(c) 2012-2013 by Curtis Wensley</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	public class BitmapDataHandler : BaseBitmapData
	{
		public BitmapDataHandler(Image image, IntPtr data, int scanWidth, int bitsPerPixel, object controlObject)
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

	/// <summary>
	/// Bitmap handler.
	/// </summary>
	/// <copyright>(c) 2012-2013 by Curtis Wensley</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	public class BitmapHandler : WidgetHandler<swmi.BitmapSource, Bitmap>, Bitmap.IHandler, IWpfImage
	{
		public BitmapHandler()
		{
		}

		public BitmapHandler(swmi.BitmapSource source)
		{
			this.Control = source;
		}

		public void Create(string fileName)
		{
			Control = swmi.BitmapFrame.Create(new Uri(fileName), swmi.BitmapCreateOptions.None, swmi.BitmapCacheOption.OnLoad);
		}

		public void Create(Stream stream)
		{
			Control = swmi.BitmapFrame.Create(stream, swmi.BitmapCreateOptions.None, swmi.BitmapCacheOption.OnLoad);
		}

		public void Create(int width, int height, PixelFormat pixelFormat)
		{
			swm.PixelFormat format;
			switch (pixelFormat)
			{
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
					throw new NotSupportedException();
			}
			var bf = new swmi.WriteableBitmap(width, height, 96, 96, format, null);
			Control = bf;

		}

		public void Create(int width, int height, Graphics graphics)
		{
			Create(width, height, PixelFormat.Format32bppRgba);
		}

		public void Create(Image image, int width, int height, ImageInterpolation interpolation)
		{
			var source = image.ToWpf(1, new Size(width, height));
			// use drawing group to allow for better quality scaling
			var group = new swm.DrawingGroup();
			swm.RenderOptions.SetBitmapScalingMode(group, interpolation.ToWpf());
			group.Children.Add(new swm.ImageDrawing(source, new sw.Rect(0, 0, width, height)));

			var drawingVisual = new swm.DrawingVisual();
			using (var drawingContext = drawingVisual.RenderOpen())
				drawingContext.DrawDrawing(group);

			var resizedImage = new swmi.RenderTargetBitmap(width, height, source.DpiX, source.DpiY, swm.PixelFormats.Default);
			resizedImage.RenderWithCollect(drawingVisual);
			Control = resizedImage;
		}

		protected override void Initialize()
		{
			base.Initialize();
			SetFrozen();
		}

		public void SetBitmap(swmi.BitmapSource bitmap)
		{
			Control = bitmap;
			SetFrozen();
		}

		public Color GetPixel(int x, int y)
		{
			var rect = new sw.Int32Rect(x, y, 1, 1);
			var control = FrozenControl;

			var pixelStride = (rect.Width * control.Format.BitsPerPixel + 7) / 8;

			var pixels = new byte[pixelStride * rect.Height];

			control.CopyPixels(rect, pixels, stride: pixelStride, offset: 0);

			if (control.Format == swm.PixelFormats.Rgb24)
				return Color.FromArgb(red: pixels[0], green: pixels[1], blue: pixels[2]);
			if (control.Format == swm.PixelFormats.Bgr24)
				return Color.FromArgb(blue: pixels[0], green: pixels[1], red: pixels[2]);
			if (control.Format == swm.PixelFormats.Bgr32)
				return Color.FromArgb(blue: pixels[0], green: pixels[1], red: pixels[2]);
			if (control.Format == swm.PixelFormats.Bgra32)
				return Color.FromArgb(blue: pixels[0], green: pixels[1], red: pixels[2], alpha: pixels[3]);
			if (control.Format == swm.PixelFormats.Pbgra32)
				return Color.FromArgb(blue: pixels[0], green: pixels[1], red: pixels[2], alpha: pixels[3]);
			throw new NotSupportedException();
		}

		public BitmapData Lock()
		{
			var wb = Control as swmi.WriteableBitmap;
			if (wb == null || RequiresFrozen)
			{
				wb = new swmi.WriteableBitmap(FrozenControl);
				SetBitmap(wb);
			}
			wb.Lock();
			return new BitmapDataHandler(Widget, wb.BackBuffer, wb.BackBufferStride, wb.Format.BitsPerPixel, wb);
		}

		public void Unlock(BitmapData bitmapData)
		{
			var wb = Control as swmi.WriteableBitmap;
			if (wb != null)
			{
				wb.AddDirtyRect(new sw.Int32Rect(0, 0, Size.Width, Size.Height));
				wb.Unlock();
				SetFrozen();
			}
		}

 		public void Save(string fileName, ImageFormat format)
 		{
			using (var stream = new FileStream(fileName, FileMode.Create, FileAccess.Write))
			{
				Save(stream, format);
			}
 		}

		public void Save(Stream stream, ImageFormat format)
		{
			swmi.BitmapEncoder encoder;
			switch (format)
			{
				case ImageFormat.Png:
					encoder = new swmi.PngBitmapEncoder();
					break;
				case ImageFormat.Gif:
					encoder = new swmi.GifBitmapEncoder();
					break;
				case ImageFormat.Bitmap:
					encoder = new swmi.BmpBitmapEncoder();
					break;
				case ImageFormat.Jpeg:
					encoder = new swmi.JpegBitmapEncoder();
					break;
				case ImageFormat.Tiff:
					encoder = new swmi.TiffBitmapEncoder();
					break;
				default:
					throw new NotSupportedException();
			}
			encoder.Frames.Add(swmi.BitmapFrame.Create(Control));
			encoder.Save(stream);
		}

		public Size Size
		{
			get { return new Size(FrozenControl.PixelWidth, FrozenControl.PixelHeight); }
		}


		bool RequiresFrozen => Control.Dispatcher?.CheckAccess() != true;
		swmi.BitmapSource frozenControl;
		swmi.BitmapSource FrozenControl => frozenControl ?? Control;

		void SetFrozen()
		{
			frozenControl = Control.GetAsFrozen() as swmi.BitmapSource;
		}

		public swmi.BitmapSource GetImageClosestToSize(float scale, Size? fittingSize)
		{
			return FrozenControl;
		}

		public Bitmap Clone(Rectangle? rectangle = null)
		{
			if (rectangle != null)
			{
				var rect = rectangle.Value;
				var data = new byte[Stride * Control.PixelHeight];
				FrozenControl.CopyPixels(data, Stride, 0);
				var target = new swmi.WriteableBitmap(rect.Width, rect.Height, Control.DpiX, Control.DpiY, Control.Format, Control.Palette);
				target.WritePixels(rect.ToWpfInt32(), data, Stride, destinationX: 0, destinationY: 0);
				return new Bitmap(new BitmapHandler(target));
			}
			else
				return new Bitmap(new BitmapHandler(FrozenControl.Clone()));
		}

		int Stride
		{
			get { return (FrozenControl.PixelWidth * FrozenControl.Format.BitsPerPixel + 7) / 8; }
		}
	}
}