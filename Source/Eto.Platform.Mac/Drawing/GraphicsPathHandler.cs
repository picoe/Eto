using System;
using System.Linq;
using Eto.Drawing;
using MonoMac.CoreGraphics;
using System.Collections.Generic;

namespace Eto.Platform.Mac.Drawing
{
	public class GraphicsPathHandler : WidgetHandler<CGPath, GraphicsPath>, IGraphicsPath
	{
		public GraphicsPathHandler()
		{
			Control = new CGPath();
		}
		
		public void AddLines (IEnumerable<Point> points)
		{
			var sdpoints = from p in points select Generator.ConvertF (p);
			Control.AddLines (sdpoints.ToArray());
		}
	}
}

