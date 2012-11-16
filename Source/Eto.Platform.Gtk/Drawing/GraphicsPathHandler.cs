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
			public PointF[] Points { get; set; }

			public override void Apply (GraphicsHandler handler, bool first)
			{
				var context = handler.Control;
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

        public void AddLines(PointF[] points)
        {
            commands.Add(new Lines { Points = points });
        }

		public void MoveTo (Point point)
		{
			Add ((handler, first) => {
				handler.Control.MoveTo (point.X, point.Y);
			});
		}
		
		public void LineTo (Point point)
		{
			Add ((handler, first) => {
				handler.Control.LineTo (point.X, point.Y);
			});
		}
		
		public void AddLine (Point point1, Point point2)
		{
			Add ((handler, first) => {
				handler.Control.MoveTo (point1.X, point1.Y);
				handler.Control.LineTo (point2.X, point2.Y);
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


        #region IGraphicsPath Members


        public RectangleF GetBounds()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IGraphicsPath Members


        public void Translate(PointF point)
        {
            throw new NotImplementedException();
        }

        public IGraphicsPath Clone()
        {
            throw new NotImplementedException();
        }

        public void AddRectangle(RectangleF rectangle)
        {
            throw new NotImplementedException();
        }

        public void CloseFigure()
        {
            throw new NotImplementedException();
        }

        public bool IsEmpty
        {
            get { throw new NotImplementedException(); }
        }

        public void AddCurve(PointF[] points)
        {
            throw new NotImplementedException();
        }

        public void AddLine(PointF point1, PointF point2)
        {
            throw new NotImplementedException();
        }

        public void AddBezier(PointF pt1, PointF pt2, PointF pt3, PointF pt4)
        {
            throw new NotImplementedException();
        }

        public void AddPath(IGraphicsPathBase addingPath, bool connect)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IGraphicsPath Members

        public FillMode FillMode
        {
            set { throw new NotImplementedException(); }
        }

        public void Transform(Matrix matrix)
        {
            throw new NotImplementedException();
        }

        public void AddArc(RectangleF rect, float startAngle, float sweepAngle)
        {
            throw new NotImplementedException();
        }

        public void AddBeziers(Point[] points)
        {
            throw new NotImplementedException();
        }

        public void AddEllipse(RectangleF rect)
        {
            throw new NotImplementedException();
        }

        #endregion

        public void AddEllipse(float x, float y, float width, float height)
        {
            throw new NotImplementedException();
        }

        public GraphicsPath ToGraphicsPath()
        {
            throw new NotImplementedException(); // should never get called
        }
    }
}

