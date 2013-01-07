using System;
using System.Collections.Generic;

namespace Eto.Drawing
{
	/// <summary>
	/// Graphics path to be used for drawing or filling using a <see cref="Graphics"/> object
	/// </summary>
	/// <remarks>
	/// A graphics path can contain multiple figures comprised of various components such as line, arc, curve, etc.
	/// </remarks>
	/// <copyright>(c) 2012 by Curtis Wensley</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	public interface IGraphicsPath : IDisposable, IControlObjectSource
	{
		/// <summary>
		/// Gets the bounding rectangle for this path
		/// </summary>
		RectangleF Bounds { get; }
		
		/// <summary>
		/// Gets a value indicating that this graphics path is empty and has no segments
		/// </summary>
		/// <value><c>true</c> if this path is empty; otherwise, <c>false</c>.</value>
		bool IsEmpty { get; }
		
		/// <summary>
		/// Gets the current point
		/// </summary>
		/// <remarks>
		/// If the current figure in the path is not closed, and <see cref="StartFigure"/> is not called,
		/// the next segment will be connected from this point to its start point.
		/// </remarks>
		/// <value>The current point</value>
		PointF CurrentPoint { get; }

		/// <summary>
		/// Adds a line to the path with the specified start and end points
		/// </summary>
		/// <remarks>
		/// If the current figure is not closed, it will connect with the start of this line.
		/// The current position will be moved to the specified end location.
		/// </remarks>
		/// <param name="startX">X co-ordinate of the starting point</param>
		/// <param name="startY">Y co-ordinate of the starting point</param>
		/// <param name="endX">X co-ordinate of the end point</param>
		/// <param name="endY">Y co-ordinate of the end point</param>
		void AddLine (float startX, float startY, float endX, float endY);

		/// <summary>
		/// Adds lines to each of the specified <paramref name="points"/> to the path 
		/// </summary>
		/// <remarks>
		/// If the current figure is not closed, it will connect with the first point specified.
		/// The current position will be moved to the last point specified
		/// </remarks>
		/// <param name="points">Points for each part of the line</param>
		void AddLines (IEnumerable<PointF> points);

		/// <summary>
		/// Adds a line from the current position to the specified location
		/// </summary>
		/// <param name="x">X co-ordinate to draw the line to</param>
		/// <param name="y">Y co-ordinate to draw the line to</param>
		void LineTo (float x, float y);

		/// <summary>
		/// Moves the current position to the specified location without adding anything to the path
		/// </summary>
		/// <param name="x">X co-ordinate to move to</param>
		/// <param name="y">Y co-ordinate to move to</param>
		void MoveTo (float x, float y);

		/// <summary>
		/// Adds an arc into the specified rectangle
		/// </summary>
		/// <remarks>
		/// If the current figure is not closed, it will connect with the start of the arc.
		/// The current position will be moved to the ending point of the arc
		/// </remarks>
		/// <param name="x">The x coordinate of the upper left of the arc</param>
		/// <param name="y">The y coordinate of the upper left of the arc</param>
		/// <param name="width">Width of the rectangle containing the arc</param>
		/// <param name="height">Height of the rectangle containing the arc</param>
		/// <param name="startAngle">Start angle to begin the arc, in degrees</param>
		/// <param name="sweepAngle">Sweep angle (positive or negative) to specify how long the arc is, in degrees</param>
		void AddArc (float x, float y, float width, float height, float startAngle, float sweepAngle);

		/// <summary>
		/// Adds a bezier curve to the path with two control points
		/// </summary>
		/// <remarks>
		/// If the current figure is not closed, it will connect with the <paramref name="start"/> of the bezier curve.
		/// The current position will be moved to the <paramref name="end"/> point.
		/// </remarks>
		/// <param name="start">Starting point of the bezier curve</param>
		/// <param name="control1">First control point of the curve</param>
		/// <param name="control2">Second control point of the curve</param>
		/// <param name="end">Ending point of the bezier curve</param>
		void AddBezier (PointF start, PointF control1, PointF control2, PointF end);

		/// <summary>
		/// Adds a curve that intersects with the specified <paramref name="points"/> to the path
		/// </summary>
		/// <remarks>
		/// Each point in the list will fall on the line based on the <paramref name="tension"/> parameter
		/// </remarks>
		/// <param name="points">Points to calculate the curve</param>
		/// <param name="tension">Tension between points in the curve.  Should be between 0 (no curve) and 1 (more curve)</param>
		void AddCurve (IEnumerable<PointF> points, float tension = 0.5f);

		/// <summary>
		/// Adds an ellipse to the path
		/// </summary>
		/// <remarks>
		/// Rectangles are separate figures and will not connect to the current or next figure in the path.
		/// The starting point of the path will no longer be set after this call.
		/// </remarks>
		/// <param name="x">X co-ordinate of the top left of the ellipse</param>
		/// <param name="y">Y co-ordinate of the top left of the ellipse's bounding rectangle</param>
		/// <param name="width">Width of the rectangle</param>
		/// <param name="height">Height of the rectangle</param>
		void AddEllipse (float x, float y, float width, float height);

		/// <summary>
		/// Adds a rectangle to the path
		/// </summary>
		/// <remarks>
		/// Rectangles are separate figures and will not connect to the current or next figure in the path.
		/// The starting point of the path will no longer be set after this call.
		/// </remarks>
		/// <param name="x">X co-ordinate of the top left of the rectangle</param>
		/// <param name="y">Y co-ordinate of the top left of the rectangle</param>
		/// <param name="width">Width of the rectangle</param>
		/// <param name="height">Height of the rectangle</param>
		void AddRectangle (float x, float y, float width, float height);

		/// <summary>
		/// Adds the specified <paramref name="path"/> to the current path, optionally connecting the current figure to the start of the path
		/// </summary>
		/// <remarks>
		/// The <paramref name="connect"/> parameter only specifies that the path should be connected to the current path
		/// at the beginning. The end of the specified path will always be connected to the next segment added to this path,
		/// unlesss <see cref="CloseFigure"/> or <see cref="StartFigure"/> are called after this.
		/// </remarks>
		/// <param name="path">Child path to add to this instance</param>
		/// <param name="connect">True to connect the current figure to the first figure of the specified path, if it is not closed</param>
		void AddPath (IGraphicsPath path, bool connect = false);

		/// <summary>
		/// Transforms the points in the path with the specified matrix
		/// </summary>
		/// <param name="matrix">Matrix to transform the path</param>
		void Transform (IMatrix matrix);

		/// <summary>
		/// Starts a new figure without closing the current figure
		/// </summary>
		/// <remarks>
		/// This will make the next segment added to the path independent (unconnected) to the last segment.
		/// </remarks>
		void StartFigure ();

		/// <summary>
		/// Closes the current figure by connecting a line to the beginning of the figure
		/// </summary>
		/// <remarks>
		/// This will also make the next segment added to the path start independently from the last figure.
		/// To start a new figure without closing the current one, use <see cref="StartFigure"/>
		/// </remarks>
		void CloseFigure ();

		IGraphicsPath Clone();
	}

