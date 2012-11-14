using System;
using System.Collections.Generic;

namespace Eto.Drawing
{
	/// <summary>
	/// Platform handler interface for the <see cref="GraphicsPath"/> class
	/// </summary>
	public interface IGraphicsPath : IInstanceWidget
	{
		/// <summary>
		/// Adds the <paramref name="lines"/> to the path 
		/// </summary>
		/// <param name="lines"></param>
		void AddLines (IEnumerable<Point> lines);

		/// <summary>
		/// Adds a single line to the path
		/// </summary>
		/// <param name="point1">Starting point for the line</param>
		/// <param name="point2">Ending point for the line</param>
		void AddLine (Point point1, Point point2);
		
		/// <summary>
		/// Adds a line to the specified <paramref name="point"/> from the last location
		/// </summary>
		/// <param name="point">Ending point for the line</param>
		void LineTo (Point point);
		
		/// <summary>
		/// Moves the current position to the specified <paramref name="point"/>, without adding anything to the path
		/// </summary>
		/// <param name="point">Location to move the current position</param>
		void MoveTo (Point point);
	}
	
	/// <summary>
	/// Defines primitives that can be used to draw or fill a path on a <see cref="Graphics"/> object
	/// </summary>
	public class GraphicsPath : InstanceWidget
	{
		IGraphicsPath inner;
		
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
			inner = (IGraphicsPath)Handler;
		}

		/// <summary>
		/// Moves the current position to the specified <paramref name="point"/>, without adding anything to the path
		/// </summary>
		/// <param name="point">Location to move the current position</param>
		public void MoveTo (Point point)
		{
			inner.MoveTo (point);
		}

		/// <summary>
		/// Adds a line to the specified <paramref name="point"/> from the last location
		/// </summary>
		/// <param name="point">Ending point for the line</param>
		public void LineTo (Point point)
		{
			inner.LineTo (point);
		}

		/// <summary>
		/// Adds a single line to the path
		/// </summary>
		/// <param name="point1">Starting point for the line</param>
		/// <param name="point2">Ending point for the line</param>
		public void AddLine (Point point1, Point point2)
		{
			inner.AddLine (point1, point2);
		}

		/// <summary>
		/// Adds the <paramref name="lines"/> to the path 
		/// </summary>
		/// <param name="lines"></param>
		public void AddLines (IEnumerable<Point> lines)
		{
			inner.AddLines (lines);
		}
	}
}

