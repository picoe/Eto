using Eto.Drawing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using swm = System.Windows.Media;

namespace Eto.Platform.Wpf.Drawing
{
	public class PenHandler : IPenHandler
	{
		swm.Pen pen;
		swm.SolidColorBrush brush;

		public void Create (Color color, float thickness)
		{
			brush = new swm.SolidColorBrush (color.ToWpf ());
			pen = new swm.Pen (brush, thickness);
			pen.MiterLimit = 10f;
		}

		public Color Color
		{
			get { return brush.Color.ToEto (); }
			set { brush.Color = value.ToWpf (); }
		}

		public float Thickness
		{
			get { return (float)pen.Thickness; }
			set { pen.Thickness = value; }
		}

		public PenLineJoin LineJoin
		{
			get { return pen.LineJoin.ToEto (); }
			set { pen.LineJoin = value.ToWpf (); }
		}

		public PenLineCap LineCap
		{
			get { return pen.EndLineCap.ToEto (); }
			set { pen.EndLineCap = pen.StartLineCap = value.ToWpf (); }
		}

		public float MiterLimit
		{
			get { return (float)pen.MiterLimit; }
			set { pen.MiterLimit = value; }
		}

		public object ControlObject
		{
			get { return pen; }
		}

		public void Dispose ()
		{
		}
	}
}