	/// <summary>
	/// Handler interface for the <see cref="IGraphicsPath"/>
	/// </summary>
	/// <copyright>(c) 2012 by Curtis Wensley</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	public interface IGraphicsPathHandler : IGraphicsPath
	{
	}

	/// <summary>
	/// Extensions for the <see cref="IGraphicsPath"/>
	/// </summary>
	/// <copyright>(c) 2012 by Curtis Wensley</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	public static class GraphicsPathExtensions
	{
		/// <summary>
		/// Moves the current position to the specified <paramref name="point"/> without adding anything to the path
		/// </summary>
		/// <param name="path">Path to move the current position</param>
		/// <param name="point">Point to move to</param>
		public static void MoveTo (this IGraphicsPath path, PointF point)
		{
			path.MoveTo (point.X, point.Y);
		}

		/// <summary>
		/// Adds a line from the current position to the specified location
		/// </summary>
		/// <param name="path">Path to add the line to</param>
		/// <param name="point">Ending point for the line</param>
		public static void LineTo (this IGraphicsPath path, PointF point)
		{
			path.LineTo (point.X, point.Y);
		}

		/// <summary>
		/// Adds lines to each of the specified <paramref name="points"/>
		/// </summary>
		/// <param name="path">Path to add the lines to</param>
		/// <param name="points">Points for each line</param>
		public static void AddLines (this IGraphicsPath path, params PointF[] points)
		{
			path.AddLines (points);
		}

		/// <summary>
		/// Adds a line to the path
		/// </summary>
		/// <param name="path">Path to add the line to</param>
		/// <param name="start">Starting point for the line</param>
		/// <param name="end">Ending point for the line</param>
		public static void AddLine (this IGraphicsPath path, PointF start, PointF end)
		{
			path.AddLine (start.X, start.Y, end.X, end.Y);
		}

		/// <summary>
		/// Adds an arc to the path at the specified <paramref name="location"/>
		/// </summary>
		/// <param name="path">Path to add the arc to</param>
		/// <param name="location">Location of the bounding rectangle of the arc</param>
		/// <param name="startAngle">Start angle in degrees</param>
		/// <param name="sweepAngle">Sweep angle (positive or negative) in degrees</param>
		public static void AddArc (this IGraphicsPath path, RectangleF location, float startAngle, float sweepAngle)
		{
			path.AddArc (location.X, location.Y, location.Width, location.Height, startAngle, sweepAngle);
		}

		/// <summary>
		/// Adds a curve that intersects with the specified <paramref name="points"/> to the path
		/// </summary>
		/// <param name="path">Path to add the curve to</param>
		/// <param name="points">Points that define where the curve intersects</param>
		public static void AddCurve (this IGraphicsPath path, params PointF[] points)
		{
			path.AddCurve (points);
		}

		/// <summary>
		/// Adds a curve that intersects with the specified <paramref name="points"/> to the path with the given <paramref name="tension"/>
		/// </summary>
		/// <param name="path">Path to add the curve to</param>
		/// <param name="tension">Tension between points in the curve.  Should be between 0 (no curve) and 1 (more curve)</param>
		/// <param name="points">Points that intersect with the curve</param>
		public static void AddCurve (this IGraphicsPath path, float tension, params PointF[] points)
		{
			path.AddCurve (points, tension);
		}

		/// <summary>
		/// Adds an ellipse to the path at the specified <paramref name="location"/>
		/// </summary>
		/// <param name="path">Path to add the ellipse to</param>
		/// <param name="location">Location of the bounding rectangle of the ellipse</param>
		public static void AddEllipse (this IGraphicsPath path, RectangleF location)
		{
			path.AddEllipse (location.X, location.Y, location.Width, location.Height);
		}

		/// <summary>
		/// Adds a rectangle to the path at the specified <paramref name="location"/>
		/// </summary>
		/// <param name="path">Path to add the rectangle to</param>
		/// <param name="location">Location of the rectangle</param>
		public static void AddRectangle (this IGraphicsPath path, RectangleF location)
		{
			path.AddRectangle (location.X, location.Y, location.Width, location.Height);
		}
	}
	
