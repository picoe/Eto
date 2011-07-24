using System;
using System.Collections.Generic;

namespace Eto.Drawing
{
	public interface IGraphicsPath : IInstanceWidget
	{
		void AddLines(IEnumerable<Point> points);
	}
	
	public class GraphicsPath : InstanceWidget
	{
		IGraphicsPath inner;
		
		public GraphicsPath()
			: this(Generator.Current)
		{
		}
		
		public GraphicsPath (Generator g)
			: base(g, typeof(IGraphicsPath))
		{
			inner = (IGraphicsPath)Handler;
		}
		
		public void AddLines(IEnumerable<Point> points)
		{
			inner.AddLines (points);
		}
		
	}
}

