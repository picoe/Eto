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

        #region IGraphicsPath Members


        public RectangleF GetBounds()
        {
            return Generator.Convert(
                Control.BoundingBox);
        }

        public FillMode FillMode
        {
            get;
            set; /* TODO: use this in DrawPath */
        }

        public bool IsEmpty
        {
            get 
            {
                return Control == null ||
                Control.IsEmpty;
            }
        }

        public void AddCurve(PointF[] points)
        {
            /* TODO */ // currently only used for testing
        }

        public void AddLines(PointF[] points)
        {
            if(Control != null)
                Control.AddLines(
                    Generator.Convert(points));
        }

        public void AddLine(PointF point1, PointF point2)
        {
            if (Control != null)
            {
                Control.MoveToPoint(
                    Generator.Convert(point1));

                Control.AddLineToPoint(
                    Generator.Convert(point2));
            }
        }

        public void AddBezier(
            PointF pt1, 
            PointF pt2, 
            PointF pt3, 
            PointF pt4)
        {
            if (Control != null)
            {
                if (Control.IsEmpty)
                    Control.MoveToPoint(
                        Generator.Convert(pt1));
                else
                    Control.AddLineToPoint(
                        Generator.Convert(pt1));

                Control.AddCurveToPoint(
                    Generator.Convert(pt2),
                    Generator.Convert(pt3),
                    Generator.Convert(pt4));
            }
        }

        public void AddPath(
            IGraphicsPathBase addingPath, 
            bool connect)
        {
            var p = addingPath as GraphicsPath;

            if (p != null)
            {
                var h =
                    p.Handler as GraphicsPathHandler;

                if (h != null &&
                    h.Control != null)
                    Control.AddPath(
                        h.Control);
            }
        }

        public void Transform(Matrix matrix)
        {
            Transform(
                Generator.Convert(
                    matrix));
        }

        private void Transform(CGAffineTransform transform)
        {
            var result =
                new CGPath();

            // Add the original path specifying 
            // the transform
            result.AddPath(
                transform,
                Control);

            Control = result;

            // TODO: This hasn't been tested
        }

        public void AddArc(RectangleF rect, float startAngle, float sweepAngle)
        {
            var result =
                new CGPath();

            result.AddRect(
                Generator.Convert(rect));

            // BUGBUG: FIXFIX: start and sweep angle

            Control = result;
        }

        public void AddBeziers(Point[] points)
        {
            /* TODO */
        }

        public void AddEllipse(RectangleF rect)
        {
            if (Control != null)
                Control.AddElipseInRect(
                    Generator.Convert(rect));
        }

        public void Translate(PointF p)
        {
            Transform(
                CGAffineTransform.MakeTranslation(
                    p.X, p.Y));
        }

        public IGraphicsPath Clone()
        {
            return 
                this.Control != null
                ? new GraphicsPathHandler(                
                    new CGPath(this.Control))
                : new GraphicsPathHandler();
        }

        public void AddRectangle(RectangleF rect)
        {
            if(Control != null)
                Control.AddRect(
                    Generator.Convert(rect));
        }

        public void CloseFigure()
        {
            if (Control != null)
                Control.CloseSubpath();
        }

        #endregion

        public GraphicsPath ToGraphicsPath()
        {
            throw new NotImplementedException(); // should never get called
        }
    }
}

