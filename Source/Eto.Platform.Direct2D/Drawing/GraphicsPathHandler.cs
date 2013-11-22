using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using s = SharpDX;
using sd = SharpDX.Direct2D1;
using sw = SharpDX.DirectWrite;
using Eto.Drawing;

namespace Eto.Platform.Direct2D.Drawing
{
	/// <summary>
	/// Handler for <see cref="IGraphicsPath"/>
	/// </summary>
	/// <copyright>(c) 2013 by Vivek Jhaveri</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	public class GraphicsPathHandler : IGraphicsPathHandler
	{
		public sd.PathGeometry Control { get; private set; }
		private sd.GeometrySink sink;
		bool isInFigure = false;
		public PointF CurrentPoint { get; private set; }
		public object ControlObject { get { return this.Control; } }
		public RectangleF Bounds { get { return Control.GetBounds().ToEto(); } }

		public GraphicsPathHandler()
		{
			this.Control = new sd.PathGeometry(SDFactory.D2D1Factory);
			this.sink = this.Control.Open();
		}

		public void CloseSink()
		{
			// This must be called before rendering the path.
			if (this.sink != null)
			{
				this.sink.Close();
				this.sink.Dispose();
			}
		}

		public void AddBezier(PointF pt1, PointF pt2, PointF pt3, PointF pt4)
		{
			ConnectTo(pt1);

			sink.AddBezier(new sd.BezierSegment
			{
				Point1 = pt2.ToDx(),
				Point2 = pt3.ToDx(),
				Point3 = pt4.ToDx()
			});

			CurrentPoint = pt4;
		}

		public void AddPath(IGraphicsPath path, bool connect)
		{
			throw new NotImplementedException();
		}

		public IGraphicsPath Clone()
		{
			throw new NotImplementedException();
		}

		public FillMode FillMode
		{
			get { throw new NotImplementedException(); }
			set { throw new NotImplementedException(); }
		}

		public bool IsEmpty
		{
			get { throw new NotImplementedException(); }
		}

		void ConnectTo(PointF p)
		{
			var pt = p.ToDx();
			if (isInFigure)
				sink.AddLine(pt);
			else
				sink.BeginFigure(pt, sd.FigureBegin.Hollow); // what is hollow vs. filled?

			CurrentPoint = p;
		}

		public void StartFigure()
		{
			CloseFigure();
		}

		public void CloseFigure()
		{
			if (isInFigure)
			{
				sink.EndFigure(sd.FigureEnd.Closed);
				isInFigure = false;
			}
		}

		public void Transform(IMatrix matrix)
		{
			throw new NotImplementedException();
		}

		public void AddLine(float startX, float startY, float endX, float endY)
		{
			ConnectTo(new PointF(startX, startY));
			ConnectTo(new PointF(endX, endY));
		}

		public void AddLines(IEnumerable<PointF> points)
		{
			foreach (var p in points)
				ConnectTo(p);
		}

		public void LineTo(float x, float y)
		{
			ConnectTo(new PointF(x, y));
		}

		public void MoveTo(float x, float y)
		{
			sink.BeginFigure(
				new s.DrawingPointF(x, y),
				sd.FigureBegin.Filled); // is this correct?
		}

		public void AddCurve(IEnumerable<PointF> points, float tension = 0.5f)
		{
			//throw new NotImplementedException();
		}

		public void AddArc(float x, float y, float width, float height, float startAngle, float sweepAngle)
		{			
			//throw new NotImplementedException();
		}

		public void AddEllipse(float x, float y, float width, float height)
		{			
			//throw new NotImplementedException();
		}

		public void AddRectangle(float x, float y, float width, float height)
		{
			var left = x;
			var top = y;
			var right = x + width - 1;
			var bottom = y + height - 1;

            ConnectTo(new PointF(left, top));
            ConnectTo(new PointF(right, top));
            ConnectTo(new PointF(right, bottom));
            ConnectTo(new PointF(left, bottom));
			ConnectTo(new PointF(left, top));
		}

		public void Dispose()
		{
			Control.Dispose();
		}
	}
}
