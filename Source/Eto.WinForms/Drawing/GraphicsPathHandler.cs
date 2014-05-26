using System.Linq;
using Eto.Drawing;
using sd = System.Drawing;
using sd2 = System.Drawing.Drawing2D;
using System.Collections.Generic;

namespace Eto.WinForms.Drawing
{
	/// <summary>
	/// Handler for <see cref="IGraphicsPath"/>
	/// </summary>
	/// <copyright>(c) 2012-2014 by Curtis Wensley</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	public class GraphicsPathHandler : GraphicsPath.IHandler
	{
		sd2.GraphicsPath Control { get; set; }
		sd.PointF position;

		public GraphicsPathHandler ()
		{
			Control = new sd2.GraphicsPath ();
		}

		GraphicsPathHandler (sd2.GraphicsPath control)
		{
			Control = control;
		}

		public void LineTo (float x, float y)
		{
			var point = new sd.PointF (x, y);
			Control.AddLine (position, point);
			position = point;
		}

		public void MoveTo (float x, float y)
		{
			position = new sd.PointF (x, y);
		}

		public void AddLine (float startX, float startY, float endX, float endY)
		{
			Control.AddLine (new sd.PointF (startX, startY), new sd.PointF (endX, endY));
			position = new sd.PointF (endX, endY);
		}

		public void AddLines (IEnumerable<PointF> points)
		{
			var sdpoints = from p in points select p.ToSD ();
			var pointArray = sdpoints.ToArray ();
			Control.AddLines (pointArray);
			position = pointArray.Last ();
		}

		public void AddBezier (PointF start, PointF control1, PointF control2, PointF end)
		{
			Control.AddBezier (start.ToSD (), control1.ToSD (), control2.ToSD (), end.ToSD ());
		}

		public void AddPath (IGraphicsPath path, bool connect = false)
		{
			if (path != null && !path.IsEmpty) // avoid throwing an exception if the path is empty - consistent across platforms.
				Control.AddPath(path.ToSD(), connect);
		}

		public void Transform (IMatrix matrix)
		{
			Control.Transform (matrix.ToSD ());
		}

		public void CloseFigure ()
		{
			Control.CloseFigure ();
		}

		public void StartFigure ()
		{
			Control.StartFigure ();
		}

		public void AddCurve (IEnumerable<PointF> points, float tension = 0.5f)
		{
			var sdpoints = from p in points select p.ToSD ();
			var pointArray = sdpoints.ToArray ();
			Control.AddCurve (pointArray, tension);
			position = pointArray.Last ();
		}

		public RectangleF Bounds
		{
			get { return Control.GetBounds ().ToEto (); }
		}

		public object ControlObject
		{
			get { return Control; }
		}

		public void Dispose ()
		{
			Control.Dispose ();
		}


		public void AddArc (float x, float y, float width, float height, float startAngle, float sweepAngle)
		{
			Control.AddArc (x, y, width, height, startAngle, sweepAngle);
		}

		public void AddRectangle (float x, float y, float width, float height)
		{
			Control.AddRectangle (new sd.RectangleF (x, y, width, height));
		}

		public void AddEllipse (float x, float y, float width, float height)
		{
			Control.AddEllipse (x, y, width, height);
		}

		public bool IsEmpty
		{
			get { return Control.PointCount == 0; }
		}

		public PointF CurrentPoint
		{
			get { return Control.GetLastPoint ().ToEto (); }
		}

		public IGraphicsPath Clone ()
		{
			return new GraphicsPathHandler ((sd.Drawing2D.GraphicsPath)Control.Clone ());
		}

		public FillMode FillMode
		{
			set { Control.FillMode = value == FillMode.Alternate ? sd2.FillMode.Alternate : sd2.FillMode.Winding; }
			get { return Control.FillMode == sd2.FillMode.Alternate ? FillMode.Alternate : FillMode.Winding; }
		}
	}
}

