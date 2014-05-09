using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using s = SharpDX;
using sd = SharpDX.Direct2D1;
using sw = SharpDX.DirectWrite;
using Eto.Drawing;

namespace Eto.Direct2D.Drawing
{
	/// <summary>
	/// Handler for <see cref="IGraphicsPath"/>
	/// </summary>
	/// <copyright>(c) 2013 by Vivek Jhaveri</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	public class GraphicsPathHandler : GraphicsPath.IHandler
	{
		List<sd.Geometry> geometries = new List<sd.Geometry>();
		IMatrix transform;
		sd.PathGeometry path;
		bool isInFigure = false;
		sd.FillMode fillMode;
		sd.Geometry control;

		public PointF CurrentPoint { get; private set; }
		public object ControlObject { get { return this.Control; } }
	
		public sd.Geometry Control
		{
			get
			{
				if (control == null)
				{
					if (geometries.Count > 0)
					{
						control = new sd.GeometryGroup(SDFactory.D2D1Factory, fillMode, geometries.ToArray());
					}
					if (transform != null && control != null)
						control = new sd.TransformedGeometry(SDFactory.D2D1Factory, control, transform.ToDx());
				}
				return control;
			}
		}
		public RectangleF Bounds
		{
			get
			{
				return Control.GetBounds().ToEto();
			}
		}

		sd.GeometrySink sink;
		public sd.GeometrySink Sink
		{
			get
			{
				if (sink == null)
				{
					geometries.Add(path = new sd.PathGeometry(SDFactory.D2D1Factory));
					control = null;
					sink = path.Open();
				}
				return sink;
			}
		}

		public GraphicsPathHandler()
		{
		}

		public void CloseSink()
		{
			// This must be called before rendering the path.
			if (sink != null)
			{
				if (isInFigure)
					sink.EndFigure(sd.FigureEnd.Open);
				isInFigure = false;
				sink.Close();
				sink.Dispose();
				sink = null;
			}
		}

		public void AddBezier(PointF pt1, PointF pt2, PointF pt3, PointF pt4)
		{
			ConnectTo(pt1);
			AddBezier(pt2, pt3, pt4);
		}

		void AddBezier(PointF pt2, PointF pt3, PointF pt4)
		{
			Sink.AddBezier(new sd.BezierSegment
			{
				Point1 = pt2.ToDx(),
				Point2 = pt3.ToDx(),
				Point3 = pt4.ToDx()
			});

			CurrentPoint = pt4;
		}

		public IGraphicsPath Clone()
		{
			throw new NotImplementedException();
		}

		public FillMode FillMode
		{
			get { return fillMode.ToEto(); ; }
			set { fillMode = value.ToDx(); }
		}

		public bool IsEmpty
		{
			get { return geometries.Count == 0; }
		}

		void ConnectTo(PointF p)
		{
			var pt = p.ToDx();
			if (isInFigure)
				Sink.AddLine(pt);
			else
			{
				isInFigure = true;
				// create filled for when we fill with a brush
				Sink.BeginFigure(pt, sd.FigureBegin.Filled);
			}

			CurrentPoint = p;
		}

		public void StartFigure()
		{
			if (isInFigure)
			{
				Sink.EndFigure(sd.FigureEnd.Open);
				isInFigure = false;
			}
		}

		public void CloseFigure()
		{
			if (isInFigure)
			{
				Sink.EndFigure(sd.FigureEnd.Closed);
				isInFigure = false;
			}
		}

		public void Transform(IMatrix matrix)
		{
			if (matrix != null)
			{
				if (transform != null)
					transform.Prepend(matrix);
				else
					transform = matrix.Clone();
			}
			else
				transform = null;
			control = null;
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
			StartFigure();
			ConnectTo(new PointF(x, y));
		}

		public void AddCurve(IEnumerable<PointF> points, float tension = 0.5f)
		{
			var temp = SplineHelper.SplineCurve(points, tension);
			SplineHelper.Draw(temp, ConnectTo, AddBezier);
		}

		public void AddArc(float x, float y, float width, float height, float startAngle, float sweepAngle)
		{
			PointF start;
			var arc = GraphicsHandler.CreateArc(x, y, width, height, startAngle, sweepAngle, out start);
			ConnectTo(start);
			Sink.AddArc(arc);
		}

		public void AddEllipse(float x, float y, float width, float height)
		{
			CloseSink();
			var ellipse = new sd.Ellipse(new s.Vector2(x + width / 2, y + height / 2), width / 2, height / 2);
			geometries.Add(new sd.EllipseGeometry(SDFactory.D2D1Factory, ellipse));
			control = null;
		}

		public void AddPath(IGraphicsPath path, bool connect)
		{
			var inputGeometry = path.ToHandler();
			if (connect)
			{
				// TODO: how do we attach to the existing sink?  throws an exception otherwise
				StartFigure();
				inputGeometry.Control.Simplify(sd.GeometrySimplificationOption.CubicsAndLines, Sink);
			}
			else
			{
				CloseSink();
				geometries.Add(inputGeometry.Control);
			}
			control = null;
		}

		public void AddRectangle(float x, float y, float width, float height)
		{
			CloseSink();
			geometries.Add(new sd.RectangleGeometry(SDFactory.D2D1Factory, new s.RectangleF(x, y, width, height)));
			control = null;
		}

		public void Dispose()
		{
			Control.Dispose();
		}
	}
}
