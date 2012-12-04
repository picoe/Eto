using System;
using System.Collections.Generic;

namespace Eto.Drawing
{
	public interface IGraphicsPath : IInstanceWidget
	{
		/// <summary>
		/// Adds a single line to the path
		/// </summary>
		/// <param name="point1">Starting point for the line</param>
		/// <param name="point2">Ending point for the line</param>
		void AddLine (PointF point1, PointF point2);

		/// <summary>
		/// Adds the <paramref name="points"/> to the path 
		/// </summary>
		/// <param name="points"></param>
		void AddLines (PointF[] points);

		/// <summary>
		/// Adds a line to the specified <paramref name="point"/> from the last location
		/// </summary>
		/// <param name="point">Ending point for the line</param>
		void LineTo (PointF point);

		/// <summary>
		/// Moves the current position to the specified <paramref name="point"/>, without adding anything to the path
		/// </summary>
		/// <param name="point">Location to move the current position</param>
		void MoveTo (PointF point);
	}
	
	/// <summary>
	/// Defines primitives that can be used to draw or fill a path on a <see cref="Graphics"/> object
	/// </summary>
	public class GraphicsPath : InstanceWidget
	{
		new IGraphicsPath Handler { get { return (IGraphicsPath)base.Handler; } }

		/// <summary>
		/// Initializes a new instance of the GraphicsPath class
		/// </summary>
		public GraphicsPath ()
			: this(Generator.Current)
		{
		}
		
		/// <summary>
		/// Initializes a new instance of the GraphicsPath class
		/// </summary>
		/// <param name="generator">Platform generator for the object</param>
		public GraphicsPath (Generator generator)
			: base(generator, typeof(IGraphicsPath))
		{
		}

		public GraphicsPath (Generator g, IGraphicsPath handler) : base(g, handler)
		{
		}

		/// <summary>
		/// Moves the current position to the specified <paramref name="point"/>, without adding anything to the path
		/// </summary>
		/// <param name="point">Location to move the current position</param>
		public void MoveTo (Point point)
		{
			Handler.MoveTo (point);
		}

		/// <summary>
		/// Adds a line to the specified <paramref name="point"/> from the last location
		/// </summary>
		/// <param name="point">Ending point for the line</param>
		public void LineTo (Point point)
		{
			Handler.LineTo (point);
		}

		/// <summary>
		/// Adds a single line to the path
		/// </summary>
		/// <param name="point1">Starting point for the line</param>
		/// <param name="point2">Ending point for the line</param>
		public void AddLine (Point point1, Point point2)
		{
			Handler.AddLine (point1, point2);
		}

		/// <summary>
		/// Adds the <paramref name="points"/> to the path 
		/// </summary>
		/// <param name="points"></param>
		public void AddLines (params PointF[] points)
		{
			Handler.AddLines (points);
		}
	}
}

