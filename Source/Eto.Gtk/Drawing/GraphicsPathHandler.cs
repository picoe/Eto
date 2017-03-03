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
		IMatrix transform;
		readonly List<Command> commands = new List<Command> ();

		bool firstFigureClosed;
		bool isFirstFigure = true;

		public IEnumerable<PointF> Points
		{
			get
			{
				foreach (var command in commands) {
					if (command.Points == null)
						continue;
					foreach (var point in command.Points) {
						if (transform != null)
							yield return transform.TransformPoint (point);
						else
							yield return point;
					}
				}
			}
		}

		class CommandExecutor
		{
			public Cairo.Context Context { get; private set; }

			public bool First { get; private set; }

			public bool ForceConnect { get; set; }

			public CommandExecutor (Cairo.Context context)
			{
				Context = context;
				First = true;
			}

			public bool SetStart (float x, float y)
			{
				return SetStart (new PointF (x, y));
			}

			public void ResetStart ()
			{
				First = true;
			}

			public bool SetStart (PointF point)
			{
				First = false;
				if (ForceConnect) {
					ConnectTo (point);
					return false;
				}
				return true;
			}

			public void ConnectTo (float x, float y)
			{
				ConnectTo (new PointF(x, y));
			}

			public void ConnectTo (PointF point)
			{
				if (First) {
					Context.MoveTo (point.X, point.Y);
					First = false;
				} else
					Context.LineTo (point.X, point.Y);
				ForceConnect = false;
			}

			public void CloseFigure ()
			{
				Context.ClosePath ();
				First = true;
				Context.NewSubPath ();
			}

		}
		
		struct Command
		{
			public PointF[] Points;

			public object Data;

			public GraphicsPathCommandDelegate Action;

			public Command(GraphicsPathCommandDelegate action, PointF[] points = null, object data = null)
			{
				Points = points;
				Action = action;
				Data = data;
			}
		}
		
		delegate void GraphicsPathCommandDelegate(CommandExecutor exec, PointF[] points, object data);

		public void AddLines (IEnumerable<PointF> points)
		{
			commands.Add(new Command(
				ExecuteLines,
				points.ToArray()
			));
		}

		void ExecuteLines(CommandExecutor exec, PointF[] points, object data)
		{
			for (int i = 0; i < points.Length; i++)
			{
				var p = points[i];
				if (i == 0)
					exec.ConnectTo(p.X, p.Y);
				else
					exec.Context.LineTo(p.X, p.Y);
			}
		}

		public void MoveTo (float x, float y)
		{
			commands.Add(new Command(
				ExecuteMoveTo,
				new[] { new PointF(x, y) }
			));
		}

		void ExecuteMoveTo(CommandExecutor exec, PointF[] points, object data)
		{
			var point = points[0];
			if (exec.SetStart(point.X, point.Y))
				exec.Context.MoveTo(point.X, point.Y);
		}

		public void LineTo (float x, float y)
		{
			commands.Add(new Command(
				ExecuteLineTo,
				new[] { new PointF(x, y) }
			));
		}

		void ExecuteLineTo(CommandExecutor exec, PointF[] points, object data)
		{
			var point = points[0];
			exec.SetStart(point.X, point.Y);
			exec.Context.LineTo(point.X, point.Y);
		}

		public void AddLine (float startX, float startY, float endX, float endY)
		{
			commands.Add(new Command(
				ExecuteAddLine,
				new[] { new PointF(startX, startY), new PointF(endX, endY) }
			));
		}

		void ExecuteAddLine(CommandExecutor exec, PointF[] points, object data)
		{
			var start = points[0];
			var end = points[1];
			exec.ConnectTo(start.X, start.Y);
			exec.Context.LineTo(end.X, end.Y);
		}

		public void Apply (Cairo.Context context)
		{
			var exec = new CommandExecutor (context);
			Connect (exec);
		}

		void Connect (CommandExecutor exec)
		{
			if (transform != null) {
				exec.Context.Save ();
				exec.Context.Transform(transform.ToCairo());
			}
			foreach (var command in commands)
			{
				command.Action(exec, command.Points, command.Data);
			}
			if (transform != null)
			{
				exec.Context.Restore();
			}
		}

		public void AddArc (float x, float y, float width, float height, float startAngle, float sweepAngle)
		{
			commands.Add(new Command(
				ExecuteArc,
				new[] { new PointF(x, y), new PointF(x + width, y + height) },
				new[] { x, y, width, height, startAngle, sweepAngle }
			));
		}

		void ExecuteArc(CommandExecutor exec, PointF[] points, object data)
		{
			var args = data as float[];
			var x = args[0];
			var y = args[1];
			var width = args[2];
			var height = args[3];
			var startAngle = args[4];
			var sweepAngle = args[5];
			// degrees to radians conversion
			double startRadians = startAngle * Math.PI / 180.0;

			// x and y radius
			float dx = width / 2;
			float dy = height / 2;

			// determine the start point
			double xs = x + dx + (Math.Cos(startRadians) * dx);
			double ys = y + dy + (Math.Sin(startRadians) * dy);
			exec.SetStart((float)xs, (float)ys);

			exec.Context.Save();
			exec.Context.Translate(x + width / 2, y + height / 2);
			double radius = Math.Max(width / 2.0, height / 2.0);
			if (width > height)
				exec.Context.Scale(1.0, height / width);
			else
				exec.Context.Scale(width / height, 1.0);

			if (sweepAngle < 0)
				exec.Context.ArcNegative(0, 0, radius, Conversions.DegreesToRadians(startAngle), Conversions.DegreesToRadians(startAngle + sweepAngle));
			else
				exec.Context.Arc(0, 0, radius, Conversions.DegreesToRadians(startAngle), Conversions.DegreesToRadians(startAngle + sweepAngle));
			exec.Context.Restore();
		}

		public void AddBezier(PointF start, PointF control1, PointF control2, PointF end)
		{
			commands.Add(new Command(
				ExecuteBezier,
				new[] { start, end },
				new[] { control1, control2 }
			));
		}

		void ExecuteBezier(CommandExecutor exec, PointF[] points, object data)
		{
			var start = points[0];
			var end = points[1];
			var controlPoints = data as PointF[];
			var control1 = controlPoints[0];
			var control2 = controlPoints[1];

			exec.ConnectTo(start);
			exec.Context.CurveTo(control1.ToCairo(), control2.ToCairo(), end.ToCairo());
		}

		public void AddRectangle(float x, float y, float width, float height)
		{
			commands.Add(new Command(
				ExecuteRectangle,
				new[] { new PointF(x, y), new PointF(x + width, y + height) },
				new[] { width, height }
			));
			isFirstFigure = false;
		}

		void ExecuteRectangle(CommandExecutor exec, PointF[] points, object data)
		{
			var point = points[0];
			var x = point.X;
			var y = point.Y;
			var args = data as float[];
			var width = args[0];
			var height = args[1];

			exec.Context.NewSubPath();
			exec.Context.Rectangle(x, y, width, height);
			exec.ResetStart();
		}

		public void AddPath (IGraphicsPath path, bool connect = false)
		{
			var handler = path.ToHandler ();
			commands.Add(new Command(
				ExecutePath,
				handler.Points.ToArray(),
				new object[] { handler, connect }
			));
		}

		void ExecutePath(CommandExecutor exec, PointF[] points, object data)
		{
			var args = data as object[];
			var handler = args[0] as GraphicsPathHandler;
			var connect = (bool)args[1];

			if (connect && !handler.firstFigureClosed)
			{
				exec.ForceConnect = true;
				handler.Connect(exec);
			}
			else
				handler.Apply(exec.Context);
		}

		public void Transform (IMatrix matrix)
		{
			if (transform != null)
				transform.Prepend (matrix);
			else
				transform = matrix;
		}

		public void CloseFigure ()
		{
			commands.Add(new Command(
				ExecuteCloseFigure
			));
			if (isFirstFigure)
				firstFigureClosed = true;
		}

		void ExecuteCloseFigure(CommandExecutor exec, PointF[] points, object data)
		{
			exec.CloseFigure();
		}

		public void StartFigure ()
		{
			commands.Add(new Command(
				ExecuteStartFigure
			));
			isFirstFigure = false;
		}

		void ExecuteStartFigure(CommandExecutor exec, PointF[] points, object data)
		{
			exec.Context.NewSubPath();
			exec.ResetStart();
		}

		public void AddEllipse (float x, float y, float width, float height)
		{
			commands.Add(new Command(
				ExecuteEllipse,
				new[] { new PointF(x, y), new PointF(x + width, y + height) },
				new[] { width, height }
			));
			isFirstFigure = false;
		}

		void ExecuteEllipse(CommandExecutor exec, PointF[] points, object data)
		{
			var point = points[0];
			var x = point.X;
			var y = point.Y;
			var args = data as float[];
			var width = args[0];
			var height = args[1];

			exec.Context.NewSubPath();
			exec.Context.Save();
			exec.Context.Translate(x + width / 2, y + height / 2);
			double radius = Math.Max(width / 2.0, height / 2.0);
			if (width > height)
				exec.Context.Scale(1.0, height / width);
			else
				exec.Context.Scale(width / height, 1.0);
			exec.Context.Arc(0, 0, radius, 0, 2 * Math.PI);
			exec.Context.Restore();
			exec.ResetStart();
		}

		public void AddCurve (IEnumerable<PointF> points, float tension = 0.5f)
		{
			var pointArray = SplineHelper.SplineCurve(points, tension).ToArray();
			commands.Add(new Command(
				ExecuteCurve,
				pointArray,
				points.First()
			));
		}

		void ExecuteCurve(CommandExecutor exec, PointF[] points, object data)
		{
			var first = (PointF)data;
			exec.SetStart(first);
			SplineHelper.Draw(points, exec.ConnectTo, (c1, c2, end) => exec.Context.CurveTo(c1.ToCairo(), c2.ToCairo(), end.ToCairo()));
		}

		public void Dispose ()
		{
		}

		public RectangleF Bounds
		{
			get {
				bool first = true;
				PointF minPoint = PointF.Empty;
				PointF maxPoint = PointF.Empty;
				foreach (var point in Points) {
					if (first) {
						minPoint = maxPoint = point;
						first = false;
					} else {
						minPoint = PointF.Min (point, minPoint);
						maxPoint = PointF.Max (point, maxPoint);
					}
				}
				return new RectangleF (minPoint, maxPoint);
			}
		}

		public bool IsEmpty
		{
			get { return commands.Count == 0; }
		}

		public PointF CurrentPoint
		{
			get {
				// slow to get current point, but no other way?
				using (var context = Playback ()) {
					return context.CurrentPoint.ToEto ();
				}
			}
		}

		Cairo.Context Playback ()
		{
			using (var surface = new Cairo.ImageSurface (Cairo.Format.ARGB32, 10, 10))
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
			var handler = new GraphicsPathHandler ();
			handler.commands.AddRange(commands);
			handler.transform = transform != null ? transform.Clone () : null;
			handler.firstFigureClosed = firstFigureClosed;
			handler.isFirstFigure = isFirstFigure;
			return handler;
		}

		public FillMode FillMode
		{
			get; set;
		}
	}
}

