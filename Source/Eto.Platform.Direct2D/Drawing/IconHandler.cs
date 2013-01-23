using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Eto.Drawing;
using sd = SharpDX.Direct2D1;

namespace Eto.Platform.Direct2D.Drawing
{
    public class IconHandler : WidgetHandler<object, Icon>, IIcon
    {
		public void Create(System.IO.Stream stream)
		{
			throw new NotImplementedException();
		}

		public void Create(string fileName)
		{
			throw new NotImplementedException();
		}

		public Size Size
		{
			get { throw new NotImplementedException(); }
		}

		public object ControlObject
		{
			get { throw new NotImplementedException(); }
		}
	}
}
