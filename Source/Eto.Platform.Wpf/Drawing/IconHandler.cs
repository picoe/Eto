using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Eto.Drawing;
using System.IO;

namespace Eto.Platform.Wpf.Drawing
{
	public class IconHandler : WidgetHandler<System.Windows.Media.Imaging.BitmapFrame, Icon>, IIcon
	{
		public void Create(System.IO.Stream stream)
		{
			Control = System.Windows.Media.Imaging.BitmapFrame.Create(stream);
		}

		public void Create(string fileName)
		{
			using (var stream = File.OpenRead(fileName))
			{
				Control = System.Windows.Media.Imaging.BitmapFrame.Create(stream);
			}
		}

		public Size Size
		{
			get { return new Size(Control.PixelWidth, Control.PixelHeight); }
		}
	}
}
