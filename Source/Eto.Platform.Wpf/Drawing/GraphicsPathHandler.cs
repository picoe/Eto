using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using swm = System.Windows.Media;
using Eto.Drawing;

namespace Eto.Platform.Wpf.Drawing
{
	public class GraphicsPathHandler : WidgetHandler<swm.PathGeometry, GraphicsPath>, IGraphicsPath
	{
		swm.PathFigure figure;

		public GraphicsPathHandler ()
		{
			Control = new swm.PathGeometry ();
			Control.Figures = new swm.PathFigureCollection ();
		}

		void StartNewFigure (Point startPoint)
		{
			figure = new swm.PathFigure ();
			figure.StartPoint = Generator.Convert (startPoint);
			figure.Segments = new swm.PathSegmentCollection ();
			Control.Figures.Add (figure);
		}

		public void AddLines (IEnumerable<Point> points)
		{
			var enumerator = points.GetEnumerator();
			if (!enumerator.MoveNext ())
				return;
			StartNewFigure (enumerator.Current);
			while (enumerator.MoveNext())
			{
				figure.Segments.Add (new swm.LineSegment (Generator.Convert (enumerator.Current), true));
			}
		}


		public void AddLine (Point point1, Point point2)
		{
			StartNewFigure (point1);
			figure.Segments.Add (new swm.LineSegment (Generator.Convert(point2), true));
		}

		public void LineTo (Point point)
		{
			figure.Segments.Add (new swm.LineSegment (Generator.Convert (point), true));
		}

		public void MoveTo (Point point)
		{
			StartNewFigure (point);
		}
	}
}
