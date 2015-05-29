using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Eto.Drawing;

using aa = Android.App;
using ac = Android.Content;
using ao = Android.OS;
using ar = Android.Runtime;
using av = Android.Views;
using aw = Android.Widget;
using ag = Android.Graphics;
using System.IO;

namespace Eto.Android.Drawing
{
	/// <summary>
	/// Interface for all Android images
	/// </summary>
	/// <copyright>(c) 2013 by Vivek Jhaveri</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	public interface IAndroidImage
	{
		ag.Bitmap GetImageWithSize(int? size);

		void DrawImage(GraphicsHandler graphics, RectangleF source, RectangleF destination);

		void DrawImage(GraphicsHandler graphics, float x, float y);

		void DrawImage(GraphicsHandler graphics, float x, float y, float width, float height);
	}


	/// <summary>
	/// Bitmap handler.
	/// </summary>
	/// <copyright>(c) 2013 by Vivek Jhaveri</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	class BitmapHandler : WidgetHandler<ag.Bitmap, Bitmap>, Bitmap.IHandler, IAndroidImage
	{
		public BitmapHandler()
		{
		}

		public BitmapHandler(ag.Bitmap image)
		{
			Control = image;
		}

		public void Create(string fileName)
		{
			Control = ag.BitmapFactory.DecodeFile(fileName);
		}

		public void Create(System.IO.Stream stream)
		{
			Control = ag.BitmapFactory.DecodeStream(stream);
		}

		public void Create(int width, int height, PixelFormat pixelFormat)
		{
			ag.Bitmap.Config config = ag.Bitmap.Config.Argb8888;
			switch(pixelFormat)
			{
				/*case PixelFormat.Format24bppRgb:
					config = ag.Bitmap.Config.Argb8888;
					break;*/
				case PixelFormat.Format32bppRgb:
				case PixelFormat.Format32bppRgba:
					config = ag.Bitmap.Config.Argb8888;
					break;
				default:
					throw new ArgumentOutOfRangeException("pixelFormat", pixelFormat, "Not supported");
			}

			Control = ag.Bitmap.CreateBitmap(width, height, config);
		}

		public void Create(int width, int height, Graphics graphics)
		{
			throw new NotImplementedException();
		}

		public void Create(Image image, int width, int height, ImageInterpolation interpolation)
		{
			Control = ag.Bitmap.CreateScaledBitmap(image.ToAndroid(), width, height, false);
		}

		public void Save(string fileName, ImageFormat format)
		{
			using (var stream = new FileStream(fileName, FileMode.Create, FileAccess.Write))
			{
				Save(stream, format);
			}
		}

		public void Save(System.IO.Stream stream, ImageFormat format)
		{
			ag.Bitmap.CompressFormat compressFormat;
			if(format == ImageFormat.Jpeg)
				compressFormat =ag.Bitmap.CompressFormat.Jpeg;
			else if(format == ImageFormat.Png)
				compressFormat = ag.Bitmap.CompressFormat.Png;
			else 
				throw new ArgumentException("ImageFormat must be Jpeg or Png");
			Control.Compress(compressFormat, 100, stream); // 100 means maximum quality. Png ignores this since it is lossless.
		}

		public Bitmap Clone(Rectangle? rectangle = null)
		{
			if (rectangle != null)
			{
				var r = rectangle.Value;
				return new Bitmap(new BitmapHandler(
					ag.Bitmap.CreateBitmap(this.Control, r.X, r.Y, r.Width, r.Height)));
			}
			else
				return new Bitmap(new BitmapHandler(ag.Bitmap.CreateBitmap(this.Control)));
		}

		public Color GetPixel(int x, int y)
		{
			return new ag.Color(Control.GetPixel(x, y)).ToEto();
		}

		public Size Size
		{
			get { return new Size(Control.Width, Control.Height); }
		}

		public BitmapData Lock()
		{
			throw new NotImplementedException();
		}

		public void Unlock(BitmapData bitmapData)
		{
			throw new NotImplementedException();
		}

		public ag.Bitmap GetImageWithSize(int? size)
		{
			return Control;
		}

		public void DrawImage(GraphicsHandler graphics, RectangleF source, RectangleF destination)
		{
			graphics.Control.DrawBitmap(Control, new Rectangle(source).ToAndroid(), destination.ToAndroid(), paint: null);
		}

		public void DrawImage(GraphicsHandler graphics, float x, float y)
		{
			graphics.Control.DrawBitmap(Control, x, y, paint: null);
		}

		public void DrawImage(GraphicsHandler graphics, float x, float y, float width, float height)
		{
			graphics.Control.DrawBitmap(Control, null, new RectangleF(x, y, width, height).ToAndroid(), paint: null);
		}
	}
}