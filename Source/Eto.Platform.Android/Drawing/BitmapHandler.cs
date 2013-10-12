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

namespace Eto.Platform.Android.Drawing
{
	/// <summary>
	/// Bitmap handler.
	/// </summary>
	/// <copyright>(c) 2013 by Vivek Jhaveri</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	class BitmapHandler : WidgetHandler<ag.Bitmap, Bitmap>, IBitmap
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
			throw new NotImplementedException();
		}

		public void Create(int width, int height, PixelFormat pixelFormat)
		{
			ag.Bitmap.Config config = ag.Bitmap.Config.Argb8888;
			switch(pixelFormat)
			{
				case PixelFormat.Format32bppRgb:
					throw new NotImplementedException(); // TODO
					//config = ag.Bitmap.Config.Argb8888;
					break;
				case PixelFormat.Format24bppRgb:
					throw new NotImplementedException(); // TODO
					//config = ag.Bitmap.Config.Argb8888;
					break;
				/*case PixelFormat.Format16bppRgb555:
					config = ag.Bitmap.Config.Argb8888;
					break;*/
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
			throw new NotImplementedException();
		}

		public void Save(System.IO.Stream stream, ImageFormat format)
		{
			throw new NotImplementedException();
		}

		public Bitmap Clone(Rectangle? rectangle = null)
		{
			throw new NotImplementedException();
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
	}
}