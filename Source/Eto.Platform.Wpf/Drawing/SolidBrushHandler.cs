using Eto.Drawing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using swm = System.Windows.Media;

namespace Eto.Platform.Wpf.Drawing
{
	public class SolidBrushHandler : ISolidBrushHandler
	{
		swm.SolidColorBrush brush;

		public void Create (Color color)
		{
			brush = new swm.SolidColorBrush (color.ToWpf ());
		}

		public Color Color
		{
			get { return brush.Color.ToEto (); }
			set { brush.Color = value.ToWpf (); }
		}

		public object ControlObject
		{
			get { return brush; }
		}

		public void Dispose ()
		{
		}
	}
}
