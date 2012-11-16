using System;
using System.Linq;
using Eto.Drawing;
using SD = System.Drawing;
using System.Collections.Generic;

namespace Eto.Platform.Windows.Drawing
{
	public class GraphicsPathHandler : WidgetHandler<SD.Drawing2D.GraphicsPath, GraphicsPath>, IGraphicsPath
	{
		Point position;

		public GraphicsPathHandler ()
		{
			Control = new SD.Drawing2D.GraphicsPath ();
		}

        private GraphicsPathHandler(SD.Drawing2D.GraphicsPath control)
        {
            Control = control;
        }
		
		public void LineTo (Point point)
		{
			this.Control.AddLine (Generator.Convert (position), Generator.Convert (point));
            position = point;
		}
		
		public void MoveTo (Point point)
		{
			position = point;
		}

		public void AddLine (Point point1, Point point2)
		{
			this.Control.AddLine (Generator.Convert (point1), Generator.Convert (point2));
			position = point2;
		}

        public RectangleF GetBounds()
        {
            return this.Control.GetBounds().ToRectangleF();
        }

        public void Translate(PointF point)
        {
            this.Control.Transform(
                ToTranslationMatrix(
                    Generator.Convert(point)));
        }

        public static SD.Drawing2D.Matrix ToTranslationMatrix(SD.PointF p)
        {
            return 
                new SD.Drawing2D.Matrix(
                    1, 0,
                    0, 1,
                    p.X,
                    p.Y);
        }

        public IGraphicsPath Clone()
        {
            return new GraphicsPathHandler(
                (SD.Drawing2D.GraphicsPath)
                this.Control.Clone());        
        }

        public void AddRectangle(RectangleF rectangle)
        {
            this.Control.AddRectangle(rectangle.ToRectangleF());
        }

        public void CloseFigure()
        {
            this.Control.CloseFigure();
        }

        public bool IsEmpty
        {
            get 
            { 
                return 
                    this.Control == null ||
                    this.Control.PointCount == 0; 
            }
        }

        public void AddCurve(PointF[] points)
        {
            this.Control.AddCurve(Generator.Convert(points));
        }

        public void AddLines(PointF[] points)
        {
            this.Control.AddLines(
                Generator.Convert(points));
        }

        public void AddLine(PointF point1, PointF point2)
        {
            this.Control.AddLines(
                new SD.PointF[] {
                    point1.ToPointF(), 
                    point2.ToPointF()});
        }

        public void AddBezier(PointF pt1, PointF pt2, PointF pt3, PointF pt4)
        {
            this.Control.AddBezier(
                pt1.ToPointF(),
                pt2.ToPointF(),
                pt3.ToPointF(),
                pt4.ToPointF());
        }

        public void AddPath(IGraphicsPathBase addingPath, bool connect)
        {
            var sdPath =
                (SD.Drawing2D.GraphicsPath)
                    ((IGraphicsPath)addingPath).ControlObject;

            if (sdPath.PointCount > 0)
                this.Control.AddPath(
                    sdPath,
                    connect);
        }

        public FillMode FillMode
        {
            set { Control.FillMode = (SD.Drawing2D.FillMode)value; }
        }

        public void Transform(Matrix matrix)
        {
            Control.Transform((SD.Drawing2D.Matrix)matrix.ControlObject);
        }

        public void AddArc(RectangleF rect, float startAngle, float sweepAngle)
        {
            Control.AddArc(Generator.Convert(rect), startAngle, sweepAngle);
        }

        public void AddBeziers(Point[] points)
        {
            Control.AddBeziers(Generator.Convert(points));
        }

        public void AddEllipse(RectangleF rect)
        {
            Control.AddEllipse(Generator.Convert(rect));
        }

        public void AddEllipse(float x, float y, float width, float height)
        {
            Control.AddEllipse(x, y, width, height);
        }

        public GraphicsPath ToGraphicsPath()
        {
            throw new NotImplementedException(); // should never get called
        }
    }
}

