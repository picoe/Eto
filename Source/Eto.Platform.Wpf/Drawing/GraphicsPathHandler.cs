using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using swm = System.Windows.Media;
using Eto.Drawing;
using Eto;

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

        private GraphicsPathHandler(swm.PathGeometry p)
        {
            Control = p;
        }

        public bool IsEmpty
        {
            get { return Control.IsEmpty(); }
        }

		void StartNewFigure (Point startPoint)
		{
			figure = new swm.PathFigure ();
			figure.StartPoint = startPoint.ToWpf ();
			figure.Segments = new swm.PathSegmentCollection ();
			Control.Figures.Add (figure);
		}

        void StartNewFigure(PointF startPoint)
        {
            figure = new swm.PathFigure();
            figure.StartPoint = startPoint.ToWpf();
            figure.Segments = new swm.PathSegmentCollection();
            Control.Figures.Add(figure);
        }

        public void CloseFigure()
        {
            figure.IsClosed = true;
        }

        public void AddLines(PointF[] points)
        {
            var enumerator = points.GetEnumerator();

            if (!enumerator.MoveNext())
                return;

            StartNewFigure(
                (PointF)enumerator.Current);

            while (enumerator.MoveNext())
            {
                figure.Segments.Add(
                    new swm.LineSegment(
                        ((PointF)enumerator.Current).ToWpf(),
                        true));
            }
        }

		public void AddLine (Point point1, Point point2)
		{
			StartNewFigure (point1);
			figure.Segments.Add (new swm.LineSegment (point2.ToWpf (), true));
		}

        public void AddLine(PointF point1, PointF point2)
        {
            StartNewFigure(point1);
            figure.Segments.Add(new swm.LineSegment(point2.ToWpf(), true));
        }

        public void AddBezier(PointF pt1, PointF pt2, PointF pt3, PointF pt4)
        {
            StartNewFigure(pt1);
            figure.Segments.Add(
                new swm.BezierSegment(pt2.ToWpf(), pt3.ToWpf(), pt4.ToWpf(), 
                isStroked: true));
        }

        public void AddBeziers(Point[] points)
        {
            throw new NotImplementedException();
        }

        public void AddPath(IGraphicsPathBase addingPath, bool connect)
        {
            throw new NotImplementedException();
        }

        public void AddRectangle(RectangleF rectangle)
        {
            throw new NotImplementedException();
        }

        public void AddArc(RectangleF rect, float startAngle, float sweepAngle)
        {
            throw new NotImplementedException();
        }

        public void AddEllipse(RectangleF rect)
        {
            throw new NotImplementedException();
        }

		public void LineTo (Point point)
		{
			figure.Segments.Add (new swm.LineSegment (point.ToWpf (), true));
		}

		public void MoveTo (Point point)
		{
			StartNewFigure (point);
		}

        public RectangleF GetBounds()
        {
            return Control.Bounds.ToEtoF();
        }

        public void AddCurve(PointF[] points)
        {
            throw new NotImplementedException();
        }

        public FillMode FillMode
        {
            set { Control.FillRule = value.ToWpf(); }
        }

        public void Translate(PointF point)
        {
            var m = new swm.Matrix();
            m.Translate(point.X, point.Y);
            Transform(m);
        }

        public void Transform(Matrix matrix)
        {
            Transform((swm.Matrix)matrix.ControlObject);
        }

        // Helper method
        private void Transform(swm.Matrix m)
        {
            var g = Control.Clone(); // clone the geometry
            var t = new swm.MatrixTransform(m);
            g.Transform = t; // apply the transform
            this.Control = g.GetFlattenedPathGeometry();
        }

        public GraphicsPath ToGraphicsPath()
        {
            throw new NotImplementedException(); // should never get called
        }

        public IGraphicsPath Clone()
        {
            return new GraphicsPathHandler(
                Control.Clone());
        }
    }
}
