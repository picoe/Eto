using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Eto.Drawing;
using sd = SharpDX.Direct2D1;
using sw = SharpDX.WIC;
using System.IO;
#if WINFORMS
using Eto.WinForms.Drawing;
#endif

namespace Eto.Direct2D.Drawing
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
