using System;
using System.IO;
using Eto.Drawing;
using System.Collections.Generic;
using System.Linq;

namespace Eto.GtkSharp.Drawing
{
	public class IconFrameHandler : IconFrame.IHandler
	{
		public object Create(IconFrame frame, Stream stream)
		{
			return new Bitmap(stream);
		}
		public object Create(IconFrame frame, Func<Stream> load)
		{
			return new Bitmap(load());
		}
		public object Create(IconFrame frame, Bitmap bitmap)
		{
			return bitmap;
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
