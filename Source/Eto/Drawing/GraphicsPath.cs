using System;
using System.Collections.Generic;

namespace Eto.Drawing
{
    /// <summary>
    /// Allows different implementations to be used so that a graphics
    /// path can be rendered to alternate media such as SVG or
    /// dumped to a string.
    /// </summary>
    public interface IGraphicsPathBase : IDisposable
    {
        //
        // Summary:
        //     Adds a cubic Bézier curve to the current figure.
        //
        void AddBezier(PointF pt1, PointF pt2, PointF pt3, PointF pt4);

        void CloseFigure();

        void AddPath(IGraphicsPathBase addingPath, bool connect);

        RectangleF GetBounds();

        GraphicsPath ToGraphicsPath();

		/// <summary>
		/// Adds the <paramref name="lines"/> to the path 
		/// </summary>
		/// <param name="lines"></param>
        void AddLines(PointF[] pointF);

        void Transform(Matrix matrix);
    };

	public interface IGraphicsPath : IInstanceWidget, IGraphicsPathBase
	{
        FillMode FillMode { set; }

        bool IsEmpty { get; }

        void AddArc(RectangleF rect, float startAngle, float sweepAngle);

        void AddBeziers(Point[] points);

        void AddCurve(PointF[] points);

        void AddEllipse(RectangleF rect);

		/// <summary>
		/// Adds a single line to the path
		/// </summary>
		/// <param name="point1">Starting point for the line</param>
		/// <param name="point2">Ending point for the line</param>
        void AddLine(Point point1, Point point2);

		/// <summary>
		/// Adds a single line to the path
		/// </summary>
		/// <param name="point1">Starting point for the line</param>
		/// <param name="point2">Ending point for the line</param>
        void AddLine(PointF point1, PointF point2);

        void AddRectangle(RectangleF rectangle);

		/// <summary>
		/// Adds a line to the specified <paramref name="point"/> from the last location
		/// </summary>
		/// <param name="point">Ending point for the line</param>
        void LineTo(Point point);

		/// <summary>
		/// Moves the current position to the specified <paramref name="point"/>, without adding anything to the path
		/// </summary>
		/// <param name="point">Location to move the current position</param>
        void MoveTo(Point point);

        void Translate(PointF point);

        IGraphicsPath Clone();
    }
	
	/// <summary>
	/// Defines primitives that can be used to draw or fill a path on a <see cref="Graphics"/> object
	/// </summary>
	public class GraphicsPath : InstanceWidget, IGraphicsPath
	/// Defines primitives that can be used to draw or fill a path on a <see cref="Graphics"/> object
	/// </summary>
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

        public GraphicsPath(Generator g, IGraphicsPath inner) : base(g, inner)
        {
            this.inner = inner;
        }

        public GraphicsPath(FillMode fillMode)
            : this()
        {
            this.inner.FillMode = fillMode;
        }

		void IWidget.Initialize()
		{
			base.Initialize();
		}

        public bool IsEmpty { get { return inner.IsEmpty; } }

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

        public void AddCurve(PointF[] points)
        {
            inner.AddCurve(points);
        }

		/// <summary>
		/// Adds a single line to the path
		/// </summary>
		/// <param name="point1">Starting point for the line</param>
		/// <param name="point2">Ending point for the line</param>
        public void AddLine(Point point1, Point point2)
		{
			inner.AddLine (point1, point2);
		}

        public void AddLine(PointF point1, PointF point2)
        {
            inner.AddLine(point1, point2);
        }
		
		/// <summary>
		/// Adds the <paramref name="lines"/> to the path 
		/// </summary>
		/// <param name="lines"></param>
        public void AddLines(params PointF[] points)
        {
            inner.AddLines(points);
        }

        public void AddBezier(PointF pt1, PointF pt2, PointF pt3, PointF pt4)
        {
            inner.AddBezier(
                pt1,
                pt2,
                pt3,
                pt4);
        }

        public void AddPath(GraphicsPath addingPath, bool connect)
        {
            inner.AddPath(addingPath, connect);
        }

        public RectangleF GetBounds()
        {
            return inner.GetBounds();
        }

        public IGraphicsPath Clone()
        {
            return 
                new GraphicsPath(
                    this.Generator,
                    inner.Clone());
        }

        public void Translate(PointF point)
        {
            inner.Translate(point);
        }

