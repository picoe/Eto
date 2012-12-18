using Eto.Drawing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sd = System.Drawing;

namespace Eto.Platform.Windows.Drawing
{
	public class PenHandler : IPenHandler
	{
		sd.Pen pen;

		public void Create (Color color, float thickness)
		{
			pen = new sd.Pen (color.ToSD (), thickness);
		}

		public Color Color
		{
			get { return pen.Color.ToEto (); }
			set { pen.Color = value.ToSD (); }
		}

		public float Thickness
		{
			get { return pen.Width; }
			set { pen.Width = value; }
		}

		public PenLineJoin LineJoin
		{
			get { return pen.LineJoin.ToEto (); }
			set { pen.LineJoin = value.ToSD (); }
		}

		public PenLineCap LineCap
		{
			get { return pen.StartCap.ToEto (); }
			set { pen.StartCap = pen.EndCap = value.ToSD (); }
		}

		public object ControlObject
		{
			get { return pen; }
		}

		public void Dispose ()
		{
			pen.Dispose ();
		}
	}
}
