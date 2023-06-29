using aa = Android.App;
using ac = Android.Content;
using ao = Android.OS;
using ar = Android.Runtime;
using av = Android.Views;
using aw = Android.Widget;
using ag = Android.Graphics;
using Org.Apache.Http.Conn;

namespace Eto.Android.Drawing
{
	/// <summary>
	/// Handler for <see cref="IGraphicsPath"/>
	/// </summary>
	/// <copyright>(c) 2013 by Vivek Jhaveri</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	public class GraphicsPathHandler : GraphicsPath.IHandler
	{
		public ag.Path Control { get; private set; }
		ag.PointF position;

		public GraphicsPathHandler()
		{
			Control = new ag.Path();
		}

		public GraphicsPathHandler(ag.Path control)
		{
			Control = control;
		}
		
		public RectangleF Bounds
		{
			get
			{
				ag.RectF Output = new ag.RectF();
				this.Control.ComputeBounds(Output, true);
				return Output.ToEto();
			}
		}

		public FillMode FillMode
		{
			get
			{
				return fillMode;
			}
			set
			{
				switch (value)
				{
					case FillMode.Alternate:
						Control.SetFillType(ag.Path.FillType.EvenOdd);
						break;
					case FillMode.Winding:
						Control.SetFillType(ag.Path.FillType.Winding);
						break;
					default:
						throw new ArgumentOutOfRangeException("Unsupported fill mode " + value);
				}

				fillMode = value;
			}
		}

		private FillMode fillMode = FillMode.Alternate;

		public bool IsEmpty
		{
			get { return Control.IsEmpty; }
		}

		public PointF CurrentPoint
		{
			get
			{
				var c = new float[2];
				var m = new ag.PathMeasure(this.Control, false);
				m.GetPosTan(1, c, null);
				return new PointF(c[0], c[1]);
			}
		}

		public void AddLine(float startX, float startY, float endX, float endY)
		{
			this.Control.MoveTo(startX, startY);
			this.Control.LineTo(endX, endY);
			position = new ag.PointF(endX, endY);
		}

		public void AddLines(IEnumerable<PointF> points)
		{
			foreach (var p in points)
				LineTo(p.X, p.Y);
		}

		public void LineTo(float x, float y)
		{
			this.Control.LineTo(x, y);
		}

		public void MoveTo(float x, float y)
		{
			this.Control.MoveTo(x, y);
		}

		public void AddArc(float x, float y, float width, float height, float startAngle, float sweepAngle)
		{
			this.Control.ArcTo(x, y, x + width, y + height, startAngle, sweepAngle, false);
		}

		public void AddBezier(PointF start, PointF control1, PointF control2, PointF end)
		{
			this.Control.MoveTo(start.X, start.Y);
			this.Control.CubicTo(control1.X, control1.Y, control2.X, control2.Y, end.X, end.Y);
		}

		public void AddCurve(IEnumerable<PointF> points, float tension = 0.5f)
		{
			throw new NotImplementedException();
		}

		public void AddEllipse(float x, float y, float width, float height)
		{
			this.Control.AddOval(x, y, x + width, y + height, ag.Path.Direction.Cw);
		}

		public void AddRectangle(float x, float y, float width, float height)
		{
			this.Control.AddRect(x, y, x + width, y + height, ag.Path.Direction.Cw);
		}

		public void AddPath(IGraphicsPath path, bool connect = false)
		{
			if (path != null && !path.IsEmpty) // avoid throwing an exception if the path is empty - consistent across platforms.
				Control.AddPath((ag.Path)path.ControlObject);
		}

		public bool FillContains(PointF point)
		{
			throw new NotImplementedException();
		}

		public bool StrokeContains(Pen pen, PointF point)
		{
			throw new NotImplementedException();
		}

		public void Transform(IMatrix matrix)
		{
			Control.Transform(matrix.ToAndroid());
		}

		public void StartFigure()
		{
			Control.RMoveTo(0, 0);
		}

		public void CloseFigure()
		{
			Control.Close();
		}

		public IGraphicsPath Clone()
		{
			return new GraphicsPathHandler(new ag.Path(this.Control));
		}

		public void Dispose()
		{
			Control.Dispose();
		}

		public object ControlObject
		{
			get { return this; }
		}
	}
}
