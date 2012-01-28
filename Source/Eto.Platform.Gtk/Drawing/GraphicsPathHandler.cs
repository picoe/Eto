using System;
using Eto.Drawing;
using System.Collections.Generic;
using System.Linq;

namespace Eto.Platform.GtkSharp.Drawing
{
	public class GraphicsPathHandler : WidgetHandler<object, GraphicsPath>, IGraphicsPath
	{
		List<Command> commands = new List<Command>();
		public GraphicsPathHandler ()
		{
		}
		
		abstract class Command
		{
			public abstract void Apply (GraphicsHandler handler, bool first);
		}
		
		class Lines : Command
		{
			public Point[] Points { get; set; }

			public override void Apply (GraphicsHandler handler, bool first)
			{
#if CAIRO
				var context = handler.Context;
				for (int i=0; i<Points.Length; i++) {
					var p = Points [i];
					if (first && i == 0)
						context.MoveTo (p.X, p.Y);
					else
						context.LineTo (p.X, p.Y);
				}
#else
				// support this?
#endif
			}
		}

		#region IGraphicsPath implementation
		
		public void AddLines (IEnumerable<Point> points)
		{
			commands.Add (new Lines{ Points = points.ToArray() });
		}
		
		#endregion
		
		public void Apply(GraphicsHandler handler)
		{
			bool first = true;
			foreach (var command in commands)
			{
				command.Apply (handler, first);
				first = false;
			}
		}

	}
}

