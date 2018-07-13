using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Eto.Drawing;
using s = SharpDX;
using sd = SharpDX.Direct2D1;
using sw = SharpDX.WIC;
using System.IO;
using System.Diagnostics;
#if WINFORMS
using Eto.WinForms.Drawing;
#elif WINRT
using ws = Windows.Storage;
using wss = Windows.Storage.Streams;
#endif

namespace Eto.Direct2D.Drawing
{
	public interface ID2DBitmapHandler
	{
		sd.Bitmap GetBitmap(sd.RenderTarget target);
		sw.Bitmap Control { get; }
	}

	public class ImageHandler<TWidget> : WidgetHandler<sw.Bitmap, TWidget>, Image.IHandler, ID2DBitmapHandler
#if WINFORMS		
		, IWindowsImageSource
#endif
		where TWidget: Image
    {
		sd.Bitmap targetBitmap;
		public sw.Bitmap[] Frames { get; protected set; }
		public sd.Bitmap GetBitmap(sd.RenderTarget target)
		{
			if (targetBitmap == null || !Equals(targetBitmap.Tag, target.NativePointer))
			{
				if (targetBitmap != null)
					targetBitmap.Dispose();
				targetBitmap = CreateDrawableBitmap(target);
				targetBitmap.Tag = target.NativePointer;
			}
			return targetBitmap;
		}

		protected virtual sd.Bitmap CreateDrawableBitmap(sd.RenderTarget target)
		{
			if (Control.PixelFormat == PixelFormat.Format24bppRgb.ToWic())
			{
				using (var wicBitmap = Control.ToBitmap(PixelFormat.Format32bppRgb.ToWic()))
				{
					return sd.Bitmap.FromWicBitmap(target, wicBitmap);
				}
			}
            return sd.Bitmap.FromWicBitmap(target, Control);
		}

		void Initialize(s.WIC.BitmapDecoder decoder)
		{
			Frames = Enumerable.Range(0, decoder.FrameCount).Select(r => decoder.GetFrame(r).ToBitmap()).ToArray();
			// find largest frame (e.g. when loading icons)
			Control = Frames.Aggregate((x,y) => x.Size.Width > y.Size.Width || x.Size.Height > y.Size.Height ? x : y);
		}

		public void Create(string filename)
        {
			using (var decoder = new s.WIC.BitmapDecoder(
				SDFactory.WicImagingFactory,
				filename,
				s.WIC.DecodeOptions.CacheOnDemand))
				Initialize(decoder);
		}

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
			Control = new sw.Bitmap(SDFactory.WicImagingFactory, width, height, pixelFormat.ToWic(), sw.BitmapCreateCacheOption.CacheOnLoad);
        }

        public void Create(int width, int height, Graphics graphics)
        {
			Create(width, height, PixelFormat.Format32bppRgba);
		}

#if WINRT
		public async void Save(string fileName, ImageFormat format)
		{
			var dir = await Windows.Storage.StorageFolder.GetFolderFromPathAsync(Path.GetDirectoryName(fileName)); // correct?
			var file = await dir.CreateFileAsync(Path.GetFileName(fileName));
			using (var fileStream = await file.OpenAsync(ws.FileAccessMode.ReadWrite))
			{
				using (var outputStream = fileStream.GetOutputStreamAt(0))
				{
					var stream = outputStream.AsStreamForWrite();
					Save(stream, format);
					await outputStream.FlushAsync();
				}
			}
		}
#else
		public void Save(string fileName, ImageFormat format)
		{
			using (var stream = new FileStream(fileName, FileMode.Create, FileAccess.Write))
			{
				Save(stream, format);
			}
		}
#endif

        public void Save(Stream stream, ImageFormat format)
        {
			using (var encoder = new s.WIC.BitmapEncoder(
				SDFactory.WicImagingFactory,
				format.ToWic()))
			{
				stream.Flush();
				encoder.Initialize(stream);
				using (var frameEncoder = new s.WIC.BitmapFrameEncode(encoder))
				{	
					frameEncoder.Initialize();
					frameEncoder.SetSize(Control.Size.Width, Control.Size.Height);
					frameEncoder.WriteSource(Control);
					frameEncoder.Commit();
				}
				encoder.Commit();
			}
        }

		public Bitmap Clone(Rectangle? rectangle = null)
        {
			sw.Bitmap bmp = Control;
			if (rectangle != null)
				bmp = new sw.Bitmap(SDFactory.WicImagingFactory, bmp, rectangle.Value.ToDx());
			else
				bmp = new sw.Bitmap(SDFactory.WicImagingFactory, bmp, sw.BitmapCreateCacheOption.CacheOnLoad);

			return new Bitmap(new BitmapHandler { Control = bmp });
        }

        public Color GetPixel(int x, int y)
        {
			try
			{
				var output = new uint[1];
				Control.CopyPixels(new s.Rectangle(x, y, 1, 1), output);
				var eto = new s.Color4(new s.ColorBGRA(output[0]).ToRgba()).ToEto();
				if (Control.PixelFormat == sw.PixelFormat.Format24bppBGR)
					return Color.FromRgb(eto.ToArgb());
				else
					return eto;
			}
			catch (s.SharpDXException ex)
			{
				Debug.WriteLine("GetPixel: {0}", ex.ToString());
				throw;
			}			
        }

        public Size Size
        {
            get { return Control.Size.ToEto(); }
        }

		public void Create(Image image, int width, int height, ImageInterpolation interpolation)
		{
			var imageHandler = (ImageHandler<TWidget>)image.Handler;

			Control = new sw.Bitmap(SDFactory.WicImagingFactory, width, height, imageHandler.Control.PixelFormat, sw.BitmapCreateCacheOption.CacheOnLoad);
			using (var graphics = new Graphics(Widget as Bitmap))
			{
				graphics.ImageInterpolation = interpolation;
				var rect = new Rectangle(0, 0, width, height);
				graphics.FillRectangle(Colors.Transparent, rect);
				graphics.DrawImage(image, rect);
			}
		}

		public virtual void Reset()
		{
			if (targetBitmap != null)
			{
				targetBitmap.Dispose();
				targetBitmap = null;
			}
#if WINFORMS
			if (sdimage != null)
			{
				sdimage.Dispose();
				sdimage = null;
			}
#endif
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				Reset();
				if (Frames != null)
				{
					foreach (var frame in Frames)
					{
						frame.Dispose();
					}
					Control = null;
				}
			}
			base.Dispose(disposing);
		}

#if WINFORMS
		System.Drawing.Image sdimage;

		public virtual System.Drawing.Image GetImageWithSize(int? size)
		{
			if (size != null && Frames != null && Control.Size.Width > size.Value)
			{
				var src = Frames.Aggregate((x, y) => Math.Abs(x.Size.Width - size.Value) < Math.Abs(y.Size.Width - size.Value) ? x : y);
				if (src != null)
				{
					return src.ToBitmap(Control.PixelFormat).ToSD();
				}
			}
			return sdimage ?? (sdimage = Control.ToSD());
		}

		public virtual System.Drawing.Image GetImageWithSize(Size? size)
		{
			return GetImageWithSize(size?.Height);
		}
#endif
	}
}
