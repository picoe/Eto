using System;
using System.IO;
using Eto.Drawing;
using sd = System.Drawing;
using SWF = System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;

namespace Eto.WinForms.Drawing
{

	public class IconFrameHandler : IconFrame.IHandler
	{
		public object Create(IconFrame frame, Bitmap bitmap)
		{
			return sd.Icon.FromHandle(((sd.Bitmap)bitmap.ToSD()).GetHicon());
		}

		public object Create(IconFrame frame, Func<Stream> load)
		{
			return sd.Icon.FromHandle(new sd.Bitmap(load()).GetHicon());
		}

		public object Create(IconFrame frame, Stream stream)
		{
			return new sd.Icon(stream);
		}

		public Bitmap GetBitmap(IconFrame frame)
		{
			return new Bitmap(new BitmapHandler(((sd.Icon)frame.ControlObject).ToBitmap()));
		}

		public Size GetPixelSize(IconFrame frame)
		{
			return ((sd.Icon)frame.ControlObject).Size.ToEto();
		}
	}
	
}
