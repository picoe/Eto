using System;
using System.Linq;
using Eto.Drawing;
using SD = System.Drawing;
using System.Collections.Generic;

namespace Eto.Platform.Windows.Drawing
{
	public class GraphicsPathHandler : WidgetHandler<SD.Drawing2D.GraphicsPath, GraphicsPath>, IGraphicsPath
	{
		Point position;

		public GraphicsPathHandler ()
		{
			Control = new SD.Drawing2D.GraphicsPath ();
		}
		
		public void LineTo (Point point)
		{
			this.Control.AddLine (Generator.Convert (position), Generator.Convert (point));
		}
		
		public void MoveTo (Point point)
		{
			position = point;
		}

		public void AddLine (Point point1, Point point2)
		{
			this.Control.AddLine (Generator.Convert (point1), Generator.Convert (point2));
			position = point2;
		}

		public void AddLines (IEnumerable<Point> points)
		{
			var sdlines = from p in points select Generator.Convert (p);
			this.Control.AddLines (sdlines.ToArray ());
			position = points.Last ();
		}
	}
}

