using System;
using System.Collections.Generic;
using System.Linq;
using sw = System.Windows;
using swm = System.Windows.Media;
using Eto.Drawing;

namespace Eto.Wpf.Drawing
{
	/// <summary>
	/// Handler for <see cref="IGraphicsPath"/>
	/// </summary>
	/// <copyright>(c) 2012-2014 by Curtis Wensley</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	public class GraphicsPathHandler : GraphicsPath.IHandler
	{
		swm.PathGeometry Control { get; set; }
		swm.PathFigure figure;


		public GraphicsPathHandler ()
		{
			Control = new swm.PathGeometry ();
			Control.Figures = new swm.PathFigureCollection ();
		}

		GraphicsPathHandler (swm.PathGeometry control)
		{
			Control = control;
		}

		public bool IsEmpty
		{
			get { return Control.IsEmpty (); }
		}

		public PointF CurrentPoint
		{
			get;
			private set;
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
			if (figure != null) {
				if (!(figure.Segments.Count == 1 && figure.Segments[0] is swm.LineSegment))
					figure.IsClosed = true;
			}
			figure = null;
		}

		public void StartFigure ()
		{
			figure = null;
		}

		public void AddLines (IEnumerable<PointF> points)
		{
			var pointsList = points as IList<PointF> ?? points.ToArray ();
			ConnectTo (pointsList.First ().ToWpf ());

			var wpfPoints = from p in pointsList select p.ToWpf ();
			figure.Segments.Add (new swm.PolyLineSegment (wpfPoints, true));
			CurrentPoint = pointsList.Last ();
		}

		public void AddLine (float startX, float startY, float endX, float endY)
		{
			ConnectTo(new sw.Point(startX, startY));
			figure.Segments.Add (new swm.LineSegment (new sw.Point (endX, endY), true));
			CurrentPoint = new PointF (endX, endY);
		}

		public void AddRectangle (float x, float y, float width, float height)
		{
            Control.AddGeometry(new swm.RectangleGeometry(WpfExtensions.NormalizedRect(x, y, width, height)));
			figure = null;
		}

		public void LineTo (float x, float y)
		{
			ConnectTo (new sw.Point (x, y));
			CurrentPoint = new PointF (x, y);
		}

		public void MoveTo (float x, float y)
		{
			ConnectTo (new sw.Point (x, y), startNewFigure: true);
			CurrentPoint = new PointF (x, y);
		}

		public void AddArc (float x, float y, float width, float height, float startAngle, float sweepAngle)
		{
			// degrees to radians conversion
			double startRadians = startAngle * Math.PI / 180.0;
			double sweepRadians = sweepAngle * Math.PI / 180.0;

			// x and y radius
			double dx = width / 2;
			double dy = height / 2;

			// determine the start point 
			double xs = x + dx + (Math.Cos (startRadians) * dx);
			double ys = y + dy + (Math.Sin (startRadians) * dy);

			// determine the end point 
			double xe = x + dx + (Math.Cos (startRadians + sweepRadians) * dx);
			double ye = y + dy + (Math.Sin (startRadians + sweepRadians) * dy);

			bool isLargeArc = Math.Abs (sweepAngle) > 180;
			var sweepDirection = sweepAngle < 0 ? swm.SweepDirection.Counterclockwise : swm.SweepDirection.Clockwise;

			ConnectTo (new sw.Point (xs, ys));
			figure.Segments.Add (new swm.ArcSegment (new sw.Point (xe, ye), new sw.Size (dx, dy), 0, isLargeArc, sweepDirection, true));
			CurrentPoint = new PointF ((float)xe, (float)ye);
		}

		public void AddBezier (PointF start, PointF control1, PointF control2, PointF end)
		{
			ConnectTo (start.ToWpf ());
			figure.Segments.Add (new swm.BezierSegment (control1.ToWpf (), control2.ToWpf (), end.ToWpf (), true));
			CurrentPoint = end;
		}

		public void AddPath (IGraphicsPath path, bool connect = false)
		{
			if (path.IsEmpty)
				return;

			var wpfPath = path.ToWpf ();
			if (!wpfPath.Transform.Value.IsIdentity) {
				var newpath = new swm.PathGeometry ();
				newpath.AddGeometry (wpfPath);
				wpfPath = newpath;
			}
			var en = wpfPath.Figures.GetEnumerator ();
			if (connect) {
				// merge current figure (if any) and first figure of new path, if they are not closed paths
				if (figure != null && !figure.IsClosed && en.MoveNext ()) {
					var firstFigure = en.Current;
					if (!firstFigure.IsClosed) {
						figure.Segments.Add (new swm.LineSegment (firstFigure.StartPoint, true));
						foreach (var seg in firstFigure.Segments)
							figure.Segments.Add (seg);
					} else {
						Control.Figures.Add (firstFigure);
					}
				}
			}
			swm.PathFigure pathFigure = null;
			while (en.MoveNext ()) {
				pathFigure = en.Current;
				Control.Figures.Add (pathFigure);
			}
			
			// continue with last figure of new path if not closed
			if (pathFigure != null && !pathFigure.IsClosed)
				figure = pathFigure;
			else
				figure = null;
		}

		public RectangleF Bounds
		{
			get { return Control.Bounds.ToEtoF (); }
		}

		public void Transform (IMatrix matrix)
		{
			if (Control.Transform != null)
				Control.Transform = new swm.MatrixTransform (swm.Matrix.Multiply (matrix.ToWpf (), Control.Transform.Value));
			else
				Control.Transform = matrix.ToWpfTransform ();
		}

		public void AddEllipse (float x, float y, float width, float height)
		{
            Control.AddGeometry(new swm.EllipseGeometry(WpfExtensions.NormalizedRect(x, y, width, height)));
			figure = null;
		}

		public void AddCurve (IEnumerable<PointF> points, float tension = 0.5f)
		{
			points = SplineHelper.SplineCurve (points, tension);
			var swpoints = (from p in points select p.ToWpf ()).ToArray();
			ConnectTo (swpoints.First ());
			figure.Segments.Add (new swm.PolyBezierSegment (swpoints, true));
			CurrentPoint = swpoints.Last ().ToEto ();
		}

		public object ControlObject
		{
			get { return Control; }
		}

		public void Dispose ()
		{
		}

		public IGraphicsPath Clone ()
		{
			return new GraphicsPathHandler (Control.Clone ());
		}

		public FillMode FillMode
		{
			set { Control.FillRule = value == FillMode.Alternate ? swm.FillRule.EvenOdd : swm.FillRule.Nonzero; }
			get { return Control.FillRule == swm.FillRule.EvenOdd ? FillMode.Alternate : FillMode.Winding; }
		}
	}
}
