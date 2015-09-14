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
				this.Context = context;
				this.First = true;
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
		
		abstract class Command
		{
			public PointF[] Points { get; set; }

			public abstract void Apply (CommandExecutor exec);
		}
		
		delegate void GraphicsPathCommandDelegate (CommandExecutor exec);

		class ActionCommand : Command
		{
			readonly GraphicsPathCommandDelegate apply;

			public ActionCommand (GraphicsPathCommandDelegate apply)
			{
				this.apply = apply; 
			}

			public override void Apply (CommandExecutor exec)
			{
				apply (exec);
			}
		}

		public void AddLines (IEnumerable<PointF> points)
		{
			var pointArray = points.ToArray ();
			Add (exec => {
				for (int i=0; i<pointArray.Length; i++) {
					var p = pointArray [i];
					if (i == 0)
						exec.ConnectTo (p.X, p.Y);
					else
						exec.Context.LineTo (p.X, p.Y);
				}
			}, pointArray);
		}

		public void MoveTo (float x, float y)
		{
			Add (exec => {
				if (exec.SetStart (x, y))
					exec.Context.MoveTo (x, y);
			}, new PointF (x, y));
		}
		
		public void LineTo (float x, float y)
		{
			Add (exec => {
				exec.SetStart (x, y);
				exec.Context.LineTo (x, y);
			}, new PointF (x, y));
		}
		
		public void AddLine (float startX, float startY, float endX, float endY)
		{
			Add (exec => {
				exec.ConnectTo (startX, startY);
				exec.Context.LineTo (endX, endY);
			}, new PointF (startX, startY), new PointF (endX, endY));
		}
		
		void Add (GraphicsPathCommandDelegate apply, params PointF[] points)
		{
			commands.Add (new ActionCommand (apply) { Points = points });
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
				exec.Context.Transform (transform.ToCairo ());
			}
			foreach (var command in commands) {
				command.Apply (exec);
			}
			if (transform != null) {
				exec.Context.Restore ();
			}
		}

		public void AddArc (float x, float y, float width, float height, float startAngle, float sweepAngle)
		{
			Add (exec => {
				// degrees to radians conversion
				double startRadians = startAngle * Math.PI / 180.0;

				// x and y radius
				float dx = width / 2;
				float dy = height / 2;
				
				// determine the start point
				double xs = x + dx + (Math.Cos (startRadians) * dx);
				double ys = y + dy + (Math.Sin (startRadians) * dy);
				exec.SetStart ((float)xs, (float)ys);

				exec.Context.Save ();
				exec.Context.Translate (x + width / 2, y + height / 2);
				double radius = Math.Max (width / 2.0, height / 2.0);
				if (width > height)
					exec.Context.Scale (1.0, height / width);
				else
					exec.Context.Scale (width / height, 1.0);

				if (sweepAngle < 0)
					exec.Context.ArcNegative(0, 0, radius, Conversions.DegreesToRadians(startAngle), Conversions.DegreesToRadians(startAngle + sweepAngle));
				else
					exec.Context.Arc(0, 0, radius, Conversions.DegreesToRadians(startAngle), Conversions.DegreesToRadians(startAngle + sweepAngle));
				exec.Context.Restore ();
			}, new PointF (x, y), new PointF (x + width, y + height));
		}

		public void AddBezier (PointF start, PointF control1, PointF control2, PointF end)
		{
			Add (exec => {
				exec.ConnectTo (start);
				exec.Context.CurveTo (control1.ToCairo (), control2.ToCairo (), end.ToCairo ());
			}, start, end);
		}

		public void AddRectangle (float x, float y, float width, float height)
		{
			Add (exec => {
				exec.Context.NewSubPath ();
				exec.Context.Rectangle (x, y, width, height);
				exec.ResetStart ();
			}, new PointF (x, y), new PointF (x + width, y + height));
			isFirstFigure = false;
		}

		public void AddPath (IGraphicsPath path, bool connect = false)
		{
			var handler = path.ToHandler ();
			Add (exec => {
				if (connect && !handler.firstFigureClosed) {
					exec.ForceConnect = true;
					handler.Connect (exec);
				} else
					handler.Apply (exec.Context);
			}, handler.Points.ToArray ());
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
			Add (exec => exec.CloseFigure());
			if (isFirstFigure)
				firstFigureClosed = true;
		}

		public void StartFigure ()
		{
			Add (exec => {
				exec.Context.NewSubPath ();
				exec.ResetStart ();
			});
			isFirstFigure = false;
		}

		public void AddEllipse (float x, float y, float width, float height)
		{
			Add (exec => {
				exec.Context.NewSubPath ();
				exec.Context.Save ();
				exec.Context.Translate (x + width / 2, y + height / 2);
				double radius = Math.Max (width / 2.0, height / 2.0);
				if (width > height)
					exec.Context.Scale (1.0, height / width);
				else
					exec.Context.Scale (width / height, 1.0);
				exec.Context.Arc (0, 0, radius, 0, 2 * Math.PI);
				exec.Context.Restore ();
				exec.ResetStart ();
			}, new PointF (x, y), new PointF (x + width, y + height));
			isFirstFigure = false;
		}

		public void AddCurve (IEnumerable<PointF> points, float tension = 0.5f)
		{
			var pointArray = SplineHelper.SplineCurve (points, tension).ToArray ();
			Add (exec => {
				exec.SetStart (points.First ());
				SplineHelper.Draw(pointArray, exec.ConnectTo, (c1, c2, end) => exec.Context.CurveTo(c1.ToCairo(), c2.ToCairo(), end.ToCairo()));
			}, pointArray);
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

