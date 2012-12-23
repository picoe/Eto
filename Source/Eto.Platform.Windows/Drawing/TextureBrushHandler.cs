using Eto.Drawing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sd = System.Drawing;
using sd2 = System.Drawing.Drawing2D;

namespace Eto.Platform.Windows.Drawing
{
	public class TextureBrushHandler : ITextureBrushHandler
	{
		Image image;
		sd.TextureBrush Control { get; set; }

		public void Create (Image image)
		{
			this.image = image;
			Control = new sd.TextureBrush (new sd.Bitmap(image.ToSD ()), sd2.WrapMode.Tile);
		}

		public Image Image
		{
			get { return image; }
		}

		public IMatrix Transform
		{
			get { return new MatrixHandler (Control.Transform); }
			set
			{
				Control.Transform = value.ToSD ();
			}
		}

		public object ControlObject
		{
			get { return Control; }
		}

		public void Dispose ()
		{
			Control.Dispose ();
		}
	}
}
