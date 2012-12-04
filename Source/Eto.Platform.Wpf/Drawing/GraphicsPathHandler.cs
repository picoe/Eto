using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sw = System.Windows;
using swm = System.Windows.Media;
using Eto.Drawing;
using Eto;

namespace Eto.Platform.Wpf.Drawing
{
	public class GraphicsPathHandler : WidgetHandler<swm.PathGeometry, GraphicsPath>, IGraphicsPath
	{
		swm.PathFigure figure;

		swm.PathFigure LastFigure
		{
			get
			{
				return Control.Figures.Count > 0 ? Control.Figures[Control.Figures.Count - 1] : null;
			}
		}

		public GraphicsPathHandler ()
		{
			Control = new swm.PathGeometry ();
			Control.Figures = new swm.PathFigureCollection ();
		}

		private GraphicsPathHandler (swm.PathGeometry p)
		{
			Control = p;
		}

		public bool IsEmpty
		{
			get { return Control.IsEmpty (); }
		}

		void ConnectTo (sw.Point startPoint, bool startNewFigure = false)
		{
			if (startNewFigure || figure == null) {
				figure = new swm.PathFigure ();
				figure.StartPoint = startPoint;
				figure.Segments = new swm.PathSegmentCollection ();
				Control.Figures.Add (figure);
			} else
				figure.Segments.Add (new swm.LineSegment (startPoint, true));
		}

		public void CloseFigure ()
		{
			LastFigure.IsClosed = true;
		}

		public void AddLines (PointF[] points)
		{
			var enumerator = points.GetEnumerator ();

			if (!enumerator.MoveNext ())
				return;

			ConnectTo (((PointF)enumerator.Current).ToWpf ());

			while (enumerator.MoveNext ()) {
				figure.Segments.Add (new swm.LineSegment (((PointF)enumerator.Current).ToWpf (), true));
			}
		}

		public void AddLine (PointF point1, PointF point2)
		{
			ConnectTo (point1.ToWpf ());
			figure.Segments.Add (new swm.LineSegment (point2.ToWpf (), true));
		}

		public void LineTo (PointF point)
		{
			var p = point.ToWpf ();
			LineTo (p);
		}

		void LineTo (sw.Point p)
		{
			figure.Segments.Add (new swm.LineSegment (p, true));
		}

		public void MoveTo (PointF point)
		{
			ConnectTo (point.ToWpf (), startNewFigure: true);
		}
	}
}
