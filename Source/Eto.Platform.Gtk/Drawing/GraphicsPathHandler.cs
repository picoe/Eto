using System;
using Eto.Drawing;
using System.Collections.Generic;
using System.Linq;

namespace Eto.Platform.GtkSharp.Drawing
{
	public class GraphicsPathHandler : WidgetHandler<object, GraphicsPath>, IGraphicsPath
	{
		List<Command> commands = new List<Command> ();

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
				var context = handler.Context;
				for (int i=0; i<Points.Length; i++) {
					var p = Points [i];
					if (i == 0)
						context.MoveTo (p.X, p.Y);
					else
						context.LineTo (p.X, p.Y);
				}
			}
		}

		delegate void GraphicsPathCommandDelegate (GraphicsHandler handler,bool first);

		class ActionCommand : Command
		{
			GraphicsPathCommandDelegate apply;

			public ActionCommand (GraphicsPathCommandDelegate apply)
			{
				this.apply = apply; 
			}

			public override void Apply (GraphicsHandler handler, bool first)
			{
				apply (handler, first);
			}
		}
		
		#region IGraphicsPath implementation
		
		public void AddLines (IEnumerable<Point> points)
		{
			commands.Add (new Lines{ Points = points.ToArray () });
		}
		
		public void MoveTo (Point point)
		{
			Add ((handler, first) => {
				handler.Context.MoveTo (point.X, point.Y);
			});
		}
		
		public void LineTo (Point point)
		{
			Add ((handler, first) => {
				handler.Context.LineTo (point.X, point.Y);
			});
		}
		
		public void AddLine (Point point1, Point point2)
		{
			Add ((handler, first) => {
				handler.Context.MoveTo (point1.X, point1.Y);
				handler.Context.LineTo (point2.X, point2.Y);
			});
		}
		
		void Add (GraphicsPathCommandDelegate apply)
		{
			commands.Add (new ActionCommand (apply));
		}
		
		#endregion
		
		public void Apply (GraphicsHandler handler)
		{
			bool first = true;
			foreach (var command in commands) {
				command.Apply (handler, first);
				first = false;
			}
		}

	}
}

