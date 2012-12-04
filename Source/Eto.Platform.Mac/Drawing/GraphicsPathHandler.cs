using System;
using System.Linq;
using Eto.Drawing;
using System.Collections.Generic;
using SD = System.Drawing;

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

		public GraphicsPathHandler(CGPath path)
		{
			Control = path;
		}
		
		public void MoveTo (PointF point)
		{
			Control.MoveToPoint (point.ToSD ());
		}
		
		public void LineTo (PointF point)
		{
			Control.AddLineToPoint (point.ToSD ());
		}
		
		public void AddLine (PointF point1, PointF point2)
		{
			Control.AddLines (new SD.PointF[] { point1.ToSD (), point2.ToSD () });
		}
		
		public void AddLines (PointF[] points)
		{
			var sdpoints = from p in points select p.ToSD ();
			Control.AddLines (sdpoints.ToArray ());
		}
	}
}

