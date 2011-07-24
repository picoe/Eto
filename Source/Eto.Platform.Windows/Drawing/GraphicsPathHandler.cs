using System;
using System.Linq;
using Eto.Drawing;
using SD = System.Drawing;
using System.Collections.Generic;

namespace Eto.Platform.Windows.Drawing
{
	public class GraphicsPathHandler : WidgetHandler<SD.Drawing2D.GraphicsPath, GraphicsPath>, IGraphicsPath
	{
		public GraphicsPathHandler ()
		{
			Control = new SD.Drawing2D.GraphicsPath();
		}
		
		public void AddLines (IEnumerable<Point> points)
		{
			var sdlines = from p in points select Generator.Convert (p);
			this.Control.AddLines (sdlines.ToArray());
		}
	}
}