        public void AddRectangle(RectangleF rectangle)
        {
            inner.AddRectangle(rectangle);
        }

        public void CloseFigure()
        {
            inner.CloseFigure();
        }

        public void Transform(Matrix matrix)
        {
            inner.Transform(matrix);
        }

        public void AddArc(RectangleF rect, float startAngle, float sweepAngle)
        {
            inner.AddArc(rect, startAngle, sweepAngle);
        }

        public void AddBeziers(Point[] points)
        {
            inner.AddBeziers(points);
        }

        public void AddEllipse(RectangleF rect)
        {
            inner.AddEllipse(rect);
        }

        public void AddEllipse(float x, float y, float width, float height)
        {
            AddEllipse(new RectangleF(x, y, width, height));
        }

        FillMode IGraphicsPath.FillMode
        {
            set { inner.FillMode = value; }
        }

        public void AddPath(IGraphicsPathBase addingPath, bool connect)
        {
            inner.AddPath(addingPath, connect);
        }

        public GraphicsPath ToGraphicsPath()
        {
            return this;
        }

        public new Widget Widget
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Creates a rounded rectangle using the specified corner radius
        /// </summary>
        /// <param name="rect">Rectangle to round</param>
        /// <param name="nwRadius">Radius of the north east corner</param>
        /// <param name="neRadius">Radius of the north west corner</param>
        /// <param name="seRadius">Radius of the south east corner</param>
        /// <param name="swRadius">Radius of the south west corner</param>
        /// <returns>GraphicsPath with the lines of the rounded rectangle ready to be painted</returns>
        public static GraphicsPath GetRoundRect(
            RectangleF rect,
            float nwRadius,
            float neRadius,
            float seRadius,
            float swRadius)
        {
            ///  NW-----NE
            ///  |       |
            ///  |       |
            ///  SW-----SE

            var result = new GraphicsPath();

            nwRadius *= 2;
            neRadius *= 2;
            seRadius *= 2;
            swRadius *= 2;

            //NW ---- NE
            result.AddLine(
                new PointF(rect.X + nwRadius, rect.Y),
                new PointF(rect.Right - neRadius, rect.Y));

            //NE Arc
            if (neRadius > 0f)
            {
                result.AddArc(
                    RectangleF.FromLTRB(
                        rect.Right - neRadius,
                        rect.Top,
                        rect.Right,
                        rect.Top + neRadius),
                    -90,
                    90);
            }

            // NE
            //  |
            // SE
            result.AddLine(
                new PointF(
                    rect.Right,
                    rect.Top + neRadius),
                new PointF(
                    rect.Right,
                    rect.Bottom - seRadius));

            //SE Arc
            if (seRadius > 0f)
            {
                result.AddArc(
                    RectangleF.FromLTRB(
                        rect.Right - seRadius,
                        rect.Bottom - seRadius,
                        rect.Right,
                        rect.Bottom),
                    0,
                    90);
            }

            // SW --- SE
            result.AddLine(
                new PointF(
                    rect.Right - seRadius,
                    rect.Bottom),
                new PointF(
                    rect.Left + swRadius,
                    rect.Bottom));

            //SW Arc
            if (swRadius > 0f)
            {
                result.AddArc(
                    RectangleF.FromLTRB(
                        rect.Left,
                        rect.Bottom - swRadius,
                        rect.Left + swRadius,
                        rect.Bottom),
                    90,
                    90);
            }

            // NW
            // |
            // SW
            result.AddLine(
                new PointF(
                    rect.Left,
                    rect.Bottom - swRadius),
                new PointF(
                    rect.Left,
                    rect.Top + nwRadius));

            //NW Arc
            if (nwRadius > 0f)
            {
                result.AddArc(
                    RectangleF.FromLTRB(
                        rect.Left,
                        rect.Top,
                        rect.Left + nwRadius,
                        rect.Top + nwRadius),
                    180,
                    90);
            }

            result.CloseFigure();

            return result;
        }

        public new Generator Generator
        {
            get
            {
                return base.Generator;
            }
            set
            {
                throw new InvalidOperationException();
            }
        }
    }

    // Summary:
    //     Specifies how the interior of a closed path is filled.
    public enum FillMode
    {
        // Summary:
        //     Specifies the alternate fill mode.
        Alternate = 0,
        //
        // Summary:
        //     Specifies the winding fill mode.
        Winding = 1,
    }
}

