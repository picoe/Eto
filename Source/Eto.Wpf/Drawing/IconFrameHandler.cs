using System;
using System.Collections.Generic;
using System.Globalization;
using Eto.Drawing;
using System.IO;
using sw = System.Windows;
using swi = System.Windows.Interop;
using swm = System.Windows.Media;
using swmi = System.Windows.Media.Imaging;
using sd = System.Drawing;
using System.Linq;

namespace Eto.Wpf.Drawing
{

	public class IconFrameHandler : IconFrame.IHandler
	{
		public object Create(IconFrame frame, Bitmap bitmap)
		{
			return bitmap;
		}

		public object Create(IconFrame frame, Func<Stream> load)
		{
			return new Bitmap(load());
		}

		public object Create(IconFrame frame, Stream stream)
		{
			return new Bitmap(stream);
		}

		public Bitmap GetBitmap(IconFrame frame)
		{
			return (Bitmap)frame.ControlObject;
		}

		public Size GetPixelSize(IconFrame frame)
		{
			return GetBitmap(frame).Size;
		}
	}
	
}
