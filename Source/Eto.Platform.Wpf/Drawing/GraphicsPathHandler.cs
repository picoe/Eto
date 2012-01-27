using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using swm = System.Windows.Media;
using Eto.Drawing;

namespace Eto.Platform.Wpf.Drawing
{
	public class GraphicsPathHandler : WidgetHandler<swm.PathFigure, GraphicsPath>, IGraphicsPath
	{
		bool started;

		public GraphicsPathHandler ()
		{
			Control = new swm.PathFigure ();
			Control.Segments = new swm.PathSegmentCollection ();
		}

		public void AddLines (IEnumerable<Point> points)
		{
			var enumerator = points.GetEnumerator();
			if (!started) {
				if (!enumerator.MoveNext ())
					return;
				Control.StartPoint = Generator.Convert (enumerator.Current);
				started = true;
			}
			while (enumerator.MoveNext())
			{
				Control.Segments.Add(new swm.LineSegment(Generator.Convert(enumerator.Current), true));
			}
		}
	}
}
