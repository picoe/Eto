using System;
using System.Collections.Generic;

namespace Eto.Drawing
{
	public interface IGraphicsPath : IInstanceWidget
	{
		void AddLines (IEnumerable<Point> points);

		void AddLine (Point point1, Point point2);
		
		void LineTo (Point point);
		
		void MoveTo (Point point);
	}
	
	public class GraphicsPath : InstanceWidget
	{
		IGraphicsPath inner;
		
		public GraphicsPath ()
			: this(Generator.Current)
		{
		}
		
		public GraphicsPath (Generator g)
			: base(g, typeof(IGraphicsPath))
		{
			inner = (IGraphicsPath)Handler;
		}

		public void MoveTo (Point point)
		{
			inner.MoveTo (point);
		}
		
		public void LineTo (Point point)
		{
			inner.LineTo (point);
		}

		public void AddLine (Point point1, Point point2)
		{
			inner.AddLine (point1, point2);
		}
		
		public void AddLines (IEnumerable<Point> points)
		{
			inner.AddLines (points);
		}
		
	}
}

