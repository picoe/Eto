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
                return Control != null && Control.Figures.Count > 0
                    ? Control.Figures[Control.Figures.Count - 1]
                    : null;
            }
        }

        public GraphicsPathHandler()
        {
            Control = new swm.PathGeometry();
            Control.Figures = new swm.PathFigureCollection();
        }

        private GraphicsPathHandler(swm.PathGeometry p)
        {
            Control = p;
        }

        public bool IsEmpty
        {
            get { return Control.IsEmpty(); }
        }

        void StartNewFigure(sw.Point startPoint)
        {
            figure = new swm.PathFigure();
            figure.StartPoint = startPoint;
            figure.Segments = new swm.PathSegmentCollection();
            Control.Figures.Add(figure);
        }

        public void CloseFigure()
        {
            LastFigure.IsClosed = true;
        }

        public void AddLines(PointF[] points)
        {
            var enumerator = points.GetEnumerator();

            if (!enumerator.MoveNext())
                return;

            StartNewFigure(
                ((PointF)enumerator.Current).ToWpf());

            while (enumerator.MoveNext())
            {
                figure.Segments.Add(
                    new swm.LineSegment(
                        ((PointF)enumerator.Current).ToWpf(),
                        true));
            }
        }

        public void AddLine(Point point1, Point point2)
        {
            StartNewFigure(point1.ToWpf());
            figure.Segments.Add(new swm.LineSegment(point2.ToWpf(), true));
        }

        public void AddLine(PointF point1, PointF point2)
        {
            StartNewFigure(point1.ToWpf());
            figure.Segments.Add(new swm.LineSegment(point2.ToWpf(), true));
        }

        public void AddBezier(PointF pt1, PointF pt2, PointF pt3, PointF pt4)
        {
            StartNewFigure(pt1.ToWpf());
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
            var path =
                (swm.PathGeometry)
                    ((IGraphicsPath)addingPath).ControlObject;

            if (!path.IsEmpty())
                this.Control.AddGeometry(
                    path);
        }

        public void AddRectangle(RectangleF rectangle)
        {
            throw new NotImplementedException();
        }

        const double DegreesToRadians = Math.PI / 180d;

        public void AddArc(RectangleF rect, float startAngle, float sweepAngle)
        {
            // sweep direction
            var sweepDir =
                sweepAngle < 0
                ? swm.SweepDirection.Counterclockwise
                : swm.SweepDirection.Clockwise;

            bool isLargeArc = Math.Abs(sweepAngle) > 180;

            // angles in radians
            var startRadians = startAngle * DegreesToRadians;
            var sweepRadians = sweepAngle * DegreesToRadians;

            double cx = rect.Width / 2;
            double cy = rect.Height / 2;

            //start point
            double x1 = rect.X + cx + (Math.Cos(startRadians) * cx);
            double y1 = rect.Y + cy + (Math.Sin(startRadians) * cy);
            var startPoint = new sw.Point(x1, y1);

            //end point
            double x2 = rect.X + cx + (Math.Cos(startRadians + sweepRadians) * cx);
            double y2 = rect.Y + cy + (Math.Sin(startRadians + sweepRadians) * cy);
            var endPoint = new sw.Point(x2, y2);

            if (figure == null)
                StartNewFigure(startPoint);
            else
                LineTo(startPoint); // connect the existing figure

            // Add a new arc segment
            figure.Segments.Add(
                new swm.ArcSegment(
                    endPoint,
                    new sw.Size(cx, cy),
                    0,
                    isLargeArc,
                    sweepDir,
                    isStroked: true));
        }

        public void AddEllipse(RectangleF rect)
        {
            AddArc(rect, 0, 360);
        }

        public void LineTo(Point point)
        {
            var p = point.ToWpf();
            LineTo(p);
        }

        private void LineTo(sw.Point p)
        {
            figure.Segments.Add(new swm.LineSegment(p, true));
        }

        public void MoveTo(Point point)
        {
            StartNewFigure(point.ToWpf());
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
