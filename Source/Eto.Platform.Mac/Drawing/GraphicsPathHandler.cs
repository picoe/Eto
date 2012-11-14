using System;
using System.Linq;
using Eto.Drawing;
using System.Collections.Generic;
using sd = System.Drawing;

#if OSX
using MonoMac.CoreGraphics;

namespace Eto.Platform.Mac.Drawing
#elif IOS
using MonoTouch.CoreGraphics;

namespace Eto.Platform.iOS.Drawing
#endif
{
	public class GraphicsPathHandler : WidgetHandler<CGPath, GraphicsPath>, IGraphicsPath
	{
		public GraphicsPathHandler ()
		{
			Control = new CGPath ();
		}
		
		public void MoveTo (Point point)
		{
			Control.MoveToPoint (point.ToSDPointF ());
		}
		
		public void LineTo (Point point)
		{
			Control.AddLineToPoint (point.ToSDPointF ());
		}
		
		public void AddLine (Point point1, Point point2)
		{
			Control.AddLines (new sd.PointF[] { point1.ToSDPointF (), point2.ToSDPointF () });
		}
		
		public void AddLines (IEnumerable<Point> points)
		{
			var sdpoints = from p in points select p.ToSDPointF ();
			Control.AddLines (sdpoints.ToArray ());
		}
	}
}

