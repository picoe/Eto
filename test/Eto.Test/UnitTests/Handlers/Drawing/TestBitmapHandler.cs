using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Eto.Drawing;

namespace Eto.Test.UnitTests.Handlers.Drawing
{
	/// <summary>
	/// A mock IBitmap implementation.
	/// </summary>
	/// <copyright>(c) 2014 by Curtis Wensley</copyright>
	/// <copyright>(c) 2014 by Vivek Jhaveri</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	class TestBitmapHandler : Bitmap.IHandler
	{
		public Eto.Platform Platform { get; set; }
		public Widget Widget { get; set; }
		public Size Size { get; set; }
		public string ID { get; set; }
		public object ControlObject { get; set; }
		public IntPtr NativeHandle { get { return IntPtr.Zero; } }

		public void Create(string fileName)
		{
			throw new NotImplementedException();
		}

		public void Create(Stream stream)
		{
			throw new NotImplementedException();
		}

		public void Create(int width, int height, PixelFormat pixelFormat)
		{
			Size = new Size(width, height);
		}

		public void Create(int width, int height, Graphics graphics)
		{
			throw new NotImplementedException();
		}

		public void Create(Image image, int width, int height, ImageInterpolation interpolation)
		{
			throw new NotImplementedException();
		}

		public void Save(string fileName, ImageFormat format)
		{
			throw new NotImplementedException();
		}

		public void Save(Stream stream, ImageFormat format)
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

		public void HandleEvent(string id, bool defaultEvent = false)
		{
		}

		public void Initialize()
		{
		}

		public BitmapData Lock()
		{
			throw new NotImplementedException();
		}

		public void Unlock(BitmapData bitmapData)
		{
		}
	}
}
