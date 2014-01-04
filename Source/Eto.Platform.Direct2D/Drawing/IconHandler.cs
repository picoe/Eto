using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Eto.Drawing;
using sd = SharpDX.Direct2D1;

namespace Eto.Platform.Direct2D.Drawing
{
    public class IconHandler : WidgetHandler<sd.Bitmap, Icon>, IIcon
    {
		public void Create(System.IO.Stream stream)
		{
			Control = BitmapHelper.Create(stream);
		}

		public void Create(string fileName)
		{
			Control = BitmapHelper.Create(fileName);
		}

		public Size Size
		{
			get { return Control.Size.ToEto(); }
		}

		public object ControlObject
		{
			get { return Control; }
		}
	}
}
