using System;
using System.Linq;
using Eto.Drawing;
using System.Collections.Generic;
using sd = System.Drawing;

#if OSX
using MonoMac.CoreGraphics;

namespace Eto.Platform.Mac.Drawing
#elif IOS
using MonoTouch.CoreGraphics;

namespace Eto.Platform.iOS.Drawing
#endif
{

	/// <summary>
	/// Handler for <see cref="IGraphicsPath"/>
	/// </summary>
	/// <copyright>(c) 2012 by Curtis Wensley</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	public class GraphicsPathHandler : IGraphicsPathHandler
	{
		public CGPath Control { get; set; }
		bool startFigure = true;
		PointF? startPoint;
		IMatrix transform;
		bool isFirstFigure = true;
		bool firstFigureClosed;

		public GraphicsPathHandler ()
		{
			Control = new CGPath ();
		}

		public GraphicsPathHandler(CGPath path)
		{
			Control = path;
		}
		
		public void MoveTo (float x, float y)
		{
			Control.MoveToPoint (x, y);
			if (startPoint == null)
				startPoint = new PointF(x, y);
			startFigure = false;
		}
		
		public void LineTo (float x, float y)
		{
			Control.AddLineToPoint (x, y);
		}

		void ConnectTo (PointF point)
		{
			ConnectTo (point.X, point.Y);
		}

		void ConnectTo (float x, float y)
		{
			if (Control.IsEmpty || startFigure)
				MoveTo (x, y);
			else
				LineTo (x, y);
		}
		
		public void AddLine (float startX, float startY, float endX, float endY)
		{
			ConnectTo (startX, startY);
			LineTo (endX, endY);
		}
		
		public void AddLines (IEnumerable<PointF> points)
		{
			var enumerator = points.GetEnumerator ();
			if (!enumerator.MoveNext ())
				return;
			ConnectTo(enumerator.Current);
			while (enumerator.MoveNext ())
			{
				var point = enumerator.Current;
				Control.AddLineToPoint(point.X, point.Y);
			}
		}

		public void AddRectangle (float x, float y, float width, float height)
		{
			Control.AddRect (new sd.RectangleF (x, y, width, height));
			startFigure = true;
			isFirstFigure = false;
		}

		public void AddArc (float x, float y, float width, float height, float startAngle, float sweepAngle)
		{
			var yscale = height / width;
			var centerY = y + height / 2;
			var transform = new CGAffineTransform (1.0f, 0, 0, yscale, 0, centerY - centerY * yscale);

			if (startFigure) {
				// degrees to radians conversion
				double startRadians = startAngle * Math.PI / 180.0;
			
				// x and y radius
				double dx = width / 2;
				double dy = height / 2;
			
				// determine the start point 
				double xs = x + dx + (Math.Cos (startRadians) * dx);
				double ys = y + dy + (Math.Sin (startRadians) * dy);
			
				MoveTo ((float)xs, (float)ys);
			}

			Control.AddArc (transform, x+ width / 2, centerY, width / 2, Conversions.DegreesToRadians (startAngle), Conversions.DegreesToRadians (startAngle + sweepAngle), sweepAngle < 0);
		}

		public void AddBezier (PointF start, PointF control1, PointF control2, PointF end)
		{
			ConnectTo (start);
			Control.AddCurveToPoint (control1.X, control1.Y, control2.X, control2.Y, end.X, end.Y);
		}

		public void AddPath (IGraphicsPath path, bool connect)
		{
			if (path.IsEmpty)
				return;

			var handler = path.ToHandler ();
			if (connect && handler.startPoint != null && !handler.firstFigureClosed) {
				var startPoint = handler.startPoint.Value;
				if (handler.transform != null)
					startPoint = handler.transform.TransformPoint (startPoint);
				var first = true;
				handler.Control.Apply (element => {
					switch (element.Type) {
					case CGPathElementType.AddCurveToPoint:
						if (first)
							ConnectTo (Platform.Conversions.ToEto (element.Point3));
						Control.AddCurveToPoint (element.Point1, element.Point2, element.Point3);
						break;
					case CGPathElementType.AddLineToPoint:
						if (first)
							ConnectTo (Platform.Conversions.ToEto (element.Point1));
						Control.AddLineToPoint(element.Point1);
						break;
					case CGPathElementType.AddQuadCurveToPoint:
						if (first)
							ConnectTo (Platform.Conversions.ToEto (element.Point2));
						Control.AddQuadCurveToPoint (element.Point1.X, element.Point1.Y, element.Point2.X, element.Point2.Y);
						break;
					case CGPathElementType.CloseSubpath:
						Control.CloseSubpath ();
						break;
					case CGPathElementType.MoveToPoint:
						if (first)
							ConnectTo (Platform.Conversions.ToEto (element.Point1));
						else
							Control.MoveToPoint (element.Point1);
						break;
					}
					first = false;
				});
			}
			else {
				Control.AddPath(handler.Control);
			}
			startFigure = handler.startFigure;
		}

		public void Transform (IMatrix matrix)
		{
			if (transform == null)
				transform = matrix;
			else
				transform = Matrix.Multiply (transform, matrix);
			var path = new CGPath ();
			path.AddPath (matrix.ToCG (), Control);
			Control = path;
		}

		public void CloseFigure ()
		{
			Control.CloseSubpath ();
			startFigure = true;
			if (isFirstFigure)
				firstFigureClosed = true;
			isFirstFigure = false;
		}

		public void StartFigure ()
		{
			startFigure = true;
			isFirstFigure = false;
		}

		public void AddEllipse (float x, float y, float width, float height)
		{
			Control.AddElipseInRect (new sd.RectangleF (x, y, width, height));
			startFigure = true;
			isFirstFigure = false;
		}

		public void AddCurve (IEnumerable<PointF> points, float tension)
		{
			points = SplineHelper.SplineCurve (points, tension);
			SplineHelper.Draw (points, start => ConnectTo (start), (c1, c2, end) => {
				Control.AddCurveToPoint (c1.X, c1.Y, c2.X, c2.Y, end.X, end.Y);
			});
		}

		public RectangleF Bounds
		{
			get { return Platform.Conversions.ToEto (Control.PathBoundingBox); }
		}

		public bool IsEmpty
		{
			get { return Control.IsEmpty; }
		}

		public PointF CurrentPoint
		{
			get { return Platform.Conversions.ToEto (Control.CurrentPoint); }
		}

		public object ControlObject
		{
			get { return this; }
		}

		public void Dispose ()
		{
			Control.Dispose ();
		}
	
		public FillMode FillMode { get; set; }

		public IGraphicsPath Clone ()
		{
			return new GraphicsPathHandler (new CGPath (this.Control));
		}
	}
}

