using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Eto.Drawing;

using aa = Android.App;
using ac = Android.Content;
using ao = Android.OS;
using ar = Android.Runtime;
using av = Android.Views;
using aw = Android.Widget;
using ag = Android.Graphics;

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
			get { throw new NotImplementedException(); }
		}

		public FillMode FillMode
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		public bool IsEmpty
		{
			get { return Control.IsEmpty; }
		}

		public PointF CurrentPoint
		{
			get { throw new NotImplementedException(); }
		}

		public void AddLine(float startX, float startY, float endX, float endY)
		{
			this.Control.MoveTo(startX, startY);
			this.Control.LineTo(endX, endY);
			position = new ag.PointF(endX, endY);
		}

		public void AddLines(IEnumerable<PointF> points)
		{
			throw new NotImplementedException();
		}

		public void LineTo(float x, float y)
		{
			throw new NotImplementedException();
		}

		public void MoveTo(float x, float y)
		{
			throw new NotImplementedException();
		}

		public void AddArc(float x, float y, float width, float height, float startAngle, float sweepAngle)
		{
			throw new NotImplementedException();
		}

		public void AddBezier(PointF start, PointF control1, PointF control2, PointF end)
		{
			throw new NotImplementedException();
		}

		public void AddCurve(IEnumerable<PointF> points, float tension = 0.5f)
		{
			throw new NotImplementedException();
		}

		public void AddEllipse(float x, float y, float width, float height)
		{
			throw new NotImplementedException();
		}

		public void AddRectangle(float x, float y, float width, float height)
		{
			throw new NotImplementedException();
		}

		public void AddPath(IGraphicsPath path, bool connect = false)
		{
			throw new NotImplementedException();
		}

		public void Transform(IMatrix matrix)
		{
			throw new NotImplementedException();
		}

		public void StartFigure()
		{
			throw new NotImplementedException();
		}

		public void CloseFigure()
		{
			throw new NotImplementedException();
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