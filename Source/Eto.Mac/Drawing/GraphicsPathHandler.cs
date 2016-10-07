using System;
using System.Collections.Generic;
using System.Globalization;
using Eto.Drawing;
using System.Diagnostics;

#if OSX
#if XAMMAC2
using AppKit;
using Foundation;
using CoreGraphics;
using ObjCRuntime;
using CoreAnimation;
#else
using MonoMac.AppKit;
using MonoMac.Foundation;
using MonoMac.CoreGraphics;
using MonoMac.ObjCRuntime;
using MonoMac.CoreAnimation;
#if Mac64
using nfloat = System.Double;
using nint = System.Int64;
using nuint = System.UInt64;
#else
using nfloat = System.Single;
using nint = System.Int32;
using nuint = System.UInt32;
#endif
#if SDCOMPAT
using CGSize = System.Drawing.SizeF;
using CGRect = System.Drawing.RectangleF;
using CGPoint = System.Drawing.PointF;
#endif
#endif

#if !UNIFIED
using sd = System.Drawing;
#endif

using Eto.Mac;
namespace Eto.Mac.Drawing
#elif IOS
using CoreGraphics;
using Eto.iOS;

namespace Eto.iOS.Drawing
#endif
{

	/// <summary>
	/// Handler for <see cref="IGraphicsPath"/>
	/// </summary>
	/// <copyright>(c) 2012-2014 by Curtis Wensley</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	public class GraphicsPathHandler : GraphicsPath.IHandler
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
			Control.AddRect (new CGRect(x, y, width, height));
			startFigure = true;
			isFirstFigure = false;
		}

		public void AddArc (float x, float y, float width, float height, float startAngle, float sweepAngle)
		{
			var yscale = height / width;
			var centerY = y + height / 2;
			var affine = new CGAffineTransform (1.0f, 0, 0, yscale, 0, centerY - centerY * yscale);

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

			Control.AddArc (affine, x+ width / 2, centerY, width / 2, CGConversions.DegreesToRadians (startAngle), CGConversions.DegreesToRadians (startAngle + sweepAngle), sweepAngle < 0);
		}

		public void AddBezier (PointF start, PointF control1, PointF control2, PointF end)
		{
			ConnectTo (start);
			this.AddCurveToPoint (control1, control2, end);
		}

		/// <summary>
		/// Check points early. If an invalid point is passed to AddCurveToPoint,
		/// iOS crashes with a SIGABRT leading to a long debugging experience.
		/// This provides early diagnostics.
		/// </summary>
		/// <param name="f"></param>
		private void Check(float f)
		{
			if (float.IsInfinity(f) || float.IsNaN(f))
				throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, "Invalid point specified to AddCurveToPoint"));
		}

		private void Check(PointF p)
		{
			Check(p.X);
			Check(p.Y);
		}

		private void AddCurveToPoint(PointF point1, PointF point2, PointF point3)
		{
			Check(point1);
			Check(point2);
			Check(point3);
			Control.AddCurveToPoint(point1.ToNS(), point2.ToNS(), point3.ToNS());
		}

		public void AddPath (IGraphicsPath path, bool connect = false)
		{
			if (path.IsEmpty)
				return;

			var handler = path.ToHandler ();
			if (connect && handler.startPoint != null && !handler.firstFigureClosed) {
				var first = true;
				handler.Control.Apply (element => {
					switch (element.Type) {
					case CGPathElementType.AddCurveToPoint:
						if (first)
							ConnectTo (element.Point3.ToEto());
						this.AddCurveToPoint(element.Point1.ToEto(), element.Point2.ToEto(), element.Point3.ToEto());
						break;
					case CGPathElementType.AddLineToPoint:
						if (first)
							ConnectTo (element.Point1.ToEto());
						Control.AddLineToPoint(element.Point1);
						break;
					case CGPathElementType.AddQuadCurveToPoint:
						if (first)
							ConnectTo (element.Point2.ToEto());
						Control.AddQuadCurveToPoint (element.Point1.X, element.Point1.Y, element.Point2.X, element.Point2.Y);
						break;
					case CGPathElementType.CloseSubpath:
						Control.CloseSubpath ();
						break;
					case CGPathElementType.MoveToPoint:
						if (first)
							ConnectTo (element.Point1.ToEto());
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
				transform.Prepend (matrix);
			var path = new CGPath ();
			path.AddPath (matrix.ToCG (), Control);
			Control = path;
		}

		public void CloseFigure ()
		{
			Control.CloseSubpath ();
			startFigure = true;
			firstFigureClosed |= isFirstFigure;
			isFirstFigure = false;
		}

		public void StartFigure ()
		{
			startFigure = true;
			isFirstFigure = false;
		}

		public void AddEllipse (float x, float y, float width, float height)
		{
			#if XAMMAC || XAMMAC2 || IOS
			Control.AddEllipseInRect(new CGRect(x, y, width, height));
			#else
			Control.AddElipseInRect (new CGRect(x, y, width, height));
			#endif
			startFigure = true;
			isFirstFigure = false;
		}

		public void AddCurve (IEnumerable<PointF> points, float tension = 0.5f)
		{
			points = SplineHelper.SplineCurve (points, tension);
			SplineHelper.Draw (points, ConnectTo, (c1, c2, end) => this.AddCurveToPoint(c1, c2, end));
		}

		public RectangleF Bounds
		{
			get { return Control.PathBoundingBox.ToEto(); }
		}

		public bool IsEmpty
		{
			get { return Control.IsEmpty; }
		}

		public PointF CurrentPoint
		{
			get { return Control.CurrentPoint.ToEto(); }
		}

		public object ControlObject
		{
			get { return this; }
		}

		public void Dispose ()
		{
			Control.Dispose ();
		}
	
		FillMode fillMode = FillMode.Alternate;
		public FillMode FillMode 
		{
			get { return fillMode; }
			set { fillMode = value;}
		}

		public IGraphicsPath Clone ()
		{
			return new GraphicsPathHandler (new CGPath (Control));
		}
	}
}

