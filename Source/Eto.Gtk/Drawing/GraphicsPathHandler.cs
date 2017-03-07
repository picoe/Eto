using System;
using Eto.Drawing;
using System.Collections.Generic;
using System.Linq;

namespace Eto.GtkSharp.Drawing
{
	/// <summary>
	/// Handler for <see cref="IGraphicsPath"/>
	/// </summary>
	/// <copyright>(c) 2012-2014 by Curtis Wensley</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	public class GraphicsPathHandler : GraphicsPath.IHandler
	{
		IMatrix _transform;
		enum CommandType : byte
		{
			Lines,
			MoveTo,
			LineTo,
			Line,
			Arc,
			Bezier,
			Rectangle,
			StartPath,
			EndPath,
			CloseFigure,
			StartFigure,
			Ellipse,
			Curve,
		}

		readonly List<CommandType> _commands = new List<CommandType>(16);
		readonly List<float> _data = new List<float>(64);

		bool _firstFigureClosed;
		bool _isFirstFigure = true;

		void AddBool(bool value) => _data.Add(value ? 1f : float.NaN);

		void AddFloat(float value) => _data.Add(value);

		void AddPoint(PointF value)
		{
			AddFloat(value.X);
			AddFloat(value.Y);
		}

		void AddTransform(IMatrix transform)
		{
			if (transform != null)
				_data.AddRange(transform.Elements);
			else
				_data.Add(float.NaN);
		}

		public IEnumerable<PointF> Points
		{
			get
			{
				if (_transform == null)
					return GetPoints();
				
				return GetPoints().Select(p => _transform.TransformPoint(p));
			}
		}

		IEnumerable<PointF> GetPoints()
		{
			int index = 0;
			float x, y, width, height;
			for (int i = 0; i < _commands.Count; i++)
			{
				switch (_commands[i])
				{
					case CommandType.Lines:
						for (;;)
						{
							x = _data[index++];
							if (float.IsNaN(x))
								break;
							y = _data[index++];
							yield return new PointF(x, y);
						}
						break;
					case CommandType.MoveTo:
						yield return new PointF(_data[index++], _data[index++]);
						break;
					case CommandType.LineTo:
						yield return new PointF(_data[index++], _data[index++]);
						break;
					case CommandType.Line:
						yield return new PointF(_data[index++], _data[index++]);
						yield return new PointF(_data[index++], _data[index++]);
						break;
					case CommandType.Arc:
						x = _data[index++];
						y = _data[index++];
						width = _data[index++];
						height = _data[index++];
						yield return new PointF(x, y);
						yield return new PointF(x + width, y + height);
						index += 2; // start & sweep
						break;
					case CommandType.Bezier:
						yield return new PointF(_data[index++], _data[index++]);
						yield return new PointF(_data[index++], _data[index++]);
						index += 4; // control1 & control2
						break;
					case CommandType.Rectangle:
						x = _data[index++];
						y = _data[index++];
						width = _data[index++];
						height = _data[index++];
						yield return new PointF(x, y);
						yield return new PointF(x + width, y + height);
						break;
					case CommandType.StartPath:
						index++; // connect
						x = _data[index++]; // transform
						if (!double.IsNaN(x))
							index += 5;
						break;
					case CommandType.EndPath:
					case CommandType.CloseFigure:
					case CommandType.StartFigure:
						break;
					case CommandType.Ellipse:
						x = _data[index++];
						y = _data[index++];
						width = _data[index++];
						height = _data[index++];
						yield return new PointF(x, y);
						yield return new PointF(x + width, y + height);
						break;
					case CommandType.Curve:
						for (;;)
						{
							x = _data[index++];
							if (float.IsNaN(x))
								break;
							y = _data[index++];
							yield return new PointF(x, y);
						}
						break;
					default:
						throw new ArgumentOutOfRangeException();
				}
			}
		}

		class CommandExecutor
		{
			int _index;
			public GraphicsPathHandler Handler { get; }

			public Cairo.Context Context { get; }

			public bool First { get; private set; }

			public bool ForceConnect { get; set; }

			public CommandExecutor(GraphicsPathHandler handler, Cairo.Context context)
			{
				Context = context;
				Handler = handler;
				First = true;
			}

			bool GetBool() => !double.IsNaN(GetFloat());

			float GetFloat() => Handler._data[_index++];

			Cairo.PointD GetCairoPoint() => new Cairo.PointD(GetFloat(), GetFloat());

			Cairo.Matrix GetTransform()
			{
				var xx = GetFloat();
				if (double.IsNaN(xx))
					return null;
				float yx = GetFloat();
				float xy = GetFloat();
				float yy = GetFloat();
				float x0 = GetFloat();
				float y0 = GetFloat();
				return new Cairo.Matrix(xx, yx, xy, yy, x0, y0);
			}

			IEnumerable<PointF> GetPoints()
			{
				for (;;)
				{
					var x = GetFloat();
					if (double.IsNaN(x))
						yield break;
					var y = GetFloat();
					yield return new PointF(x, y);
				}
			}

			public bool SetStart(float x, float y)
			{
				First = false;
				if (ForceConnect)
				{
					ConnectTo(x, y);
					return false;
				}
				return true;
			}

			public void ResetStart()
			{
				First = true;
			}

			public void ConnectTo(PointF point) => ConnectTo(point.X, point.Y);

			public void ConnectTo(float x, float y)
			{
				if (First)
				{
					Context.MoveTo(x, y);
					First = false;
				}
				else
					Context.LineTo(x, y);
				ForceConnect = false;
			}

			public void CloseFigure()
			{
				Context.ClosePath();
				First = true;
				Context.NewSubPath();
			}

			public void ExecuteLines()
			{
				var x = GetFloat();
				if (double.IsNaN(x))
					return;
				var y = GetFloat();
				ConnectTo(x, y);
				for (;;)
				{
					x = GetFloat();
					if (double.IsNaN(x))
						break;
					y = GetFloat();
					Context.LineTo(x, y);
				}
			}

			public void ExecuteMoveTo()
			{
				var x = GetFloat();
				var y = GetFloat();
				if (SetStart(x, y))
					Context.MoveTo(x, y);
			}

			public void ExecuteLineTo()
			{
				var x = GetFloat();
				var y = GetFloat();
				SetStart(x, y);
				Context.LineTo(x, y);
			}

			public void ExecuteLine()
			{
				var startX = GetFloat();
				var startY = GetFloat();
				var endX = GetFloat();
				var endY = GetFloat();
				ConnectTo(startX, startY);
				Context.LineTo(endX, endY);
			}

			public void ExecuteArc()
			{
				var x = GetFloat();
				var y = GetFloat();
				var width = GetFloat();
				var height = GetFloat();
				var startAngle = GetFloat();
				var sweepAngle = GetFloat();
				// degrees to radians conversion
				double startRadians = startAngle * Math.PI / 180.0;

				// x and y radius
				float dx = width / 2;
				float dy = height / 2;

				// determine the start point
				double xs = x + dx + (Math.Cos(startRadians) * dx);
				double ys = y + dy + (Math.Sin(startRadians) * dy);
				SetStart((float)xs, (float)ys);

				Context.Save();
				Context.Translate(x + width / 2, y + height / 2);
				double radius = Math.Max(width / 2.0, height / 2.0);
				if (width > height)
					Context.Scale(1.0, height / width);
				else
					Context.Scale(width / height, 1.0);

				if (sweepAngle < 0)
					Context.ArcNegative(0, 0, radius, Conversions.DegreesToRadians(startAngle), Conversions.DegreesToRadians(startAngle + sweepAngle));
				else
					Context.Arc(0, 0, radius, Conversions.DegreesToRadians(startAngle), Conversions.DegreesToRadians(startAngle + sweepAngle));
				Context.Restore();
			}

			public void ExecuteBezier()
			{
				var x = GetFloat();
				var y = GetFloat();
				var end = GetCairoPoint();
				var control1 = GetCairoPoint();
				var control2 = GetCairoPoint();

				ConnectTo(x, y);
				Context.CurveTo(control1, control2, end);
			}

			public void ExecuteRectangle()
			{
				var x = GetFloat();
				var y = GetFloat();
				var width = GetFloat();
				var height = GetFloat();

				Context.NewSubPath();
				Context.Rectangle(x, y, width, height);
				ResetStart();
			}

			public void ExecuteStartPath()
			{
				Context.Save();

				ForceConnect = GetBool();
				var transform = GetTransform();
				if (!ReferenceEquals(transform, null))
				{
					Context.Transform(transform);
				}
			}

			public void ExecuteEndPath()
			{
				Context.Restore();
			}

			public void ExecuteCloseFigure()
			{
				CloseFigure();
			}

			public void ExecuteStartFigure()
			{
				Context.NewSubPath();
				ResetStart();
			}

			public void ExecuteEllipse()
			{
				var x = GetFloat();
				var y = GetFloat();
				var width = GetFloat();
				var height = GetFloat();

				Context.NewSubPath();
				Context.Save();
				Context.Translate(x + width / 2, y + height / 2);
				double radius = Math.Max(width / 2.0, height / 2.0);
				if (width > height)
					Context.Scale(1.0, height / width);
				else
					Context.Scale(width / height, 1.0);
				Context.Arc(0, 0, radius, 0, 2 * Math.PI);
				Context.Restore();
				ResetStart();
			}

			public void ExecuteCurve()
			{
				var x = GetFloat();
				var y = GetFloat();
				SetStart(x, y);
				SplineHelper.Draw(GetPoints(), ConnectTo, (c1, c2, end) => Context.CurveTo(c1.ToCairo(), c2.ToCairo(), end.ToCairo()));
			}
		}

		public void AddLines(IEnumerable<PointF> points)
		{
			_commands.Add(CommandType.Lines);
			foreach (var point in points)
			{
				AddPoint(point);
			}
			AddFloat(float.NaN);
		}

		public void MoveTo(float x, float y)
		{
			_commands.Add(CommandType.MoveTo);
			AddFloat(x);
			AddFloat(y);
		}


		public void LineTo(float x, float y)
		{
			_commands.Add(CommandType.LineTo);
			AddFloat(x);
			AddFloat(y);
		}

		public void AddLine(float startX, float startY, float endX, float endY)
		{
			_commands.Add(CommandType.Line);
			AddFloat(startX);
			AddFloat(startY);
			AddFloat(endX);
			AddFloat(endY);
		}


		public void Apply(Cairo.Context context)
		{
			var exec = new CommandExecutor(this, context);
			if (_transform != null)
			{
				exec.Context.Save();
				exec.Context.Transform(_transform.ToCairo());
			}
			for (int i = 0; i < _commands.Count; i++)
			{
				switch (_commands[i])
				{
					case CommandType.Lines:
						exec.ExecuteLines();
						break;
					case CommandType.MoveTo:
						exec.ExecuteMoveTo();
						break;
					case CommandType.LineTo:
						exec.ExecuteLineTo();
						break;
					case CommandType.Line:
						exec.ExecuteLine();
						break;
					case CommandType.Arc:
						exec.ExecuteArc();
						break;
					case CommandType.Bezier:
						exec.ExecuteBezier();
						break;
					case CommandType.Rectangle:
						exec.ExecuteRectangle();
						break;
					case CommandType.StartPath:
						exec.ExecuteStartPath();
						break;
					case CommandType.EndPath:
						exec.ExecuteEndPath();
						break;
					case CommandType.CloseFigure:
						exec.ExecuteCloseFigure();
						break;
					case CommandType.StartFigure:
						exec.ExecuteStartFigure();
						break;
					case CommandType.Ellipse:
						exec.ExecuteEllipse();
						break;
					case CommandType.Curve:
						exec.ExecuteCurve();
						break;
					default:
						throw new ArgumentOutOfRangeException();
				}
			}
			if (_transform != null)
			{
				exec.Context.Restore();
			}
		}

		public void AddArc(float x, float y, float width, float height, float startAngle, float sweepAngle)
		{
			_commands.Add(CommandType.Arc);
			AddFloat(x);
			AddFloat(y);
			AddFloat(width);
			AddFloat(height);
			AddFloat(startAngle);
			AddFloat(sweepAngle);
		}

		public void AddBezier(PointF start, PointF control1, PointF control2, PointF end)
		{
			_commands.Add(CommandType.Bezier);
			AddPoint(start);
			AddPoint(end);
			AddPoint(control1);
			AddPoint(control2);
		}

		public void AddRectangle(float x, float y, float width, float height)
		{
			_commands.Add(CommandType.Rectangle);
			AddFloat(x);
			AddFloat(y);
			AddFloat(width);
			AddFloat(height);
			_isFirstFigure = false;
		}

		public void AddPath(IGraphicsPath path, bool connect = false)
		{
			var handler = path.ToHandler();

			_commands.Add(CommandType.StartPath);
			AddBool(connect && !handler._firstFigureClosed);
			AddTransform(handler._transform);

			_commands.AddRange(handler._commands);
			_data.AddRange(handler._data);

			_commands.Add(CommandType.EndPath);
		}

		public void Transform(IMatrix matrix)
		{
			if (_transform != null)
				_transform.Prepend(matrix);
			else
				_transform = matrix;
		}

		public void CloseFigure()
		{
			_commands.Add(CommandType.CloseFigure);
			if (_isFirstFigure)
				_firstFigureClosed = true;
		}

		public void StartFigure()
		{
			_commands.Add(CommandType.StartFigure);
			_isFirstFigure = false;
		}

		public void AddEllipse(float x, float y, float width, float height)
		{
			_commands.Add(CommandType.Ellipse);
			AddFloat(x);
			AddFloat(y);
			AddFloat(width);
			AddFloat(height);

			_isFirstFigure = false;
		}

		public void AddCurve(IEnumerable<PointF> points, float tension = 0.5f)
		{
			_commands.Add(CommandType.Curve);
			foreach (var point in SplineHelper.SplineCurve(points, tension))
			{
				AddPoint(point);
			}
			AddFloat(float.NaN);
		}

		public void Dispose()
		{
		}

		public RectangleF Bounds
		{
			get
			{
				bool first = true;
				PointF minPoint = PointF.Empty;
				PointF maxPoint = PointF.Empty;
				foreach (var point in Points)
				{
					if (first)
					{
						minPoint = maxPoint = point;
						first = false;
					}
					else
					{
						minPoint = PointF.Min(point, minPoint);
						maxPoint = PointF.Max(point, maxPoint);
					}
				}
				return new RectangleF(minPoint, maxPoint);
			}
		}

		public bool IsEmpty
		{
			get { return _commands.Count == 0; }
		}

		public PointF CurrentPoint
		{
			get
			{
				// slow to get current point, but no other way?
				using (var context = Playback())
				{
					return context.CurrentPoint.ToEto();
				}
			}
		}

		Cairo.Context Playback()
		{
			using (var surface = new Cairo.ImageSurface(Cairo.Format.ARGB32, 10, 10))
			{
				var context = new Cairo.Context(surface);
				Apply(context);
				return context;
			}
		}

		public object ControlObject
		{
			get { return this; }
		}

		public IGraphicsPath Clone()
		{
			var handler = new GraphicsPathHandler();
			handler._commands.AddRange(_commands);
			handler._data.AddRange(_data);
			handler._transform = _transform?.Clone();
			handler._firstFigureClosed = _firstFigureClosed;
			handler._isFirstFigure = _isFirstFigure;
			return handler;
		}

		public FillMode FillMode
		{
			get; set;
		}
	}
}