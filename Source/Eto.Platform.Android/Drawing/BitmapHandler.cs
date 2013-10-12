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
		public void Create(string fileName)
		{
			throw new NotImplementedException();
		}

		public void Create(System.IO.Stream stream)
		{
			throw new NotImplementedException();
		}

		public void Create(int width, int height, PixelFormat pixelFormat)
		{
			throw new NotImplementedException();
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
			throw new NotImplementedException();
		}

		public Size Size
		{
			get { throw new NotImplementedException(); }
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