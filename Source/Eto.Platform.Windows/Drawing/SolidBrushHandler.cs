using Eto.Drawing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sd = System.Drawing;

namespace Eto.Platform.Windows.Drawing
{
	public class SolidBrushHandler : ISolidBrushHandler
	{
		sd.SolidBrush brush;

		public void Create (Color color)
		{
			brush = new sd.SolidBrush (color.ToSD ());
		}

		public Color Color
		{
			get { return brush.Color.ToEto (); }
			set { brush.Color = value.ToSD (); }
		}

		public object ControlObject
		{
			get { return brush; }
		}

		public void Dispose ()
		{
			brush.Dispose ();
		}
	}
}
