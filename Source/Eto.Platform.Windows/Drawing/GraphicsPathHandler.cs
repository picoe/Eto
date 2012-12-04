using System;
using System.Linq;
using Eto.Drawing;
using SD = System.Drawing;
using System.Collections.Generic;

namespace Eto.Platform.Windows.Drawing
{
	public class GraphicsPathHandler : WidgetHandler<SD.Drawing2D.GraphicsPath, GraphicsPath>, IGraphicsPath
	{
		PointF position;

		public GraphicsPathHandler ()
		{
			Control = new SD.Drawing2D.GraphicsPath ();
		}

		private GraphicsPathHandler (SD.Drawing2D.GraphicsPath control)
		{
			Control = control;
		}

		public void LineTo (PointF point)
		{
			this.Control.AddLine (position.ToSD (), point.ToSD ());
			position = point;
		}

		public void MoveTo (PointF point)
		{
			position = point;
		}

		public void AddLine (PointF point1, PointF point2)
		{
			this.Control.AddLine (point1.ToSD (), point2.ToSD ());
			position = point2;
		}

		public void AddLines (PointF[] points)
		{
			var sdlines = from p in points select p.ToSD ();
			this.Control.AddLines (sdlines.ToArray ());
			position = points.Last ();
		}
	}
}

