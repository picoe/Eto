using System;
using System.Linq;
using Eto.Drawing;
using MonoMac.CoreGraphics;
using System.Collections.Generic;
using sd = System.Drawing;

namespace Eto.Platform.Mac.Drawing
{
	public class GraphicsPathHandler : WidgetHandler<CGPath, GraphicsPath>, IGraphicsPath
	{
		public GraphicsPathHandler ()
		{
			Control = new CGPath ();
		}
		
		public void MoveTo (Point point)
		{
			Control.MoveToPoint (Generator.Convert (point));
		}
		
		public void LineTo (Point point)
		{
			Control.AddLineToPoint (Generator.Convert (point));
		}
		
		public void AddLine (Point point1, Point point2)
		{
			Control.AddLines (new sd.PointF[] { Generator.ConvertF (point1), Generator.ConvertF (point2) });
		}
		
		public void AddLines (IEnumerable<Point> points)
		{
			var sdpoints = from p in points select Generator.ConvertF (p);
			Control.AddLines (sdpoints.ToArray ());
		}
	}
}