	/// <summary>
	/// Defines primitives that can be used to draw or fill a path on a <see cref="Graphics"/> object
	/// </summary>
	/// <remarks>
	/// This is a thin wrapper around the <see cref="IGraphicsPath"/> interface, which is created via
	/// <see cref="GraphicsPath.Create"/>.
	/// </remarks>
	/// <copyright>(c) 2012 by Curtis Wensley</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	public class GraphicsPath : IGraphicsPath, IHandlerSource
	{
		IGraphicsPath Handler { get; set; }

		/// <summary>
		/// Gets the bounding rectangle for this path
		/// </summary>
		public RectangleF Bounds
		{
			get { return Handler.Bounds; }
		}
		
		/// <summary>
		/// Gets a value indicating that this graphics path is empty and has no segments
		/// </summary>
		/// <value><c>true</c> if this path is empty; otherwise, <c>false</c>.</value>
		public bool IsEmpty
		{
			get { return Handler.IsEmpty; }
		}
		
		/// <summary>
		/// Gets the current point
		/// </summary>
		/// <remarks>
		/// If the current figure in the path is not closed, and <see cref="StartFigure"/> is not called,
		/// the next segment will be connected from this point to its start point.
		/// </remarks>
		/// <value>The current point</value>
		public PointF CurrentPoint
		{
			get { return Handler.CurrentPoint; }
		}

		/// <summary>
		/// Creates a delegate that can be used to create instances of the <see cref="IGraphicsPath"/>
		/// </summary>
		/// <remarks>
		/// This is useful when creating a very large number of graphics path objects
		/// </remarks>
		/// <param name="generator">Generator used to create the graphics path objects</param>
		public static Func<IGraphicsPath> Instantiator (Generator generator = null)
		{
			var instantiator = generator.Find<IGraphicsPathHandler>();
			return () => {
				return instantiator ();
			};
		}
		
		/// <summary>
		/// Creates a new instance of the IGraphicsPath for the specified generator
		/// </summary>
		/// <param name="generator">Platform generator for the object, or null to use the current generator</param>
		public static IGraphicsPath Create (Generator generator = null)
		{
			return generator.Create<IGraphicsPathHandler> ();
		}

		/// <summary>
		/// Initializes a new instance of the GraphicsPath class
		/// </summary>
		/// <param name="generator">Platform generator for the object, or null to use the current generator</param>
		public GraphicsPath (Generator generator = null)
		{
			Handler = Create (generator);
		}

		private GraphicsPath(IGraphicsPath handler)
		{
			Handler = handler;
		}

		/// <summary>
		/// Adds a line to the path with the specified start and end points
		/// </summary>
		/// <remarks>
		/// If the current figure is not closed, it will connect with the start of this line.
		/// The current position will be moved to the specified end location.
		/// </remarks>
		/// <param name="startX">X co-ordinate of the starting point</param>
		/// <param name="startY">Y co-ordinate of the starting point</param>
		/// <param name="endX">X co-ordinate of the end point</param>
		/// <param name="endY">Y co-ordinate of the end point</param>
		public void AddLine (float startX, float startY, float endX, float endY)
		{
			Handler.AddLine (startX, startY, endX, endY);
		}
		
		/// <summary>
		/// Adds lines to each of the specified <paramref name="points"/> to the path 
		/// </summary>
		/// <remarks>
		/// If the current figure is not closed, it will connect with the first point specified.
		/// The current position will be moved to the last point specified
		/// </remarks>
		/// <param name="points">Points for each part of the line</param>
		public void AddLines (IEnumerable<PointF> points)
		{
			Handler.AddLines (points);
		}

		/// <summary>
		/// Adds a line from the current position to the specified location
		/// </summary>
		/// <param name="x">X co-ordinate to draw the line to</param>
		/// <param name="y">Y co-ordinate to draw the line to</param>
		public void LineTo (float x, float y)
		{
			Handler.LineTo (x, y);
		}

		/// <summary>
		/// Moves the current position to the specified location without adding anything to the path
		/// </summary>
		/// <param name="x">X co-ordinate to move to</param>
		/// <param name="y">Y co-ordinate to move to</param>
		public void MoveTo (float x, float y)
		{
			Handler.MoveTo (x, y);
		}

		/// <summary>
		/// Adds an arc into the specified rectangle
		/// </summary>
		/// <remarks>
		/// If the current figure is not closed, it will connect with the start of the arc.
		/// The current position will be moved to the ending point of the arc
		/// </remarks>
		/// <param name="x">The x coordinate of the upper left of the arc</param>
		/// <param name="y">The y coordinate of the upper left of the arc</param>
		/// <param name="width">Width of the rectangle containing the arc</param>
		/// <param name="height">Height of the rectangle containing the arc</param>
		/// <param name="startAngle">Start angle to begin the arc, in degrees</param>
		/// <param name="sweepAngle">Sweep angle (positive or negative) to specify how long the arc is, in degrees</param>
		public void AddArc (float x, float y, float width, float height, float startAngle, float sweepAngle)
		{
			Handler.AddArc (x, y, width, height, startAngle, sweepAngle);
		}

		/// <summary>
		/// Adds a bezier curve to the path with two control points
		/// </summary>
		/// <remarks>
		/// If the current figure is not closed, it will connect with the <paramref name="start"/> of the bezier curve.
		/// The current position will be moved to the <paramref name="end"/> point.
		/// </remarks>
		/// <param name="start">Starting point of the bezier curve</param>
		/// <param name="control1">First control point of the curve</param>
		/// <param name="control2">Second control point of the curve</param>
		/// <param name="end">Ending point of the bezier curve</param>
		public void AddBezier (PointF start, PointF control1, PointF control2, PointF end)
		{
			Handler.AddBezier (start, control1, control2, end);
		}

		/// <summary>
		/// Adds a curve that intersects with the specified <paramref name="points"/> to the path
		/// </summary>
		/// <remarks>
		/// Each point in the list will fall on the line based on the <paramref name="tension"/> parameter
		/// </remarks>
		/// <param name="points">Points to calculate the curve</param>
		/// <param name="tension">Tension between points in the curve.  Should be between 0 (no curve) and 1 (more curve)</param>
		public void AddCurve (IEnumerable<PointF> points, float tension = 0.5f)
		{
			Handler.AddCurve (points, tension);
		}

		/// <summary>
		/// Adds an ellipse to the path
		/// </summary>
		/// <remarks>
		/// Rectangles are separate figures and will not connect to the current or next figure in the path.
		/// The starting point of the path will no longer be set after this call.
		/// </remarks>
		/// <param name="x">X co-ordinate of the top left of the ellipse</param>
		/// <param name="y">Y co-ordinate of the top left of the ellipse's bounding rectangle</param>
		/// <param name="width">Width of the rectangle</param>
		/// <param name="height">Height of the rectangle</param>
		public void AddEllipse (float x, float y, float width, float height)
		{
			Handler.AddEllipse (x, y, width, height);
		}

		/// <summary>
		/// Adds a rectangle to the path
		/// </summary>
		/// <remarks>
		/// Rectangles are separate figures and will not connect to the current or next figure in the path.
		/// The starting point of the path will no longer be set after this call.
		/// </remarks>
		/// <param name="x">X co-ordinate of the top left of the rectangle</param>
		/// <param name="y">Y co-ordinate of the top left of the rectangle</param>
		/// <param name="width">Width of the rectangle</param>
		/// <param name="height">Height of the rectangle</param>
		public void AddRectangle (float x, float y, float width, float height)
		{
			Handler.AddRectangle (x, y, width, height);
		}

		/// <summary>
		/// Adds the specified <paramref name="path"/> to the current path, optionally connecting the current figure to the start of the path
		/// </summary>
		/// <remarks>
		/// The <paramref name="connect"/> parameter only specifies that the path should be connected to the current path
		/// at the beginning. The end of the specified path will always be connected to the next segment added to this path,
		/// unlesss <see cref="CloseFigure"/> or <see cref="StartFigure"/> are called after this.
		/// </remarks>
		/// <param name="path">Child path to add to this instance</param>
		/// <param name="connect">True to connect the current figure to the first figure of the specified path, if it is not closed</param>
		public void AddPath (IGraphicsPath path, bool connect = false)
		{
			Handler.AddPath (path, connect);
		}

		/// <summary>
		/// Transforms the points in the path with the specified matrix
		/// </summary>
		/// <param name="matrix">Matrix to transform the path</param>
		public void Transform (IMatrix matrix)
		{
			Handler.Transform (matrix);
		}

		/// <summary>
		/// Starts a new figure without closing the current figure
		/// </summary>
		/// <remarks>
		/// This will make the next segment added to the path independent (unconnected) to the last segment.
		/// </remarks>
		public void StartFigure ()
		{
			Handler.StartFigure ();
		}

		/// <summary>
		/// Closes the current figure by connecting a line to the beginning of the figure
		/// </summary>
		/// <remarks>
		/// This will also make the next segment added to the path start independently from the last figure.
		/// To start a new figure without closing the current one, use <see cref="StartFigure"/>
		/// </remarks>
		public void CloseFigure ()
		{
			Handler.CloseFigure ();
		}

		/// <summary>
		/// Releases all resources used by the <see cref="Eto.Drawing.GraphicsPath"/> object
		/// </summary>
		public void Dispose ()
		{
			Handler.Dispose ();
		}

		public IGraphicsPath Clone()
		{
			return new GraphicsPath(Handler.Clone());
		}
		/// <summary>
		/// Gets the platform-specific control object
		/// </summary>
		object IControlObjectSource.ControlObject
		{
			get { return Handler.ControlObject; }
		}

		object IHandlerSource.Handler
		{
			get { throw new NotImplementedException (); }
		}
	}
}

